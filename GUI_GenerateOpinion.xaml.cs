﻿using Microsoft.Win32;
using SGMP_Client.SGPMService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
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

        public List<SGPMService.File> Files { get; set; }
        SGPMService.ProjectsManagementClient Client;
        private Project project;

        public SGMP_Client.SGPMService.Person Person { get; set; }
        public SGMP_Client.SGPMService.Company Company { get; set; }

        private Request request;

        public SGPMService.Beneficiary Beneficiary { get; set; }

        public List<SGMP_Client.SGPMService.ProjectPolicy> ProjectPolicies { get; set; }


        public GUI_GenerateOpinion(Project project, Request request)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Files = new List<SGPMService.File>();
            Client = new SGPMService.ProjectsManagementClient();
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
            tb_type.Text = project.SupportAmount.ToString() + " MXN";

        }


        private void GetPolicies()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible establecer conexión con el servidor, por favor inténtelo más tarde",
                 "Problema de conexión con el servidor", MessageBoxButton.OK, MessageBoxImage.Error);
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
            try
            {
                SGPMService.RequestManagementClient client = new RequestManagementClient();
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
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible establecer conexión con el servidor, por favor inténtelo más tarde",
                 "Problema de conexión con el servidor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
          
        }



        private void DownloadFile(File selectedFile)
        {
            string rutaArchivoOriginal = selectedFile.Description;

            string directorioDestino = SelectDirectory();

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


        private string SelectDirectory()
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
            "Confirmar cancelación", MessageBoxButton.OKCancel, MessageBoxImage.Question);

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
                    "Confirmación de registro", MessageBoxButton.OKCancel, MessageBoxImage.Question);


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
                            EmployeeNumber = DTO_s.User.UserClient.EmployeeNumber
                        
                        };

                        if (project.RemainingBeneficiaries <= 0 && opinion.State.Equals("aceptado"))
                        {
                            MessageBox.Show("El número máximo de beneficiarios para el proyecto ha sido alcanzado. Debe rechazar esta solicitud",
                                "Número máximo de beneficiarios alcanzado", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            try
                            {
                                SGPMService.RequestManagementClient client = new SGPMService.RequestManagementClient();
                                int result = client.RegisterOpinion(opinion, request.Id);
                                if (result >= 1)
                                {
                                    updateRemainingBeneficiaries();
                                    MessageBox.Show("El dictamen ha sido registrado en el sistema", "Registro exitoso",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                                    GUI_RequestsManagement requestsManagement = new GUI_RequestsManagement(project);
                                    requestsManagement.Show();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Ocurrió un problema con la base de datos, por favor inténtelo más tarde",
                                        "Problema de conexión con la base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("No fue posible establecer conexión con el servidor, por favor inténtelo más tarde",
                                    "Problema de conexión con el servidor", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }

                    }
                }
                else
                {
                    MessageBox.Show("Por favor selecciona una opción para el cumplimiento de las políticas", 
                        "Datos faltantes", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        }


        private bool ValidateData()
        {
            bool valid = true;

            if(tb_comments.Text == "")
            {
                valid = false;
                MessageBox.Show("Un dictamen debe tener comentarios, por favor ingrese uno", "Datos faltantes", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
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


        private void updateRemainingBeneficiaries()
        {
            SGPMService.ProjectsManagementClient client = new SGPMService.ProjectsManagementClient();
            try
            {
                int result = client.updateRemainingBeneficiaries(project.Folio);

            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible establecer conexión con el servidor, por favor inténtelo más tarde",
                                 "Problema de conexión con el servidor", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
