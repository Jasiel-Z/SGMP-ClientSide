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

        SGPMManagerService.UserManagementClient Client { get; set; }
        public GUI_Login()
        {
            InitializeComponent();

           Client = new SGPMManagerService.UserManagementClient();

        }

        private void Btn_Login(object sender, RoutedEventArgs e)
        {
           bool validData = ValidateFields();
            if (validData)
            {
                Client = new SGPMManagerService.UserManagementClient();

                String hashedPassword = Utility.ComputeSha256Hash(pb_password.Password);
                SGPMManagerService.User user = Client.GetUser(tb_email.Text, hashedPassword);

                if (user != null)
                {
                    MessageBox.Show("Inicio de sesión exitoso", "Puede ingresar al sistema");

                    DTO_s.User.UserClient = new DTO_s.User()
                    {
                        UserId = user.UserId,

                        EmployerId = user.EmployeeNumber,
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
