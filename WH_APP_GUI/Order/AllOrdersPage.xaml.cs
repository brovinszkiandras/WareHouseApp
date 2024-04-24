using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Xml.Linq;

namespace WH_APP_GUI.Order
{
    public partial class AllOrdersPage : Page
    {
        public AllOrdersPage()
        {
            InitializeComponent();

            DisplayAllOrders();

            Ini_unassigned_cities();
            Ini_warehouses();

            if (Navigation.ReturnParam == null)
            {
                BackBORDER.Visibility = Visibility.Collapsed;
            }
        }
        private bool CanComplete(string username, string address)
        {
            bool canComplete = true;
            foreach (DataRow order in Tables.orders.getOrdersOfAUser(username, address))
            {
                if (User.WarehouseTable().database.Select($"product_id = {order["product_id"]}").Length == 0)
                {
                    return false;
                }
                else
                {
                    int productsInTheWarehosue = User.WarehouseTable().database.Select($"product_id = {order["product_id"]}").Sum(row => (int)row["qty"]);
                    if (productsInTheWarehosue < (int)order["qty"])
                    {
                        canComplete = false;
                    }
                }
            }
            return canComplete;
        }
        private void DisplayOneOrder(Panel panel, string username, string address)
        {
            DataRow dataOfOrder = Tables.orders.getOrdersOfAUser(username, address)[0];

            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(2);
            border.Margin = new Thickness(5);
            border.Background = Brushes.White;

            StackPanel mainStackPanel = new StackPanel();

            StackPanel imagePanel = new StackPanel();
            imagePanel.Orientation = Orientation.Horizontal;
            imagePanel.HorizontalAlignment = HorizontalAlignment.Center;

            int qtyOfAllProd = 0;
            int maxPrice = 0;
            double sumVolume = 0;
            foreach (DataRow order in Tables.orders.getOrdersOfAUser(username, address))
            {
                qtyOfAllProd += int.Parse(order["qty"].ToString());
                maxPrice += int.Parse(Tables.orders.getProduct(order)["selling_price"].ToString());
                if (Tables.features.isFeatureInUse("Storage"))
                {
                    string sum = Tables.orders.getProduct(order)["volume"].ToString();
                    sumVolume += sum != string.Empty ? double.Parse(sum) : 0;
                }

                Image image = new Image();
                image.Width = 80;
                image.Height = 80;
                image.Margin = new Thickness(5);

                string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
                if (Directory.Exists(targetDirectory))
                {
                    string imageFileName = Tables.orders.getProduct(order)["image"].ToString();
                    string imagePath = Path.Combine(targetDirectory, imageFileName);

                    if (File.Exists(imagePath))
                    {
                        string fileName = Path.GetFileName(imagePath);
                        string targetFilePath = Path.Combine(targetDirectory, fileName);

                        BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));

                        image.Source = bitmap;
                    }
                }
                imagePanel.Children.Add(image);
            }

            UniformGrid userInfoGrid = new UniformGrid();
            userInfoGrid.Rows = 3;
            userInfoGrid.Margin = new Thickness(5);

            Label userNameInfo = new Label();
            userNameInfo.Content = $"Username: {dataOfOrder["user_name"]}";
            userNameInfo.BorderBrush = Brushes.Black;
            userNameInfo.BorderThickness = new Thickness(0, 0, 1, 1);
            userNameInfo.HorizontalContentAlignment = HorizontalAlignment.Right;
            userInfoGrid.Children.Add(userNameInfo);

            Label paymentMethodInfo = new Label();
            paymentMethodInfo.Content = $"Payment method: {dataOfOrder["payment_method"]}";
            paymentMethodInfo.BorderBrush = Brushes.Black;
            paymentMethodInfo.BorderThickness = new Thickness(1, 0, 0, 1);
            paymentMethodInfo.HorizontalContentAlignment = HorizontalAlignment.Left;
            userInfoGrid.Children.Add(paymentMethodInfo);

            Label addressInfo = new Label();
            addressInfo.Content = $"Address: {dataOfOrder["address"]} - ({Tables.orders.getCity(dataOfOrder)["city_name"]})";
            addressInfo.BorderBrush = Brushes.Black;
            addressInfo.BorderThickness = new Thickness(0, 0, 1, 1);
            addressInfo.HorizontalContentAlignment = HorizontalAlignment.Right;
            userInfoGrid.Children.Add(addressInfo);

            Label productCountInfo = new Label();
            productCountInfo.Content = $"Products Count: {qtyOfAllProd}";
            productCountInfo.BorderBrush = Brushes.Black;
            productCountInfo.BorderThickness = new Thickness(1, 0, 0, 1);
            productCountInfo.HorizontalContentAlignment = HorizontalAlignment.Left;
            userInfoGrid.Children.Add(productCountInfo);

            Label orderInfo = new Label();
            orderInfo.Content = $"Order date: {dataOfOrder["order_date"]}";
            orderInfo.BorderBrush = Brushes.Black;
            orderInfo.BorderThickness = new Thickness(0, 0, 1, 1);
            orderInfo.HorizontalContentAlignment = HorizontalAlignment.Right;
            userInfoGrid.Children.Add(orderInfo);

