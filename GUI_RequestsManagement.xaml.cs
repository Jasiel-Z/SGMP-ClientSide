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
    /// Lógica de interacción para GUI_RequestsManagement.xaml
    /// </summary>
    public partial class GUI_RequestsManagement : Window
    {

        private Project project;
        private SGPMService.RequestManagementClient Client;
        private List<Request> Requests {  get; set; }

        public GUI_RequestsManagement(Project project)
        {
            InitializeComponent();
            this.project = project;
            Client = new SGPMService.RequestManagementClient();
            Requests = new List<Request>();
            GetRequestsFromProyect();
            lb_proyect_name.Content = project.Name;


        }

        private void Btn_Register_Request_Click(object sender, RoutedEventArgs e)
        {
            GUI_RegisterRequest registerRequest = new GUI_RegisterRequest(project);
            registerRequest.Show();
            this.Close();
        }

        private void Btn_Goback_Click(object sender, RoutedEventArgs e)
        {
            GUI_RequestsMenu requestsMenu = new GUI_RequestsMenu(); 
            requestsMenu.Show();
            this.Close();
        }


        private void Btn_Go_Opinion_Click(object sender, RoutedEventArgs e)
        {
            Request selectedRequest = (Request)liv_requests.SelectedItem;
            if (selectedRequest != null)
            {
                GUI_GenerateOpinion requestsManagement = new GUI_GenerateOpinion(project,selectedRequest);
                requestsManagement.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor selecciona el registro de la tabla y luego presiona el botón", 
                    "No se ha seleccionado " +
                    "un proyecto", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        private void GetRequestsFromProyect()
        {

            string folio = project.Folio;
            try
            {
                Requests = Client.GetRequestsOfProject(folio).ToList();

                if (Requests != null)
                {
                    foreach (Request request in Requests)
                    {
                        liv_requests.Items.Add(request);
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
