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
        public InspectTransportPage(DataRow transport)
        {
            InitializeComponent();
            Transport = transport;
            DisplayOneTransport(transport);
        }


        private void DisplayOneTransport(DataRow transport)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(2);
            border.Background = Brushes.White;

            StackPanel mainStackPanel = new StackPanel();

            Label driver = new Label();
            driver.Content = $"{Tables.transports.getEmployee(transport)["name"]}";
            driver.HorizontalContentAlignment = HorizontalAlignment.Center;
            driver.Margin = new Thickness(5);
            driver.BorderBrush = Brushes.Black;
            driver.BorderThickness = new Thickness(0, 0, 0, 1);

            mainStackPanel.Children.Add(driver);

            UniformGrid datas = new UniformGrid();
            datas.Rows = 2;
            datas.Margin = new Thickness(5);

            Label car = new Label();
            car.Content = $"Car: {Tables.transports.getCar(transport)["type"]}";
            car.BorderBrush = Brushes.Black;
            car.BorderThickness = new Thickness(1, 0, 0, 1);
            car.Margin = new Thickness(0, 0, 5, 0);
            datas.Children.Add(car);

            Label status = new Label();
            status.Content = $"Status: {transport["status"]}";
            status.BorderBrush = Brushes.Black;
            status.BorderThickness = new Thickness(1, 0, 0, 1);
            status.Margin = new Thickness(0, 0, 5, 0);
            datas.Children.Add(status);

            Label start_date = new Label();
            start_date.Content = $"Start date: {transport["start_date"]}";
            start_date.BorderBrush = Brushes.Black;
            start_date.BorderThickness = new Thickness(1, 0, 0, 1);
            start_date.Margin = new Thickness(0, 0, 5, 0);
            datas.Children.Add(start_date);

            Label end_date = new Label();
            end_date.Content = $"End date: {transport["end_date"]}";
            end_date.BorderBrush = Brushes.Black;
            end_date.BorderThickness = new Thickness(1, 0, 0, 1);
            end_date.Margin = new Thickness(0, 0, 5, 0);
            datas.Children.Add(end_date);

            Label warehouse = new Label();
            warehouse.Content = $"Warehouse: {Tables.transports.getWarehouse(transport)["name"]}";
            warehouse.BorderBrush = Brushes.Black;
            warehouse.BorderThickness = new Thickness(1, 0, 0, 1);
            warehouse.Margin = new Thickness(0, 0, 5, 0);
            datas.Children.Add(warehouse);

            if (Tables.features.isFeatureInUse("Dock") == true)
            {
                Label dock = new Label();
                dock.Content = $"Dock: {Tables.transports.getDock(transport)["name"]}";
                dock.BorderBrush = Brushes.Black;
                dock.BorderThickness = new Thickness(1, 0, 0, 1);
                dock.Margin = new Thickness(0, 0, 5, 0);
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
                    DisplayOneOrder(orders, name_address[0][0], name_address[0][1]);
                }
                mainStackPanel.Children.Add(orders);
            }

            border.Child = mainStackPanel;
            transportDisplay.Children.Add(border);
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

            border.Child = mainStackPanel;
            panel.Children.Add(border);
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
    }
}
