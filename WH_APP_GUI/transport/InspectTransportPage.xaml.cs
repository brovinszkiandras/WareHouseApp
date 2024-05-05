using Microsoft.Maps.MapControl.WPF;
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

namespace WH_APP_GUI.transport
{
    public partial class InspectTransportPage : Page
    {
        private DataRow Transport = null;
        private Map terkep = new Map();

        public InspectTransportPage(DataRow transport)
        {
            InitializeComponent();
            Transport = transport;
            DisplayOneTransport(transport);
            Ini_Map();
        }
        private void Ini_Map()
        {
            terkep.IsEnabled = false;
            MapDisplay.Children.Add(terkep);
            terkep.CredentialsProvider = new ApplicationIdCredentialsProvider("I28YbqAL3vpfFHWSLW5x~bGccdfvqXsmwkAA8zHurUw~Apx4iHJNCNHKm28KE8CDvxw6wAeIp4-8Yz1DDnwyIa81h9Obx4dD-xlgWz3mrIq8");

            double lat = double.Parse(Tables.warehouses.getCity(Tables.transports.getWarehouse(Transport))["latitude"].ToString());
            double lon = double.Parse(Tables.warehouses.getCity(Tables.transports.getWarehouse(Transport))["longitude"].ToString());

            terkep.Center = new Location(lat, lon);
            terkep.ZoomLevel = 10;
        }

        private void DisplayOneTransport(DataRow transport)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.Background = new SolidColorBrush(Color.FromArgb(255, 0x39, 0x52, 0x50));
            border.CornerRadius = new CornerRadius(15);
            border.BorderThickness = new Thickness(2);
            border.Margin = new Thickness(5);

            StackPanel mainStackPanel = new StackPanel();

            Label driver = new Label();
            driver.Content = $"{Tables.transports.getEmployee(transport)["name"]}";
            driver.HorizontalContentAlignment = HorizontalAlignment.Center;
            driver.Style = (Style)this.Resources["labelstyle"];

            mainStackPanel.Children.Add(driver);

            UniformGrid datas = new UniformGrid();
            datas.Rows = 2;
            datas.Margin = new Thickness(5);

            Label car = new Label();
            car.Content = $"Car: {Tables.transports.getCar(transport)["type"]}";
            car.Style = (Style)this.Resources["labelstyle"];
            datas.Children.Add(car);

