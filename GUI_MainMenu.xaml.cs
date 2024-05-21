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
            im_policies.MouseLeftButtonDown += GoPoliciesModule;
            hideModules();
        }

        private void GoPoliciesModule(object sender, MouseButtonEventArgs e)
        {
            Window policyMenuWindow = new GUI_PolicyMenu();
            this.Close();
            policyMenuWindow.Show();
        }
        
        private void GoUserSModule(object sender, MouseButtonEventArgs e)
        {
            Window userMenuWindow = new GUI_UserMenu();
            userMenuWindow.Show();
            this.Close();
        }

        private void GoProjectsModule(object sender, MouseButtonEventArgs e)
        {
            GUI_ListaProyecto projectsListWindow = new GUI_ListaProyecto();
            projectsListWindow.Show();
            this.Close();
        }

        private void GoLocationsModule(object sender, MouseButtonEventArgs e)
        {
            Window localityMenuWindow = new GUI_LocalityMenu();
            localityMenuWindow.Show();
            this.Close();
        }

        private void GoBeneficiariesModule(object sender, MouseButtonEventArgs e)
        {
            GUI_BeneficiaryList beneficiaryWindow =  new GUI_BeneficiaryList();
            beneficiaryWindow.Show();
            this.Close();
        }

        private void GoRequestsModule(object sender, MouseButtonEventArgs e)
        {
            GUI_RequestsMenu requestWindow = new GUI_RequestsMenu();
            requestWindow.Show();
            this.Close();
        }


        private void hideModules()
        {
            string role = SGMP_Client.DTO_s.User.UserClient.Role;
            if (role.Equals("Administrador")) 
            {
                im_beneficiaries.Visibility = Visibility.Collapsed;
                im_location.Visibility = Visibility.Collapsed; 
                im_policies.Visibility = Visibility.Collapsed;
                im_projects.Visibility = Visibility.Collapsed;
                im_requests.Visibility = Visibility.Collapsed;

                lb_beneficiaries.Visibility= Visibility.Collapsed;
                lb_localities.Visibility= Visibility.Collapsed;
                lb_policies.Visibility= Visibility.Collapsed;
                lb_projects.Visibility= Visibility.Collapsed;
                lb_requests.Visibility= Visibility.Collapsed;  


            }
            else if (role.Equals("Director"))
            {
                im_beneficiaries.Visibility = Visibility.Collapsed;
                im_policies.Visibility = Visibility.Collapsed;
                im_requests.Visibility = Visibility.Collapsed;
                im_users.Visibility = Visibility.Collapsed;

                lb_beneficiaries.Visibility = Visibility.Collapsed;
                lb_policies.Visibility = Visibility.Collapsed;
                lb_requests.Visibility = Visibility.Collapsed;
                lb_users.Visibility = Visibility.Collapsed;  


            }
            else
            {
                im_users.Visibility = Visibility.Collapsed;
                im_projects.Visibility = Visibility.Collapsed;

                lb_users.Visibility = Visibility.Collapsed;
                lb_projects.Visibility = Visibility.Collapsed;

            }
        }

        private void Btn_Exit(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void Btn_Logout(object sender, RoutedEventArgs e)
        {
            SGMP_Client.DTO_s.User.UserClient.Logout();
            Window loginWindow = new GUI_Login();
            this.Close();
            loginWindow.Show();

        }
    }
}
