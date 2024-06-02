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
using System.Windows.Shapes;

namespace SGMP_Client
{
    /// <summary>
    /// Interaction logic for GUI_UsersList.xaml
    /// </summary>
    public partial class GUI_UsersList : Window
    {
        private Dictionary<int, User> usersDictionary = new Dictionary<int, User>();

        public GUI_UsersList()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            AddListItems();
        }

        private void AddListItems()
        {
            try
            {
                SGPMService.UserManagementClient client = new SGPMService.UserManagementClient();
                User[] usersFromDB = client.GetUsersGeneralInfo();

                foreach (User user in usersFromDB)
                {
                    ListBoxItem item = new ListBoxItem
                    {
                        Content = user.EmployeeNumber + "  -  " + user.MiddleName + " " + user.LastName + " " + user.Name,
                        Tag = user.EmployeeNumber
                    };

                    usersDictionary.Add(user.EmployeeNumber, user);

                    lbxUsers.Items.Add(item);
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
                SGPMService.UserManagementClient client = new SGPMService.UserManagementClient();
                var item = sender as ListBoxItem;

                if (item != null)
                {
                    User remainingUserData = client.GetUserDetailsByEmployeeNumber((int)item.Tag);
                    usersDictionary[(int)item.Tag].Role = remainingUserData.Role;
                    usersDictionary[(int)item.Tag].PhoneNumber = remainingUserData.PhoneNumber;
                    usersDictionary[(int)item.Tag].Street = remainingUserData.Street;
                    usersDictionary[(int)item.Tag].City = remainingUserData.City;
                    usersDictionary[(int)item.Tag].Number = remainingUserData.Number;
                    usersDictionary[(int)item.Tag].LocationId = remainingUserData.LocationId;
                    usersDictionary[(int)item.Tag].UserId = remainingUserData.UserId;
                    usersDictionary[(int)item.Tag].Email = remainingUserData.Email;

                    GUI_User userWindow = new GUI_User();
                    userWindow.LoadUser(usersDictionary[(int)item.Tag]);
                    userWindow.Show();
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
            Window userMenuWindow = new GUI_UserMenu();
            userMenuWindow.Show();
            this.Close();
        }

        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            string filter = tbSearch.Text.ToString();
            var itemsToRemove = new List<object>();

            if (!string.IsNullOrEmpty(filter))
            {
                RestoreListBoxItems();

                foreach (var item in lbxUsers.Items)
                {
                    if (item.ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        itemsToRemove.Add(item);
                    }
                }

                foreach (var item in itemsToRemove)
                {
                    lbxUsers.Items.Remove(item);
                }
            }
            else
            {
                RestoreListBoxItems();
            }
        }

        private void RestoreListBoxItems()
        {
            lbxUsers.Items.Clear();

            foreach (var user in usersDictionary)
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = user.Key + "  -  " + user.Value.MiddleName + " " + user.Value.LastName + " " + user.Value.Name,
                    Tag = user.Key
                };
                lbxUsers.Items.Add(item);
            }
        }
    }
}
