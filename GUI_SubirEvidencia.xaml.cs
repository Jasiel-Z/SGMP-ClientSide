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

        public GUI_SubirEvidencia(int idRequest,string idProject)
        {
            InitializeComponent();
            IdRequest = idRequest;
            IdProject = idProject;
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
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            if (openFileDialog.ShowDialog() == true)
            {
                btnSave.IsEnabled = true;
                string selectedFilePath = openFileDialog.FileName;
                EvidenceFiles.Add(selectedFilePath);
                string fileName = System.IO.Path.GetFileName(selectedFilePath);
                AddFileVisual(fileName);
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
            foreach(var file in EvidenceFiles)
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
    }
}
