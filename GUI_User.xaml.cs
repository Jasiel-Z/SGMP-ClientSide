using SGMP_Client.SGPMManagerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SGMP_Client
{
    /// <summary>
    /// Interaction logic for GUI_User.xaml
    /// </summary>
    public partial class GUI_User : Window
    {
        public GUI_User()
        {
            InitializeComponent();
        }

        private void Btn_Save_User_Click(object sender, RoutedEventArgs e)
        {
            string staffNumberString = tbxStaffNumber.Text.ToString();
            string email = tbxEMail.Text.ToString();
            string password = pwbPassword.Password.ToString();
            string repeatPassword = pwbRepeatPassword.Password.ToString();

            if (ValidateFields(staffNumberString, email, password, repeatPassword))
            {
                int staffNumber = int.Parse(staffNumberString);
                int result = SaveUser(staffNumber, email, password);

                if (result == 1)
                {
                    MessageBox.Show("Se ha guardado el nuevo usuario correctamente.", "Operación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ha ocurrido un error al intentar guardar el nuevo usuario. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private int SaveUser(int staffNumber, string email, string password)
        {
            SGPMManagerService.UserManagementClient client = new SGPMManagerService.UserManagementClient();

            User user = new User
            {
                EmployeeNumber = staffNumber,
                Email = email,
                Password = Utility.ComputeSha256Hash(password)
            };

            int result = client.SaveUser(user);

            return result;
        }

        private bool ValidateFields(string staffNumber, string email, string password, string repeatPassword)
        {
            bool isStaffNumberValid = false;
            bool isEmailValid = false;
            bool isPasswordValid = false;

            if (!string.IsNullOrEmpty(staffNumber.ToString()) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(repeatPassword))
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Hidden;

                isStaffNumberValid = ValidateStaffNumber(staffNumber);
                isEmailValid = ValidateEmail(email);
                isPasswordValid = ValidatePassword(password, repeatPassword);           
            }
            else
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Visible;
            }

            return isStaffNumberValid && isEmailValid && isPasswordValid;
        }

        public bool ValidateStaffNumber(string staffNumber)
        {
            bool isStaffNumberValid;
            if (staffNumber.Length == 10 && Regex.IsMatch(staffNumber, "^[0-9]+$"))
            {
                SGPMManagerService.UserManagementClient client = new SGPMManagerService.UserManagementClient();

                // if (client.ValidateStaffNumberDoesNotExist(staffNumber)) 
                // {
                    isStaffNumberValid = true;            
                    lbInvalidStaffNumberMessage.Visibility = Visibility.Hidden;
                // }
                // else 
                // {
                //     lbInvalidStaffNumberMessage.Content = "El número de empleado ya está asociado a una cuenta.";
                //     lbInvalidStaffNumberMessage.Visibility = Visibility.Visible;
                // }
            }
            else
            {
                isStaffNumberValid = false;
                lbInvalidStaffNumberMessage.Content = "Debe contener 10 dígitos.";
                lbInvalidStaffNumberMessage.Visibility = Visibility.Visible;
            }
            return isStaffNumberValid;
        }

        public bool ValidateEmail(string email)
        {
            bool isEmailValid = false;
            if ((Regex.IsMatch(email, "^[a-zA-Z0-9\\-_]{5,20}@(gob)\\.mx$")))
            {
                SGPMManagerService.UserManagementClient client = new SGPMManagerService.UserManagementClient();

                if (client.ValidateEmailDoesNotExist(email))
                {
                    isEmailValid = true;
                    lbInvalidEmailMessage.Visibility = Visibility.Hidden;
                } else
                {
                    isEmailValid = false;
                    lbInvalidEmailMessage.Content = "El correo electrónico ingresado ya está vinculado a una cuenta.";
                    lbInvalidEmailMessage.Visibility = Visibility.Visible;
                } 
            }
            else
            {
                isEmailValid = false;
                lbInvalidEmailMessage.Content = "Revise que incluya el dominio \"@gob.mx\".";
                lbInvalidEmailMessage.Visibility = Visibility.Visible;
            }
            return isEmailValid;
        }

        public bool ValidatePassword(string password, string repeatPassword) 
        {
            bool isPasswordValid;
            if (Regex.IsMatch(password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&#.$($)\\-_])[A-Za-z\\d$@$!%*?&#.$($)\\-_]{8,16}$"))
            {
                tbInvalidPasswordMessage.Visibility = Visibility.Hidden;

                if (password.Equals(repeatPassword))
                {
                    isPasswordValid = true;
                    lbUnmatchingPasswordMessage.Visibility = Visibility.Hidden;
                }
                else
                {
                    isPasswordValid = false;
                    lbUnmatchingPasswordMessage.Visibility = Visibility.Visible;
                }
            }
            else
            {
                isPasswordValid = false;
                tbInvalidPasswordMessage.Visibility = Visibility.Visible;
            }
            return isPasswordValid;
        }

        private void OnlyNumbers_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumber(e.Text);
        }

        private bool IsNumber(string text)
        {
            return int.TryParse(text, out _);
        }

        private void TbxStaffNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V)
            {
                e.Handled = true;
            }
        }

        private void TbxStaffNumber_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window mainMenuWindow = new GUI_MainMenu();
            mainMenuWindow.Show();
            this.Close();
        }
    }
}
