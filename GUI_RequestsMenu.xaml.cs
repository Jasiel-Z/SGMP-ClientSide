using SGMP_Client.SGPMService;
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
    /// Lógica de interacción para GUI_RequestsMenu.xaml
    /// </summary>
    public partial class GUI_RequestsMenu : Window
    {

        private SGPMService.ProjectsManagementClient Client;
        private List<Project> Projects { get; set; }  

        public GUI_RequestsMenu()
        {
            InitializeComponent();
            Client = new SGPMService.ProjectsManagementClient();
            Projects = new List<Project>();
            GetProjectsFromLocality();

        }



        private void Btn_See_Requests(object sender, RoutedEventArgs e)
        {
            Project selectedProject = (Project)lib_proyects.SelectedItem;
            if(selectedProject != null)
            {
                GUI_RequestsManagement requestsManagement = new GUI_RequestsManagement(selectedProject);
                requestsManagement.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor selecciona el registro de la tabla y luego presiona el botón", "No se ha seleccionado " +
                    "un proyecto", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }


        }


         private void Btn_Go_MainMenu(object sender, RoutedEventArgs e)
        {
            GUI_MainMenu mainMenu = new GUI_MainMenu();
            mainMenu.Show();
            this.Close();   
        }


        private void GetProjectsFromLocality()
        {
            int locationId = DTO_s.User.UserClient.LocationId;
            try
            {
                Projects = Client.GetProjectsFromLocality(locationId).ToList();

                if (Projects != null)
                {
                    foreach (Project project in Projects)
                    {
                        lib_proyects.Items.Add(project);

                    }

                }
                else
                {
                    MessageBox.Show("No se pudieron recuperar los registros, por favor inténtelo más tarde"
                        , "Error de conexión con la base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (TimeoutException ex )
            {
                MessageBox.Show("No fue posible establecer conexión con el servidor, por favor inténtelo más tarde",
                    "Problema de conexión con el servidor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
 

        }
    }
}
