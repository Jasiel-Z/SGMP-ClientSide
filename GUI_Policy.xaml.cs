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
    /// Interaction logic for GUI_Policy.xaml
    /// </summary>
    public partial class GUI_Policy : Window
    {
        public GUI_Policy()
        {
            InitializeComponent();
        }

        private void Btn_Save_Policy_Click(object sender, RoutedEventArgs e)
        {
            string policyName = tbxName.Text.ToString();
            string policyDescription = tbxName.Text.ToString();

            if (ValidateFields(policyName, policyDescription))
            {
                //TODO
            }
        }

        private bool ValidateFields(string policyName, string policyDescription)
        {
            bool isPolicyNameValid = false;
            bool isPolicyDescriptionValid = false;

            if (!string.IsNullOrEmpty(policyName) && !string.IsNullOrEmpty(policyDescription)) 
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Hidden;

                if (policyName.Length <= 20)
                {
                    isPolicyNameValid = true;
                    lbInvalidNameMessage.Visibility = Visibility.Hidden;
                } 
                else
                {
                    isPolicyNameValid = false;
                    lbInvalidNameMessage.Visibility = Visibility.Visible;
                }

                if (policyDescription.Length <= 100) 
                {
                    isPolicyDescriptionValid = true;
                    lbInvalidDescriptionMessage.Visibility = Visibility.Hidden;
                } 
                else
                {
                    isPolicyDescriptionValid = false;
                    lbInvalidDescriptionMessage.Visibility = Visibility.Visible;
                }
            }
            else
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Visible;
            }

            return isPolicyNameValid && isPolicyDescriptionValid;
        }
    }
}
