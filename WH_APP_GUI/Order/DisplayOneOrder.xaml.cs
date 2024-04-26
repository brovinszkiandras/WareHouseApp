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

namespace WH_APP_GUI.Order
{
    public partial class DisplayOneOrder : Page
    {
        DataRow[] orders = null;
        private string UserName = null;
        private string Address = null;
        public DisplayOneOrder(DataRow dataOfOrder)
        {
            InitializeComponent();

            UserName = dataOfOrder["user_name"].ToString();
            Address = dataOfOrder["address"].ToString();

            if (dataOfOrder != null)
            {
                orders = Tables.orders.getOrdersOfAUser(UserName, Address);
            }

            OneOrderDisplay(OrdersDisplay, UserName, Address);
        }

        private void OneOrderDisplay(Panel panel, string username, string address)
        {
            OrdersDisplay.Children.Clear();
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

            StackPanel productPanel = new StackPanel();
            productPanel.Margin = new Thickness(5);

            foreach (DataRow order in Tables.orders.getOrdersOfAUser(username, address))
            {
                Border productBorder = new Border();
                productBorder.BorderBrush = Brushes.Black;
                productBorder.BorderThickness = new Thickness(0, 0, 0, 1);

                if (User.Warehouse() != null)
                {
                    UniformGrid productGrid = new UniformGrid();
                    productGrid.Columns = 4;

                    Label productLabel = new Label();
                    productLabel.Content = Tables.orders.getProduct(order)["name"] + ": ";
                    Label priceLabel = new Label();
                    priceLabel.Content = $"Price: {Tables.orders.getProduct(order)["selling_price"]} - Ft";
                    Label qtyLabel = new Label();
                    qtyLabel.Content = $"Quantity: {order["qty"]}";

                    Button statusOfOrder = new Button();
                    statusOfOrder.Margin = new Thickness(5);

                    if (order["status"].ToString() != "Registered")
                    {
                        if (order["status"].ToString() == "In Warehouse")
                        {
                            statusOfOrder.Content = "Complete";
                            statusOfOrder.Background = Brushes.RoyalBlue;
                            statusOfOrder.IsEnabled = true;
                            statusOfOrder.Tag = order;
                            statusOfOrder.Click += DoneClick;

                            if ((int)order["warehouse_id"] == (int)User.Warehouse()["id"])
                            {
                                statusOfOrder.IsEnabled = true;
                            }
                            else
                            {
                                statusOfOrder.IsEnabled = false;
                            }
                        }
                        else
                        {
                            statusOfOrder.Content = "Completed";
                            statusOfOrder.IsEnabled = false;
                            statusOfOrder.Background = Brushes.Green;
                        }
                    }
                    else
                    {
                        statusOfOrder.Content = "Unassigned";
                        statusOfOrder.Background = Brushes.Gray;
                        statusOfOrder.IsEnabled = false;
                    }

                    productGrid.Children.Add(productLabel);
                    productGrid.Children.Add(priceLabel);
                    productGrid.Children.Add(qtyLabel);
                    productGrid.Children.Add(statusOfOrder);

                    productBorder.Child = productGrid;
                    productPanel.Children.Add(productBorder);
                }
                else
                {
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
            }

            mainStackPanel.Children.Add(imagePanel);
            mainStackPanel.Children.Add(new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Margin = new Thickness(5), Child = userInfoGrid });

            mainStackPanel.Children.Add(productPanel);

            Border status = new Border();
            status.BorderBrush = Brushes.Black;
            status.BorderThickness = new Thickness(1, 0, 1, 1);
            status.Margin = new Thickness(5);

            Label statusLBL = new Label();
            statusLBL.Content = $"Status: {dataOfOrder["status"]}";
            statusLBL.HorizontalAlignment = HorizontalAlignment.Center;

            status.Child = statusLBL;
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

                borderForSum.Child = sumVolumeInfo;
                mainStackPanel.Children.Add(borderForSum);
            }

