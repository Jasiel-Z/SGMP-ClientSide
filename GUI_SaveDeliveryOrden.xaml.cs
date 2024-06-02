
using SGMP_Client.SGPMService;
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
    /// Lógica de interacción para GUI_SaveDeliveryOrden.xaml
    /// </summary>
    public partial class GUI_SaveDeliveryOrden : Window
    {
        private SGPMService.DeliveryOrdenManagementClient _client = new SGPMService.DeliveryOrdenManagementClient();
        private string Folio;
        private int idDeliveryOrden = 0;
        private string amount;
        private DateTime startDate;
        private DateTime endDate;
        private Window parentWindow;

        public GUI_SaveDeliveryOrden(Window parent, string folio, string _amount, DateTime start, DateTime end)
        {
            this.parentWindow = parent;
            this.Folio = folio;
            DeliveryOrden orden = null;
            this.amount = _amount;
            this.startDate = start;
            this.endDate = end;

            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            FillComboBox();
            txtAmount.Text = _amount;

            try
            {
                orden = _client.GetDeliveryOrden(Folio);
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                erroServer();
            }
            catch (TimeoutException ex)
            {
                erroServer();
            }
            
            
            if (orden != null)
            {
                idDeliveryOrden = orden.IdDeliveryOrden;
                txtPlace.Text = orden.DeliveryPlace;
                dtpDate.SelectedDate = orden.DeliveryDate;
                cmbResource.SelectedValue = orden.Resource.IdResource;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int result = 0;
            DeliveryOrden deliveryOrden = new DeliveryOrden
            {
                DeliveryPlace = txtPlace.Text,
                DeliveryDate = dtpDate.DisplayDate,
                Amount = amount
            };
            Resource Resource = new Resource
            {
                IdResource = (int)cmbResource.SelectedValue
            };
            deliveryOrden.Resource = Resource;

            if (idDeliveryOrden != 0)
            {
                deliveryOrden.IdDeliveryOrden = idDeliveryOrden;
            }


            try
            {
                result = _client.SaveDeliveryOrden(deliveryOrden, Folio);
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                erroServer();
                return;

            }
            catch (TimeoutException ex)
            {
                erroServer();
                return;
            }


            if (result > 0)
            {
                MessageBox.Show("Se ha guardado correctamente la orden de entrega", "Cambios Guardados", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("A ocurrido un error, intente nuevamente más tarde", "Cambios No Guardados", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void FillComboBox()
        {
            Resource[] Resources = null;
            try
            {
                Resources = _client.GetResources();
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                erroServer();
            }
            catch (TimeoutException ex)
            {
                erroServer();
            }

            cmbResource.Items.Clear();
            if (Resources.Length > 0)
            {
                foreach (var resource in Resources)
                {
                    cmbResource.Items.Add(resource);
                }
            }
            else
            {
                MessageBox.Show("Actualmente no hay recursos registrados en la base de datos, Intente nuevamente más tarde",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                       "¿Seguro de que desea descartar los cambios realizados?",
                       "Confirmar cancelación de cambios",
                       MessageBoxButton.OKCancel,
                       MessageBoxImage.Question
                   );

            if (result == MessageBoxResult.OK)
            {
                this.Close();
            }
        }

        private void FieldChanged(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = IsFormValid();
        }

        private bool IsFormValid()
        {
            bool isPlaceValid = !string.IsNullOrWhiteSpace(txtPlace.Text);
            bool isDateValid = dtpDate.SelectedDate.HasValue;
            bool isResourceValid = cmbResource.SelectedValue != null;
            bool isAmountValid = !string.IsNullOrWhiteSpace(txtAmount.Text);

            if (isDateValid)
            {
                isDateValid = ValidateDate(dtpDate.SelectedDate.Value);
            }

            return isPlaceValid && isDateValid && isResourceValid && isAmountValid;
        }

        private bool ValidateDate(DateTime deliveryDate)
        {
            bool result = true;
            if (deliveryDate < startDate || deliveryDate > endDate)
            {
                result = false;
                MessageBox.Show(string.Format("La fecha de entrega debe ser entre las fechas {0} y  {1}", startDate.Date, endDate.Date),
                "Fecha no valida", MessageBoxButton.OK, MessageBoxImage.Warning);
                dtpDate.SelectedDate = null;
            }

            return result;
        }

        private void erroServer()
        {
            MessageBox.Show("Ha ocurrido un error al intentar conectarse al servidor, Intente nuevamente más tarde",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            GUI_Login login = new GUI_Login();
            login.Show();
            this.parentWindow.Close();
            this.Close();
        }
    }
}
