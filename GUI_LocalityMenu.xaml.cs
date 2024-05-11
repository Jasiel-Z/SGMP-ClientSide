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
    /// Interaction logic for GUI_LocalityMenu.xaml
    /// </summary>
    public partial class GUI_LocalityMenu : Window
    {
        public GUI_LocalityMenu()
        {
            InitializeComponent();
        }

        private void Btn_Register_Locality_Click(object sender, RoutedEventArgs e)
        {
            Window localityWindow = new GUI_Locality();
            localityWindow.Show();
            this.Close();
        }

        private void Btn_Localities_List_Click(object sender, RoutedEventArgs e)
        {
            Window localitiesListWindow = new GUI_LocalitiesList();
            localitiesListWindow.Show();
            this.Close();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window mainMenuWindow = new GUI_MainMenu();
            mainMenuWindow.Show();
            this.Close();
        }
    }
}
