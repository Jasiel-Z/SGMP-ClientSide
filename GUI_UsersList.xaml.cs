using SGMP_Client.SGPMService;
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
    /// Interaction logic for GUI_UsersList.xaml
    /// </summary>
    public partial class GUI_UsersList : Window
    {
        private Dictionary<int, User> usersDictionary = new Dictionary<int, User>();

        public GUI_UsersList()
        {
            InitializeComponent();
            AddListItems();
        }

        private void AddListItems()
        {
            SGPMService.UserManagementClient cliente = new SGPMService.UserManagementClient();
            User[] usersFromDB = cliente.GetUsersGeneralInfo(1);

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

        private void ListBoxItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window userMenuWindow = new GUI_UserMenu();
            userMenuWindow.Show();
            this.Close();
        }
    }
}
