using System;
using SGMP_Client.SGPMService;
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
using System.ServiceModel;

namespace SGMP_Client
{
    /// <summary>
    /// Interaction logic for GUI_LocalitiesList.xaml
    /// </summary>
    public partial class GUI_LocalitiesList : Window
    {
        List<Locality> localities = new List<Locality>();

        public GUI_LocalitiesList()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            AddListItems();
        }

        private void AddListItems()
        {
            try
            {
                SGPMService.LocalityManagementClient client = new SGPMService.LocalityManagementClient();
                Locality[] localitiesFromDB = client.GetLocalities();

                foreach (var locality in localitiesFromDB)
                {
                    localities.Add(locality);

                    ListBoxItem item = new ListBoxItem
                    {
                        Content = locality.Name + ", " + locality.Township,
                        Tag = locality.LocalityID
                    };
                    lbxLocalities.Items.Add(item);
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
                SGPMService.LocalityManagementClient client = new SGPMService.LocalityManagementClient();
                var item = sender as ListBoxItem;

                if (item != null)
                {
                    Locality locality = client.GetLocalityByID((int)item.Tag);
                    locality.LocalityID = (int)item.Tag;

                    GUI_Locality localityWindow = new GUI_Locality();
                    localityWindow.LoadLocality(locality);
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
            Window localityMenuWindow = new GUI_LocalityMenu();
            localityMenuWindow.Show();
            this.Close();
        }

        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            string filter = tbSearch.Text.ToString();
            var itemsToRemove = new List<object>();

            if (!string.IsNullOrEmpty(filter))
            {
                RestoreListBoxItems();

                foreach (var item in lbxLocalities.Items)
                {
                    if (item.ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        itemsToRemove.Add(item);
                    }
                }

                foreach (var item in itemsToRemove)
                {
                    lbxLocalities.Items.Remove(item);
                }
            }
            else
            {
                RestoreListBoxItems();
            }
        }

        private void RestoreListBoxItems()
        {
            lbxLocalities.Items.Clear();

            foreach (var locality in localities)
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = locality.Name + ", " + locality.Township,
                    Tag = locality.LocalityID
                };
                lbxLocalities.Items.Add(item);
            }
        }
    }
}
