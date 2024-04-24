using System;
using System.Collections.Generic;
using System.Linq;
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

        }

        private void Btn_Login(object sender, RoutedEventArgs e)
        {
            bool validData = ValidateFields();
            if (validData)
            {
                Client = new SGPMService.UserManagementClient();

                String hashedPassword = Utility.ComputeSha256Hash(pb_password.Password);
                SGPMService.User user = Client.GetUser(tb_email.Text, hashedPassword);

                if (user != null)
                {
                    MessageBox.Show("Inicio de sesión exitoso", "Puede ingresar al sistema");

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
                else{
                    MessageBox.Show("No se encontró el usuario");
                }
            }


        }


        private bool ValidateFields()
        {
            bool result = true;

            if (tb_email.Text == "")
            {
                MessageBox.Show("Ingresa todos los datos","Datos faltantes");

            }
            else
            { 
                result = true;
                return result;
            }


            return result;
        }

    }
}
