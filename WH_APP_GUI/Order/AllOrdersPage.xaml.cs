using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml.Linq;
using WH_APP_GUI.Warehouse;

namespace WH_APP_GUI.Order
{
    public partial class AllOrdersPage : Page
    {
        private DataRow Warehouse = null;
        private warehouse WarehouseTable = null;
        public AllOrdersPage()
        {
            InitializeComponent();

            BackBORDER.Visibility = Visibility.Collapsed;
            AllOrdersBORDER.Visibility = Visibility.Visible;

            DisplayAllOrders();
            Ini_warehouses();
            Ini_unassigned_cities();
            Ini_docks();
            SetPreviousPage();
        }
        public AllOrdersPage(DataRow warehouse)
        {
            InitializeComponent();

            Warehouse = warehouse;
            WarehouseTable = Tables.getWarehosue(warehouse["name"].ToString());

            BackBORDER.Visibility = Visibility.Visible;
            warehouse_idBORDER.Visibility = Visibility.Collapsed;

            Ini_docks(Warehouse);
            Ini_unassigned_cities();
            SetPreviousPage();

            List<string[]> name_address = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} WHERE warehouse_id = {Warehouse["id"]} GROUP BY user_name, address");
            for (int i = 0; i < name_address.Count; i++)
            {
                DisplayOneOrder(OrdersDisplay, name_address[i][0], name_address[i][1]);
            }
        }
        private bool CanComplete(string username, string address)
        {
            bool canComplete = true;
            foreach (DataRow order in Tables.orders.getOrdersOfAUser(username, address))
            {
                if (WarehouseTable.database.Select($"product_id = {order["product_id"]}").Length == 0)
                {
                    return false;
                }
                else
                {
                    int productsInTheWarehosue = WarehouseTable.database.Select($"product_id = {order["product_id"]}").Sum(row => (int)row["qty"]);
                    if (productsInTheWarehosue < (int)order["qty"])
                    {
                        canComplete = false;
                    }
                }
            }
            return canComplete;
        }

        private void SetPreviousPage()
        {
            if (Navigation.PreviousPage.GetType() == Navigation.GetTypeByName("DisplayOneOrder"))
            {
                if (Warehouse != null)
                {
                    InspectWarehouse inspectWarehouse = new InspectWarehouse(Warehouse);
                    Navigation.PreviousPage = inspectWarehouse;
                }
                else
                {
                    WarehousesPage warehousesPage = new WarehousesPage();
                    Navigation.PreviousPage = warehousesPage;
                }
            }
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
                maxPrice += int.Parse(Tables.orders.getProduct(order)["selling_price"].ToString()) * (int)order["qty"];
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

            mainStackPanel.Children.Add(imagePanel);
            mainStackPanel.Children.Add(new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Margin = new Thickness(5), Child = userInfoGrid });

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

                if (Warehouse != null)
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

