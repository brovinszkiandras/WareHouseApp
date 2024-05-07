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
using WH_APP_GUI.Warehouse;

namespace WH_APP_GUI.sectors
{
    public partial class sectorIndexWindow : Page
    {
        private DataRow Warehouse = null;
        public sectorIndexWindow(DataRow warehouse)
        {
            InitializeComponent();
            Warehouse = warehouse;
            Navigation.ReturnParam = warehouse;
            DisplaySectors();

            if (User.DoesHavePermission("Modify all Warehouses"))
            {
                Create.Visibility = Visibility.Visible;
            }
            else if (User.DoesHavePermission("Modify Warehouses"))
            {
                if (User.currentUser.Table.TableName == "employees")
                {
                    if ((int)User.currentUser["warehouse_id"] == (int)Warehouse["id"])
                    {
                        Create.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Create.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                Create.Visibility = Visibility.Collapsed;
            }
        }

        private void DisplayOneSector(DataRow sector, int lastRow)
        {
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = GridLength.Auto;
            sectorGrid.RowDefinitions.Add(rowDefinition);

            TextBlock name = new TextBlock();
            name.Text = sector["name"].ToString();
            name.TextWrapping = TextWrapping.Wrap;
            name.Foreground = Brushes.White;
            name.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(name, lastRow);
            Grid.SetColumn(name, 0);

            sectorGrid.Children.Add(name);

            TextBlock length = new TextBlock();
            length.Text = sector["length"].ToString();
            length.Foreground = Brushes.White;
            length.TextWrapping = TextWrapping.Wrap;
            length.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(length, lastRow);
            Grid.SetColumn(length, 1);

            sectorGrid.Children.Add(length);

            TextBlock width = new TextBlock();
            width.Text = sector["width"].ToString();
            width.Foreground = Brushes.White;
            width.TextWrapping = TextWrapping.Wrap;
            width.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(width, lastRow);
            Grid.SetColumn(width, 2);

            sectorGrid.Children.Add(width);

            TextBlock area = new TextBlock();

            area.Text = sector["area"].ToString();
            area.Foreground = Brushes.White;
            area.TextWrapping = TextWrapping.Wrap;
            area.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(area, lastRow);
            Grid.SetColumn(area, 3);

            sectorGrid.Children.Add(area);

            TextBlock area_in_use = new TextBlock();
            area_in_use.Text = sector["area_in_use"].ToString();
            area_in_use.Foreground = Brushes.White;
            area_in_use.TextWrapping = TextWrapping.Wrap;
            area_in_use.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(area_in_use, lastRow);
            Grid.SetColumn(area_in_use, 4);

            sectorGrid.Children.Add(area_in_use);

            Button inspect = new Button();
            inspect.Content = "Inspect";
            inspect.Style = (Style)this.Resources["GoldenButtonStyle"];
            inspect.Margin = new Thickness(5);
            inspect.Tag = sector["id"];
            inspect.Click += Inspect_Click;
            Grid.SetRow(inspect, lastRow);
            Grid.SetColumn(inspect, 6);

            sectorGrid.Children.Add(inspect);

            if (User.DoesHavePermission("Modify all Warehouses"))
            {
                Button delete = new Button();
                delete.Content = "Delete";
                delete.Style = (Style)this.Resources["GoldenButtonStyle"];
                delete.Margin = new Thickness(5);
                delete.Tag = sector["id"];
                delete.Click += Delete_Click;
                Grid.SetRow(delete, lastRow);
                Grid.SetColumn(delete, 7);

                sectorGrid.Children.Add(delete);
            }
            else if (User.DoesHavePermission("Modify Warehouses"))
            {
                if (User.currentUser.Table.TableName == "employees")
                {
                    if (User.currentUser["warehouse"].ToString() == Warehouse["id"].ToString())
                    {
                        Button delete = new Button();
                        delete.Content = "Delete";
                        delete.Style = (Style)this.Resources["GoldenButtonStyle"];
                        delete.Margin = new Thickness(5);
                        delete.Tag = sector["id"];
                        delete.Click += Delete_Click;
                        Grid.SetRow(delete, lastRow);
                        Grid.SetColumn(delete, 7);

                        sectorGrid.Children.Add(delete);
                    }
                }
            }
        }

        public void DisplaySectors()
        {
            sectorGrid.Children.Clear();
            int lastRow = 0;
            if (Warehouse != null)
            {
                foreach (DataRow sector in Tables.sector.database.Select($"warehouse_id = {Warehouse["id"]}"))
                {
                    DisplayOneSector(sector, lastRow);
                    lastRow++;
                }
            }
            else
            {
                foreach (DataRow sector in Tables.sector.database.Rows)
                {
                    DisplayOneSector(sector, lastRow);
                    lastRow++;
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            MessageBoxResult result = MessageBox.Show("Do you want to delete this transport?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
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
            if (Warehouse != null)
            {
                Navigation.ReturnParam = Warehouse;
            }
            Navigation.content2.Navigate(createSectorPage);
            Navigation.ReturnParam = Warehouse;
        }
        private void Inspect_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            DataRow sector = Tables.sector.database.Select($"id = '{button.Tag}'")[0];
            Visual.sector = sector;

            if (Warehouse != null)
            {
                Navigation.ReturnParam = Warehouse;
            }
            else
            {
                Navigation.SkipParam = true;
            }
            Navigation.PreviousPage = this;
            SectorWindow page = new SectorWindow();
            Navigation.content2.Navigate(page);
        }
        private void SectorPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Navigation.PreviousPage != null)
            {
                if (Warehouse != null)
                {
                    InspectWarehouse inspectWarehouse = new InspectWarehouse(Warehouse);
                    Navigation.PreviousPage = inspectWarehouse;
                    Navigation.OpenPage(Navigation.PreviousPage.GetType(), Warehouse);
                }
                else
                {
                    Navigation.OpenPage(Navigation.PreviousPage.GetType());
                }
            }
            else
            {
                Navigation.OpenPage(Navigation.GetTypeByName("InspectWarehouse"));
            }
        }
    }
}
