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
    /// Interaction logic for GUI_Locality.xaml
    /// </summary>
    public partial class GUI_Locality : Window
    {
        private Locality localityToBeModified = new Locality();

        public GUI_Locality()
        {
            InitializeComponent();
        }

        public void LoadLocality(Locality locality)
        {
            localityToBeModified = locality;

            tbxLocalityName.Text = locality.Name;
            tbxTownship.Text = locality.Township;

            this.Title = "Modificar Localidad";
            this.lbTitle.Content = "Modificar Localidad";
        }

        private void Btn_Save_Locality_Click(object sender, RoutedEventArgs e)
        {
            string localityName = tbxLocalityName.Text.ToString();
            string township = tbxTownship.Text.ToString();

            if (ValidateFields(localityName, township))
            {
                if (localityToBeModified.LocalityID == 0)
                {
                    SaveLocality(localityName, township);
                }
                else
                {
                    UpdateLocality(localityName, township);
                }
            }
        }

        private void SaveLocality(string localityName, string township)
        {
            Locality locality = new Locality
            {
                Name = localityName,
                Township = township
            };

            try
            {
                SGPMService.LocalityManagementClient client = new SGPMService.LocalityManagementClient();

                if (client.ValidateLocalityDoesNotExist(locality))
                {
                    int result = client.SaveLocality(locality);

                    if (result == 1)
                    {
                        MessageBox.Show("Se ha guardado la nueva localidad correctamente.", "Operación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);

                        Window localityMenuWindow = new GUI_LocalityMenu();
                        localityMenuWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Ha ocurrido un error al intentar guardar la nueva localidad. Por favor intente de nuevo más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Ya existe una localidad con el mismo nombre y municipio.", "La Localidad ya existe", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void UpdateLocality(string localityName, string township)
        {
            Locality locality = new Locality
            {
                LocalityID = localityToBeModified.LocalityID,
                Name = localityName,
                Township = township
            };

            try
            {
                if (!locality.Name.Equals(localityToBeModified.Name) || !locality.Township.Equals(localityToBeModified.Township))
                {
                    SGPMService.LocalityManagementClient client = new SGPMService.LocalityManagementClient();
                    if (client.ValidateLocalityDoesNotExist(locality))
                    {
                        int result = client.UpdateLocality(locality);

                        if (result == 1)
                        {
                            MessageBox.Show("Se ha actualizado la información de la localidad correctamente.", "Operación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);

                            Window localityListWindow = new GUI_LocalitiesList();
                            localityListWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Ha ocurrido un error al intentar actualizar la localidad. Por favor intente de nuevo más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ya existe una localidad con el mismo nombre y municipio.", "La Localidad ya existe", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private bool ValidateFields(string localityName, string township)
        {
            bool isLocalityNameValid = false;
            bool isTownshipValid = false;

            if (!string.IsNullOrWhiteSpace(localityName) && !string.IsNullOrWhiteSpace(township))
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Hidden;

                if (localityName.Length <= 30)
                {
                    isLocalityNameValid = true;
                    lbInvalidLocalityNameMessage.Visibility = Visibility.Hidden;
                } 
                else
                {
                    lbInvalidLocalityNameMessage.Visibility = Visibility.Visible;
                }

                if (township.Length <= 40)
                {
                    isTownshipValid = true;
                    lbInvalidTownshipMessage.Visibility = Visibility.Hidden;
                }
                else
                {
                    lbInvalidLocalityNameMessage.Visibility = Visibility.Visible;
                }
            } 
            else
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Visible;
            }

            return isLocalityNameValid && isTownshipValid;
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (!this.Title.Equals("Modificar Localidad"))
            {
                Window localityMenuWindow = new GUI_LocalityMenu();
                localityMenuWindow.Show();
                this.Close();
            }
            else
            {
                Window localityListWindow = new GUI_LocalitiesList();
                localityListWindow.Show();
                this.Close();
            }
            
        }
    }
}
