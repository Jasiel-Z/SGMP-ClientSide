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
    /// Interaction logic for GUI_Locality.xaml
    /// </summary>
    public partial class GUI_Locality : Window
    {
        public GUI_Locality()
        {
            InitializeComponent();
        }

        private void Btn_Save_Locality_Click(object sender, RoutedEventArgs e)
        {
            string localityName = tbxLocalityName.Text.ToString();
            if (ValidateField(localityName))
            {
                //TODO
            }
        }

        private bool ValidateField(string localityName)
        {
            bool isLocalityNameValid = false;

            if (!string.IsNullOrEmpty(localityName))
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Visible;

                if (localityName.Length <= 30)
                {
                    isLocalityNameValid = true;
                    lbInvalidLocalityNameMessage.Visibility = Visibility.Hidden;
                } 
                else
                {
                    lbInvalidLocalityNameMessage.Visibility = Visibility.Visible;
                }
            } 
            else
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Visible;
            }

            return isLocalityNameValid;
        }
    }
}
