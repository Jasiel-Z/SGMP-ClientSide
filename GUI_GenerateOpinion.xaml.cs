using Microsoft.Win32;
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
    /// Lógica de interacción para GUI_GenerateOpinion.xaml
    /// </summary>
    public partial class GUI_GenerateOpinion : Window
    {

        public List<SGPMReference.File> Files { get; set; }
        SGPMReference.ProjectsManagementClient Client;
        private Project project;

        public SGMP_Client.SGPMReference.Person Person { get; set; }
        public SGMP_Client.SGPMReference.Company Company { get; set; }

        private Request request;

        public SGPMReference.Beneficiary Beneficiary { get; set; }

        public List<SGMP_Client.SGPMReference.ProjectPolicy> ProjectPolicies { get; set; }


        public GUI_GenerateOpinion(Project project, Request request)
        {
            InitializeComponent();

            Files = new List<SGPMReference.File>();
            Client = new SGPMReference.ProjectsManagementClient();
            this.project = project;
            this.request = request;
            GetProyectDetails();

            GetPolicies();
            GetFiles();
            GetRequest();

        }

        #region Recovery data
        private void GetProyectDetails()
        {

            tb_description.Text = project.Description;
            tb_group.Text = project.AttentionGroup;
            tb_type.Text = project.Type;


        }

        private void GetBeneficiaryDetails()
        {

        }


        private void GetPolicies()
        {
            ProjectPolicies = Client.GetProjectPolicies(project.Folio).ToList();
            foreach (var policy in ProjectPolicies)
            {
                var listBoxItem = new ListBoxItem();
                listBoxItem.Content = policy;

                var checkBox = new CheckBox();
                checkBox.Content = "Cumple";
                checkBox.IsChecked = false;

                checkBox.Checked += (sender, e) =>
                {
                    var isChecked = (sender as CheckBox).IsChecked;
                    var currentPolicy = listBoxItem.Content as ProjectPolicy;
                    if (isChecked != null && currentPolicy != null)
                    {
                    }
                };

                listBoxItem.Content = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    Children =
            {
                new TextBlock() { Text = policy.Name, Margin = new Thickness(5) },
                new TextBlock() { Text = policy.Description, Margin = new Thickness(5) },
                checkBox
            }
                };

                lib_policies.Items.Add(listBoxItem);
            }

        }


        private void GetRequest()
        {
            DateTime requestDate = request.CreationTime;
            string requestDateString = requestDate.ToString("yyyy-MM-dd HH:mm:ss");
            tb_request_date.Text = requestDateString;
            tb_folio.Text = request.ProyectFolio.ToString();
            tb_state.Text = request.State;


        }


        private void GetFiles()
        {
            SGPMReference.RequestManagementClient client = new RequestManagementClient();
            Files = client.GetRequestFiles(request.Id).ToList();
            foreach (var file in Files)
            {
                var listBoxItem = new ListBoxItem();

                listBoxItem.DataContext = file;

                var downloadButton = new Button();
                downloadButton.Content = "Descargar";
                downloadButton.Click += (sender, e) =>
                {
                    var selectedFile = (sender as Button).DataContext as File;
                    if (selectedFile != null)
                    {
                        DownloadFile(selectedFile);
                    }
                };

                listBoxItem.Content = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    Children =
            {
                new TextBlock() { Text = file.Name, Margin = new Thickness(5) },
                downloadButton
            }
                };

                lib_files.Items.Add(listBoxItem);
            }
        }



        private void DownloadFile(File selectedFile)
        {
            string rutaArchivoOriginal = selectedFile.Description;

            string directorioDestino = SeleccionarDirectorio();

            if (directorioDestino != null)
            {
                try
                {
                    string nombreArchivoOriginal = System.IO.Path.GetFileName(rutaArchivoOriginal);
                    string rutaArchivoDestino = System.IO.Path.Combine(directorioDestino, nombreArchivoOriginal);

                    System.IO.File.Copy(rutaArchivoOriginal, rutaArchivoDestino);

                    MessageBox.Show($"Archivo descargado exitosamente en {rutaArchivoDestino}", "Descarga Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al descargar el archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            #endregion
        }


        private string SeleccionarDirectorio()
        {
            OpenFileDialog dialogo = new OpenFileDialog();
            dialogo.Title = "Seleccione una carpeta";
            dialogo.Filter = "Carpeta|*.*";
            dialogo.ValidateNames = false;
            dialogo.CheckFileExists = false;
            dialogo.CheckPathExists = true;
            dialogo.FileName = "Folder Selection"; 

            if (dialogo.ShowDialog() == true)
            {
                return System.IO.Path.GetDirectoryName(dialogo.FileName);
            }

            return null; 
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult cancelationResult = MessageBox.Show("¿Estás seguro de que deseas cancelar?",
            "Confirmar cancelación", MessageBoxButton.OKCancel);

            if (cancelationResult == MessageBoxResult.OK)
            {
                GUI_RequestsManagement requestsManagement = new GUI_RequestsManagement(project);
                requestsManagement.Show();
                this.Close();
            }
        }

        private void Btn_Register_Opinion(object sender, RoutedEventArgs e)
        {
            if (ValidateData())
            {
                if (IsOptionSelected())
                {
                    MessageBoxResult confirmation = MessageBox.Show("¿Estás seguro de enviar el dictamen de la solicitud?",
                    "Confirmación de registro", MessageBoxButton.OKCancel);


                    bool solution = DeterminateSolution();
                    string state = null;

                    if (solution)
                    {
                        state = "aceptado";
                    }
                    else
                    {
                        state = "rechazado";
                    }

                    if (confirmation == MessageBoxResult.OK)
                    {

                        Opinion opinion = new Opinion
                        {
                            State = state,
                            Comment = tb_comments.Text,
                            Date = DateTime.Now,
                            EmployeeNumber = DTO_s.User.UserClient.EmployerId,
                        
                        };


                        SGPMReference.RequestManagementClient client = new SGPMReference.RequestManagementClient();
                        int result = client.RegisterOpinion(opinion, request.Id);
                        if (result >= 1)
                        {
                            MessageBox.Show("Registro realizado con éxito");
                            GUI_RequestsManagement requestsManagement = new GUI_RequestsManagement(project);
                            requestsManagement.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Ocurrió un problema con la base de datos, por favor inténtelo más tarde",
                                "Problema de conexión con la base de datos");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Por favor selecciona una opción para el cumplimiento de las políticas", "Datos faltantes");
                }
            }

        }


        private bool ValidateData()
        {
            bool valid = true;

            if(tb_comments.Text == "")
            {
                valid = false;
                MessageBox.Show("Un dictamen debe tener comentarios, por favor ingrese uno", "Datos faltantes");
            }
            return valid;
        }


        private bool DeterminateSolution()
        {
            bool acceptedRequest = false;


            if(rb_accomplish.IsChecked == true)
            {
                acceptedRequest = true;
            }

            return acceptedRequest;
 
        }


        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;

            if (radioButton == rb_accomplish)
            {
                rb_no_accomplish.IsChecked = false;
            }
            else if (radioButton == rb_no_accomplish)
            {
                rb_accomplish.IsChecked = false;
            }
        }

        private bool IsOptionSelected()
        {
            return rb_accomplish.IsChecked == true || rb_no_accomplish.IsChecked == true;
        }
    }
}