            Label maxValueCount = new Label();
            maxValueCount.Content = $"Max value: {maxPrice} - Ft";
            maxValueCount.BorderBrush = Brushes.Black;
            maxValueCount.BorderThickness = new Thickness(1, 0, 0, 1);
            maxValueCount.HorizontalContentAlignment = HorizontalAlignment.Left;
            userInfoGrid.Children.Add(maxValueCount);

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Margin = new Thickness(15, 3, 15, 0);
            scrollViewer.MinHeight = 80;
            scrollViewer.MaxHeight = 150;

            StackPanel productPanel = new StackPanel();

            foreach (DataRow order in Tables.orders.getOrdersOfAUser(username, address))
            {
                Border productBorder = new Border();
                productBorder.BorderBrush = Brushes.Black;
                productBorder.BorderThickness = new Thickness(0, 0, 0, 1);

                UniformGrid productGrid = new UniformGrid();
                productGrid.Columns = 3;

                Label productLabel = new Label();
                productLabel.Content = Tables.orders.getProduct(order)["name"] + ": ";
                Label priceLabel = new Label();
                priceLabel.Content = $"Price: {Tables.orders.getProduct(order)["selling_price"]} - Ft";
                Label qtyLabel = new Label();
                qtyLabel.Content = $"Quantity: {order["qty"]}";

                productGrid.Children.Add(productLabel);
                productGrid.Children.Add(priceLabel);
                productGrid.Children.Add(qtyLabel);

                productBorder.Child = productGrid;
                productPanel.Children.Add(productBorder);
            }

            scrollViewer.Content = productPanel;

