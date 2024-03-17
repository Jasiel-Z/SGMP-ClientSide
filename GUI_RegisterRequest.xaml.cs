using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace SGMP_Client
{
    /// <summary>
    /// Lógica de interacción para GUI_RegisterRequest.xaml
    /// </summary>
    public partial class GUI_RegisterRequest : Window
    {

        public ObservableCollection<string> AttachFiles { get; set; }

        public GUI_RegisterRequest()
        {
            InitializeComponent();
            AttachFiles = new ObservableCollection<string>();  
            lib_files.ItemsSource = AttachFiles;
        }


        private void Btn_Attach_File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Archivos PDF (*.pdf)|*.pdf";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    string fileNameOnly = System.IO.Path.GetFileName(fileName); // Obtener solo el nombre del archivo
                    AttachFiles.Add(fileNameOnly);
                }
            }
        }

        private void AgregarArchivoAdjunto(string nombreArchivo)
        {
            AttachFiles.Add(nombreArchivo);
            lib_files.UpdateLayout();
        }

        private void Btn_Cancel_Request_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_Remove_File_Click(object sender, RoutedEventArgs e)
        {
            if (lib_files.SelectedItem != null)
            {
                AttachFiles.Remove(lib_files.SelectedItem.ToString());
            }
        }
    }
}
