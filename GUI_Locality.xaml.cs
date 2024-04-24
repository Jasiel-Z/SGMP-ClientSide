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
    /// Interaction logic for GUI_Locality.xaml
    /// </summary>
    public partial class GUI_Locality : Window
    {
        private int localityId;

        public GUI_Locality()
        {
            InitializeComponent();
        }

        public void LoadLocality(Locality locality)
        {
            this.localityId = locality.LocalityID;

            tbxLocalityName.Text = locality.Name;
            tbxTownship.Text = locality.Township;
        }

        private void Btn_Save_Locality_Click(object sender, RoutedEventArgs e)
        {
            string localityName = tbxLocalityName.Text.ToString();
            string township = tbxTownship.Text.ToString();

            if (ValidateFields(localityName, township))
            {
                if (this.localityId == 0)
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

            SGPMService.LocalityManagementClient client = new   SGPMService.LocalityManagementClient();

            int result = client.SaveLocality(locality);

            if (result == 1)
            {
                MessageBox.Show("Se ha guardado la nueva localidad correctamente.", "Operación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error al intentar guardar la nueva localidad. Por favor intente de nuevo más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateLocality(string localityName, string township)
        {
            Locality locality = new Locality
            {
                LocalityID = this.localityId,
                Name = localityName,
                Township = township
            };

            SGPMService.LocalityManagementClient client = new SGPMService.LocalityManagementClient();
            int result = client.UpdateLocality(locality);

            if (result == 1)
            {
                MessageBox.Show("Se ha actualizado información de la localidad correctamente.", "Operación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error al intentar actualizar la localidad. Por favor intente de nuevo más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateFields(string localityName, string township)
        {
            bool isLocalityNameValid = false;
            bool isTownshipValid = false;

            if (!string.IsNullOrEmpty(localityName) && !string.IsNullOrEmpty(township))
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
            Window mainMenuWindow = new GUI_MainMenu();
            mainMenuWindow.Show();
            this.Close();
        }
    }
}
