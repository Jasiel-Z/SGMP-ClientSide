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
        SGPMReference.ProjectsManagementClient client;
        public SGMP_Client.SGPMReference.Project Project { get; set; }

        public SGMP_Client.SGPMReference.Person Person { get; set; }
        public SGMP_Client.SGPMReference.Company Company { get; set; }

        public SGPMReference.Request Request { get; set; }

        public SGPMReference.Beneficiary Beneficiary { get; set; }

        public List<SGMP_Client.SGPMReference.ProjectPolicy> ProjectPolicies { get; set; }


        public GUI_GenerateOpinion()
        {
            InitializeComponent();

            Files = new List<SGPMReference.File>();
            client = new SGPMReference.ProjectsManagementClient();
            Project = new Project();
            GetProyectDetails();
            GetPolicies();
            GetFiles();
            GetRequest();



        }




        #region Recovery data
        private void GetProyectDetails()
        {

            Project = client.GetProjectDetails(7);
            tb_modality.Text = Project.Modality;
            tb_group.Text = Project.AttentionGroup;
            tb_type.Text = Project.Type;


        }

        private void GetBeneficiaryDetails()
        {

        }


        private void GetPolicies()
        {
            ProjectPolicies = client.GetProjectPolicies(Project.Folio).ToList();
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
            SGPMReference.RequestManagementClient client = new SGPMReference.RequestManagementClient();
            Request = client.RecoverRequestDetails(30);

            //client = new BeneficiaryManagementClient();
            //Beneficiary = client.Ge
        }


        private void GetFiles()
        {
            SGPMReference.RequestManagementClient client = new RequestManagementClient();
            Files = client.GetRequestFiles(30).ToList();
            foreach (var file in Files)
            {
                // Crea un ListBoxItem para cada archivo
                var listBoxItem = new ListBoxItem();

                // Asigna el objeto File como DataContext del ListBoxItem
                listBoxItem.DataContext = file;

                // Agrega un botón de descarga al ListBoxItem
                var downloadButton = new Button();
                downloadButton.Content = "Descargar";
                downloadButton.Click += (sender, e) =>
                {
                    // Maneja el evento Click del botón de descarga para realizar la descarga del archivo
                    var selectedFile = (sender as Button).DataContext as File;
                    if (selectedFile != null)
                    {
                        DownloadFile(selectedFile);
                    }
                };

                // Agrega el botón de descarga al ListBoxItem
                listBoxItem.Content = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    Children =
            {
                new TextBlock() { Text = file.Name, Margin = new Thickness(5) },
                downloadButton
            }
                };

                // Agrega el ListBoxItem al ListBox
                lib_files.Items.Add(listBoxItem);
            }
        }



        private void DownloadFile(File selectedFile)
        {
            // Obtén la ruta del archivo original desde el atributo Description del archivo
            string rutaArchivoOriginal = selectedFile.Description;

            // Mostrar el diálogo de selección de directorio personalizado
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
            dialogo.FileName = "Folder Selection"; // Nombre ficticio para que funcione el diálogo de selección de carpeta

            // Muestra el cuadro de diálogo y devuelve la ruta seleccionada si el usuario hace clic en "Aceptar"
            if (dialogo.ShowDialog() == true)
            {
                return System.IO.Path.GetDirectoryName(dialogo.FileName);
            }

            return null; 
        }
    }
}
