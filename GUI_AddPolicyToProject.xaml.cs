using SGMP_Client.SGPMService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SGMP_Client
{
    /// <summary>
    /// Lógica de interacción para GUI_AddPolicyToProject.xaml
    /// </summary>
    public partial class GUI_AddPolicyToProject : Window
    {
        private SGPMService.PolicyManagementClient client = new SGPMService.PolicyManagementClient();
        private string folio;
        private Window parentWindow;

        public GUI_AddPolicyToProject(Window parent, string folio)
        {
            this.folio = folio;
            int[] list = null;
            parentWindow = parent;
            InitializeComponent();

            try
            {
                AddPolicyVisual();
                list = client.GetPolicysOfProject(folio);
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                erroServer();
            }
            catch (TimeoutException ex)
            {
                erroServer();
            }


            if (list != null)
            {
                MarkSelectedValues(list);
            }
        }

        private void AddPolicyVisual()
        {
            Policy[] policyList = null;

            try
            {
                policyList  = client.GetAllPolicies();
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                erroServer();
            }
            catch (TimeoutException ex)
            {
                erroServer();
            }
            
            foreach (var policy in policyList)
            {
                var border = new Border();
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(1);
                border.CornerRadius = new CornerRadius(5);
                border.Padding = new Thickness(5);
                border.Margin = new Thickness(0, 0, 0, 10);
                border.Width = 480;

                var stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Vertical;

                var innerStackPanel = new StackPanel();
                innerStackPanel.Orientation = Orientation.Horizontal;

                var toggleButton = new ToggleButton();
                toggleButton.Style = (Style)FindResource("SelectableToggleButtonStyle");
                toggleButton.HorizontalAlignment = HorizontalAlignment.Left;
                toggleButton.Margin = new Thickness(5);
                toggleButton.VerticalAlignment = VerticalAlignment.Center;
                toggleButton.Width = 20;
                toggleButton.Height = 20;
                toggleButton.Tag = policy.PolicyID;

                var nameLabel = new Label();
                nameLabel.Content = policy.Name;
                nameLabel.HorizontalAlignment = HorizontalAlignment.Left;
                nameLabel.Margin = new Thickness(5);
                nameLabel.FontWeight = FontWeights.Bold;
                nameLabel.FontSize = 10;
                nameLabel.VerticalAlignment = VerticalAlignment.Center;

                var descriptionLabel = new TextBlock();
                descriptionLabel.Text = policy.Description;
                descriptionLabel.HorizontalAlignment = HorizontalAlignment.Left;
                descriptionLabel.Margin = new Thickness(5);
                descriptionLabel.FontSize = 10;
                descriptionLabel.TextWrapping = TextWrapping.Wrap;
                descriptionLabel.MaxWidth = 480;

                innerStackPanel.Children.Add(toggleButton);
                innerStackPanel.Children.Add(nameLabel);

                stackPanel.Children.Add(innerStackPanel);
                stackPanel.Children.Add(descriptionLabel);

                border.Child = stackPanel;

                WrpPolicyList.Children.Add(border);
            }

        }

        private List<int> GetSelectedValues()
        {
            var selectedValues = new List<int>();

            foreach (var item in WrpPolicyList.Children)
            {
                if (item is Border border)
                {
                    var stackPanel = border.Child as StackPanel;
                    if (stackPanel != null)
                    {
                        var innerStackPanel = stackPanel.Children[0] as StackPanel;
                        if (innerStackPanel != null)
                        {
                            var toggleButton = innerStackPanel.Children[0] as ToggleButton;
                            if (toggleButton != null && toggleButton.IsChecked == true)
                            {
                                if (toggleButton.Tag is int intValue)
                                {
                                    selectedValues.Add(intValue);
                                }
                            }
                        }
                    }
                }
            }

            return selectedValues;
        }

        private void MarkSelectedValues(int[] selectedValues)
        {
            foreach (var item in WrpPolicyList.Children)
            {
                if (item is Border border)
                {
                    var stackPanel = border.Child as StackPanel;
                    if (stackPanel != null)
                    {
                        var innerStackPanel = stackPanel.Children[0] as StackPanel;
                        if (innerStackPanel != null)
                        {
                            var toggleButton = innerStackPanel.Children[0] as ToggleButton;
                            if (toggleButton != null && toggleButton.Tag is int intValue)
                            {
                                if (selectedValues.Contains(intValue))
                                {
                                    toggleButton.IsChecked = true;
                                }
                                else
                                {
                                    toggleButton.IsChecked = false;
                                }
                            }
                        }
                    }
                }
            }
        }


        private void SavePolicyToProject(object sender, RoutedEventArgs e)
        {
            var list = GetSelectedValues();
            int[] array = list.ToArray();
            int result = 0;
            try
            {
                result = client.AddPolicyToProject(folio, array);
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

            if(result >= 1)
            {
                MessageBox.Show("Se han registrado correctamente las politicas");
                this.Close();
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error al intentar guardar las politicas, Intente nuevamente más tarde", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Seguro que desea cancelar el registro?", 
                "Confirmar salida", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                this.Close();
            }
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
