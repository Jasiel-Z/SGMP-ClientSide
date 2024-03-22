using SGMP_Client.SGPMReference;
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

        private SGPMReference.ProjectsManagementClient Client;
        private List<Project> Projects { get; set; }  

        public GUI_RequestsMenu()
        {
            InitializeComponent();
            Client = new SGPMReference.ProjectsManagementClient();
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
                    "un proyecto");
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
            Projects = Client.GetProjectsFromLocality(locationId).ToList();

            if(Projects != null)
            {
                foreach(Project project in Projects)
                {
                    MessageBox.Show(project.ToString());
                    lib_proyects.Items.Add(project);

                }

            }

        }
    }
}
