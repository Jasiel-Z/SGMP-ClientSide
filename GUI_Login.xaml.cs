using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
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
    /// Lógica de interacción para GUI_Login.xaml
    /// </summary>
    public partial class GUI_Login : Window
    {

        SGPMService.UserManagementClient Client { get; set; }
        public GUI_Login()
        {
           InitializeComponent();
           Client = new SGPMService.UserManagementClient();
            this.KeyDown += GUI_Login_KeyDown;
        }

        private void GUI_Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Btn_Login(sender, e);
            }
        }

        private void Btn_Login(object sender, RoutedEventArgs e)
        {
            bool validData = ValidateFields();
            if (validData)
            {
                try
                {
                    Client = new SGPMService.UserManagementClient();

                    String hashedPassword = Utility.ComputeSha256Hash(pb_password.Password);
                    SGPMService.User user = Client.GetUser(tb_email.Text, hashedPassword);

                    if (user != null)
                    {
                        MessageBox.Show("Inicio de sesión exitoso", "Puede ingresar al sistema", MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        DTO_s.User.UserClient = new DTO_s.User()
                        {
                            UserId = user.UserId,
                            Email = user.Email,
                            Password = user.Password,

                            EmployeeNumber = user.EmployeeNumber,
                            Name = user.Name,
                            MiddleName = user.MiddleName,
                            LastName = user.LastName,
                            Role = user.Role,
                            PhoneNumber = user.PhoneNumber,
                            City = user.City,
                            Street = user.Street,
                            Number = user.Number,
                            LocationId = user.LocationId,
                        };

                        GUI_MainMenu mainMenu = new GUI_MainMenu();
                        mainMenu.Show();
                        this.Close();

                    }
                    else
                    {
                        MessageBox.Show("El correo o contraseña ingresados son incorrectos, por favor inténtelo de nuevo",
                            "Problema de autenticación", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No fue posible establecer conexión con el servidor, por favor inténtelo más tarde",
                 "Problema de conexión con el servidor", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }


        }


        private bool ValidateFields()
        {
            bool result = true;

            if (string.IsNullOrEmpty(tb_email.Text)  || 
                string.IsNullOrEmpty(pb_password.Password))
            {
                MessageBox.Show("Ingresa todos los datos","Datos faltantes", 
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }
            else
            { 
                result = true;
                return result;
            }


            return result;
        }

        private void Btn_Exit(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
