using SGMP_Client.SGPMService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel.Configuration;
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
    /// Lógica de interacción para GUI_UpdateBeneficiary.xaml
    /// </summary>
    public partial class GUI_UpdateBeneficiary : Window
    {

        private Beneficiary Beneficiary { get; set; }
        private SGMP_Client.SGPMService.BeneficiaryManagementClient client;
        private bool HaveBankAccount { get; set; }
        public GUI_UpdateBeneficiary(Beneficiary beneficiary)
        {
            InitializeComponent();
            HaveBankAccount = false;
            this.Beneficiary = beneficiary;
            ShowBeneficiaryInformation();
            GetBeneficiaryTypeInformation();
            GetAccountInformation();
            im_help.MouseLeftButtonDown += ShowInformation;

        }

        private void ShowInformation(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Los apartados que se pueden modificar son:" +
                "teléfono, ciudad y calle", "Actualización de beneficiario", MessageBoxButton.OK, 
                MessageBoxImage.Information);
        }



        #region ShowInformation

        private void ShowBeneficiaryInformation()
        {
            tb_phone.Text = Beneficiary.PhoneNumber;
            tb_rfc.Text = Beneficiary.RFC;
            tb_street.Text = Beneficiary.Street;
            tb_city.Text = Beneficiary.City;
        }

        private void GetBeneficiaryTypeInformation()
        {
            client = new BeneficiaryManagementClient();
            try
            {
                if (Beneficiary.PersonId != 0)
                {
                    Person person = client.getPerson(Beneficiary.Id);
                    ShowPersonInformation(person);
                }
                else
                {
                    Company company = client.getCompany(Beneficiary.Id);
                    ShowCompanyInformation(company);
                }
            }
            catch (TimeoutException ex) 
            {
                
            }

        }

        private void ShowPersonInformation(Person person)
        {
            tb_name.Text = person.Name;
            tb_lastname.Text = person.LastName;
            tb_middlename.Text = person.SurName;
            tb_curp.Text = person.CURP;
        }

        private void ShowCompanyInformation(Company company)
        {
            tb_name.Text=company.Name; 
            tb_lastname.Visibility = Visibility.Collapsed;
            tb_middlename.Visibility = Visibility.Collapsed;
            tb_curp.Visibility = Visibility.Collapsed;

            lb_curp.Visibility = Visibility.Collapsed;
            lb_Lastname.Visibility = Visibility.Collapsed;
            lb_middlename.Visibility = Visibility.Collapsed;
        }

        private void GetAccountInformation()
        {
            SGPMService.BankAccountManagementClient bankClient  = new SGPMService.BankAccountManagementClient();
            BankAccount bankAccount = bankClient.GetBankAccount(Beneficiary.Id);
            if(bankAccount != null)
            {
                tb_account_holder.Text = bankAccount.Name; 
                tb_account_number.Text = bankAccount.AccountNumber;
                btn_add_account.Content = "Modificar cuenta";
                HaveBankAccount = true;
                
            }

        }
        #endregion



        #region input configurations
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text);
        }

        private bool AreAllValidNumericChars(string str)
        {
            foreach (char c in str)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

       
        #endregion

        private bool validateData()
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(tb_phone.Text))
            {
                tb_phone.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else
            {
                tb_phone.BorderBrush = Brushes.Black;
            }

            if (string.IsNullOrWhiteSpace(tb_city.Text))
            {
                tb_city.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else
            {
                tb_city.BorderBrush = Brushes.Black;
            }
            if (string.IsNullOrWhiteSpace(tb_street.Text))
            {
                tb_street.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else
            {
                tb_street.BorderBrush = Brushes.Black;
            }

            return isValid;
        }

        private void Btn_Update_Beneficiary(object sender, RoutedEventArgs e)
        {
            if (validateData())
            {
                Beneficiary updatedBeneficiary = new Beneficiary
                {
                    Id = Beneficiary.Id,
                    Street = tb_street.Text,
                    City = tb_city.Text,
                    PhoneNumber = tb_phone.Text,

                };
                client = new BeneficiaryManagementClient();
                try
                {
                    int result = client.setBeneficiaryDetails(updatedBeneficiary);
                    if (result != -1) 
                    {
                        MessageBox.Show("La información del beneficiario se ha guardado", "Actualización realizada",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        GUI_BeneficiaryList gUI_Beneficiary = new GUI_BeneficiaryList();
                        gUI_Beneficiary.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("No se ha podido establecer conexión con la base de datos", "Actualización fallida",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (TimeoutException ex)
                {
                    MessageBox.Show("No fue posible establecer conexión con el servidor, por favor inténtelo más tarde",
                        "Problema de conexión con el servidor", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                MessageBox.Show("Se han identificado celdas sin llenar", "Información incompleta", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Btn_Goback_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult cancelationResult = MessageBox.Show("¿Deseas volver a la ventana anterior? Todos los" +
                "cambios realizados serán descartados",
                    "Confirmar cancelación", MessageBoxButton.OKCancel);

            if (cancelationResult == MessageBoxResult.OK)
            {
                GUI_BeneficiaryList gUI_Beneficiary = new GUI_BeneficiaryList();
                gUI_Beneficiary.Show();
                this.Close();
            }
        }

        private void Btn_AddAccount_Click(object sender, RoutedEventArgs e)
        {
            GUI_SaveBankAccount bankAccount = new GUI_SaveBankAccount(this, Beneficiary.Id);
            try
            {
                bankAccount.ShowDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
