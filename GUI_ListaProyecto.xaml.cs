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
            foreach (var project in ProjectsManagementClient.GetProjectsFromLocality(1))
            {
                Border border = new Border
                {
                    Width = 600,
                    Height = 80,
                    Background = Brushes.LightBlue,
                    Margin = new Thickness(2),
                };

                Grid grid = new Grid();

                Button editarButton = new Button
                {
                    Content = "Editar",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                Ellipse ellipse = new Ellipse
                {
                    Width = 40,
                    Height = 40,
                    Margin = new Thickness(10, 0, 0, 0),
                };

                ImageBrush imageBrush = new ImageBrush();
                //imageBrush.ImageSource = new BitmapImage(new Uri(imagePath));
                //ellipse.Fill = imageBrush;

                Label lblNombreProyecto = new Label
                {
                    Content = project.Modality,
                    FontSize = 18,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(60, 0, 0, 0),
                };

                TextBlock txtDescripcion = new TextBlock
                {
                    Text = "Hola",//project.Description.Length > 100 ? project.Description.Substring(0, 100) : project.Description,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(60, 20, 0, 0),
                };

                editarButton.Click += (sender, e) => {
                    GUI_RegistroProyecto registroProyecto = new GUI_RegistroProyecto();
                    this.Close();
                    registroProyecto.Show();
                };

                grid.Children.Add(editarButton);
                grid.Children.Add(ellipse);
                grid.Children.Add(lblNombreProyecto);
                grid.Children.Add(txtDescripcion);

                border.Child = grid;

                WrpProjectList.Children.Add(border);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SGPMService.Project project = new SGPMService.Project();
            GUI_RegistroProyecto registroProyecto = new GUI_RegistroProyecto();
            this.Close();
            registroProyecto.Show();
        }
    }
}
