using Microsoft.Win32;
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
using System.IO;
using System.IO.Compression;
using System.Security.Principal;

namespace SGMP_Client
{
    /// <summary>
    /// Lógica de interacción para GUI_SubirEvidencia.xaml
    /// </summary>
    public partial class GUI_SubirEvidencia : Window
    {

        private List<string> EvidenceFiles = new List<string>();
        private SGPMService.EvidenceManagementClient EvidenceManagementClient = new SGPMService.EvidenceManagementClient();
        private int IdRequest; 
        private string IdProject;
        private Window ParentWindow;

        public GUI_SubirEvidencia(Window parent, int idRequest,string idProject)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            IdRequest = idRequest;
            IdProject = idProject;
            ParentWindow = parent;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveEvidence();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UploadFiles(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFileName = openFileDialog.FileName;
                FileInfo fileInfo = new FileInfo(selectedFileName);

                if (fileInfo.Length > 50 * 1024)
                {
                    MessageBox.Show("El archivo seleccionado es mayor a 50 KB. Por favor, seleccione un archivo más pequeño.", "Archivo demasiado grande", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    string selectedFilePath = openFileDialog.FileName;
                    EvidenceFiles.Add(selectedFilePath);
                    string fileName = System.IO.Path.GetFileName(selectedFilePath);
                    AddFileVisual(fileName);
                    btnSave.IsEnabled = true;
                }
            }
        }

        private void AddFileVisual(string file)
        {
            var border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1);
            border.CornerRadius = new CornerRadius(5);
            border.Padding = new Thickness(5);
            border.Margin = new Thickness(0, 0, 0, 10);

 
            var stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical; 

            var image = new Image();
            image.HorizontalAlignment = HorizontalAlignment.Left;
            image.Margin = new Thickness(5);
            image.Width = 50; 
            image.Height = 50; 
            image.Stretch = Stretch.Uniform; 
            image.Source = new BitmapImage(new System.Uri("Icons/solicitud.png", System.UriKind.Relative));

            var label = new Label();
            label.Content = file;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.Margin = new Thickness(5);
            label.FontWeight = FontWeights.Bold;
            label.FontSize = 10; 


            stackPanel.Children.Add(image);
            stackPanel.Children.Add(label);

            border.Child = stackPanel;

            WrpFileList.Children.Add(border);
        }

        private void SaveEvidence()
        {
            var result = 0;

            try
            {
                foreach (var file in EvidenceFiles)
                {
                    byte[] compressedFileBytes = CompressFile(file);
                    string fileName = System.IO.Path.GetFileName(file);
                    Evidence evidence = new Evidence
                    {
                        Name = IdRequest + IdProject + fileName,
                        IdRequest = IdRequest,
                        file = compressedFileBytes
                    };
                    result += EvidenceManagementClient.SaveEvidence(evidence);
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                ErroServer();
                return;
            }
            catch (TimeoutException ex)
            {
                ErroServer();
                return;
            }

            if (result == EvidenceFiles.Count())
            {
                MessageBox.Show("Archivos guardados correctamente");
                this.Close();
            }
        }

        private byte[] CompressFile(string filePath)
        {
            using (var originalFileStream = new FileStream(filePath, FileMode.Open))
            {
                using (var compressedFileStream = new MemoryStream())
                {
                    using (var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                    {
                        originalFileStream.CopyTo(compressionStream);
                    }
                    return compressedFileStream.ToArray();
                }
            }
        }

        private void ErroServer()
        {
            MessageBox.Show("Ha ocurrido un error al intentar conectarse al servidor, Intente nuevamente más tarde", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            GUI_Login login = new GUI_Login();
            ParentWindow.Close();
            this.Close();
            login.Show();
        }
    }
}