            mainStackPanel.Children.Add(imagePanel);
            mainStackPanel.Children.Add(new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Margin = new Thickness(5), Child = userInfoGrid });

            if (Tables.features.isFeatureInUse("Storage"))
            {
                Border borderForSum = new Border();
                borderForSum.BorderBrush = Brushes.Black;
                borderForSum.BorderThickness = new Thickness(1, 0, 1, 1);
                borderForSum.Margin = new Thickness(5);

                Label sumVolumeInfo = new Label();
                sumVolumeInfo.Content = $"Sum volume: {sumVolume}(m^2)";
                sumVolumeInfo.HorizontalAlignment = HorizontalAlignment.Center;

                borderForSum.Child = sumVolumeInfo;
                mainStackPanel.Children.Add(borderForSum);
            }

            if (dataOfOrder["warehouse_id"] == DBNull.Value)
            {
                Button button = new Button();
                button.Content = "Take";
                button.Margin = new Thickness(5);
                button.MaxWidth = 150;
                mainStackPanel.Children.Add(button);

                if (User.Warehouse() != null)
                {
                    if (CanComplete(dataOfOrder["user_name"].ToString(), dataOfOrder["address"].ToString()))
                    {
                        button.IsEnabled = true;
                    }
                    else
                    {
                        button.IsEnabled = false;
                    }
                }
            }
            else
            {
                Label inWarehouse = new Label();
                inWarehouse.Background = Brushes.LightSteelBlue;
                inWarehouse.Foreground = Brushes.Black;
                inWarehouse.Margin = new Thickness(5);
                inWarehouse.BorderBrush = Brushes.Black;
                inWarehouse.BorderThickness = new Thickness(1);
                inWarehouse.MaxWidth = 350;
                inWarehouse.HorizontalContentAlignment = HorizontalAlignment.Center;
                //TODO
                inWarehouse.Content = $"This order already in {Tables.orders.getWarehouse(dataOfOrder)["name"]}";
                //inWarehouse.Content = $"This order already in a warehouse";
                mainStackPanel.Children.Add(inWarehouse);
            }

            mainStackPanel.Children.Add(scrollViewer);

            border.Child = mainStackPanel;
            panel.Children.Add(border);
        }

        private Dictionary<string, DataRow> unassigned_city_id_Dictionary = new Dictionary<string, DataRow>();
        private void Ini_unassigned_cities()
        {
            unassigned_city_id_Dictionary.Clear();
            unassigned_city_id.Items.Clear();
            foreach (DataRow order in Tables.orders.database.Rows)
            {
                if (!unassigned_city_id_Dictionary.ContainsKey(Tables.orders.getCity(order)["city_name"].ToString()) && order["warehouse_id"].ToString() == string.Empty)
                {
                    unassigned_city_id.Items.Add(Tables.orders.getCity(order)["city_name"]);
                    unassigned_city_id_Dictionary.Add(Tables.orders.getCity(order)["city_name"].ToString(), Tables.orders.getCity(order));
                }
            }
        }

        private Dictionary<string, DataRow> warehouse_id_Dictionary = new Dictionary<string, DataRow>();
        private void Ini_warehouses()
        {
            warehouse_id_Dictionary.Clear();
            warehouse_id.Items.Clear();
            foreach (DataRow order in Tables.orders.database.Rows)
            {
                if (order["warehouse_id"].ToString() != string.Empty)
                {
                    if (!warehouse_id_Dictionary.ContainsKey(Tables.orders.getWarehouse(order)["name"].ToString()))
                    {
                        warehouse_id.Items.Add(Tables.orders.getWarehouse(order)["name"].ToString());
                        warehouse_id_Dictionary.Add(Tables.orders.getWarehouse(order)["name"].ToString(), Tables.orders.getWarehouse(order));
                    }
                }
            }
        }

        private Dictionary<string, DataRow> dock_id_Dictionary = new Dictionary<string, DataRow>();
        private void Ini_docks()
        {
            //Relation need
            //dock_id_Dictionary.Clear();
            //dock_id.Items.Clear();

            //MessageBox.Show("Warehouses:" + Tables.warehouses.database.Rows.Count.ToString());
            //MessageBox.Show("Docks:" + Tables.warehouses.getDocks(Tables.warehouses.database.Rows[0]).Count().ToString());

            //if (Tables.features.isFeatureInUse("Fleet"))
            //{
            //    foreach (DataRow warehouse in Tables.warehouses.database.Rows)
            //    {
            //        if (Tables.warehouses.getDocks(warehouse).Length != 0)
            //        {
            //            foreach (DataRow transport in Tables.warehouses.get)
            //            {
            //                if (Tables.docks.getOrders(dock).Length != 0 && !dock_id_Dictionary.ContainsKey(dock["name"].ToString()))
            //                {
            //                    dock_id_Dictionary.Add(dock["name"].ToString(), dock);
            //                    dock_id.Items.Add(dock["name"].ToString());
            //                }
            //            }
            //        }
            //    }
            //}
            //else if (Tables.features.isFeatureInUse("Dock"))
            //{
            //    foreach (DataRow dock in Tables.docks.database.Rows)
            //    {
            //        if (Tables.docks.getOrders(dock).Length != 0 && !dock_id_Dictionary.ContainsKey(dock["name"].ToString()))
            //        {
            //            dock_id_Dictionary.Add(dock["name"].ToString(), dock);
            //            dock_id.Items.Add(dock["name"].ToString());
            //        }
            //    }
            //}
            //else
            //{
            //    dock_idBORDER.Visibility = Visibility.Collapsed;
            //}
        }

        private void Ini_docks(DataRow warehouse)
        {
            //Relation need
            //dock_id_Dictionary.Clear();
            //dock_id.Items.Clear();

            //foreach (DataRow dock in Tables.warehouses.getDocks(warehouse))
            //{
            //    if (Tables.docks.getOrders(dock).Length != 0 && !dock_id_Dictionary.ContainsKey(dock["name"].ToString()))
            //    {
            //        dock_id_Dictionary.Add(dock["name"].ToString(), dock);
            //        dock_id.Items.Add(dock["name"].ToString());
            //    }
            //}
        }

        private Dictionary<string, DataRow> TransportsDicitionary = new Dictionary<string, DataRow>();
        private void Ini_transports()
        {
            //RelationNeed
        }

        private void unassigned_city_id_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (unassigned_city_id.SelectedIndex != -1)
            {
                OrdersDisplay.Children.Clear();
                int city_id = int.Parse(unassigned_city_id_Dictionary[unassigned_city_id.SelectedItem.ToString()]["id"].ToString());
                List<string[]> name_address = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} WHERE warehouse_id IS NULL AND city_id = {city_id} GROUP BY user_name, address");
                for (int i = 0; i < name_address.Count; i++)
                {
                    DisplayOneOrder(OrdersDisplay, name_address[i][0], name_address[i][1]);
                }
            }
        }

        private void warehouse_id_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dock_id_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void transport_id_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AssignedOrders_Click(object sender, RoutedEventArgs e)
        {
            OrdersDisplay.Children.Clear();
            List<string[]> name_address = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} WHERE warehouse_id IS NOT NULL GROUP BY user_name, address");
            for (int i = 0; i < name_address.Count; i++)
            {
                DisplayOneOrder(OrdersDisplay, name_address[i][0], name_address[i][1]);
            }
        }

        private void UnassignedOrders_Click(object sender, RoutedEventArgs e)
        {
            OrdersDisplay.Children.Clear();
            List<string[]> name_address = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} WHERE warehouse_id IS NULL GROUP BY user_name, address");
            for (int i = 0; i < name_address.Count; i++)
            {
                DisplayOneOrder(OrdersDisplay, name_address[i][0], name_address[i][1]);
            }
        }
        private void DisplayAllOrders()
        {
            OrdersDisplay.Children.Clear();
            List<string[]> name_address = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} GROUP BY user_name, address");
            for (int i = 0; i < name_address.Count; i++)
            {
                DisplayOneOrder(OrdersDisplay, name_address[i][0], name_address[i][1]);
            }
        }

        private void AllOrders_Click(object sender, RoutedEventArgs e)
        {
            DisplayAllOrders();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Navigation.PreviousPage != null)
            {
                Navigation.OpenPage(Navigation.PreviousPage.GetType());
            }
            else
            {
                Navigation.OpenPage(Navigation.GetTypeByName("InspectWarehouse"));
            }
        }
    }
}
