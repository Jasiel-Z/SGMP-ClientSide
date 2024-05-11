using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Lógica de interacción para GUI_EditarProyecto.xaml
    /// </summary>
    public partial class GUI_ListaProyecto : Window
    {

        private SGPMService.ProjectsManagementClient ProjectsManagementClient = new SGPMService.ProjectsManagementClient();
        public GUI_ListaProyecto()
        {
            InitializeComponent();
            DisplayProjectList();
        }

        private void DisplayProjectList()
        {
            var f = ProjectsManagementClient.GetAllProjects();
            Console.WriteLine(f.Length);
            foreach (var project in f)
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

                // Crear TextBlock
                var textBlock = new TextBlock();
                textBlock.Text = project.Description.Length > 200 ? project.Description.Substring(0, 200) : project.Description;
                textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock.Margin = new Thickness(119, 45, 0, 0);
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.VerticalAlignment = VerticalAlignment.Top;
                textBlock.Height = 50;
                textBlock.Width = 702;
                textBlock.FontSize = 14;

                // Crear Button
                var button = new Button();
                button.Content = "Ver más";
                button.HorizontalAlignment = HorizontalAlignment.Left;
                button.Margin = new Thickness(800, 100, 0, 0);
                button.VerticalAlignment = VerticalAlignment.Top;
                button.Width = 100;
                button.Height = 29;
                button.Background = Brushes.LightGreen;
                button.Foreground = Brushes.White;

                button.Click += (sender, e) => {
                    GUI_RegistroProyecto registroProyecto = new GUI_RegistroProyecto(project.Folio);
                    this.Close();
                    registroProyecto.Show();
                };

                // Agregar elementos al Grid
                grid.Children.Add(image);
                grid.Children.Add(label);
                grid.Children.Add(textBlock);
                grid.Children.Add(button);

                // Agregar Grid al Border
                border.Child = grid;

                WrpProjectList.Children.Add(border);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GUI_RegistroProyecto registroProyecto = new GUI_RegistroProyecto("");
            this.Close();
            registroProyecto.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GUI_MainMenu menu = new GUI_MainMenu();
            menu.Show();
            this.Close();
        }
    }
}
