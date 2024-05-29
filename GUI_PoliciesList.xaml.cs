using SGMP_Client.SGPMService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SGMP_Client
{
    /// <summary>
    /// Interaction logic for GUI_PoliciesList.xaml
    /// </summary>
    public partial class GUI_PoliciesList : Window
    {
        private List<Policy> policies = new List<Policy>();

        public GUI_PoliciesList()
        {
            InitializeComponent();
            AddListItems();
        }

        private void AddListItems()
        {
            try
            {
                SGPMService.PolicyManagementClient client = new SGPMService.PolicyManagementClient();
                Policy[] policiesFromDB = client.GetPolicies();

                foreach (var policy in policiesFromDB)
                {
                    policies.Add(policy);

                    ListBoxItem item = new ListBoxItem
                    {
                        Content = policy.Name,
                        Tag = policy.PolicyID
                    };
                    lbxPolicies.Items.Add(item);
                }
            }
            catch (EndpointNotFoundException)
            {
                throw new EndpointNotFoundException();
            }
            
        }

        private void ListBoxItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SGPMService.PolicyManagementClient client = new SGPMService.PolicyManagementClient();
                var item = sender as ListBoxItem;

                if (item != null)
                {
                    Policy policy = client.GetPolicy((int)item.Tag);
                    policy.PolicyID = (int)item.Tag;

                    GUI_Policy localityWindow = new GUI_Policy();
                    localityWindow.LoadPolicy(policy);
                    localityWindow.Show();
                    this.Close();
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

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window policyMenuWindow = new GUI_PolicyMenu();
            policyMenuWindow.Show();
            this.Close();
        }

        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            string filter = tbSearch.Text.ToString();
            var itemsToRemove = new List<object>();

            if (!string.IsNullOrEmpty(filter))
            {
                RestoreListBoxItems();

                foreach (var item in lbxPolicies.Items)
                {
                    if (item.ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        itemsToRemove.Add(item);
                    }
                }

                foreach (var item in itemsToRemove)
                {
                    lbxPolicies.Items.Remove(item);
                }
            }
            else
            {
                RestoreListBoxItems();
            }
        }

        private void RestoreListBoxItems()
        {
            lbxPolicies.Items.Clear();

            foreach (var policy in policies)
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = policy.Name,
                    Tag = policy.PolicyID
                };
                lbxPolicies.Items.Add(item);
            }
        }
    }
}
