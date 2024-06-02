using SGMP_Client.SGPMService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
        private User userToBeModified = new User();

        public GUI_User()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

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
            this.userToBeModified = user;

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

                if (string.IsNullOrWhiteSpace(password))
                {
                    user.Password = "";
                }
                else
                {
                    user.Password = Utility.ComputeSha256Hash(password);
                }

                if (userToBeModified.EmployeeNumber == 0)
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
            try
            {
                SGPMService.UserManagementClient client = new SGPMService.UserManagementClient();
                int result = client.SaveUser(user);

                if (result > 0)
                {
                    MessageBox.Show("Se ha guardado el nuevo usuario correctamente.", "Operación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);

                    Window userMenuWindow = new GUI_UserMenu();
                    userMenuWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Ha ocurrido un error al intentar guardar el nuevo usuario. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("Ha ocurrido un error al conectar con el Servidor. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);

                SGMP_Client.DTO_s.User.UserClient.Logout();
                Window loginWindow = new GUI_Login();
                this.Close();
                loginWindow.Show();
            }
        }

        private void UpdateUser(User user) 
        {
            try
            {
                SGPMService.UserManagementClient client = new SGPMService.UserManagementClient();
                int result = 0;

                if (!user.Name.Equals(userToBeModified.Name) || !user.MiddleName.Equals(userToBeModified.MiddleName) || !user.LastName.Equals(userToBeModified.LastName) ||
                    !user.City.Equals(userToBeModified.City) || !user.Street.Equals(userToBeModified.Street) || user.Number != userToBeModified.Number ||
                    !user.PhoneNumber.Equals(userToBeModified.PhoneNumber) || !user.Role.Equals(userToBeModified.Role) || !user.Email.Equals(userToBeModified.Email) ||
                    !string.IsNullOrWhiteSpace(pwbPassword.Password))
                {
                    result = client.UpdateUser(user);

                    if (result > 0)
                    {
                        MessageBox.Show("Se ha actualizado la información del usuario correctamente.", "Operación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);

                        if (areSingletonUserUpdated(user))
                        {
                            updateSingleton(user);
                            MessageBox.Show("Ha actualizado su propia cuenta, será redirigido al inicio de sesión para cargar los cambios.", "Redireccionando a inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            Window usersListWindow = new GUI_UsersList();
                            usersListWindow.Show();
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ha ocurrido un error al intentar actualizar la información del nuevo usuario. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Modifique alguno de los campos antes de continuar.", "Error en la operación", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("Ha ocurrido un error al conectar con el Servidor. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);

                SGMP_Client.DTO_s.User.UserClient.Logout();
                Window loginWindow = new GUI_Login();
                this.Close();
                loginWindow.Show();
            }
        }

        private bool ValidateUserFields(string employeeNumber, string email, string password, string repeatPassword)
        {
            bool isStaffNumberValid = false;
            bool isEmailValid = false;
            bool isPasswordValid = false;

            if (!string.IsNullOrWhiteSpace(employeeNumber.ToString()) && !string.IsNullOrWhiteSpace(email))
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

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(middleName) && !string.IsNullOrWhiteSpace(lastName) && !string.IsNullOrWhiteSpace(city) && !string.IsNullOrWhiteSpace(street) 
                && !string.IsNullOrWhiteSpace(number) && !string.IsNullOrWhiteSpace(phoneNumber) && !string.IsNullOrWhiteSpace(role) && !string.IsNullOrWhiteSpace(locality))
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
            if ((employeeNumber.Length == 8 && Regex.IsMatch(employeeNumber, "^[0-9]+$")) || userToBeModified.EmployeeNumber > 0)
            {
                try
                {
                    SGPMService.UserManagementClient client = new SGPMService.UserManagementClient();

                    if (userToBeModified.EmployeeNumber > 0 || client.ValidateEmployeeNumberDoesNotExist(employeeNumber))
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
                catch (EndpointNotFoundException)
                {
                    MessageBox.Show("Ha ocurrido un error al conectar con el Servidor. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    SGMP_Client.DTO_s.User.UserClient.Logout();
                    Window loginWindow = new GUI_Login();
                    this.Close();
                    loginWindow.Show();
                }
            }
            else
            {
                isEmployeeNumberValid = false;
                lbInvalidEmployeeNumberMessage.Content = "Debe contener 8 dígitos.";
                lbInvalidEmployeeNumberMessage.Visibility = Visibility.Visible;
            }
            return isEmployeeNumberValid;
        }

        public bool ValidateEmail(string email)
        {
            bool isEmailValid = false;
            if ((Regex.IsMatch(email, "^[a-zA-Z0-9\\-_]{5,20}@(gob)\\.mx$")))
            {
                try
                {
                    SGPMService.UserManagementClient client = new SGPMService.UserManagementClient();

                    if (email.Equals(userToBeModified.Email) || (client.ValidateEmailDoesNotExist(email)))
                    {
                        isEmailValid = true;
                        lbInvalidEmailMessage.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        isEmailValid = false;
                        lbInvalidEmailMessage.Content = "El correo electrónico ingresado ya está vinculado a una cuenta.";
                        lbInvalidEmailMessage.Visibility = Visibility.Visible;
                    }
                }
                catch (EndpointNotFoundException)
                {
                    MessageBox.Show("Ha ocurrido un error al conectar con el Servidor. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    SGMP_Client.DTO_s.User.UserClient.Logout();
                    Window loginWindow = new GUI_Login();
                    this.Close();
                    loginWindow.Show();
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

            if (!string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(repeatPassword))
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
                if (userToBeModified.EmployeeNumber > 0)
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
            if (userToBeModified.EmployeeNumber > 0)
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

            try
            {
                SGPMService.LocalityManagementClient client = new SGPMService.LocalityManagementClient();

                var localitiesFromDatabaseList = client.GetLocalities();

                foreach (var locality in localitiesFromDatabaseList)
                {
                    localities.Add(locality);
                }
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("Ha ocurrido un error al conectar con el Servidor. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);

                SGMP_Client.DTO_s.User.UserClient.Logout();
                Window loginWindow = new GUI_Login();
                this.Close();
                loginWindow.Show();
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