            if (dataOfOrder["warehouse_id"] == DBNull.Value)
            {
                Button button = new Button();
                button.Content = "Take";
                button.Margin = new Thickness(5);
                button.MaxWidth = 150;
                button.IsEnabled = false;
                mainStackPanel.Children.Add(button);

                if (User.Warehouse() != null)
                {
                    if (CanComplete(dataOfOrder["user_name"].ToString(), dataOfOrder["address"].ToString()))
                    {
                        button.IsEnabled = true;
                        button.Tag = Tables.orders.getOrdersOfAUser(username, address);
                        button.Click += TakeClick;
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
                inWarehouse.HorizontalContentAlignment = HorizontalAlignment.Center;
                inWarehouse.Content = $"This order already in {Tables.orders.getWarehouse(dataOfOrder)["name"]}";
                mainStackPanel.Children.Add(inWarehouse);
            }

            if (User.Warehouse() != null)
            {
                if (Tables.features.isFeatureInUse("Fleet"))
                {
                    if (IsOrderInTransport(dataOfOrder["user_name"].ToString(), dataOfOrder["address"].ToString()))
                    {
                        Label inTransport = new Label();
                        inTransport.Background = Brushes.LightSteelBlue;
                        inTransport.Foreground = Brushes.Black;
                        inTransport.Margin = new Thickness(5);
                        inTransport.BorderBrush = Brushes.Black;
                        inTransport.BorderThickness = new Thickness(1);
                        inTransport.HorizontalContentAlignment = HorizontalAlignment.Center;
                        DataRow transport = Tables.orders.getTransport(dataOfOrder);
                        inTransport.Content = $"This order will be transported at {transport["start_date"]}, by {Tables.transports.getEmployee(transport)["name"]}";
                        mainStackPanel.Children.Add(inTransport);
                    }
                    else if (!IsOrederFinished(dataOfOrder["user_name"].ToString(), dataOfOrder["address"].ToString()))
                    {
                        Button addToTransport = new Button();
                        addToTransport.Background = Brushes.Green;
                        addToTransport.Foreground = Brushes.Black;
                        addToTransport.Margin = new Thickness(5);
                        addToTransport.BorderBrush = Brushes.Black;
                        addToTransport.BorderThickness = new Thickness(1);
                        addToTransport.HorizontalContentAlignment = HorizontalAlignment.Center;
                        addToTransport.Content = $"Add to transport";
                        addToTransport.Tag = Tables.orders.getOrdersOfAUser(username, address);
                        addToTransport.Click += AddToTransport;

                        mainStackPanel.Children.Add(addToTransport);
                    }
                }
                else if (Tables.features.isFeatureInUse("Dock") && Tables.features.isFeatureInUse("Fleet"))
                {
                    if (IsOrderInDock(dataOfOrder["user_name"].ToString(), dataOfOrder["address"].ToString()))
                    {
                        Label inDock = new Label();
                        inDock.Background = Brushes.LightSteelBlue;
                        inDock.Foreground = Brushes.Black;
                        inDock.Margin = new Thickness(5);
                        inDock.BorderBrush = Brushes.Black;
                        inDock.BorderThickness = new Thickness(1);
                        inDock.MaxWidth = 350;
                        inDock.HorizontalContentAlignment = HorizontalAlignment.Center;
                        DataRow dock = Tables.orders.getDock(dataOfOrder);
                        inDock.Content = $"{dock["name"]} dock already contains this order.";
                        mainStackPanel.Children.Add(inDock);
                    }
                    else if (!IsOrederFinished(dataOfOrder["user_name"].ToString(), dataOfOrder["address"].ToString()))
                    {
                        Button addToDock = new Button();
                        addToDock.Background = Brushes.Green;
                        addToDock.Foreground = Brushes.Black;
                        addToDock.Margin = new Thickness(5);
                        addToDock.BorderBrush = Brushes.Black;
                        addToDock.BorderThickness = new Thickness(1);
                        addToDock.MaxWidth = 350;
                        addToDock.HorizontalContentAlignment = HorizontalAlignment.Center;
                        addToDock.Content = $"Add to dock";
                        addToDock.Tag = Tables.orders.getOrdersOfAUser(username, address);
                        addToDock.Click += AddToDock;
                        if (IsOrederFinished(dataOfOrder["user_name"].ToString(), dataOfOrder["address"].ToString()))
                        {
                            addToDock.IsEnabled = false;
                        }
                        mainStackPanel.Children.Add(addToDock);
                    }
                }
            }

            border.Child = mainStackPanel;
            panel.Children.Add(border);
        }
        private void AddToDock(object sender, RoutedEventArgs e)
        {
            DataRow[] orders = (sender as Button).Tag as DataRow[];
            if (orders.Length != 0)
            {
                AddOrdersToDock addOrderToDock = new AddOrdersToDock(orders);
                addOrderToDock.ShowDialog();

                OneOrderDisplay(OrdersDisplay, UserName, Address);
            }
        }

        private void AddToTransport(object sender, RoutedEventArgs e)
        {
            DataRow[] orders = (sender as Button).Tag as DataRow[];
            if (orders.Length != 0)
            {
                AddOrderToTransport addOrderToTransport = new AddOrderToTransport(orders);
                addOrderToTransport.ShowDialog();

                OneOrderDisplay(OrdersDisplay, UserName, Address);
            }
        }

        private bool IsOrderInTransport(string username, string address)
        {
            foreach (DataRow order in Tables.orders.getOrdersOfAUser(username, address))
            {
                if (Tables.features.isFeatureInUse("Fleet"))
                {
                    if (order["transport_id"] == DBNull.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        private bool IsOrderInDock(string username, string address)
        {
            foreach (DataRow order in Tables.orders.getOrdersOfAUser(username, address))
            {
                if (Tables.features.isFeatureInUse("Dock") && !Tables.features.isFeatureInUse("Fleet"))
                {
                    if (order["dock_id"] == DBNull.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsOrederFinished(string username, string address)
        {
            foreach (DataRow order in Tables.orders.getOrdersOfAUser(username, address))
            {
                if (Tables.features.isFeatureInUse("Fleet"))
                {
                    if (order["transport_id"] == DBNull.Value && order["status"].ToString() == "Finished")
                    {
                        return false;
                    }
                }
                else if (Tables.features.isFeatureInUse("Dock") && !Tables.features.isFeatureInUse("Fleet"))
                {
                    if (order["dock_id"] == DBNull.Value && order["status"].ToString() == "Finished")
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void DoneClick(object sender, RoutedEventArgs e)
        {
            DataRow order = (sender as Button).Tag as DataRow;
            if (order != null)
            {
                if (User.Warehouse() != null)
                {
                    DataRow orderInWarehouse = null;
                    try
                    {
                        orderInWarehouse = User.WarehouseTable().database.Select($"product_id = {order["product_id"]}")[0];
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("The warehouse does not contains this product.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        //throw;
                    }

                    if (orderInWarehouse != null)
                    {
                        if (int.Parse(orderInWarehouse["qty"].ToString()) >= int.Parse(order["qty"].ToString()))
                        {
                            orderInWarehouse["qty"] = int.Parse(orderInWarehouse["qty"].ToString()) - int.Parse(order["qty"].ToString());
                            order["status"] = "Finished";
                            User.WarehouseTable().updateChanges();
                            Tables.orders.updateChanges();

                            MessageBox.Show("The product(s) has been added to the order!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            if (MessageBox.Show("The warehouse does not contain all the products related to this order. Would you like to continue the transaction?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                            {
                                orderInWarehouse["qty"] = 0;
                                order["status"] = "Finished";
                                User.WarehouseTable().updateChanges();
                                Tables.orders.updateChanges();

                                MessageBox.Show("The product(s) has been added to the order!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }

                        OneOrderDisplay(OrdersDisplay, UserName, Address);

                        bool OrderDone = true;
                        foreach (DataRow orderGroupBy in Tables.orders.getOrdersOfAUser(order["user_name"], order["address"]))
                        {
                            if (orderGroupBy["status"].ToString() != "Finished")
                            {
                                OrderDone = false;
                            }
                        }

                        if (OrderDone)
                        {
                            if (!Tables.features.isFeatureInUse("Dock") && !Tables.features.isFeatureInUse("Fleet"))
                            {
                                foreach (DataRow orderGroupBy in Tables.orders.getOrdersOfAUser(order["user_name"], order["address"]))
                                {
                                    orderGroupBy.Delete();
                                    Tables.orders.updateChanges();
                                }
                            }
                            MessageBox.Show("This order has been successfully completed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
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

        private void TakeClick(object sender, RoutedEventArgs e)
        {
            DataRow[] ordersByPersonAddress = (sender as Button).Tag as DataRow[];
            if (User.Warehouse() != null && ordersByPersonAddress.Length != 0)
            {
                foreach (DataRow order in ordersByPersonAddress)
                {
                    order["warehouse_id"] = User.Warehouse()["id"];
                    order["status"] = "In Warehouse";
                }
                Tables.orders.updateChanges();
                MessageBox.Show($"The order has been added to the warehouse[{User.Warehouse()["name"]}]!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                OneOrderDisplay(OrdersDisplay, UserName, Address);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("AllOrdersPage"));
        }
    }
}
