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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SGMP_Client
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Btn_Save_User_Click(object sender, RoutedEventArgs e)
        {
            Window userWindow = new GUI_User();
            userWindow.Show();
            this.Close();
        }

        private void Btn_Save_Locality_Click(object sender, RoutedEventArgs e)
        {
            Window localityWindow = new GUI_Locality();
            localityWindow.Show();
            this.Close();
        }

        private void Btn_Save_Policy_Click(object sender, RoutedEventArgs e)
        {
            Window policyWindow = new GUI_Policy();
            policyWindow.Show();
            this.Close();
        }
    }
}
