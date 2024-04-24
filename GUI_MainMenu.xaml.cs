using SGMP_Client.Properties;
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

namespace SGMP_Client
{
    /// <summary>
    /// Lógica de interacción para GUI_MainMenu.xaml
    /// </summary>
    public partial class GUI_MainMenu : Window
    {
        public GUI_MainMenu()
        {
            InitializeComponent();
            im_requests.MouseLeftButtonDown += GoRequestsModule;
            im_beneficiaries.MouseLeftButtonDown += GoBeneficiariesModule;
            im_location.MouseLeftButtonDown += GoLocationsModule;
            im_projects.MouseLeftButtonDown += GoProjectsModule;
            im_users.MouseLeftButtonDown += GoUserSModule;
        }

        private void GoUserSModule(object sender, MouseButtonEventArgs e)
        {
            Window userWindow = new GUI_User();
            userWindow.Show();
            this.Close();
        }

        private void GoProjectsModule(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GoLocationsModule(object sender, MouseButtonEventArgs e)
        {
            Window locationWindow = new GUI_Locality();
            locationWindow.Show();
            this.Close();
        }

        private void GoBeneficiariesModule(object sender, MouseButtonEventArgs e)
        {
            GUI_BeneficiaryList gUI_Beneficiary =  new GUI_BeneficiaryList();
            gUI_Beneficiary.Show();
            this.Close();
        }

        private void GoRequestsModule(object sender, MouseButtonEventArgs e)
        {
            GUI_RequestsMenu requestsMenu = new GUI_RequestsMenu();
            requestsMenu.Show();
            this.Close();
        }
    }
}
