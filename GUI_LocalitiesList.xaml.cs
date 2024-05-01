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

namespace SGMP_Client
{
    /// <summary>
    /// Interaction logic for GUI_LocalitiesList.xaml
    /// </summary>
    public partial class GUI_LocalitiesList : Window
    {
        public GUI_LocalitiesList()
        {
            InitializeComponent();
            AddListItems();
        }

        private void AddListItems()
        {
            SGPMService.LocalityManagementClient client = new SGPMService.LocalityManagementClient();
            Locality[] localitiesFromDB = client.GetLocalities();

            foreach (var locality in localitiesFromDB)
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = locality.Name + ", " + locality.Township,
                    Tag = locality.LocalityID
                };
                lbxLocalities.Items.Add(item);
            }
        }

        private void ListBoxItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window localityMenuWindow = new GUI_LocalityMenu();
            localityMenuWindow.Show();
            this.Close();
        }
    }
}
