using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for GUI_User.xaml
    /// </summary>
    public partial class GUI_User : Window
    {
        public GUI_User()
        {
            InitializeComponent();
        }

        private void Btn_Save_User_Click(object sender, RoutedEventArgs e)
        {
            int staffNumber = int.Parse(tbxStaffNumber.Text.ToString());
            string email = tbxEMail.Text.ToString();
            string password = pwbPassword.Password.ToString();
            string repeatPassword = pwbRepeatPassword.Password.ToString();

            if (ValidateFields(staffNumber, email, password, repeatPassword))
            {
                //TODO
            }
        }

        private bool ValidateFields(int staffNumber, string email, string password, string repeatPassword)
        {
            bool isStaffNumberValid = false;
            bool isEmailValid = false;
            bool isPasswordValid = false;

            if (!string.IsNullOrEmpty(staffNumber.ToString()) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(repeatPassword))
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Hidden;

                string staffNumberString = staffNumber.ToString();
                if (staffNumberString.Length == 10)
                {
                    isStaffNumberValid = true;
                    lbInvalidStaffNumberMessage.Visibility = Visibility.Hidden;
                }
                else
                {
                    isStaffNumberValid = false;
                    lbInvalidStaffNumberMessage.Visibility = Visibility.Visible;
                }

                if ((Regex.IsMatch(email, "^[a-zA-Z0-9\\-_]{5,20}@(gob)\\.mx$")))
                {
                    isEmailValid = true;
                    lbInvalidEmailMessage.Visibility = Visibility.Hidden;
                }
                else
                {
                    isEmailValid = false;
                    lbInvalidEmailMessage.Visibility = Visibility.Visible;
                }

                if (Regex.IsMatch(password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&#.$($)\\-_])[A-Za-z\\d$@$!%*?&#.$($)\\-_]{8,16}$"))
                {
                    isPasswordValid = true;
                    tbInvalidPasswordMessage.Visibility = Visibility.Hidden;
                }
                else
                {
                    isPasswordValid = false;
                    tbInvalidPasswordMessage.Visibility = Visibility.Visible;
                }
            }
            else
            {
                lbEmptyFieldsMessage.Visibility = Visibility.Visible;
            }

            return isStaffNumberValid && isEmailValid && isPasswordValid;
        }
    }
}
