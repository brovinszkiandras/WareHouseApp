using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using MahApps.Metro.IconPacks;
using Microsoft.Maps.MapControl.WPF;
using WH_APP_GUI.sectors;
using WH_APP_GUI.WarehouseTableFolder;

namespace WH_APP_GUI
{
    public partial class WarehousesPage : Page  
    {
        public void DisplayWarehousesOnPanel(Panel panel)
        {
            panel.Children.Clear();
            panel.Visibility = Visibility.Visible;
            for (int i = 0; i < Tables.warehouses.database.Rows.Count; i++)
            {
                Grid grid = new Grid();
                grid.Height = 150;

                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                Border border = new Border();
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(1);
                Grid.SetColumnSpan(border, 3);
                grid.Children.Add(border);

                PackIconMaterial icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Warehouse };
                icon.Height = 50;
                icon.Width = 50;
                icon.SetValue(Grid.RowSpanProperty, 3);
                grid.Children.Add(icon);

                Label label = new Label();
                label.Content = Tables.warehouses.database.Rows[i]["name"];
                Grid.SetColumn(label, 1);
                grid.Children.Add(label);

                Grid innerGrid = new Grid();
                innerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                innerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                Grid.SetColumn(innerGrid, 2);
                grid.Children.Add(innerGrid);

                Button inspectButton = new Button();
                inspectButton.Tag = Tables.warehouses.database.Rows[i];
                inspectButton.Content = "Inspect Warehouse";
                inspectButton.Click += inspect_warehouse_Click;
                inspectButton.Margin = new Thickness(5);
                Grid.SetRow(inspectButton, 0);
                innerGrid.Children.Add(inspectButton);

                Button deleteButton = new Button();
                deleteButton.Content = "Delete Warehouse";
                deleteButton.Tag = Tables.warehouses.database.Rows[i];
                deleteButton.Click += delete_warehouse_Click;
                deleteButton.Margin = new Thickness(5);
                Grid.SetRow(deleteButton, 1);
                innerGrid.Children.Add(deleteButton);

                panel.Children.Add(grid);
            }
        }
        public WarehousesPage()
        {
            InitializeComponent();
            DisplayWarehousesOnPanel(DisplayWarehousesStackpanel);
        }

        private void AddNewWarehouse_Click(object sender, RoutedEventArgs e)
        {
            DisplayWarehouses.Visibility = Visibility.Collapsed;
            DisplayWarehousesStackpanel.Visibility = Visibility.Collapsed;
            AddNewWarehouse.Visibility = Visibility.Collapsed;
            
            name.ValueDataType = typeof(string);

            WHlenghtLBL.Visibility = Visibility.Visible;
            Grid.SetRow(WHlenghtLBL, 3);
            length.Visibility = Visibility.Visible;
            length.ValueDataType = typeof(double);
            Grid.SetRow(length, 4);

            WHwidthLBL.Visibility = Visibility.Visible;
            Grid.SetRow(WHwidthLBL, 5);
            width.Visibility = Visibility.Visible;
            width.ValueDataType = typeof(double);
            Grid.SetRow(width, 6);

            WHheightLBL.Visibility = Visibility.Visible;
            Grid.SetRow(WHheightLBL, 7);
            height.Visibility = Visibility.Visible;
            height.ValueDataType = typeof(double);
            Grid.SetRow(height, 8);

            int index = 9;
            if (SQL.BoolQuery($"SElECT in_use FROM feature WHERE name = 'City'"))
            {
                city_id.Items.Clear();
                for (int i = 0; i < Tables.cities.database.Rows.Count; i++)
                {
                    city_id.Items.Add(Tables.cities.database.Rows[i]["city_name"]);
                }

                WHcityLBL.Visibility = Visibility.Visible;
                Grid.SetRow(WHcityLBL, index);
                index++;
                city_id.Visibility = Visibility.Visible;
                Grid.SetRow(city_id, index);
                index++;
            }
            else
            {
                WHcityLBL.Visibility = Visibility.Collapsed;
                city_id.Visibility = Visibility.Collapsed;
            }

            Grid.SetRow(CreateWarehouse, index);
            index++;
            Grid.SetRow(CancelCreation, index);

            AddNewWarehouseDisplay.Visibility = Visibility.Visible;
        }

