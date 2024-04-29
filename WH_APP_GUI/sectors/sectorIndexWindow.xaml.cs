using System;
using System.Collections.Generic;
using System.Data;
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
using WH_APP_GUI.transport;

namespace WH_APP_GUI.sectors
{
    /// <summary>
    /// Interaction logic for sectorIndexWindow.xaml
    /// </summary>
    public partial class sectorIndexWindow : Page
    {
        public void DisplaySectors()
        {
            sectorGrid.Children.Clear();
            int lastRow = 0;
            foreach (DataRow sector in Tables.sector.database.Rows)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                sectorGrid.RowDefinitions.Add(rowDefinition);

                TextBlock name = new TextBlock();
                name.Text = sector["name"].ToString();
                name.FontSize = 15;
                name.TextWrapping = TextWrapping.Wrap;
                name.Foreground = Brushes.White;
                name.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(name, lastRow);
                Grid.SetColumn(name, 0);

                sectorGrid.Children.Add(name);

                TextBlock length = new TextBlock();
                length.Text = sector["length"].ToString();
                length.FontSize = 15;
                length.Foreground = Brushes.White;
                length.TextWrapping = TextWrapping.Wrap;
                length.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(length, lastRow);
                Grid.SetColumn(length, 1);

                sectorGrid.Children.Add(length);

                TextBlock width = new TextBlock();
                width.Text = sector["width"].ToString();
                width.FontSize = 15;
                width.Foreground = Brushes.White;
                width.TextWrapping = TextWrapping.Wrap;
                width.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(width, lastRow);
                Grid.SetColumn(width, 2);

                sectorGrid.Children.Add(width);

                TextBlock area = new TextBlock();
                
                area.Text = sector["area"].ToString();
                area.FontSize = 15;
                area.Foreground = Brushes.White;
                area.TextWrapping = TextWrapping.Wrap;
                area.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(area, lastRow);
                Grid.SetColumn(area, 3);

                sectorGrid.Children.Add(area);

                TextBlock area_in_use = new TextBlock();
                area_in_use.Text = sector["area_in_use"].ToString();
                area_in_use.FontSize = 15;
                area_in_use.Foreground = Brushes.White;
                area_in_use.TextWrapping = TextWrapping.Wrap;
                area_in_use.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(area_in_use, lastRow);
                Grid.SetColumn(area_in_use, 4);

                sectorGrid.Children.Add(area_in_use);


                


                
                    Button inspect = new Button();
                    inspect.Content = "Inspect";
                    inspect.FontSize = 15;
                    inspect.Foreground = Brushes.White;
                    inspect.Background = Brushes.Green;
                    inspect.Tag = sector["id"];
                    inspect.Click += Inspect_Click;
                    Grid.SetRow(inspect, lastRow);
                    Grid.SetColumn(inspect, 5);

                    sectorGrid.Children.Add(inspect);
                

                if (User.DoesHavePermission("Modify Warehouse") || User.DoesHavePermission("Modify all Warehouse"))
                {
                    Button delete = new Button();
                    delete.Content = "Delete";
                    delete.FontSize = 15;
                    delete.Foreground = Brushes.White;
                    delete.Background = Brushes.Green;
                    delete.Tag = sector["id"];
                    delete.Click += Delete_Click;
                    Grid.SetRow(delete, lastRow);
                    Grid.SetColumn(delete, 7);

                    sectorGrid.Children.Add(delete);
                }
                else
                {
                    Create.Visibility = Visibility.Collapsed;
                }

                lastRow++;
            }
        }
        public sectorIndexWindow()
        {
            InitializeComponent();

            DisplaySectors();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Do you want to delete this transport?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show(button.Tag.ToString());
                DataRow sector = Tables.sector.database.Select($"id = '{button.Tag}'")[0];
                if (sector != null)
                {
                    

                    sector.Delete();
                    Tables.sector.updateChanges();



                    DisplaySectors();
                }
            }


        }
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            CreateSectorPage createSectorPage = new CreateSectorPage();


            Navigation.content2.Navigate(createSectorPage);
        }
        private void Inspect_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            DataRow sector = Tables.sector.database.Select($"id = '{button.Tag}'")[0];
            Visual.sector = sector;

            SectorWindow page = new SectorWindow();
            Navigation.content2.Navigate(page);
        }

    }
}
