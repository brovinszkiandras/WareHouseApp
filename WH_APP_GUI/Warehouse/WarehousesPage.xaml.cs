using MahApps.Metro.IconPacks;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WH_APP_GUI.Forklift;
using WH_APP_GUI.sectors;
using WH_APP_GUI.WarehouseTableFolder;

namespace WH_APP_GUI.Warehouse
{
    public partial class WarehousesPage : Page
    {
        public WarehousesPage()
        {
            InitializeComponent();
            DisplayWarehousesOnPanel(DisplayWarehousesStackpanel);
        }
        public void DisplayWarehousesOnPanel(Panel panel)
        {
            panel.Children.Clear();
            panel.Visibility = Visibility.Visible;
            for (int i = 0; i < Tables.warehouses.database.Rows.Count; i++)
            {
                Grid grid = new Grid();
                grid.Height = 150;
                grid.Margin = new Thickness(5);

                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                Border border = new Border();
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(1);
                Grid.SetColumnSpan(border, 3);
                grid.Children.Add(border);

                Image image = new Image();
                image.Width = 100;
                image.Height = 100;
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.SetValue(Grid.RowSpanProperty, 3);

                string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
                if (Directory.Exists(targetDirectory))
                {
                    string imageFileName = "WarehouseDefaultPicture.png";
                    string imagePath = Path.Combine(targetDirectory, imageFileName);

                    if (File.Exists(imagePath))
                    {
                        string fileName = Path.GetFileName(imagePath);
                        string targetFilePath = Path.Combine(targetDirectory, fileName);

                        BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));

                        image.Source = bitmap;
                    }
                }
                image.Margin = new Thickness(5);
                image.SetValue(Grid.RowSpanProperty, 3);
                grid.Children.Add(image);

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

        private void AddNewWarehouse_Click(object sender, RoutedEventArgs e)
        {
            CreateWarehouse createWarehouse = new CreateWarehouse(new WarehousesPage());
            WarehouseContent.Content = null;
            WarehouseContent.Navigate(createWarehouse);
            WarehouseContent.Visibility = Visibility.Visible;
        }
        
        private void Cancel()
        {
            DisplayWarehouses.Visibility = Visibility.Visible;
            DisplayWarehousesStackpanel.Visibility = Visibility.Visible;
            AddNewWarehouse.Visibility = Visibility.Visible;
            DisplayWarehousesOnPanel(DisplayWarehousesStackpanel);
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
                Navigation.OpenPage(Navigation.GetTypeByName("InspectWarehouse"), SelectedWarehouse);
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
                        employee["warehouse_id"] = DBNull.Value;
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
    }
}
