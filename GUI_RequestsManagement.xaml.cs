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
    /// Lógica de interacción para GUI_RequestsManagement.xaml
    /// </summary>
    public partial class GUI_RequestsManagement : Window
    {

        private Project project;
        private SGPMReference.RequestManagementClient Client;
        private List<Request> Requests {  get; set; }

        public GUI_RequestsManagement(Project project)
        {
            InitializeComponent();
            this.project = project;
            Client = new SGPMReference.RequestManagementClient();
            Requests = new List<Request>();
            GetRequestsFromProyect();

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