            ComboBox status = new ComboBox();
            status.BorderBrush = Brushes.Black;
            status.VerticalContentAlignment = VerticalAlignment.Center;
            status.FontFamily = new FontFamily("Baskerville Old Face");
            status.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xCE, 0xA2));
            status.Background = new SolidColorBrush(Color.FromRgb(0x39, 0x52, 0x50));
            status.BorderBrush = Brushes.Black;
            status.Margin = new Thickness(5);
            status.Tag = transport;

            if (transport["status"].ToString() == "Docking")
            {
                status.Items.Add("Docking");
                status.Items.Add("On route");
                status.SelectedItem = "Docking";
                if (Tables.transports.getOrders(transport).Length == 0)
                {
                    status.IsEnabled = false;
                }
            }
            else if (transport["status"].ToString() == "On route")
            {
                status.Items.Add("On route");
                status.Items.Add("On Way Back");
                status.SelectedItem = "On route";
            }
            else if (transport["status"].ToString() == "On Way Back")
            {
                status.Items.Add("On Way Back");
                status.Items.Add("Finished");
                status.SelectedItem = "On Way Back";
            }

            status.SelectionChanged += TransportStatusChange;
            datas.Children.Add(status);

            Label start_date = new Label();
            start_date.Content = $"Start date: {transport["start_date"]}";
            start_date.Style = (Style)this.Resources["labelstyle"];
            datas.Children.Add(start_date);

            Label end_date = new Label();
            string endDate = transport["end_date"] != DBNull.Value ? transport["end_date"].ToString() : "No end date yet.";
            end_date.Content = $"End date: {transport["end_date"]}";
            end_date.Style = (Style)this.Resources["labelstyle"];
            datas.Children.Add(end_date);

            Label warehouse = new Label();
            warehouse.Content = $"Warehouse: {Tables.transports.getWarehouse(transport)["name"]}";
            warehouse.Style = (Style)this.Resources["labelstyle"];
            datas.Children.Add(warehouse);

            if (Tables.features.isFeatureInUse("Dock") == true)
            {
                Label dock = new Label();
                dock.Content = $"Dock: {Tables.transports.getDock(transport)["name"]}";
                dock.Style = (Style)this.Resources["labelstyle"];
                datas.Children.Add(dock);

                mainStackPanel.Children.Add(datas);
            }

            StackPanel buttons = new StackPanel();
            buttons.Orientation = Orientation.Horizontal;
            buttons.HorizontalAlignment = HorizontalAlignment.Center;

            mainStackPanel.Children.Add(buttons);

            StackPanel orders = new StackPanel();
            orders.Margin = new Thickness(5);

            if (Tables.transports.getOrders(transport).Length != 0)
            {
                List<string[]> name_address = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} WHERE transport_id = {transport["id"]} GROUP BY user_name, address");
                for (int i = 0; i < name_address.Count; i++)
                {
                    DisplayOneOrder(orders, name_address[i][0], name_address[i][1]);
                }
                mainStackPanel.Children.Add(orders);
            }

            border.Child = mainStackPanel;
            transportDisplay.Children.Add(border);
        }

        private bool CanTransportFinished(DataRow transport)
        {
            foreach (DataRow order in Tables.transports.getOrders(transport))
            {
                if (order["status"].ToString() != "Delivered" && order["status"].ToString() != "Sent Back")
                {
                    return false;
                }
            }
            return true;
        }
        private void TransportStatusChange(object sender, RoutedEventArgs e)
        {
            DataRow transport = (sender as ComboBox).Tag as DataRow;
            if (transport != null)
            {
                if ((sender as ComboBox).SelectedItem.ToString() == "On Way Back")
                {
                    if (!CanTransportFinished(transport))
                    {
                        MessageBox.Show("The transport can not be finished while still have active orders in it.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        (sender as ComboBox).SelectedItem = "On route";
                    }
                    else
                    {
                        Transport["status"] = (sender as ComboBox).SelectedItem.ToString();
                        Tables.transports.updateChanges();
                        transportDisplay.Children.Clear();
                        DisplayOneTransport(Transport);
                    }

                }
                else if ((sender as ComboBox).SelectedItem.ToString() == "Finished")
                {
                    if (Tables.features.isFeatureInUse("Fuel"))
                    {
                        SetGasPrices routeDetails = new SetGasPrices(transport);
                        routeDetails.ShowDialog();

                        if (Navigation.PreviousPage != null)
                        {
                            Navigation.OpenPage(Navigation.PreviousPage.GetType());
                        }
                        else
                        {
                            Navigation.OpenPage(Navigation.GetTypeByName("TransportsPage"));
                        }
                    }
                    else
                    {
                        transport.Delete();
                        Tables.transports.updateChanges();
                        Tables.transports.Refresh();
                        MessageBox.Show("The transport has been completed", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        if (Navigation.PreviousPage != null)
                        {
                            Navigation.OpenPage(Navigation.PreviousPage.GetType());
                        }
                        else
                        {
                            Navigation.OpenPage(Navigation.GetTypeByName("TransportsPage"));
                        }
                    }
                }
                else
                {
                    Transport["status"] = (sender as ComboBox).SelectedItem.ToString();
                    Tables.transports.updateChanges();
                    transportDisplay.Children.Clear();
                    DisplayOneTransport(Transport);
                }
            }
        }

        private void DisplayOneOrder(Panel panel, string username, string address)
        {
            DataRow dataOfOrder = Tables.orders.getOrdersOfAUser(username, address)[0];

            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.Background = new SolidColorBrush(Color.FromArgb(255, 0x39, 0x52, 0x50));
            border.CornerRadius = new CornerRadius(15);
            border.BorderThickness = new Thickness(2);
            border.Margin = new Thickness(5);

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
                image.Width = 100;
                image.Height = 100;
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
            userNameInfo.Style = (Style)this.Resources["labelstyle"];
            userNameInfo.HorizontalContentAlignment = HorizontalAlignment.Right;
            userInfoGrid.Children.Add(userNameInfo);

            Label paymentMethodInfo = new Label();
            paymentMethodInfo.Content = $"Payment method: {dataOfOrder["payment_method"]}";
            paymentMethodInfo.Style = (Style)this.Resources["labelstyle"];
            paymentMethodInfo.HorizontalContentAlignment = HorizontalAlignment.Left;
            userInfoGrid.Children.Add(paymentMethodInfo);

            Label addressInfo = new Label();
            addressInfo.Content = $"Address: {dataOfOrder["address"]} - ({Tables.orders.getCity(dataOfOrder)["city_name"]})";
            addressInfo.Style = (Style)this.Resources["labelstyle"];
            addressInfo.HorizontalContentAlignment = HorizontalAlignment.Right;
            userInfoGrid.Children.Add(addressInfo);

            Label productCountInfo = new Label();
            productCountInfo.Content = $"Products Count: {qtyOfAllProd}";
            productCountInfo.Style = (Style)this.Resources["labelstyle"];
            productCountInfo.HorizontalContentAlignment = HorizontalAlignment.Left;
            userInfoGrid.Children.Add(productCountInfo);

            Label orderInfo = new Label();
            orderInfo.Content = $"Order date: {dataOfOrder["order_date"]}";
            orderInfo.Style = (Style)this.Resources["labelstyle"];
            orderInfo.HorizontalContentAlignment = HorizontalAlignment.Right;
            userInfoGrid.Children.Add(orderInfo);

            Label maxValueCount = new Label();
            maxValueCount.Content = $"Max value: {maxPrice} - Ft";
            maxValueCount.Style = (Style)this.Resources["labelstyle"];
            maxValueCount.HorizontalContentAlignment = HorizontalAlignment.Left;
            userInfoGrid.Children.Add(maxValueCount);

            mainStackPanel.Children.Add(imagePanel);
            mainStackPanel.Children.Add(new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Margin = new Thickness(5), Child = userInfoGrid });

            Border status = new Border();
            status.BorderBrush = Brushes.Black;
            status.BorderThickness = new Thickness(1, 0, 1, 1);
            status.Margin = new Thickness(5);

            
            if (dataOfOrder["status"].ToString() == "Finished" || dataOfOrder["status"].ToString() == "Sent Back" || dataOfOrder["status"].ToString() == "Delivered") 
            {
                ComboBox statusCBX = new ComboBox();
                statusCBX.BorderBrush = Brushes.Black;
                statusCBX.VerticalContentAlignment = VerticalAlignment.Center;
                statusCBX.FontFamily = new FontFamily("Baskerville Old Face");
                statusCBX.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xCE, 0xA2));
                statusCBX.Background = new SolidColorBrush(Color.FromRgb(0x39, 0x52, 0x50));
                statusCBX.BorderBrush = Brushes.Black;
                statusCBX.Margin = new Thickness(5);

                statusCBX.HorizontalAlignment = HorizontalAlignment.Center;
                statusCBX.Tag = Tables.orders.getOrdersOfAUser(dataOfOrder["user_name"], dataOfOrder["address"]);
                statusCBX.Items.Add("Delivered");
                statusCBX.Items.Add("Sent Back");
                if (statusCBX.Items.Contains(dataOfOrder["status"].ToString()))
                {
                    statusCBX.SelectedItem = dataOfOrder["status"].ToString();
                }
                statusCBX.SelectionChanged += OrderStatusChange;
                status.Child = statusCBX;

                if (Transport != null)
                {
                    if (Transport["status"].ToString() != "On route")
                    {
                        statusCBX.IsEnabled = false;
                    }
                }
            }
            else
            {
                Label statusLBL = new Label();
                statusLBL.Content = $"Status: {dataOfOrder["status"]}";
                statusLBL.HorizontalAlignment = HorizontalAlignment.Center;
                statusLBL.Style = (Style)this.Resources["labelstyle"];
                status.Child = statusLBL;
            }

            mainStackPanel.Children.Add(status);

            if (Tables.features.isFeatureInUse("Storage"))
            {
                Border borderForSum = new Border();
                borderForSum.BorderBrush = Brushes.Black;
                borderForSum.BorderThickness = new Thickness(1, 0, 1, 1);
                borderForSum.Margin = new Thickness(5);

                Label sumVolumeInfo = new Label();
                sumVolumeInfo.Content = $"Sum volume: {sumVolume}(m^2)";
                sumVolumeInfo.HorizontalAlignment = HorizontalAlignment.Center;
                sumVolumeInfo.Style = (Style)this.Resources["labelstyle"];

                borderForSum.Child = sumVolumeInfo;
                mainStackPanel.Children.Add(borderForSum);
            }

            if (dataOfOrder["warehouse_id"] != DBNull.Value)
            {
                Label inWarehouse = new Label();
                inWarehouse.Background = Brushes.LightSteelBlue;
                inWarehouse.Foreground = Brushes.Black;
                inWarehouse.Margin = new Thickness(5);
                inWarehouse.BorderBrush = Brushes.Black;
                inWarehouse.BorderThickness = new Thickness(1);
                inWarehouse.HorizontalContentAlignment = HorizontalAlignment.Center;
                inWarehouse.Content = $"This order already in {Tables.orders.getWarehouse(dataOfOrder)["name"]}";
                mainStackPanel.Children.Add(inWarehouse);
            }

            Button viewOnMap = new Button();
            viewOnMap.Content = "View On Map";
            viewOnMap.Margin = new Thickness(5);
            viewOnMap.Tag = dataOfOrder;
            viewOnMap.Click += ViewOnMapClick;
            mainStackPanel.Children.Add(viewOnMap);

            Expander orderExpander = new Expander();
            orderExpander.Header = "Order";
            orderExpander.Margin = new Thickness(5);

            border.Child = mainStackPanel;
            orderExpander.Content = border;
            panel.Children.Add(orderExpander);
        }

        private void ViewOnMapClick(object sender, RoutedEventArgs e)
        {
            DataRow order = (sender as Button).Tag as DataRow;
            if (order != null)
            {
                double lat = double.Parse(Tables.orders.getCity(order)["latitude"].ToString());
                double lon = double.Parse(Tables.orders.getCity(order)["longitude"].ToString());

                terkep.Center = new Location(lat, lon);
                terkep.ZoomLevel = 10;
            }
        }

        private void OrderStatusChange(object sender, RoutedEventArgs e)
        {
            DataRow[] orders = (sender as ComboBox).Tag as DataRow[];
            if (orders.Length != 0)
            {
                foreach (DataRow order in orders)
                {
                    if ((sender as ComboBox).SelectedItem.ToString() == "Delivered")
                    {
                        if (order["warehouse_id"] != DBNull.Value && order["product_id"] != DBNull.Value)
                        {
                            DataRow warehouse = Tables.orders.getWarehouse(order);
                            DataRow product = Tables.orders.getProduct(order);
                            double totalincome = warehouse["total_income"] != DBNull.Value ? (double)warehouse["total_income"] : 0;
                            double sellingPrice = product["selling_price"] != DBNull.Value ? (double)product["selling_price"] : 0;
                            int qty = order["qty"] != DBNull.Value ? (int)order["qty"] : 0;
                            warehouse["total_income"] = totalincome + (sellingPrice * qty);

                            Controller.AddToRevnue_A_Day_Income(warehouse, totalincome + (sellingPrice * qty));                        
                            Tables.warehouses.updateChanges();
                        }
                    }
                    else if ((sender as ComboBox).SelectedItem.ToString() == "Sent Back")
                    {
                        if (order["warehouse_id"] != DBNull.Value && order["product_id"] != DBNull.Value)
                        {
                            DataRow warehouse = Tables.orders.getWarehouse(order);
                            DataRow product = Tables.orders.getProduct(order);
                            double totalValue = warehouse["total_value"] != DBNull.Value ? (double)warehouse["total_value"] : 0;
                            double sellingPrice = product["selling_price"] != DBNull.Value ? (double)product["selling_price"] : 0;
                            int qty = order["qty"] != DBNull.Value ? (int)order["qty"] : 0;
                            warehouse["total_value"] = totalValue + (sellingPrice * qty);

                            warehouse WarehouseTable = Tables.getWarehosue(warehouse["name"].ToString());
                            if (WarehouseTable.database.Select($"product_id = {product["id"]}").Length != 0)
                            {
                                DataRow productInWarehouse = WarehouseTable.database.Select($"product_id = {product["id"]}")[0];
                                productInWarehouse["qty"] = (int)productInWarehouse["qty"] + qty;
                                WarehouseTable.updateChanges();
                            }
                            else
                            {
                                DataRow productInWarehouse = WarehouseTable.database.NewRow();
                                productInWarehouse["product_id"] = product["id"];
                                productInWarehouse["qty"] = order["qty"];
                                productInWarehouse["shelf_id"] = DBNull.Value;
                                productInWarehouse["width"] = 0;
                                productInWarehouse["height"] = 0;
                                productInWarehouse["length"] = 0;
                                productInWarehouse["on_shelf_level"] = DBNull.Value;
                                productInWarehouse["is_in_box"] = false;

                                WarehouseTable.database.Rows.Add(productInWarehouse);
                                WarehouseTable.updateChanges();
                            }
                        }
                    }

                    order["status"] = (sender as ComboBox).SelectedItem.ToString();
                    Tables.orders.updateChanges();
                }

                MessageBox.Show("Order has been updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Navigation.PreviousPage != null)
            {
                Navigation.OpenPage(Navigation.PreviousPage.GetType());
            }
            else
            {
                Navigation.OpenPage(Navigation.GetTypeByName("TransportsPage"));
            }
        }

        private void InspectTransportPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
        }
    }
}
