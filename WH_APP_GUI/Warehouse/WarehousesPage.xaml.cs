using MahApps.Metro.IconPacks;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using WH_APP_GUI.warehouseTableFolder;

namespace WH_APP_GUI.Warehouse
{
    public partial class WarehousesPage : Page
    {
        public WarehousesPage()
        {
            InitializeComponent();
            DisplayWarehousesOnPanel(DisplayWarehousesStackpanel);

            if (!User.DoesHavePermission("Modify all Warehouses"))
            {
                AddNewWarehouse.Visibility = Visibility.Collapsed;
            }
        }
        public void DisplayWarehousesOnPanel(Panel panel)
        {
            panel.Children.Clear();
            panel.Visibility = Visibility.Visible;
            for (int i = 0; i < Tables.warehouses.database.Rows.Count; i++)
            {
                Border border = new Border();
                border.BorderBrush = Brushes.Black;
                border.CornerRadius = new CornerRadius(30);
                border.BorderThickness = new Thickness(1);
                border.Background = new SolidColorBrush(Color.FromRgb(57, 82, 80));
                border.Margin = new Thickness(5);

                Grid grid = new Grid();

                StackPanel outerStack = new StackPanel();
                if (Tables.features.isFeatureInUse("Date Log"))
                {
                    Label dateLog = new Label();
                    dateLog.Content = $"Created at: {Tables.warehouses.database.Rows[i]["created_at"]} \tUpdated at: {Tables.warehouses.database.Rows[i]["updated_at"]}";
                    dateLog.FontFamily = new FontFamily("Baskerville Old Face");
                    dateLog.HorizontalContentAlignment = HorizontalAlignment.Center;
                    outerStack.Children.Add(dateLog);
                }

                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                // Grid.SetColumnSpan(border, 3);
                outerStack.Children.Add(grid);
                border.Child = outerStack;

                Image image = new Image();
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.Margin = new Thickness(5);

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
                grid.Children.Add(image);

                Label label = new Label();
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.Content = Tables.warehouses.database.Rows[i]["name"];
                label.FontFamily = new FontFamily("Baskerville Old Face");
                Grid.SetColumn(label, 1);
                grid.Children.Add(label);

                Grid innerGrid = new Grid();
                innerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                innerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                Grid.SetColumn(innerGrid, 2);
                grid.Children.Add(innerGrid);

                if (User.DoesHavePermission("Inspect all Warehouses") || User.DoesHavePermission("Inspect Warehouse"))
                {
                    if (Tables.staff.database.Select($"email = '{User.currentUser["email"]}'").Length == 0)
                    {
                        if (User.currentUser["warehouse_id"] == Tables.warehouses.database.Rows[i]["id"])
                        {
                            Button inspectButton = new Button();
                            inspectButton.Tag = Tables.warehouses.database.Rows[i];
                            inspectButton.Content = "Inspect Warehouse";
                            inspectButton.Click += inspect_warehouse_Click;
                            inspectButton.Margin = new Thickness(10);
                            inspectButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                            Grid.SetRow(inspectButton, 0);
                            innerGrid.Children.Add(inspectButton);
                        }
                    }
                    else
                    {
                        Button inspectButton = new Button();
                        inspectButton.Tag = Tables.warehouses.database.Rows[i];
                        inspectButton.Content = "Inspect Warehouse";
                        inspectButton.Click += inspect_warehouse_Click;
                        inspectButton.Margin = new Thickness(10);
                        inspectButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                        Grid.SetRow(inspectButton, 0);
                        innerGrid.Children.Add(inspectButton);
                    }
                }


                if (User.DoesHavePermission("Modify Warehouse") || User.DoesHavePermission("Modify all Warehouses"))
                {
                    if (Tables.staff.database.Select($"email = '{User.currentUser["email"]}'").Length == 0)
                    {
                        if (User.currentUser["warehouse_id"] == Tables.warehouses.database.Rows[i]["id"])
                        {
                            Button deleteButton = new Button();
                            deleteButton.Content = "Delete Warehouse";
                            deleteButton.Tag = Tables.warehouses.database.Rows[i];
                            deleteButton.Click += delete_warehouse_Click;
                            deleteButton.Margin = new Thickness(10);
                            deleteButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                            Grid.SetRow(deleteButton, 1);
                            innerGrid.Children.Add(deleteButton);
                        }
                    }
                    else
                    {
                        Button deleteButton = new Button();
                        deleteButton.Content = "Delete Warehouse";
                        deleteButton.Tag = Tables.warehouses.database.Rows[i];
                        deleteButton.Click += delete_warehouse_Click;
                        deleteButton.Margin = new Thickness(10);
                        deleteButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                        Grid.SetRow(deleteButton, 1);
                        innerGrid.Children.Add(deleteButton);
                    }
                }

                panel.Children.Add(border);
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

                    for (int i = 0; i < Tables.warehouseTables.Count; i++)
                    {
                        if (Tables.warehouseTables[i].database.TableName == warehouse["name"].ToString())
                        {
                            Tables.databases.Tables.Remove(Tables.warehouseTables[i].database);
                            Tables.warehouseTables.Remove(Tables.warehouseTables[i]);
                        }
                    }

                    foreach (DataRow sector in Tables.warehouses.getSectors(warehouse))
                    {
                        sector.Delete();
                    }
                    Tables.sector.updateChanges();

                    SQL.SqlCommand($"DROP TABLE `{warehouse["name"]}`");
                    warehouse.Delete();
                    Tables.warehouses.updateChanges();

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

        private void WarehausePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
            AddNewWarehouse.FontSize = e.NewSize.Height * 0.03;
            AddNewWarehouse.Width = e.NewSize.Width * 0.4;
        }
    }
}