            if (Warehouse != null)
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
                }
                else if(Tables.features.isFeatureInUse("Dock") && ! Tables.features.isFeatureInUse("Fleet"))
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
                }
            }

            Button inspectOrder = new Button();
            inspectOrder.Content = "Inspect order";
            inspectOrder.Margin = new Thickness(5);
            inspectOrder.MaxWidth = 150;
            inspectOrder.Tag = dataOfOrder;
            inspectOrder.Click += InspectOrder;
            mainStackPanel.Children.Add(inspectOrder);

            border.Child = mainStackPanel;
            panel.Children.Add(border);
        }
        private void InspectOrder(object sender, RoutedEventArgs e)
        {
            DataRow orderDatas = (sender as Button).Tag as DataRow;
            if (orderDatas != null)
            {
                if (Warehouse != null)
                {
                    Navigation.OpenPageWithParameters(Navigation.GetTypeByName("DisplayOneOrder"), orderDatas, Warehouse);
                    Navigation.ReturnParam = Warehouse;
                }
                else
                {
                    Navigation.OpenPage(Navigation.GetTypeByName("DisplayOneOrder"), orderDatas);
                }
                //Navigation.OpenPage(Navigation.GetTypeByName("DisplayOneOrder"), orderDatas);
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
                if (Tables.features.isFeatureInUse("Dock") && ! Tables.features.isFeatureInUse("Fleet"))
                {
                    if (order["dock_id"] == DBNull.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private Dictionary<string, DataRow> unassigned_city_id_Dictionary = new Dictionary<string, DataRow>();
        private void Ini_unassigned_cities()
        {
            unassigned_city_idBORDER.Visibility = Visibility.Visible;
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
            warehouse_idBORDER.Visibility = Visibility.Visible;
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
            if (Tables.features.isFeatureInUse("Fleet"))
            {
                dock_idBORDER.Visibility = Visibility.Visible;

                foreach (DataRow order in Tables.orders.database.Rows)
                {
                    if (order["transport_id"] != DBNull.Value)
                    {
                        DataRow transport = Tables.orders.getTransport(order);
                        if (! dock_id_Dictionary.ContainsKey(Tables.transports.getDock(transport)["name"].ToString()))
                        {
                            dock_id_Dictionary.Add(Tables.transports.getDock(transport)["name"].ToString(), Tables.transports.getDock(transport));
                            dock_id.Items.Add(Tables.transports.getDock(transport)["name"].ToString());
                        }
                    }
                }
            }
            else if (Tables.features.isFeatureInUse("Dock") && !Tables.features.isFeatureInUse("Fleet"))
            {
                dock_idBORDER.Visibility = Visibility.Visible;

                foreach (DataRow order in Tables.orders.database.Rows)
                {
                    if (order["dock_id"] != DBNull.Value)
                    {
                        DataRow dock = Tables.orders.getDock(order);
                        if (! dock_id_Dictionary.ContainsKey(dock["name"].ToString()))
                        {
                            dock_id_Dictionary.Add(dock["name"].ToString(), dock);
                            dock_id.Items.Add(dock["name"].ToString());
                        }
                    }
                }
            }
            else
            {
                dock_idBORDER.Visibility = Visibility.Collapsed;
            }
        }

        private void Ini_docks(DataRow warehouse)
        {
            if (Tables.features.isFeatureInUse("Fleet"))
            {
                dock_idBORDER.Visibility = Visibility.Visible;

                List<string> datas = SQL.GetElementOfListArray(SQL.SqlQuery($"SELECT transport_id FROM {Tables.orders.actual_name} WHERE transport_id IS NOT NULL AND warehouse_id = {warehouse["id"]}"));
                for (int i = 0; i < datas.Count; i++)
                {
                    DataRow transport = Tables.transports.database.Select($"id = {datas[i]}")[0];
                    if (!dock_id_Dictionary.ContainsKey(Tables.transports.getDock(transport)["name"].ToString()))
                    {
                        dock_id_Dictionary.Add(Tables.transports.getDock(transport)["name"].ToString(), Tables.transports.getDock(transport));
                        dock_id.Items.Add(Tables.transports.getDock(transport)["name"].ToString());
                    }
                }
            }
            else if (Tables.features.isFeatureInUse("Dock") && !Tables.features.isFeatureInUse("Fleet"))
            {
                dock_idBORDER.Visibility = Visibility.Visible;

                List<string[]> datas = SQL.SqlQuery($"SELECT dock_id FROM {Tables.orders.actual_name} WHERE dock_id IS NOT NULL AND warehouse_id = {warehouse["id"]}");
                for (int i = 0; i < datas.Count; i++)
                {
                    DataRow dock = Tables.docks.database.Select($"id = {datas[i][0]}")[0];
                    if (!dock_id_Dictionary.ContainsKey(dock["name"].ToString()))
                    {
                        dock_id_Dictionary.Add(dock["name"].ToString(), dock);
                        dock_id.Items.Add(dock["name"].ToString());
                    }
                }
            }
            else
            {
                dock_idBORDER.Visibility = Visibility.Collapsed;
            }
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
            ClearDatasOfComboBox();
        }

        private void warehouse_id_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (warehouse_id.SelectedIndex != -1)
            {
                OrdersDisplay.Children.Clear();
                int warehouseid = int.Parse(warehouse_id_Dictionary[warehouse_id.SelectedItem.ToString()]["id"].ToString());
                List<string[]> name_address = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} WHERE warehouse_id = {warehouseid} GROUP BY user_name, address");
                for (int i = 0; i < name_address.Count; i++)
                {
                    DisplayOneOrder(OrdersDisplay, name_address[i][0], name_address[i][1]);
                }
            }
            ClearDatasOfComboBox();
        }

        private void dock_id_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dock_id.SelectedIndex != -1)
            {
                OrdersDisplay.Children.Clear();
                int dockid = int.Parse(dock_id_Dictionary[dock_id.SelectedItem.ToString()]["id"].ToString());

                if (Tables.features.isFeatureInUse("Fleet"))
                {
                    DataRow transport = Tables.transports.database.Select($"dock_id = {dockid}")[0];
                    List<string[]> namesAddresses = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} WHERE transport_id = {transport["id"]} GROUP BY user_name, address");
                    for (int i = 0; i < namesAddresses.Count; i++)
                    {
                        DisplayOneOrder(OrdersDisplay, namesAddresses[i][0], namesAddresses[i][1]);
                    }
                }
                else if (Tables.features.isFeatureInUse("Dock") && !Tables.features.isFeatureInUse("Fleet"))
                {
                    List<string[]> namesAddresses = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} WHERE dock_id = {dockid} GROUP BY user_name, address");
                    for (int i = 0; i < namesAddresses.Count; i++)
                    {
                        DisplayOneOrder(OrdersDisplay, namesAddresses[i][0], namesAddresses[i][1]);
                    }
                }
            }
            ClearDatasOfComboBox();
        }

        private void ClearDatasOfComboBox()
        {
            unassigned_city_id.SelectedIndex = -1;
            warehouse_id.SelectedIndex = -1;
            dock_id.SelectedIndex = -1;
        }

        private void TakeClick(object sender, RoutedEventArgs e)
        {
            DataRow[] ordersByPersonAddress = (sender as Button).Tag as DataRow[];
            if (Warehouse != null && ordersByPersonAddress.Length != 0)
            {
                foreach (DataRow order in ordersByPersonAddress)
                {
                    order["warehouse_id"] = Warehouse["id"];
                    order["status"] = "In Warehouse";

                    if (Tables.features.isFeatureInUse("Storage"))
                    {
                        if (order["sum_volume"] == DBNull.Value)
                        {
                            if (order["product_id"] != DBNull.Value)
                            {
                                DataRow product = Tables.orders.getProduct(order);
                                double volume = product["volume"] != DBNull.Value ? (double)product["volume"] : 0;
                                order["sum_volume"] = volume * (int)order["qty"];
                            }
                        }
                    }
                }

                Tables.orders.updateChanges();
                MessageBox.Show($"The order has been added to the warehouse[{Warehouse["name"]}]!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                OrdersDisplay.Children.Clear();
                List<string[]> name_address = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} WHERE warehouse_id = {Warehouse["id"]} GROUP BY user_name, address");
                for (int i = 0; i < name_address.Count; i++)
                {
                    DisplayOneOrder(OrdersDisplay, name_address[i][0], name_address[i][1]);
                }
            }
        }

        private void AssignedOrders_Click(object sender, RoutedEventArgs e)
        {
            if (Warehouse != null)
            {
                OrdersDisplay.Children.Clear();
                List<string[]> name_address = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} WHERE warehouse_id = {Warehouse["id"]} GROUP BY user_name, address");
                for (int i = 0; i < name_address.Count; i++)
                {
                    DisplayOneOrder(OrdersDisplay, name_address[i][0], name_address[i][1]);
                }
                ClearDatasOfComboBox();
            }
            else
            {
                OrdersDisplay.Children.Clear();
                List<string[]> name_address = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} WHERE warehouse_id IS NOT NULL GROUP BY user_name, address");
                for (int i = 0; i < name_address.Count; i++)
                {
                    DisplayOneOrder(OrdersDisplay, name_address[i][0], name_address[i][1]);
                }
                ClearDatasOfComboBox();
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
            ClearDatasOfComboBox();
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
            ClearDatasOfComboBox();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Navigation.PreviousPage != null)
            {
                if (Warehouse != null)
                {
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
