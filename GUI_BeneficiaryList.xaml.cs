using SGMP_Client.SGPMReference;
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
        private SGPMReference.BeneficiaryManagementClient Client;
        private List<Beneficiary> Beneficiaries { get; set; }
        public GUI_BeneficiaryList()
        {
            InitializeComponent();
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
            Beneficiaries = Client.GetRequestsOfProject(folio).ToList();

            if (Beneficiaries != null)
            {
                foreach (Request request in Requests)
                {
                    liv_requests.Items.Add(request);
                }
            }
            else
            {
                MessageBox.Show("No se pudieron recuperar los registros, por favor inténtelo más tarde"
                 , "Error de conexión con la base de datos");
            }
        }
    }
}
