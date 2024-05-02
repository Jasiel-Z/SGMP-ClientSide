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
        private string idProject;
        public GUI_RegistroProyecto(string _idProject)
        {
            idProject = _idProject;
            InitializeComponent();

            Project project = ProjectsManagementClient.GetProjectDetails(idProject);

            if (project != null)
            {
                FillData(project);
                btnOrdenEntrega.Visibility = Visibility.Visible;
            }
            FillComboBoxes();
            dapcStart.SelectedDateChanged += ValidateStartDate;
            dapcEnd.SelectedDateChanged += ValidateEndDate;
            dapcSolicitud.SelectedDateChanged += ValidateSolicitudDate;
            dapcEvidencia.SelectedDateChanged += ValidateEvidenceDate;

            dapcEnd.IsEnabled = false;
            dapcSolicitud.IsEnabled = false;
            dapcEvidencia.IsEnabled = false;

            BlockDatePicker();
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

            cmbDependencia.ItemsSource = ProjectsManagementClient.GetDependencies();
            cmbLocalidad.ItemsSource = ProjectsManagementClient.GetLocalidads();


        }

        private void FillData(SGPMService.Project project)
        {
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
            cmbLocalidad.SelectedValue = project.Location;
            txtBeneficiarios.Text = project.BeneficiaryNumbers.ToString();
            txtAmount.Text = project.SupportAmount.ToString();
            dapcSolicitud.SelectedDate = project.Solicitud;
        }

        private void btnOrdenEntrega_Click(object sender, RoutedEventArgs e)
        {
            GUI_SaveDeliveryOrden deliveryOrden = new GUI_SaveDeliveryOrden(idProject);
            deliveryOrden.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GUI_ListaProyecto menu = new GUI_ListaProyecto();
            this.Close();
            menu.Show();
        }

        private void ValidateStartDate(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;

            if (datePicker.SelectedDate.HasValue)
            {
                dapcEnd.SelectedDate = null;
                dapcEvidencia.SelectedDate = null;
                dapcSolicitud.SelectedDate = null;

                dapcEnd.IsEnabled = true;
                dapcSolicitud.IsEnabled = false;
                dapcEvidencia.IsEnabled = false;

                DateTime selectedDate = datePicker.SelectedDate.Value;
                DateTime currentDate = DateTime.Today;         

                if (selectedDate < currentDate)
                {
                    MessageBox.Show("¡Error! Por favor, seleccione una fecha futura.");
                    datePicker.SelectedDate = null;
                    dapcEnd.IsEnabled = false;
                    dapcSolicitud.IsEnabled = false;
                    dapcEvidencia.IsEnabled = false;
                }

                CheckFieldsAndEnableSaveButton();
            }
        }


        private void ValidateEndDate(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePickerEnd = sender as DatePicker;

            if (datePickerEnd.SelectedDate.HasValue && dapcStart.SelectedDate.HasValue)
            {
                DateTime selectedDateEnd = datePickerEnd.SelectedDate.Value;
                DateTime selectedDateStart = dapcStart.SelectedDate.Value;

                dapcEvidencia.SelectedDate = null;
                dapcSolicitud.SelectedDate = null;

                dapcSolicitud.IsEnabled = true;
                dapcEvidencia.IsEnabled = true;

                if (selectedDateEnd <= selectedDateStart)
                {
                    MessageBox.Show("¡Error! La fecha de finalización debe ser posterior a la fecha de inicio.");
                    datePickerEnd.SelectedDate = null;
                    dapcSolicitud.IsEnabled = false;
                    dapcEvidencia.IsEnabled = false;
                }

                CheckFieldsAndEnableSaveButton();
            }
        }
        
        private void ValidateSolicitudDate(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePickerSolicitud = sender as DatePicker;

            if (dapcStart.SelectedDate.HasValue && dapcEnd.SelectedDate.HasValue && datePickerSolicitud.SelectedDate.HasValue)
            {
                DateTime selectedDateStart = dapcStart.SelectedDate.Value;
                DateTime selectedDateEnd = dapcEnd.SelectedDate.Value;
                DateTime selectedDateSolicitud = datePickerSolicitud.SelectedDate.Value;

                if (selectedDateSolicitud < selectedDateStart || selectedDateSolicitud > selectedDateEnd)
                {
                    MessageBox.Show("¡Error! La fecha de solicitud debe estar dentro del rango entre la fecha de inicio y la fecha de fin.");
                    datePickerSolicitud.SelectedDate = null;
                }

                CheckFieldsAndEnableSaveButton();
            }
        }
        
        private void ValidateEvidenceDate(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePickerEvidencia = sender as DatePicker;

            if (dapcEnd.SelectedDate.HasValue && datePickerEvidencia.SelectedDate.HasValue)
            {
                DateTime selectedDateEnd = dapcEnd.SelectedDate.Value;
                DateTime selectedDateEvidencia = datePickerEvidencia.SelectedDate.Value;

                if (selectedDateEvidencia <= selectedDateEnd)
                {
                    MessageBox.Show("¡Error! La fecha de evidencia debe ser posterior a la fecha de fin.");
                    datePickerEvidencia.SelectedDate = null;
                }

                CheckFieldsAndEnableSaveButton();
            }
        }

        private void BlockDatePicker()
        {
            dapcStart.PreviewTextInput += (sender, e) =>
            {
                e.Handled = true;
            };
            dapcStart.PreviewKeyDown += (sender, e) =>
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    e.Handled = true;
                }
            };

            dapcEnd.PreviewTextInput += (sender, e) =>
            {
                e.Handled = true;
            };
            dapcEnd.PreviewKeyDown += (sender, e) =>
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    e.Handled = true;
                }
            };

            dapcEvidencia.PreviewTextInput += (sender, e) =>
            {
                e.Handled = true;
            };
            dapcEvidencia.PreviewKeyDown += (sender, e) =>
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    e.Handled = true;
                }
            };

            dapcSolicitud.PreviewTextInput += (sender, e) =>
            {
                e.Handled = true;
            };
            dapcSolicitud.PreviewKeyDown += (sender, e) =>
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    e.Handled = true;
                }
            };

        }

        private void CheckFieldsAndEnableSaveButton()
        {
            bool allFieldsFilled = !string.IsNullOrWhiteSpace(txtName.Text) &&
                                   !string.IsNullOrWhiteSpace(txtFolio.Text) &&
                                   dapcStart.SelectedDate.HasValue &&
                                   dapcEnd.SelectedDate.HasValue &&
                                   cmbEstado.SelectedItem != null &&
                                   cmbLocalidad.SelectedItem != null &&
                                   cmbDependencia.SelectedItem != null &&
                                   cmbModalidad.SelectedItem != null &&
                                   !string.IsNullOrWhiteSpace(tbxDescription.Text) &&
                                   cmbGrupoAtencion.SelectedItem != null &&
                                   dapcSolicitud.SelectedDate.HasValue &&
                                   !string.IsNullOrWhiteSpace(txtAmount.Text) &&
                                   !string.IsNullOrWhiteSpace(txtBeneficiarios.Text);

            btnSave.IsEnabled = allFieldsFilled;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFieldsAndEnableSaveButton();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckFieldsAndEnableSaveButton();
        }

    }
}
