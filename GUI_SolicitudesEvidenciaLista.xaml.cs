using SGMP_Client.SGPMService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Lógica de interacción para GUI_SolicitudesEvidenciaLista.xaml
    /// </summary>
    public partial class GUI_SolicitudesEvidenciaLista : Window
    {
        private SGPMService.EvidenceManagementClient ProjectsManagementClient = new SGPMService.EvidenceManagementClient();
        private string IdProject;
        public GUI_SolicitudesEvidenciaLista()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            DisplayProjectList();
        }

        private void DisplayProjectList()
        {
            ProjectShow[] projectList;
            try
            {
                projectList = ProjectsManagementClient.GetFinishProjectsd();
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                ErroServer();
                return;
            }
            catch (TimeoutException ex)
            {
                ErroServer();
                return;
            }

            WrpProjectList.Children.Clear();
            foreach (var project in projectList)
            {
                var border = new Border();
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(1);
                border.CornerRadius = new CornerRadius(5);
                border.Padding = new Thickness(5);
                border.Margin = new Thickness(0, 0, 0, 10);

                // Crear Grid
                var grid = new Grid();
                grid.Height = 134;
                grid.Width = 935;
                grid.Background = Brushes.White;

                // Crear Image
                var image = new Image();
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.Height = 100;
                image.Margin = new Thickness(14, 15, 0, 0);
                image.VerticalAlignment = VerticalAlignment.Top;
                image.Width = 100;
                image.Source = new BitmapImage(new System.Uri("Icons/folder.png", System.UriKind.Relative));

                // Crear Label
                var label = new Label();
                label.Content = project.Name;
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.Margin = new Thickness(118, 3, 0, 0);
                label.VerticalAlignment = VerticalAlignment.Top;
                label.Width = 607;
                label.FontWeight = FontWeights.Bold;
                label.FontSize = 20;

                // Crear Button
                var button = new Button();
                button.Content = "Ver Solicitudes";
                button.HorizontalAlignment = HorizontalAlignment.Left;
                button.Margin = new Thickness(800, 100, 0, 0);
                button.VerticalAlignment = VerticalAlignment.Top;
                button.Width = 100;
                button.Height = 29;
                button.Background = Brushes.LightGreen;
                button.Foreground = Brushes.White;

                button.Click += (sender, e) => {
                    lblTitle.Content = "Lista de Solicitudes";
                    btnBack.Visibility = Visibility.Visible;
                    DisplayRequestList(project.Folio);
                    IdProject = project.Folio;
                };

                // Agregar elementos al Grid
                grid.Children.Add(image);
                grid.Children.Add(label);
                grid.Children.Add(button);

                // Agregar Grid al Border
                border.Child = grid;

                WrpProjectList.Children.Add(border);
            }
        }

        private void DisplayRequestList(string folio)
        {
            RequestShow[] requestList;

            try
            {
                requestList = ProjectsManagementClient.GetFinishRequestsd(folio);
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                ErroServer();
                return;
            }
            catch (TimeoutException ex)
            {
                ErroServer();
                return;
            }

            WrpProjectList.Children.Clear();
            foreach (var project in requestList)
            {
                var border = new Border();
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(1);
                border.CornerRadius = new CornerRadius(5);
                border.Padding = new Thickness(5);
                border.Margin = new Thickness(0, 0, 0, 10);

                // Crear Grid
                var grid = new Grid();
                grid.Height = 134;
                grid.Width = 935;
                grid.Background = Brushes.White;

                // Crear Image
                var image = new Image();
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.Height = 100;
                image.Margin = new Thickness(14, 15, 0, 0);
                image.VerticalAlignment = VerticalAlignment.Top;
                image.Width = 100;
                image.Source = new BitmapImage(new System.Uri("Icons/solicitud.png", System.UriKind.Relative));

                // Crear Label
                var label = new Label();
                label.Content = project.BeneficiaryName;
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.Margin = new Thickness(118, 3, 0, 0);
                label.VerticalAlignment = VerticalAlignment.Top;
                label.Width = 607;
                label.FontWeight = FontWeights.Bold;
                label.FontSize = 20;

                // Crear Button
                var button = new Button();
                button.Content = "Agregar Evidencia";
                button.HorizontalAlignment = HorizontalAlignment.Left;
                button.Margin = new Thickness(800, 100, 0, 0);
                button.VerticalAlignment = VerticalAlignment.Top;
                button.Width = 100;
                button.Height = 29;
                button.Background = Brushes.LightGreen;
                button.Foreground = Brushes.White;

                button.Click += (sender, e) => {
                    GUI_SubirEvidencia uploadEvidence = new GUI_SubirEvidencia(this, project.Id, IdProject);
                    uploadEvidence.ShowDialog();

                };

                // Agregar elementos al Grid
                grid.Children.Add(image);
                grid.Children.Add(label);
                grid.Children.Add(button);

                // Agregar Grid al Border
                border.Child = grid;

                WrpProjectList.Children.Add(border);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lblTitle.Content = "Lista de Proyectos";
            btnBack.Visibility = Visibility.Hidden;
            DisplayProjectList();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GUI_MainMenu menu = new GUI_MainMenu();
            menu.Show();
            this.Close();
        }

        private void ErroServer()
        {
            MessageBox.Show("Ha ocurrido un error al intentar conectarse al servidor, Intente nuevamente más tarde", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            GUI_Login login = new GUI_Login();
            this.Close();
            login.Show();
        }
    }
}
