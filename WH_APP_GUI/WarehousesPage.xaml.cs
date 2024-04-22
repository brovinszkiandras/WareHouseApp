using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using MahApps.Metro.IconPacks;
using Microsoft.Maps.MapControl.WPF;

namespace WH_APP_GUI
{
    //TODO: Warehouses calls kell majd ahoz hogy az Onclick(Inspect ONE warehouse) meg lehessen csinálni, CityId kiíratásaát is meg kell csinálni
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

            WHlenghtLBL.Visibility = Visibility.Visible;
            Grid.SetRow(WHlenghtLBL, 3);
            WarehouseLength.Visibility = Visibility.Visible;
            Grid.SetRow(WarehouseLength, 4);

            WHwidthLBL.Visibility = Visibility.Visible;
            Grid.SetRow(WHwidthLBL, 5);
            WarehouseWidth.Visibility = Visibility.Visible;
            Grid.SetRow(WarehouseWidth, 6);

            WHheightLBL.Visibility = Visibility.Visible;
            Grid.SetRow(WHheightLBL, 7);
            WarehouseHeight.Visibility = Visibility.Visible;
            Grid.SetRow(WarehouseHeight, 8);

            int index = 9;
            if (SQL.BoolQuery($"SElECT in_use FROM feature WHERE name = 'City'"))
            {
                CityID.Items.Clear();
                for (int i = 0; i < Tables.cities.database.Rows.Count; i++)
                {
                    CityID.Items.Add(Tables.cities.database.Rows[i]["city_name"]);
                }

                WHcityLBL.Visibility = Visibility.Visible;
                Grid.SetRow(WHcityLBL, index);
                index++;
                CityID.Visibility = Visibility.Visible;
                Grid.SetRow(CityID, index);
                index++;
            }

            Grid.SetRow(CreateWarehouse, index);
            index++;
            Grid.SetRow(CancelCreation, index);

            AddNewWarehouseDisplay.Visibility = Visibility.Visible;
        }

        private void CreateWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (SQL.BoolQuery($"SElECT in_use FROM feature WHERE name = 'City'"))
            {
                if (WarehouseName.Text != string.Empty && WarehouseLength.Text != string.Empty && WarehouseHeight.Text != string.Empty && WarehouseWidth.Text != string.Empty && CityID.SelectedIndex != -1)
                {
                    try
                    {
                        Controller.CreateWarehouse(WarehouseName.Text, CityID.SelectedIndex + 1, double.Parse(WarehouseLength.Text), double.Parse(WarehouseWidth.Text), double.Parse(WarehouseHeight.Text));
                        MessageBox.Show("The warehouse has been created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        Tables.warehouses.Refresh();
                        Cancel();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteError(ex);
                        throw;
                    }
                }
                else
                {
                    MessageBox.Show("Missing datas!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                if (WarehouseName.Text != string.Empty && WarehouseLength.Text != string.Empty && WarehouseHeight.Text != string.Empty && WarehouseWidth.Text != string.Empty)
                {
                    try
                    {
                        Controller.CreateWarehouse(WarehouseName.Text, double.Parse(WarehouseLength.Text), double.Parse(WarehouseWidth.Text), double.Parse(WarehouseHeight.Text));
                        MessageBox.Show("The warehouse has been created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        Tables.warehouses.Refresh();
                        Cancel();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteError(ex);
                        throw;
                    }
                }
                else
                {
                    MessageBox.Show("Missing datas!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            AddNewWarehouse.Visibility = Visibility.Visible;
        }
        private void Cancel()
        {
            AddNewWarehouseDisplay.Visibility = Visibility.Collapsed;
            DisplayWarehouses.Visibility = Visibility.Visible;
            DisplayWarehousesStackpanel.Visibility = Visibility.Visible;
            AddNewWarehouse.Visibility = Visibility.Visible;
            DisplayWarehousesOnPanel(DisplayWarehousesStackpanel);

            MapDisplay.Children.Remove(terkep);

            WarehouseName.Text = string.Empty;
            WarehouseLength.Text = string.Empty;
            WarehouseWidth.Text = string.Empty;
            WarehouseHeight.Text = string.Empty;
            CityID.SelectedIndex = -1;
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

                    //for (int i = 0; i < Tables.warehouses.getEmployees(warehouse).Length; i++)
                    //{
                    //    SQL.SqlCommand($"UPDATE `{Tables.employees.actual_name}` SET `warehouse_id`= NULL WHERE id = {Tables.warehouses.getEmployees(warehouse)[i]["id"]}");
                    //}
                    //Tables.employees.Refresh();
                    if ((bool)Tables.features.getFeature("Dock")["in_use"] == true)
                    {
                        foreach (DataRow dock in Tables.warehouses.getDocks(warehouse))
                        {
                            dock.Delete();
                        }
                    }
                    Tables.docks.updateChanges();
                    //if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Dock'"))
                    //{
                    //    List<string> DockID = SQL.GetElementOfListArray(SQL.SqlQuery($"SELECT id FROM {Tables.docks.actual_name}"));
                    //    for (int i = 0; i < DockID.Count; i++)
                    //    {
                    //        SQL.SqlCommand($"DELETE FROM `{Tables.docks.actual_name}` WHERE id = {DockID[i]}");
                    //    }
                    //    Tables.docks.Refresh();
                    //}
                    foreach (DataRow order in Tables.warehouses.getOrders(warehouse))
                    {
                        order.Delete();
                    }
                    Tables.orders.updateChanges();
                    //for (int i = 0; i < Tables.warehouses.getOrders(warehouse).Length; i++)
                    //{
                    //    SQL.SqlCommand($"DELETE FROM `orders` WHERE warehouse_id = {warehouse["id"]}");
                    //}
                    //Tables.orders.Refresh();

                    //if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Forklift'"))
                    //{
                    //    List<string> ForkliftID = SQL.GetElementOfListArray(SQL.SqlQuery($"SELECT id FROM forklift"));
                    //    for (int i = 0; i < ForkliftID.Count; i++)
                    //    {
                    //        SQL.SqlCommand($"UPDATE `forklift` SET `warehouse_id`= NULL WHERE id = {ForkliftID[i]}");
                    //    }
                    //TODO: Forklift class need
                    //}

                    //if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Revenue'"))
                    //{
                    //    List<string> RevnueADayID = SQL.GetElementOfListArray(SQL.SqlQuery($"SELECT id FROM revenue_a_day"));
                    //    for (int i = 0; i < RevnueADayID.Count; i++)
                    //    {
                    //        SQL.SqlCommand($"DELETE FROM `revenue_a_day` WHERE id = {RevnueADayID[i]}");
                    //    }
                    //    //TODO: revnue_a_day class need
                    //}

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
                sectorIndexWindow.Show();

                //this.Close();
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
