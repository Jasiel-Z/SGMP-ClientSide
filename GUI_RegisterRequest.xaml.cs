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
using SGMP_Client.SGPMService;
using SGMP_Client.DTO_s;
using System.Runtime.InteropServices.WindowsRuntime;

namespace SGMP_Client
{
    /// <summary>
    /// Lógica de interacción para GUI_RegisterRequest.xaml
    /// </summary>
    public partial class GUI_RegisterRequest : Window
    {

        public ObservableCollection<string> AttachFiles { get; set; }
        public List<SGPMService.File> Files { get; set; }
        SGPMService.ProjectsManagementClient client;
        private Project project;

        public GUI_RegisterRequest(Project project)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            AttachFiles = new ObservableCollection<string>();
            Files = new List<SGPMService.File>();
            lib_files.ItemsSource = AttachFiles;
            client = new SGPMService.ProjectsManagementClient();
            this.project = project;
            GetProyectDetails();
        }


        private void Btn_Attach_File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos PDF (*.pdf)|*.pdf";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string ruta = openFileDialog.FileName;
                AttachFiles.Add(ruta);

                try
                {
                    using (FileStream fileStream = new FileStream(ruta, FileMode.Open, FileAccess.Read))
                    {
                        byte[] fileBytes = new byte[fileStream.Length];
                        fileStream.Read(fileBytes, 0, (int)fileStream.Length);

                        SGPMService.File file = new SGPMService.File
                        {
                            Name = System.IO.Path.GetFileName(ruta),
                            Content = new byte[0],
                            Extension = System.IO.Path.GetExtension(ruta),
                            RequestId = 1,
                            Description = ruta,
                        };

                        Files.Add(file);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al leer el archivo: {ex.Message}");
                }


            }
        }



        private void Btn_Cancel_Request_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult cancelationResult = MessageBox.Show("¿Estás seguro de que deseas cancelar?", 
                "Confirmar cancelación", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (cancelationResult == MessageBoxResult.OK)
            {
                GUI_RequestsManagement requestsManagement = new GUI_RequestsManagement(project);
                requestsManagement.Show();
                this.Close();
            }

        }

        private void Btn_Remove_File_Click(object sender, RoutedEventArgs e)
        {
            if (lib_files.SelectedItem != null)
            {
                string selectedFileName = lib_files.SelectedItem.ToString();
                AttachFiles.Remove(selectedFileName);

                SGPMService.File fileToRemove = null;
                foreach (SGPMService.File file in Files)
                {
                    if (file.Name == selectedFileName)
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

            bool validData = validateInformation();

            if (validData)
            {
                SGPMService.Solicitudes request = new SGPMService.Solicitudes
                {
                    estado = "creada",
                    fechaCreacion = System.DateTime.Now,
                    Folio = project.Folio,
                    IdBeneficiario = GetBeneficiaryId(),
                };

                try
                {
                    SGPMService.RequestManagementClient client = new SGPMService.RequestManagementClient();

                    bool beneficiaryFound = client.BeneficiaryHasRequest((int)request.IdBeneficiario, request.Folio);

                    if (beneficiaryFound)
                    {
                        MessageBox.Show("El beneficiario ya cuenta con una solicitud para el proyecto actual", "Beneficiario registrado", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        int result = client.RegisterRequestWithDocuments(request, Files.ToArray());
                        if (result >= 1)
                        {
                            MessageBox.Show("La solicitud  ha sido registrada", "Registro exitoso", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                            GUI_RequestsManagement requestsManagement = new GUI_RequestsManagement(project);
                            requestsManagement.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("No fue posible guardar el registro, por favor inténtelo más tarde",
                                "Problema de conexión con la base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (TimeoutException ex)
                {
                    MessageBox.Show("No fue posible establecer conexión con el servidor, por favor inténtelo más tarde",
                 "Problema de conexión con el servidor", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private bool validateInformation()
        {
            bool result = true;
            if (Files.Count == 0)
            {
                MessageBox.Show("Una solicitud no puede ser registrada sin documentos. Por favor inténtelo de nuevo",
                    "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                result = false;

            }

            return result;
        }



        private int GetBeneficiaryId()
        {
            int id = -1;

            if (lib_beneficiaries.SelectedItem != null)
            {
                if (lib_beneficiaries.SelectedItem is Person)
                {
                    Person person = (Person)lib_beneficiaries.SelectedItem;
                    id = person.BeneficiaryId;
                }
                else if (lib_beneficiaries.SelectedItem is Company)
                {
                    Company company = (Company)lib_beneficiaries.SelectedItem;
                    id = company.BeneficiaryId;
                }
            }

            return id;
        }

        #region Data recuperation

        private void GetProyectDetails()
        {

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
                SGPMService.BeneficiaryManagementClient client = new SGPMService.BeneficiaryManagementClient();
                List<SGMP_Client.SGPMService.Person> personsList = client.GetPersons(beneficiaryName).ToList();


                lib_beneficiaries.Items.Clear();

                foreach (var person in personsList)
                {
                    lib_beneficiaries.Items.Add(person);
                }

                lib_beneficiaries.DisplayMemberPath = "Name";

            }
            else if (tipoBeneficiario == "Empresa")
            {
                SGPMService.BeneficiaryManagementClient client = new SGPMService.BeneficiaryManagementClient();
                List<SGMP_Client.SGPMService.Company> companiesList = client.GetCompanies(beneficiaryName).ToList();

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


                if (selectedItem is SGMP_Client.SGPMService.Person)
                {
                    SGMP_Client.SGPMService.Person selectedPerson = (SGMP_Client.SGPMService.Person)selectedItem;

                    tb_beneficiary_name.Text = selectedPerson.Name;
                    tb_benef_lastN.Text = selectedPerson.LastName;
                    tb_address.Text = selectedPerson.Street;
                    tb_cellphone.Text = selectedPerson.PhoneNumber;

                }
                else if (selectedItem is SGMP_Client.SGPMService.Company)
                {
                    SGMP_Client.SGPMService.Company selectedCompany = (SGMP_Client.SGPMService.Company)selectedItem;

                    tb_beneficiary_name.Text = selectedCompany.Name;
                    tb_cellphone.Text = selectedCompany.PhoneNumber;
                    tb_address.Text = selectedCompany.Street;
                }
            }
        }


        private void ClearFields()
        {
            tb_beneficiary_name.Text = "";
            tb_beneficiary_name.Text = "";
            tb_address.Text = "";
            tb_cellphone.Text = "";
            lib_beneficiaries.Items.Clear();
            Files.Clear();
            lib_files.Items.Clear();


        }
    }
}