        private void CreateWarehouse_Click(object sender, RoutedEventArgs e)
        {
            DataRow warehouse = Tables.warehouses.database.NewRow();
            if (SQL.BoolQuery($"SElECT in_use FROM feature WHERE name = 'City'"))
            {
                if (!Validation.ValidateTextbox(name, warehouse) && !Validation.ValidateTextbox(length, warehouse) && !Validation.ValidateTextbox(width, warehouse) && !Validation.ValidateTextbox(height, warehouse) && city_id.SelectedIndex != -1)
                {
                    warehouse["name"] = name.Text;
                    warehouse["length"] = length.Text;
                    warehouse["width"] = width.Text;
                    warehouse["height"] = height.Text;
                    warehouse["city_id"] = city_id.SelectedIndex + 1;
                    warehouse["volume"] = double.Parse(length.Text) * double.Parse(width.Text) * double.Parse(height.Text);

                    Controller.CreateWarehouse(name.Text);

                    Tables.warehouses.database.Rows.Add(warehouse);
                    Tables.warehouses.updateChanges();
                    Cancel();
                    AddNewWarehouse.Visibility = Visibility.Visible;

                    MessageBox.Show("Warehouse created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                if (!Validation.ValidateTextbox(name, warehouse) && !Validation.ValidateTextbox(length, warehouse) && !Validation.ValidateTextbox(width, warehouse) && !Validation.ValidateTextbox(height, warehouse))
                {
                    warehouse["name"] = name.Text;
                    warehouse["length"] = length.Text;
                    warehouse["width"] = width.Text;
                    warehouse["height"] = height.Text;
                    warehouse["volume"] = double.Parse(length.Text) * double.Parse(width.Text) * double.Parse(height.Text);
                    Tables.warehouses.database.Rows.Add(warehouse);
                    Tables.warehouses.updateChanges();
                    Cancel();
                    AddNewWarehouse.Visibility = Visibility.Visible;

                    MessageBox.Show("Warehouse created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }     
        }
        private void Cancel()
        {
            AddNewWarehouseDisplay.Visibility = Visibility.Collapsed;
            DisplayWarehouses.Visibility = Visibility.Visible;
            DisplayWarehousesStackpanel.Visibility = Visibility.Visible;
            AddNewWarehouse.Visibility = Visibility.Visible;
            DisplayWarehousesOnPanel(DisplayWarehousesStackpanel);

            MapDisplay.Children.Remove(terkep);

            name.Text = string.Empty;
            length.Text = string.Empty;
            width.Text = string.Empty;
            height.Text = string.Empty;
            city_id.SelectedIndex = -1;
        }
        private void CancelCreation_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }
        public DataRow SelectedWarehouse = null;
        public Map terkep = new Map();
        private void inspect_warehouse_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            SelectedWarehouse = btn.Tag as DataRow;
            if (SelectedWarehouse != null)
            {
                DisplayWarehouses.Visibility = Visibility.Collapsed;
                AddNewWarehouse.Visibility = Visibility.Collapsed;
                DisplayOneWarehouse.Visibility = Visibility.Visible;
                if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'City'"))
                {
                   
                    terkep.IsEnabled = true;
                    MapDisplay.Children.Add(terkep);
                    terkep.CredentialsProvider = new ApplicationIdCredentialsProvider("I28YbqAL3vpfFHWSLW5x~bGccdfvqXsmwkAA8zHurUw~Apx4iHJNCNHKm28KE8CDvxw6wAeIp4-8Yz1DDnwyIa81h9Obx4dD-xlgWz3mrIq8");

                    MapPolyline polyline = new MapPolyline();
                    polyline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
                    polyline.StrokeThickness = 5;
                    polyline.Opacity = 0.7;

                    double lat = double.Parse(Tables.warehouses.getCity(SelectedWarehouse)["latitude"].ToString());
                    double lon = double.Parse(Tables.warehouses.getCity(SelectedWarehouse)["longitude"].ToString());

                    terkep.Center = new Location(lat, lon);
                    terkep.ZoomLevel = 10;

                    terkep.Children.Add(polyline);
                }
                if(User.currentUser.Table == Tables.staff.database)
                {
                    User.tempWarehouse = SelectedWarehouse;
                }
            }
        }

        private void delete_warehouse_Click(object sender, RoutedEventArgs e)
        {

            Button btn = sender as Button;
            DataRow warehouse = btn.Tag as DataRow;
            if (warehouse != null)
            {
                try
                {
                    foreach (DataRow employee in Tables.warehouses.getEmployees(warehouse))
                    {
                        employee["warehouse_id"] = null;
                    }
                    Tables.employees.updateChanges();

                    if ((bool)Tables.features.getFeature("Dock")["in_use"] == true)
                    {
                        foreach (DataRow dock in Tables.warehouses.getDocks(warehouse))
                        {
                            dock.Delete();
                        }
                    }
                    Tables.docks.updateChanges();

                    SQL.SqlCommand($"DROP TABLE `{warehouse["name"]}`");
                    warehouse.Delete();
                    Tables.warehouses.updateChanges();

                    foreach (DataRow sector in Tables.warehouses.getSectors(warehouse))
                    {
                        sector.Delete();
                    }
                    Tables.sector.Refresh();

                    DisplayWarehousesOnPanel(DisplayWarehousesStackpanel);

                    MessageBox.Show("Warehouse has been deleted", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }

        private void SectorsInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWarehouse != null)
            {
                sectorIndexWindow sectorIndexWindow = new sectorIndexWindow();
                Navigation.content2.Navigate(sectorIndexWindow);
            }
        }

        private void EmployeesInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OrdersInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProductsInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {
            WarehouseProductsPage page = new WarehouseProductsPage();
            Navigation.content2.Navigate(page);
        }

        private void FleetInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DocksInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ForkliftInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            MapDisplay.Children.Remove(terkep);
        }

        private void Back_AllWarehouse_Click(object sender, RoutedEventArgs e)
        {
            DisplayOneWarehouse.Visibility = Visibility.Collapsed;
            AddNewWarehouseDisplay.Visibility = Visibility.Collapsed;
            AddNewWarehouse.Visibility = Visibility.Visible;
            DisplayWarehouses.Visibility = Visibility.Visible;
            DisplayWarehousesStackpanel.Visibility = Visibility.Visible;

            MapDisplay.Children.Remove(terkep);
        }
    }
}
