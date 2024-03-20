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
using System.Runtime.CompilerServices;
using SGMP_Client.SGPMReference;
using SGMP_Client.DTO_s;

namespace SGMP_Client
{
    /// <summary>
    /// Lógica de interacción para GUI_RegisterRequest.xaml
    /// </summary>
    public partial class GUI_RegisterRequest : Window
    {

        public ObservableCollection<string> AttachFiles { get; set; }
        public List<SGPMReference.ArchivoSet> Files { get; set; }
        SGPMReference.ProjectsManagementClient client;
        public SGMP_Client.SGPMReference.Project project { get; set; }

        public GUI_RegisterRequest()
        {
            InitializeComponent();
            AttachFiles = new ObservableCollection<string>();  
            Files = new List<SGPMReference.ArchivoSet>();
            lib_files.ItemsSource = AttachFiles;
            client = new SGPMReference.ProjectsManagementClient();
            project = new Project();
            GetProyectDetails();
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
                    string fileNameOnly = System.IO.Path.GetFileName(fileName);
                    AttachFiles.Add(fileNameOnly);

                

                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileName);
                    long fileSize = fileBytes.Length;

                    // Mostrar el tamaño del archivo
                    MessageBox.Show($"Tamaño del archivo '{fileNameOnly}': {fileSize} bytes");


                    SGPMReference.ArchivoSet file = new SGPMReference.ArchivoSet
                    {
                        nombre = fileNameOnly,
                        contenido = fileBytes,
                        extension = System.IO.Path.GetExtension(fileName),
                        SolicitudIdSolicitud = 1,
                        descripcion = "Prueba 1"
                    };




                    if (file.contenido != null)
                    {
                        MessageBox.Show("Hay contenido en el archivo");
                    }
                    else
                    {
                        MessageBox.Show("No hay contenido");
                    }
;                    Files.Add(file);

                }
            }
        }



        private void Btn_Cancel_Request_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult cancelationResult = MessageBox.Show("¿Estás seguro de que deseas cancelar?", 
                "Confirmar cancelación", MessageBoxButton.OKCancel);

            if (cancelationResult == MessageBoxResult.OK)
            {

            }
            else
            {

            }
        }

        private void Btn_Remove_File_Click(object sender, RoutedEventArgs e)
        {
            if (lib_files.SelectedItem != null)
            {
                string selectedFileName = lib_files.SelectedItem.ToString();
                AttachFiles.Remove(selectedFileName);

                SGPMReference.ArchivoSet fileToRemove = null;
                foreach (SGPMReference.ArchivoSet file in Files)
                {
                    if (file.nombre == selectedFileName)
                    {
                        fileToRemove = file;
                        break;
                    }
                }

                if (fileToRemove != null)
                {
                    Files.Remove(fileToRemove);
                }
            }
        }

        private void Btn_Register_Request_Click(object sender, RoutedEventArgs e)
        {

            RegisterDocumentation();

            SGPMReference.SolicitudSet request = new SGPMReference.SolicitudSet
            {
                estado = "creada",
                fechaCreacion = System.DateTime.Now,
                ProyectoFolio = project.Folio,
            };

            SGPMReference.RequestManagementClient client = new SGPMReference.RequestManagementClient();


            int result = client.RegisterRequest(request);
            if (result == 1)
            {
                MessageBox.Show("Éxito en el registro de la solicitud");
            }
            else
            {
                MessageBox.Show("Ocurrió un error al registrar la solicitud");
            }

        }



        private void RegisterDocumentation()
        {
            SGPMReference.RequestManagementClient client = new RequestManagementClient();
            List<SGPMReference.File> sFiles = new List<SGPMReference.File>();

            foreach(ArchivoSet file in Files)
            {
                SGPMReference.File sFile = new SGPMReference.File
                {
                    Name = file.nombre,
                    Content = file.contenido,
                    Description = file.descripcion,
                    Extension = file.extension,
                    
                };

                sFiles.Add(sFile);

            }

            int result = client.RegisterRequestDocumentation(sFiles.ToArray());
            if (result == 1)
            {
                MessageBox.Show("se guardaron con éxito");
            }
            else
            {
                MessageBox.Show("ya vete a dormir wey");
            }


        }


        #region Data recuperation

        private void GetProyectDetails()
        {

            project = client.GetProjectDetails(7);
            tb_modality.Text = project.Modality;
            tb_group.Text = project.AttentionGroup;
            tb_type.Text = project.Type;

        }


        private void Btn_Search_Beneficiary(object sender, RoutedEventArgs e)
        {
            string tipoBeneficiario = ((ComboBoxItem)cb_beneficiary_type.SelectedItem).Content.ToString();

            string beneficiaryName = tb_search_beneficiary.Text;

            if (tipoBeneficiario == "Persona")
            {
                SGPMReference.BeneficiaryManagementClient client = new SGPMReference.BeneficiaryManagementClient();
                List<SGMP_Client.SGPMReference.Person> personsList = client.GetPersons(beneficiaryName).ToList();
                MessageBox.Show("Todo bien");

                lib_beneficiaries.Items.Clear();

                foreach (var person in personsList)
                {
                    lib_beneficiaries.Items.Add(person);
                }

                lib_beneficiaries.DisplayMemberPath = "Name";

            }
            else if (tipoBeneficiario == "Empresa")
            {
                SGPMReference.BeneficiaryManagementClient client = new SGPMReference.BeneficiaryManagementClient();
                List<SGMP_Client.SGPMReference.Company> companiesList = client.GetCompanies(beneficiaryName).ToList();
                MessageBox.Show("Todo bien");

                lib_beneficiaries.Items.Clear();

                foreach (var company in companiesList)
                {
                    lib_beneficiaries.Items.Add(company);
                }

                lib_beneficiaries.DisplayMemberPath = "Name";
            }
        }

        #endregion



        private void tb_search_beneficiary_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void lib_beneficiaries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox.SelectedItem != null)
            {
                object selectedItem = listBox.SelectedItem;


                if (selectedItem is SGMP_Client.SGPMReference.Person)
                {
                    SGMP_Client.SGPMReference.Person selectedPerson = (SGMP_Client.SGPMReference.Person)selectedItem;

                    tb_beneficiary_name.Text = selectedPerson.Name;
                    tb_benef_lastN.Text = selectedPerson.LastName;
                    tb_address.Text = selectedPerson.Street;
                    tb_cellphone.Text = selectedPerson.PhoneNumber;

                }
                else if (selectedItem is SGMP_Client.SGPMReference.Company)
                {
                    SGMP_Client.SGPMReference.Company selectedCompany = (SGMP_Client.SGPMReference.Company)selectedItem;

                    tb_beneficiary_name.Text = selectedCompany.Name;
                    tb_cellphone.Text = selectedCompany.PhoneNumber;
                    tb_address.Text = selectedCompany.Street;
                }
            }
        }
    }
}
