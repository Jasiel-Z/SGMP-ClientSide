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
    /// Lógica de interacción para GUI_BeneficiaryList.xaml
    /// </summary>
    public partial class GUI_BeneficiaryList : Window
    {
        private SGPMService.BeneficiaryManagementClient Client;
        private List<Beneficiary> Beneficiaries { get; set; }
        public GUI_BeneficiaryList()
        {
            InitializeComponent();
            Client = new SGPMService.BeneficiaryManagementClient();
            Beneficiaries = new List<Beneficiary>();
            GetBeneficiaries();
        }

        private void Btn_Go_Register(object sender, RoutedEventArgs e)
        {
            GUI_BeneficiaryDetails gUI_Beneficiary = new GUI_BeneficiaryDetails();
           gUI_Beneficiary.Show();
            this.Close();   
            
        }

        private void Btn_Go_MainMenu(object sender, RoutedEventArgs e)
        {
            GUI_MainMenu gUI_Main = new GUI_MainMenu();
            gUI_Main.Show();
            this.Close();   
        }


        private void GetBeneficiaries()
        {
            //mover cuando se tenga el id del empleado
            int localityId = 1;

            try
            {
                Beneficiaries = Client.GetBeneficiaries().ToList();

                if (Beneficiaries != null)
                {
                    foreach (Beneficiary beneficiary in Beneficiaries)
                    {
                        liv_requests.Items.Add(beneficiary);
                    }
                }
                else
                {
                    MessageBox.Show("No se pudieron recuperar los registros, por favor inténtelo más tarde"
                     , "Error de conexión con la base de datos");
                }
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show("No fue posible establecer conexión con el servidor, por favor inténtelo más tarde",
                   "Problema de conexión con el servidor", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void liv_requests_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Beneficiary selectedBeneficiary = (Beneficiary)liv_requests.SelectedItem;

            if (selectedBeneficiary != null)
            {
                GUI_UpdateBeneficiary gUI_Update = new GUI_UpdateBeneficiary(selectedBeneficiary);
                gUI_Update.Show();
                this.Close();
            }
        }

        private bool ValidateData()
        {
            bool validData = true;



            return validData;
        }
    }
}
