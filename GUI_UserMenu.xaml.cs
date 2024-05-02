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
    /// Interaction logic for GUI_UserMenu.xaml
    /// </summary>
    public partial class GUI_UserMenu : Window
    {
        public GUI_UserMenu()
        {
            InitializeComponent();
        }

        private void Btn_Users_List_Click(object sender, RoutedEventArgs e)
        {
            Window usersListWindow = new GUI_UsersList();
            usersListWindow.Show();
            this.Close();
        }

        private void Btn_Register_User_Click(object sender, RoutedEventArgs e)
        {
            Window userWindow = new GUI_User();
            userWindow.Show();
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
