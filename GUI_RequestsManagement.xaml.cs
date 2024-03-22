using SGMP_Client.SGPMManagerService;
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
        private SGPMManagerService.RequestManagementClient Client;
        private List<Request> Requests {  get; set; }

        public GUI_RequestsManagement(Project project)
        {
            InitializeComponent();
            this.project = project;
            Client = new SGPMManagerService.RequestManagementClient();
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
                MessageBox.Show("Por favor selecciona el registro de la tabla y luego presiona el botón", "No se ha seleccionado " +
                    "un proyecto");
            }

        }

        private void GetRequestsFromProyect()
        {

            int folio = project.Folio;
            Requests = Client.GetRequestsOfProject(folio).ToList();

            if(Requests != null)
            {
                foreach (Request request in Requests)
                {
                    MessageBox.Show(request.State);
                    liv_requests.Items.Add(request);
                }
            }
            else
            {
                MessageBox.Show("Errro");
            }
        }
    }
}
