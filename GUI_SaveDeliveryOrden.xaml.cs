
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

        public GUI_SaveDeliveryOrden()
        {
            InitializeComponent();
            FillComboBox();
            var orden = _client.GetDeliveryOrden(3);
            if (orden != null)
            {
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
            Resource Resource = new Resource
            {
                IdResource = (int)cmbResource.SelectedValue
            };
            deliveryOrden.Resource = Resource;
            _client.SaveDeliveryOrden(deliveryOrden, "F1234");
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
    }
}
