using SGMP_Client.SGPMService;
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
        private Dictionary<int, string> localitiesDictionary = new Dictionary<int, string>();
        private int employeeNumber = 0;
        private string userEmail = string.Empty;

        public GUI_User()
        {
            InitializeComponent();
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            List<string> roles = new List<string>
            {
                "Administrador",
                "Director",
                "Analista"
            };
            
            List<string> localitiesNames = new List<string>();
            localitiesDictionary = new Dictionary<int, string>();
            var localities = GetLocalitiesFromDatabase();
            foreach (var locality in localities)
            {
                string localityTownship;
                localityTownship = locality.Name + ", " + locality.Township;
                localitiesNames.Add(localityTownship);
                localitiesDictionary[locality.LocalityID] = localityTownship;
            }
            localitiesNames.Sort();

            cbRole.ItemsSource = roles;
            cbLocality.ItemsSource = localitiesNames;
        }

        public void LoadUser(User user)
        {
            this.employeeNumber = user.EmployeeNumber;
            this.userEmail = user.Email;

            this.Title = "Modificar Usuario";
            lbTitle.Content = "Modificar Usuario";
            tbxStaffNumber.IsEnabled = false;

            tbxStaffNumber.Text = user.EmployeeNumber.ToString();
            tbxName.Text = user.Name;
            tbxMiddleName.Text = user.MiddleName;
            tbxLastName.Text = user.LastName;
            tbxPhoneNumber.Text = user.PhoneNumber;
            tbxCity.Text = user.City;
            tbxStreet.Text = user.Street;
            tbxNumber.Text = user.Number.ToString();
            cbRole.SelectedItem = user.Role;
            cbLocality.SelectedItem = localitiesDictionary[user.LocationId];
            tbxEMail.Text = user.Email;
        }

        private void Btn_Save_User_Click(object sender, RoutedEventArgs e)
        {
            string staffNumberString = tbxStaffNumber.Text.ToString();
            string email = tbxEMail.Text.ToString();
            string password = pwbPassword.Password.ToString();
            string repeatPassword = pwbRepeatPassword.Password.ToString();
            string name = tbxName.Text.ToString();
            string middleName = tbxMiddleName.Text.ToString();
            string lastName = tbxLastName.Text.ToString();
            string city = tbxCity.Text.ToString();
            string street = tbxStreet.Text.ToString();
            string number = tbxNumber.Text.ToString();
            string phoneNumber = tbxPhoneNumber.Text.ToString();
            string role = "";
            string locality = "";
            if (cbRole.SelectedItem != null && cbLocality.SelectedItem != null)
            {
                role = cbRole.SelectedItem.ToString();
                locality = cbLocality.SelectedItem.ToString();
            }

            if (ValidateUserFields(staffNumberString, email, password, repeatPassword) 
                && ValidateEmployeeFields(name, middleName, lastName, city, street, number, phoneNumber, role, locality))
            {
                User user = new User
                {
                    Name = name,
                    MiddleName = middleName,
                    LastName = lastName,
                    City = city,
                    Street = street,
                    Number = int.Parse(number),
                    PhoneNumber = phoneNumber,
                    Role = role,
                    LocationId = localitiesDictionary.FirstOrDefault(x => x.Value == locality).Key,
                    EmployeeNumber = int.Parse(staffNumberString),
                    Email = email,             
                };

                if (string.IsNullOrEmpty(password))
                {
                    user.Password = "";
                }
                else
                {
                    user.Password = Utility.ComputeSha256Hash(password);
                }

                if (employeeNumber == 0)
                {
                    SaveUser(user);
                }
                else
                {
                    UpdateUser(user);
                }
            }
        }

        private void SaveUser(User user)
        {
            SGPMService.UserManagementClient client = new SGPMService.UserManagementClient();

            int result = client.SaveUser(user);

            if (result > 0)
            {
                MessageBox.Show("Se ha guardado el nuevo usuario correctamente.", "Operación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error al intentar guardar el nuevo usuario. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateUser(User user) 
        {
            SGPMService.UserManagementClient client = new SGPMService.UserManagementClient();

            int result = client.UpdateUser(user);

            if (result > 0)
            {
                MessageBox.Show("Se ha actualizado la información del usuario correctamente.", "Operación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);

                if (areSingletonUserUpdated(user))
                {
                    updateSingleton(user);
                    MessageBox.Show("Ha actualizado su propia cuenta, será redirigido al inicio de sesión para cargar los cambios.", "Redireccionando a inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error al intentar actualizar la información del nuevo usuario. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateUserFields(string employeeNumber, string email, string password, string repeatPassword)
        {
            bool isStaffNumberValid = false;
            bool isEmailValid = false;
            bool isPasswordValid = false;

            if (!string.IsNullOrEmpty(employeeNumber.ToString()) && !string.IsNullOrEmpty(email))
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Hidden;

                isStaffNumberValid = ValidateEmployeeNumber(employeeNumber);
                isEmailValid = ValidateEmail(email);
                isPasswordValid = ValidatePassword(password, repeatPassword); 
            }
            else
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Visible;
            }

            return isStaffNumberValid && isEmailValid && isPasswordValid;
        }

        private bool ValidateEmployeeFields(string name, string middleName, string lastName, string city, string street, string number, string phoneNumber, string role, string locality)
        {
            bool areFieldsValid = false;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(middleName) && !string.IsNullOrEmpty(lastName) && !string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(street) 
                && !string.IsNullOrEmpty(number) && !string.IsNullOrEmpty(phoneNumber) && !string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(locality))
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Hidden;
                areFieldsValid = true;
            }
            else
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Visible;
            }

            return areFieldsValid;
        }

        public bool ValidateEmployeeNumber(string employeeNumber)
        {
            bool isEmployeeNumberValid = false;
            if (employeeNumber.Length == 8 && Regex.IsMatch(employeeNumber, "^[0-9]+$"))
            {
                SGPMService.UserManagementClient client = new SGPMService.UserManagementClient();
                
                if (this.employeeNumber > 0 || client.ValidateEmployeeNumberDoesNotExist(employeeNumber))
                {
                    isEmployeeNumberValid = true;            
                    lbInvalidEmployeeNumberMessage.Visibility = Visibility.Hidden;
                }
                else
                {
                    lbInvalidEmployeeNumberMessage.Content = "El número de empleado ya existe.";
                    lbInvalidEmployeeNumberMessage.Visibility = Visibility.Visible;
                }
            }
            else
            {
                isEmployeeNumberValid = false;
                lbInvalidEmployeeNumberMessage.Content = "Debe contener 10 dígitos.";
                lbInvalidEmployeeNumberMessage.Visibility = Visibility.Visible;
            }
            return isEmployeeNumberValid;
        }

        public bool ValidateEmail(string email)
        {
            bool isEmailValid = false;
            if ((Regex.IsMatch(email, "^[a-zA-Z0-9\\-_]{5,20}@(gob)\\.mx$")))
            {
                SGPMService.UserManagementClient client = new SGPMService.UserManagementClient();

                if (email.Equals(this.userEmail) || (client.ValidateEmailDoesNotExist(email)))
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

            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(repeatPassword))
            {
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
            }
            else
            {
                if (employeeNumber > 0)
                {
                    isPasswordValid = true;
                }
                else
                {
                    isPasswordValid = false;
                    lbEmptyFieldsMessage.Visibility = Visibility.Visible;
                }
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
            if (employeeNumber > 0)
            {
                Window usersListWindow = new GUI_UsersList();
                usersListWindow.Show();
                this.Close();
            }
            else
            {
                Window userMenuWindow = new GUI_UserMenu();
                userMenuWindow.Show();
                this.Close();
            }
            
        }

        private List<Locality> GetLocalitiesFromDatabase()
        {
            var localities = new List<Locality>();
            SGPMService.LocalityManagementClient client = new SGPMService.LocalityManagementClient();

            var localitiesFromDatabaseList = client.GetLocalities();

            foreach ( var locality in localitiesFromDatabaseList ) 
            { 
                localities.Add( locality ); 
            }

            return localities;
        }

        private void updateSingleton(User user)
        {
            SGMP_Client.DTO_s.User.UserClient.Logout();
            Window loginWindow = new GUI_Login();
            this.Close();
            loginWindow.Show();

        }

        private bool areSingletonUserUpdated(User user)
        {
            if (SGMP_Client.DTO_s.User.UserClient.EmployeeNumber.Equals(user.EmployeeNumber))
            {
                return true;
            }

            return false;
        }
    }
}
