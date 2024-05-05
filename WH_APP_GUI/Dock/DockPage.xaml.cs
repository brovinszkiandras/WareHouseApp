using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using System.Windows.Navigation;
using WH_APP_GUI.Employee;
using WH_APP_GUI.Warehouse;

namespace WH_APP_GUI.Dock
{
    public partial class DockPage : Page
    {
        private DataRow WarehouseFromPage = null;
        public DockPage(DataRow warehouseFromPage)
        {
            InitializeComponent();

            if (User.DoesHavePermission("Modify Dock"))
            {
                AddNewDock.Visibility = Visibility.Visible;
            }
            else
            {
                AddNewDock.Visibility = Visibility.Collapsed;
            }

            WarehouseFromPage = warehouseFromPage;
            InitializeAllDocks();
        }
        #region Display
        public void InitializeAllDocks()
        {
            DockDisplaySTACK.Children.Clear();

            foreach (DataRow dock in Tables.docks.database.Rows)
            {
                DisplayOneDock(dock);
            }
        }
        private void DisplayOneDock(DataRow dock)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.Background = new SolidColorBrush(Color.FromArgb(255, 0x39, 0x52, 0x50));
            border.CornerRadius = new CornerRadius(15);
            border.BorderThickness = new Thickness(2);
            border.Margin = new Thickness(5);

            Grid mainGrid = new Grid();
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

            Image image = new Image();
            image.Width = 100;
            image.Height = 100;

            Grid.SetColumn(image, 0);
            mainGrid.Children.Add(image);

            string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
            if (Directory.Exists(targetDirectory))
            {
                string imageFileName = "DockDefaultImage.png";
                string imagePath = Path.Combine(targetDirectory, imageFileName);

                if (File.Exists(imagePath))
                {
                    string fileName = Path.GetFileName(imagePath);
                    string targetFilePath = Path.Combine(targetDirectory, fileName);

                    BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));

