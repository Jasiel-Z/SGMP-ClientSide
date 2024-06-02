using SGMP_Client.SGPMService;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
			this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

		}

		private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult cancelationResult = MessageBox.Show("¿Deseas volver a la ventana anterior? Todos los" +
				"cambios realizados serán descartados", "Confirmar cancelación", MessageBoxButton.OKCancel,
				MessageBoxImage.Question);

			if (cancelationResult == MessageBoxResult.OK)
			{
				GUI_BeneficiaryList gUI_Beneficiary = new GUI_BeneficiaryList();
				gUI_Beneficiary.Show();
				this.Close();
			}
		}



		private void Btn_Register_Beneficiary(object sender, RoutedEventArgs e)
		{
			if (!IsOptionSelected())
			{
				MessageBox.Show("Seleccione el tipo de beneficiario que desea registrar", "Información faltante",
					MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else
			{

				if (rb_person.IsChecked == true)
				{
					if (validatePersonInformation() && validateBeneficiaryInformation())
					{
						registerPerson();
					}
					else
					{
						MessageBox.Show("Se han encontrado celdas vacías o valores no válidos, por favor ingresa toda la información",
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
						MessageBox.Show("Se han encontrado celdas vacías o valores inválidos, por favor ingresa toda la información",
							"Información faltante", MessageBoxButton.OK, MessageBoxImage.Warning);
					}
				}
			}
		}


		private void registerPerson()
		{
            try
            {
                SGPMService.BeneficiaryManagementClient client = new BeneficiaryManagementClient();
                if (client.CurpInUse(tb_curp.Text))
                {
                    MessageBox.Show("La CURP que ha ingresado se encuentra en uso dentro del sistema", "CURP en uso",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (!string.IsNullOrWhiteSpace(tb_rfc.Text) && client.RfcInUse(tb_rfc.Text))
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
                        LocalityId = DTO_s.User.UserClient.LocationId
                    };

                    Person newPerson = new Person
                    {
                        Name = tb_name.Text,
                        LastName = tb_lastname.Text,
                        SurName = tb_middlename.Text,
                        CURP = tb_curp.Text,
                    };

                    if (!string.IsNullOrWhiteSpace(tb_rfc.Text))
                    {
                        newBeneficiary.RFC = tb_rfc.Text;
                    }

                    int result = client.RegisterPerson(newBeneficiary, newPerson);
                    if (result != -1)
                    {
                        MessageBox.Show("El beneficiario se ha registrado en el sistema", "Registro realizado",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        Window guiBeneficiaries = new GUI_BeneficiaryList();
                        this.Close();
                        guiBeneficiaries.Show();
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
				SGPMService.BeneficiaryManagementClient client = new BeneficiaryManagementClient();
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
						LocalityId = DTO_s.User.UserClient.LocationId
					};

					Company newCompany = new Company
					{
						Name = tb_name.Text,
					};

					int result = client.RegisterCompany(newBeneficiary, newCompany);
					if (result != -1)
					{
						MessageBox.Show("El beneficiario se he registrado en el sistema", "Registro realizado",
							MessageBoxButton.OK, MessageBoxImage.Information);
						Window guiBeneficiaries = new GUI_BeneficiaryList();
						this.Close();
						guiBeneficiaries.Show();



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

            if (!EsCURPValido(tb_curp.Text))
            {
                tb_curp.BorderBrush = Brushes.Red;
                result = false;
            }
            else
            {
                tb_curp.BorderBrush = Brushes.Black;
                MessageBox.Show("CURP VÁLIDA", "CURP");

            }

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

			return result;
		}

		private bool validateBeneficiaryInformation()
		{
			bool result = true;

            if (!EsRFCValido(tb_rfc.Text))
            {
                tb_rfc.BorderBrush = Brushes.Red;
                Console.WriteLine(tb_rfc.Text);
                result = false;
            }
            else
            {
                tb_rfc.BorderBrush = Brushes.Black;
            }

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
				tb_name.BorderBrush = Brushes.Black;
			}
			return result;

		}


		private bool EsNumeroTelefonoValido(string telefono)
		{
			return !string.IsNullOrWhiteSpace(telefono);
		}

		private bool EsRFCValido(string rfc)
		{

			string RFC = tb_rfc.Text;
			Console.WriteLine("Rfc recibido: " + tb_rfc.Text.ToString());

			if (string.IsNullOrWhiteSpace(rfc) && rb_person.IsChecked == true)
			{
				Console.WriteLine("RFC vacío para persona. Válido.");
				return true;
			}


			string rfcPersonaFisicaPattern = @"^[A-ZÑ&]{4}\d{6}[A-Z0-9]{3}$";
			string rfcPersonaMoralPattern = @"^[A-ZÑ&]{3}\d{6}[A-Z0-9]{3}$";

			if (rb_company.IsChecked == true)
			{
				Console.WriteLine("Entró a validar rfc de empresa");
				if (Regex.IsMatch(RFC, rfcPersonaMoralPattern))
				{
					return true;
				}
				else
				{
					Console.WriteLine("El texto no coincide con el patron");
				}
			}
			else if (rb_person.IsChecked == true)
			{
				Console.WriteLine("Entró a validar rfc de persona");

				if (Regex.IsMatch(RFC, rfcPersonaFisicaPattern))
				{
					return true;
				}
			}
			Console.WriteLine("No valida nada");


			return false;

		}


		private bool EsCURPValido(string curp)
		{
            curp = curp.Trim();

            if (curp.Length != 18)
            {
                Console.WriteLine("La CURP debe tener una longitud de 18 caracteres.");
                return false;
            }

            for (int i = 0; i < 4; i++)
            {
                if (!char.IsLetter(curp[i]))
                {
                    Console.WriteLine("Los primeros 4 caracteres de la CURP deben ser letras.");
                    return false;
                }
            }

            for (int i = 4; i < 10; i++)
            {
                if (!char.IsDigit(curp[i]))
                {
                    Console.WriteLine("Los siguientes 6 caracteres de la CURP deben ser dígitos.");
                    return false;
                }
            }

            return true;
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

		private void tb_rfc_TextChanged(object sender, TextChangedEventArgs e)
		{

		}
	}
}
