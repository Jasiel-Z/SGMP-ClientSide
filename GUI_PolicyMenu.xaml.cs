﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Interaction logic for GUI_PolicyMenu.xaml
    /// </summary>
    public partial class GUI_PolicyMenu : Window
    {
        public GUI_PolicyMenu()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        }

        private void Btn_Register_Policy_Click(object sender, RoutedEventArgs e)
        {
            Window policyWindow = new GUI_Policy();
            policyWindow.Show();
            this.Close();
        }

        private void Btn_Policies_List_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Window policiesListWindow = new GUI_PoliciesList();
                policiesListWindow.Show();
                this.Close();
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
            Window mainMenuWindow = new GUI_MainMenu();
            mainMenuWindow.Show();
            this.Close();
        }
    }
}