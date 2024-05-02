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
    /// Lógica de interacción para GUI_RegistroProyecto.xaml
    /// </summary>
    public partial class GUI_RegistroProyecto : Window
    {

        private SGPMService.ProjectsManagementClient ProjectsManagementClient = new SGPMService.ProjectsManagementClient();
        private string idProject = "FC12345";
        public GUI_RegistroProyecto()
        {
            
            InitializeComponent();

            Project project = ProjectsManagementClient.GetProjectDetails(idProject);

            if (project != null)
            {
                FillData(project);
                btnOrdenEntrega.Visibility = Visibility.Visible;
            }

            cmbDependencia.ItemsSource = ProjectsManagementClient.GetDependencies();
            cmbLocalidad.ItemsSource = ProjectsManagementClient.GetLocalidads();
            FillComboBoxes();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            SGPMService.Project project = new SGPMService.Project();
            project.AttentionGroup = (string)cmbGrupoAtencion.SelectedValue;
            project.Start = (DateTime)dapcStart.SelectedDate;
            project.Folio = txtFolio.Text;
            project.Name = txtName.Text.Trim();
            project.Description = tbxDescription.Text.Trim();
            project.Status = (string)cmbEstado.SelectedValue;
            project.Modality = (string)cmbModalidad.SelectedValue;
            project.End = (DateTime)dapcEnd.SelectedDate;
            project.Evidence = (DateTime)dapcEvidencia.SelectedDate;
            project.Dependecy = (int)cmbDependencia.SelectedValue;
            project.Location = (int)cmbLocalidad.SelectedValue;
            project.BeneficiaryNumbers = int.Parse(txtBeneficiarios.Text.Trim());
            project.SupportAmount = int.Parse(txtAmount.Text);
            project.Solicitud = (DateTime)dapcSolicitud.SelectedDate;

            ProjectsManagementClient.RegisteredProjects(project);

            GUI_ListaProyecto lista = new GUI_ListaProyecto();
            this.Close();
            lista.Show();
        }

        private ProjectsManagementClient GetProjectsManagementClient()
        {
            return ProjectsManagementClient;
        }

        private void FillComboBoxes()
        {
            List<string> estados = new List<string>();
            estados.Add("Activo");
            estados.Add("Finalizado");
            estados.Add("Por Comenzar");

            cmbEstado.ItemsSource = estados;

            List<string> modalidad = new List<string>();
            modalidad.Add("Comida");
            modalidad.Add("Beca");
            modalidad.Add("Construccion");

            cmbModalidad.ItemsSource = modalidad;

            List<String> grupoAtencion = new List<String>();
            grupoAtencion.Add("Todos");
            grupoAtencion.Add("Alumnos");
            grupoAtencion.Add("Personas Mayores");

            cmbGrupoAtencion.ItemsSource = grupoAtencion;

            var dependencies = ProjectsManagementClient.GetDependencies();

        }

        private void FillData(SGPMService.Project project)
        {
            int IdLocalidad = project.Location;
            cmbGrupoAtencion.SelectedValue = project.AttentionGroup;
            dapcStart.SelectedDate = project.Start;
            txtFolio.Text = project.Folio;
            txtName.Text = project.Name;
            tbxDescription.Text = project.Description;
            cmbEstado.SelectedValue = project.Status;
            cmbModalidad.SelectedValue = project.Modality;
            dapcEnd.SelectedDate = project.End;
            dapcEvidencia.SelectedDate = project.Evidence;
            cmbDependencia.SelectedValue = project.Dependecy;
            cmbLocalidad.SelectedValue = IdLocalidad;
            txtBeneficiarios.Text = project.BeneficiaryNumbers.ToString();
            txtAmount.Text = project.SupportAmount.ToString();
            dapcSolicitud.SelectedDate = project.Solicitud;
        }

        private void btnOrdenEntrega_Click(object sender, RoutedEventArgs e)
        {
            GUI_SaveDeliveryOrden deliveryOrden = new GUI_SaveDeliveryOrden(idProject);
            deliveryOrden.Show();
        }
    }
}
