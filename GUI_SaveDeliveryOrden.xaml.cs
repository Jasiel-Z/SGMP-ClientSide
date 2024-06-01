
using SGMP_Client.SGPMService;
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
    /// Lógica de interacción para GUI_SaveDeliveryOrden.xaml
    /// </summary>
    public partial class GUI_SaveDeliveryOrden : Window
    {
        private SGPMService.DeliveryOrdenManagementClient _client = new SGPMService.DeliveryOrdenManagementClient();
        private string Folio;
        private int idDeliveryOrden = 0;
        public GUI_SaveDeliveryOrden(string folio)
        {
            Folio = folio;
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            FillComboBox();
            var orden = _client.GetDeliveryOrden(Folio);
            if (orden != null)
            {
                idDeliveryOrden = orden.IdDeliveryOrden;
                txtPlace.Text = orden.DeliveryPlace;
                txtAmount.Text = orden.Amount;
                dtpDate.SelectedDate = orden.DeliveryDate;
                cmbResource.SelectedValue = orden.Resource.IdResource;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DeliveryOrden deliveryOrden = new DeliveryOrden
            {
                DeliveryPlace = txtPlace.Text,
                DeliveryDate = dtpDate.DisplayDate,
                Amount = txtAmount.Text
            };
            if (idDeliveryOrden != 0)
            {
                deliveryOrden.IdDeliveryOrden = idDeliveryOrden;
            }
            Resource Resource = new Resource
            {
                IdResource = (int)cmbResource.SelectedValue
            };
            deliveryOrden.Resource = Resource;
            int result = _client.SaveDeliveryOrden(deliveryOrden, Folio);
            if(result == 0)
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
            cmbResource.Items.Clear();

            var Resources = _client.GetResources();
            foreach (var resource in Resources)
            {
                cmbResource.Items.Add(resource);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
