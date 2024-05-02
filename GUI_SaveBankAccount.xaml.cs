using SGMP_Client.SGPMService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica de interacción para GUI_SaveBankAccount.xaml
    /// </summary>
    /// 

    public partial class GUI_SaveBankAccount : Window
    {
        private SGPMService.BankAccountManagementClient BankAccountManager = new SGPMService.BankAccountManagementClient();
        private Collection<bool> AccountValidate = new Collection<bool>();
        private int IdBeneficary;


        public GUI_SaveBankAccount(int _IdBeneficiary)
        {
            InitializeComponent();
            IdBeneficary = _IdBeneficiary;
            AccountValidate.Add(false);
            AccountValidate.Add(false);

            var account = BankAccountManager.GetBankAccount(IdBeneficary);
            if (account != null)
            {
                txtAccount.Text = account.AccountNumber;
                txtName.Text = account.Name;
                btnSave.Visibility = Visibility.Collapsed;
                btnUpdate.Visibility = Visibility.Visible;
                btnUpdate.IsEnabled = true;

            }
        }

        private void ValidateNameInput(object sender, KeyEventArgs e)
        {
            AccountValidate[0] = false;
            btnSave.IsEnabled = false;
            btnUpdate.IsEnabled = false;
            var name = txtName.Text.Trim();
            if (name.Length > 9)
            {
                if (!Regex.IsMatch(name, @"^[\p{L} \.\-]+$"))
                {
                    lblErrorName.Visibility = Visibility.Visible;
                }
                else
                {
                    lblErrorName.Visibility = Visibility.Collapsed;
                    AccountValidate[0] = true;
                }
            }
            if (AccountValidate[0] == true && AccountValidate[1] == true)
            {
                btnSave.IsEnabled = true;
                btnUpdate.IsEnabled = true;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            BankAccount account = new BankAccount
            {
                Name = txtName.Text,
                AccountNumber = txtAccount.Text,
                IdBeneficiary = IdBeneficary
            };
            var result = BankAccountManager.SaveBankAccount(account);
            if (result == 0)
            {
                MessageBox.Show("Se ha registrado correctamente la cuenta bancaria", "Cambios Guardados", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("A ocurrido un error, intente nuevamente más tarde", "Cambios No Guardados", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ValidateAccountInput(object sender, KeyEventArgs e)
        {
            lblErrorAccountLength.Visibility = Visibility.Hidden;
            btnSave.IsEnabled = false;
            btnUpdate.IsEnabled = false;
            AccountValidate[1] = false;
            var account = txtAccount.Text.Trim();
            if (account.Length != 18)
            {
                lblErrorAccountLength.Visibility = Visibility.Visible;
            }
            else
            {
                AccountValidate[1] = true;
            }

            if (AccountValidate[0] == true && AccountValidate[1] == true)
            {
                btnSave.IsEnabled = true;
                btnUpdate.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Seguro que desea cancelar el registro?", "Confirmar salida", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                this.Close();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            BankAccount account = new BankAccount
            {
                Name = txtName.Text,
                AccountNumber = txtAccount.Text,
                IdBeneficiary = IdBeneficary
            };
            var result = BankAccountManager.UpdateBankAccount(account);
            if (result == 0)
            {
                MessageBox.Show("Se ha actualizado correctamente la cuenta bancaria", "Cambios Guardados", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("A ocurrido un error, intente nuevamente más tarde", "Cambios No Guardados", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
