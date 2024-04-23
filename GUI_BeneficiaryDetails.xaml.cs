using SGMP_Client.SGPMReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Lógica de interacción para GUI_BeneficiaryDetails.xaml
    /// </summary>
    public partial class GUI_BeneficiaryDetails : Window
    {
        public GUI_BeneficiaryDetails()
        {
            InitializeComponent();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            GUI_BeneficiaryList gUI_Beneficiary = new GUI_BeneficiaryList();
            gUI_Beneficiary.Show();
            this.Close();
        }

        private int registerBeneficiary()
        {
            return 1;
        }

        private void Btn_Register_Beneficiary(object sender, RoutedEventArgs e)
        {
            if(!IsOptionSelected())
            {
                MessageBox.Show("Seleccione el tipo de beneficiario que desea registrar", "Información faltante",
                    MessageBoxButton.OK,MessageBoxImage.Warning);
            }
            else
            {

                    if (rb_person.IsChecked == true)
                    {
                        if (validatePersonInformation() && validateBeneficiaryInformation())
                        {
                            registerBeneficiary();
                        }
                        else
                        {
                        MessageBox.Show("Se han encontrado celdas vacías, por favor ingresa toda la información",
                                "Información faltante", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }

                    else
                    {
                        if (validateCompanyInformation() && validateBeneficiaryInformation())
                        {
                            registerCompany();
                        }
                        else
                        {
                        MessageBox.Show("Se han encontrado celdas vacías, por favor ingresa toda la información",
                            "Información faltante", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }                 
            }
        }


        private void registerPerson()
        {
            try
            {
                SGPMReference.BeneficiaryManagementClient client = new BeneficiaryManagementClient();
                if (client.CurpInUse(tb_curp.Text))
                {
                    MessageBox.Show("La CURP que ha ingresado se encuentra en uso dentro del sistema", "CURP en uso",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (client.RfcInUse(tb_rfc.Text))
                {
                    MessageBox.Show("El RFC que ha ingresado se encuentra en uso dentro del sistema", "RFC en uso",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Beneficiary newBeneficiary = new Beneficiary
                    {
                        PhoneNumber = tb_phone.Text,
                        City = tb_city.Text,
                        Street = tb_street.Text,   
                        RFC = tb_rfc.Text,

                    };

                    Person newPerson = new Person
                    {
                        Name = tb_name.Text,
                        LastName = tb_lastname.Text,
                        SurName = tb_middlename.Text,
                        CURP = tb_curp.Text,    
                    };

                    int result = client.RegisterPerson(newBeneficiary, newPerson);
                    if (result > 1)
                    {
                        MessageBox.Show("El beneficiario se he registrado en el sistema", "Registro realizado",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se ha podido establecer conexión con la base de datos", "Registro fallido",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show("No fue posible establecer conexión con el servidor, por favor inténtelo más tarde",
                    "Problema de conexión con el servidor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void registerCompany()
        {
            try
            {
                SGPMReference.BeneficiaryManagementClient client = new BeneficiaryManagementClient();
                if (client.RfcInUse(tb_rfc.Text))
                {
                    MessageBox.Show("El RFC que ha ingresado se encuentra en uso dentro del sistema", "RFC en uso",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Beneficiary newBeneficiary = new Beneficiary
                    {
                        PhoneNumber = tb_phone.Text,
                        City = tb_city.Text,
                        Street = tb_street.Text,
                        RFC = tb_rfc.Text,

                    };

                    Company newCompany = new Company
                    {
                        Name = tb_name.Text,
                    };

                    int result = client.RegisterCompany(newBeneficiary, newCompany);
                    if(result > 1)
                    {
                        MessageBox.Show("El beneficiario se he registrado en el sistema", "Registro realizado",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se ha podido establecer conexión con la base de datos", "Registro fallido",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show("No fue posible establecer conexión con el servidor, por favor inténtelo más tarde",
                    "Problema de conexión con el servidor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region  Validations

        private bool validatePersonInformation()
        {
            bool result = true;

            if (string.IsNullOrWhiteSpace(tb_name.Text))
            {
                tb_name.BorderBrush = Brushes.Red;
                result = false;
            }
            else
            {
                tb_city.BorderBrush = Brushes.Black;
            }

            if (string.IsNullOrWhiteSpace(tb_lastname.Text))
            {
                tb_lastname.BorderBrush = Brushes.Red;
                result = false;
            }
            else
            {
                tb_lastname.BorderBrush = Brushes.Black;
            }
            if (string.IsNullOrWhiteSpace(tb_middlename.Text))
            {
                tb_middlename.BorderBrush = Brushes.Red;
                result = false;
            }
            else
            {
                tb_middlename.BorderBrush = Brushes.Black;
            }
            if (string.IsNullOrWhiteSpace(tb_curp.Text))
            {
                tb_curp.BorderBrush = Brushes.Red;
                result = false;
            }
            else
            {
                tb_curp.BorderBrush = Brushes.Black;
            }


            return result;
        }

        private bool validateBeneficiaryInformation()
        {

            bool result = true;
            if (!EsNumeroTelefonoValido(tb_phone.Text))
            {
                tb_phone.BorderBrush = Brushes.Red;
                result = false;
            }
            else
            {
                tb_phone.BorderBrush = Brushes.Black;
            }

            if (string.IsNullOrWhiteSpace(tb_city.Text))
            {
                tb_city.BorderBrush = Brushes.Red;
                result = false;
            }
            else
            {
                tb_city.BorderBrush = Brushes.Black;
            }

            if (!EsRFCValido(tb_rfc.Text))
            {
                tb_rfc.BorderBrush = Brushes.Red;
                result = false;
            }
            else
            {
                tb_rfc.BorderBrush = Brushes.Black; 
            }

            if (string.IsNullOrWhiteSpace(tb_street.Text))
            {
                tb_street.BorderBrush = Brushes.Red;
                result = false;
            }
            else
            {
                tb_street.BorderBrush = Brushes.Black;
            }

            return result;
        }

        private bool validateCompanyInformation()
        {
            bool result = true; 
            if (string.IsNullOrWhiteSpace(tb_name.Text))
            {
                tb_name.BorderBrush = Brushes.Red;
                result = false;
            }
            else
            {
                tb_name.BorderBrush= Brushes.Black;
            }
            return result;

        }


        private bool EsNumeroTelefonoValido(string telefono)
        {
            return !string.IsNullOrWhiteSpace(telefono);
        }

        private bool EsRFCValido(string rfc)
        {

            if (rb_company.IsChecked == true && !string.IsNullOrWhiteSpace(rfc))
            {
                return true;
            }
            else if (rb_person.IsChecked == true)
            {
                return true;
            }
            else
            {
                return false;
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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton clickedRadioButton = (RadioButton)sender;

            if (clickedRadioButton == rb_person)
            {
                tb_lastname.Visibility = Visibility.Visible;
                tb_middlename.Visibility = Visibility.Visible;
                tb_curp.Visibility = Visibility.Visible;
                lb_curp.Visibility = Visibility.Visible;
                lb_Lastname.Visibility = Visibility.Visible;
                lb_middlename.Visibility = Visibility.Visible;

            }
            else if (clickedRadioButton == rb_company)
            {
                lb_curp.Visibility = Visibility.Collapsed;
                lb_Lastname.Visibility = Visibility.Collapsed;
                lb_middlename.Visibility = Visibility.Collapsed;
                tb_curp.Visibility = Visibility.Collapsed;
                tb_lastname.Visibility = Visibility.Collapsed;
                tb_middlename.Visibility = Visibility.Collapsed;
            }
        }

        private bool IsOptionSelected()
        {
            return rb_company.IsChecked == true || rb_person.IsChecked == true;
        }

        #endregion


    }
}