                    image.Source = bitmap;
                }
            }

            StackPanel leftStackPanel = new StackPanel();
            Grid.SetColumn(leftStackPanel, 1);
            mainGrid.Children.Add(leftStackPanel);

            Label nameLabel = new Label();
            nameLabel.Content = dock["name"];
            nameLabel.Style = (Style)this.Resources["labelstyle"];
            leftStackPanel.Children.Add(nameLabel);

            Label roleLabel = new Label();
            roleLabel.Style = (Style)this.Resources["labelstyle"];
            leftStackPanel.Children.Add(roleLabel);

            if (dock["warehouse_id"] != DBNull.Value)
            {
                roleLabel.Content = Tables.docks.getWarehouse(dock)["name"];
            }
            else
            {
                roleLabel.Content = "This dock does not belongs to a warehouse";
            }

            StackPanel rightStackPanel = new StackPanel();
            Grid.SetColumn(rightStackPanel, 2);
            mainGrid.Children.Add(rightStackPanel);

            Button isDockFree = new Button();
            isDockFree.Content = bool.Parse(dock["free"].ToString()) ? "Free" : "In use";
            isDockFree.Background = bool.Parse(dock["free"].ToString()) ? Brushes.RoyalBlue : Brushes.Red;
            isDockFree.Click += isDockFree_Click;
            isDockFree.Tag = dock;
            isDockFree.Margin = new Thickness(5);
            isDockFree.IsEnabled = false;
            rightStackPanel.Children.Add(isDockFree);

            if (User.DoesHavePermission("Modify Dock"))
            {
                isDockFree.IsEnabled = true;

                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.Click += deleteDock_Click;
                deleteButton.Tag = dock;
                deleteButton.Style = (Style)this.Resources["GoldenButtonStyle"];

                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.Click += EditDock_Click;
                editButton.Tag = dock;
                editButton.Style = (Style)this.Resources["GoldenButtonStyle"];

                isDockFree.IsEnabled = true;
                rightStackPanel.Children.Add(deleteButton);
                rightStackPanel.Children.Add(editButton);
            }
            else
            {
                isDockFree.IsEnabled = false;
            }


            if (Tables.features.isFeatureInUse("Fleet"))
            {
                Label transport = new Label();
                transport.Style = (Style)this.Resources["labelstyle"];
                if (Tables.docks.getTransports(dock).Length != 0)
                {
                    DataRow transportRow = Tables.docks.getTransports(dock)[0];
                    transport.Content = $"{Tables.transports.getEmployee(transportRow)["name"]} - {Tables.transports.getCar(transportRow)["type"]}: {transportRow["end_date"]}";
                }
                else
                {
                    transport.Content = "This dock does not have a transport";

                }
                leftStackPanel.Children.Add(transport);
            }
            else if (Tables.features.isFeatureInUse("Dock") && ! Tables.features.isFeatureInUse("Fleet"))
            {
                if (Tables.docks.getOrders(dock).Length != 0)
                {
                    Expander orderExpander = new Expander();
                    StackPanel ordersStackapnel = new StackPanel();
                    UniformGrid ordersGrid = new UniformGrid();
                    ordersGrid.Columns = 3;

                    if (WarehouseFromPage != null)
                    {
                        List<string[]> name_address = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} WHERE transport_id = {Tables.docks.getTransports(dock)[0]["id"]} AND warehouse_id {WarehouseFromPage["id"]} GROUP BY user_name, address");
                        for (int i = 0; i < name_address.Count; i++)
                        {
                            DataRow order = Tables.orders.getOrdersOfAUser(name_address[i][0], name_address[i][1])[0];

                            Label name = new Label();
                            name.Content = $"Order by: {order["user_name"]}";
                            name.Style = (Style)this.Resources["labelstyle"];

                            Label address = new Label();
                            address.Content = $"Address: {order["address"]}";
                            address.Style = (Style)this.Resources["labelstyle"];

                            Label order_date = new Label();
                            order_date.Content = $"Date: {order["order_date"]}";
                            order_date.Style = (Style)this.Resources["labelstyle"];

                            ordersGrid.Children.Add(name);
                            ordersGrid.Children.Add(address);
                            ordersGrid.Children.Add(order_date);
                        }

                        orderExpander.Content = ordersGrid;
                        ordersStackapnel.Children.Add(orderExpander);
                    }
                    else
                    {
                        List<string[]> name_address = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} WHERE transport_id = {Tables.docks.getTransports(dock)[0]["id"]} AND warehouse_id IS NOT NULL GROUP BY user_name, address");
                        for (int i = 0; i < name_address.Count; i++)
                        {
                            DataRow order = Tables.orders.getOrdersOfAUser(name_address[i][0], name_address[i][1])[0];

                            Label name = new Label();
                            name.Content = $"Order by: {order["user_name"]}";
                            name.Style = (Style)this.Resources["labelstyle"];

                            Label address = new Label();
                            address.Content = $"Address: {order["address"]}";
                            address.Style = (Style)this.Resources["labelstyle"];

                            Label order_date = new Label();
                            order_date.Content = $"Date: {order["order_date"]}";
                            order_date.Style = (Style)this.Resources["labelstyle"];

                            ordersGrid.Children.Add(name);
                            ordersGrid.Children.Add(address);
                            ordersGrid.Children.Add(order_date);
                        }
                        orderExpander.Content = ordersGrid;
                        ordersStackapnel.Children.Add(orderExpander);
                    }
                }
            }

            border.Child = mainGrid;

            DockDisplaySTACK.Children.Add(border);
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Navigation.PreviousPage != null)
            {
                if (WarehouseFromPage != null)
                {
                    Navigation.PreviousPage = new InspectWarehouse(WarehouseFromPage);
                    Navigation.OpenPage(Navigation.PreviousPage.GetType(), WarehouseFromPage);
                }
                else
                {
                    Navigation.OpenPage(Navigation.PreviousPage.GetType());
                }
            }
        }
        private void DockPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
                foreach (var child in alapgrid.Children)
                {
                    FontSize = e.NewSize.Height * 0.03;
                }
        }
        #endregion
        #region Modify methods
        private void deleteDock_Click(object sender, RoutedEventArgs e)
        {
            DataRow dock = (sender as Button).Tag as DataRow;
            if (dock != null)
            {
                dock.Delete();
                Tables.docks.updateChanges();
                DockDisplaySTACK.Children.Clear();
                InitializeAllDocks();

                MessageBox.Show("Dock deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void EditDock_Click(object sender, RoutedEventArgs e)
        {
            DataRow dock = (sender as Button).Tag as DataRow;
            if (dock != null)
            {
                if (WarehouseFromPage != null)
                {
                    Navigation.OpenPage(Navigation.GetTypeByName("EditDockPage"), dock);
                    Navigation.ReturnParam = WarehouseFromPage;
                }
                else
                {
                    Navigation.OpenPage(Navigation.GetTypeByName("EditDockPage"), dock);
                }
            }
        }
        private void AddNewDock_Click(object sender, RoutedEventArgs e)
        {
            if (WarehouseFromPage != null)
            {
                Navigation.OpenPage(Navigation.GetTypeByName("CreateDockPage"));
                Navigation.ReturnParam = WarehouseFromPage;
            }
            else
            {
                Navigation.OpenPage(Navigation.GetTypeByName("CreateDockPage"));
            }
        }
        private void isDockFree_Click(object sender, RoutedEventArgs e)
        {
            DataRow dock = (sender as Button).Tag as DataRow;
            if (dock != null)
            {
                dock["free"] = bool.Parse(dock["free"].ToString()) ? false : true;
                Tables.docks.updateChanges();
                InitializeAllDocks();
            }
        }
        #endregion
    }
}
