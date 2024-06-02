using SGMP_Client.SGPMService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
    /// Interaction logic for GUI_Policy.xaml
    /// </summary>
    public partial class GUI_Policy : Window
    {
        private Policy policyToBeModified = new Policy();

        public GUI_Policy()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        }

        public void LoadPolicy(Policy policy)
        {
            this.policyToBeModified = policy;

            tbxName.Text = policy.Name;
            tbxDescription.Text = policy.Description;

            this.Title = "Modificar Política";
            this.lbTitle.Content = "Modificar Política";
        }

        private void Btn_Save_Policy_Click(object sender, RoutedEventArgs e)
        {
            string policyName = tbxName.Text.ToString();
            string policyDescription = tbxDescription.Text.ToString();

            if (ValidateFields(policyName, policyDescription))
            {
                if (!string.IsNullOrWhiteSpace(this.policyToBeModified.Name))
                {
                    UpdatePolicy(policyName, policyDescription);
                }
                else
                {
                    SavePolicy(policyName, policyDescription);
                }
            }
        }

        private void SavePolicy(string policyName, string policyDescription)
        {
            Policy policy = new Policy
            {
                Name = policyName,
                Description = policyDescription
            };

            try
            {
                SGPMService.PolicyManagementClient client = new SGPMService.PolicyManagementClient();

                if (client.ValidatePolicyDoesNotExist(policy))
                {
                    int result = client.SavePolicy(policy);

                    if (result == 1)
                    {
                        MessageBox.Show("Se ha guardado la nueva política de participación correctamente.", "Operación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);

                        Window policyMenuWindow = new GUI_PolicyMenu();
                        policyMenuWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Ha ocurrido un error al intentar guardar la nueva política de participación. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Ya existe una política con el mismo nombre y descripción.", "La política ya existe.", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("Ha ocurrido un error al conectar con el Servidor. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);

                SGMP_Client.DTO_s.User.UserClient.Logout();
                Window loginWindow = new GUI_Login();
                this.Close();
                loginWindow.Show();
            }
        }

        private void UpdatePolicy(string policyName, string policyDescription)
        {
            Policy policy = new Policy
            {
                PolicyID = policyToBeModified.PolicyID,
                Name = policyName,
                Description = policyDescription
            };

            try
            {
                if (!policy.Name.Equals(policyToBeModified.Name) || !policy.Description.Equals(policyToBeModified.Description))
                {
                    SGPMService.PolicyManagementClient client = new SGPMService.PolicyManagementClient();

                    if (client.ValidatePolicyDoesNotExist(policy))
                    {
                        int result = client.UpdatePolicy(policy);

                        if (result == 1)
                        {
                            MessageBox.Show("Se ha actualizado la información de la política de participación correctamente.", "Operación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);

                            Window policiesListWindow = new GUI_PoliciesList();
                            policiesListWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Ha ocurrido un error al actualizar la política de participación. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ya existe una política con el mismo nombre y descripción.", "La política ya existe.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }  
                }
                else
                {
                    MessageBox.Show("Modifique alguno de los campos antes de continuar.", "Error en la operación", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("Ha ocurrido un error al conectar con el Servidor. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);

                SGMP_Client.DTO_s.User.UserClient.Logout();
                Window loginWindow = new GUI_Login();
                this.Close();
                loginWindow.Show();
            }
        }

        private bool ValidateFields(string policyName, string policyDescription)
        {
            bool isPolicyNameValid = false;
            bool isPolicyDescriptionValid = false;

            if (!string.IsNullOrWhiteSpace(policyName) && !string.IsNullOrWhiteSpace(policyDescription)) 
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Hidden;

                if (policyName.Length <= 20)
                {
                    isPolicyNameValid = true;
                    lbInvalidNameMessage.Visibility = Visibility.Hidden;
                } 
                else
                {
                    isPolicyNameValid = false;
                    lbInvalidNameMessage.Visibility = Visibility.Visible;
                }

                if (policyDescription.Length <= 100) 
                {
                    isPolicyDescriptionValid = true;
                    lbInvalidDescriptionMessage.Visibility = Visibility.Hidden;
                } 
                else
                {
                    isPolicyDescriptionValid = false;
                    lbInvalidDescriptionMessage.Visibility = Visibility.Visible;
                }
            }
            else
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Visible;
            }

            return isPolicyNameValid && isPolicyDescriptionValid;
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.policyToBeModified.Name))
            {
                Window policyMenuWindow = new GUI_PolicyMenu();
                policyMenuWindow.Show();
                this.Close();
            }
            else
            {
                Window policiesListWindow = new GUI_PoliciesList();
                policiesListWindow.Show();
                this.Close();
            }
        }
    }
}
