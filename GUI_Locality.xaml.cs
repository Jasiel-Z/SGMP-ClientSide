using SGMP_Client.SGPMManagerService;
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
        public GUI_Locality()
        {
            InitializeComponent();
        }

        private void Btn_Save_Locality_Click(object sender, RoutedEventArgs e)
        {
            string localityName = tbxLocalityName.Text.ToString();
            if (ValidateField(localityName))
            {
                int result = SaveLocality(localityName);

                if (result == 1)
                {
                    MessageBox.Show("Se ha guardado la nueva localidad correctamente.", "Operación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ha ocurrido un error al intentar guardar la nueva localidad. Por favor intente más tarde.", "Ocurrió un Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private int SaveLocality(string localityName)
        {
            Locality locality = new Locality
            {
                Name = localityName
            };

            SGPMManagerService.LocalityManagementClient client = new SGPMManagerService.LocalityManagementClient();

            int result = client.SaveLocality(locality);

            return result;
        }

        private bool ValidateField(string localityName)
        {
            bool isLocalityNameValid = false;

            if (!string.IsNullOrEmpty(localityName))
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
            } 
            else
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Visible;
            }

            return isLocalityNameValid;
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window mainMenuWindow = new GUI_MainMenu();
            mainMenuWindow.Show();
            this.Close();
        }
    }
}
