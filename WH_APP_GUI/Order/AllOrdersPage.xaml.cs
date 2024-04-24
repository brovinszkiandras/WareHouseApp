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

namespace WH_APP_GUI.Order
{
    public partial class AllOrdersPage : Page
    {
        public AllOrdersPage()
        {
            InitializeComponent();
            IniCities();

            DisplayProducts(NearbyOrders);
        }

        private void DisplayProducts(Panel panel)
        {
            List<string[]> name_address = SQL.SqlQuery($"SELECT user_name, address FROM {Tables.orders.actual_name} GROUP BY user_name, address");
            for (int i = 0; i < name_address.Count; i++)
            {
                if (Tables.orders.getOrdersOfAUser(name_address[i][0], name_address[i][1]).Length != 0)
                {
                    DataRow dataOfOrder = Tables.orders.getOrdersOfAUser(name_address[i][0], name_address[i][1])[0];

                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;
                    border.BorderThickness = new Thickness(2);
                    border.Margin = new Thickness(5);

                    StackPanel mainStackPanel = new StackPanel();

                    StackPanel imagePanel = new StackPanel();
                    imagePanel.Orientation = Orientation.Horizontal;
                    imagePanel.HorizontalAlignment = HorizontalAlignment.Center;

                    int qtyOfAllProd = 0;
                    int maxPrice = 0;
                    double sumVolume = 0;
                    foreach (DataRow order in Tables.orders.getOrdersOfAUser(name_address[i][0], name_address[i][1]))
                    {
                        qtyOfAllProd += int.Parse(order["qty"].ToString());
                        maxPrice += int.Parse(Tables.orders.getProduct(order)["selling_price"].ToString());
                        sumVolume += double.Parse(Tables.orders.getProduct(order)["volume"].ToString());

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

                    foreach (DataRow order in Tables.orders.getOrdersOfAUser(name_address[i][0], name_address[i][1]))
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

                    Button button = new Button();
                    button.Content = "Take";
                    button.Margin = new Thickness(5);
                    button.MaxWidth = 150;

                    mainStackPanel.Children.Add(imagePanel);
                    mainStackPanel.Children.Add(new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Margin = new Thickness(5), Child = userInfoGrid });

                    Border borderForSum = new Border();
                    borderForSum.BorderBrush = Brushes.Black;
                    borderForSum.BorderThickness = new Thickness(1, 0, 1, 1);
                    borderForSum.Margin = new Thickness(5);

                    Label sumVolumeInfo = new Label();
                    sumVolumeInfo.Content = $"Sum volume: {sumVolume}(m^2)";
                    sumVolumeInfo.HorizontalAlignment = HorizontalAlignment.Center;

                    borderForSum.Child = sumVolumeInfo;

                    mainStackPanel.Children.Add(borderForSum);
                    mainStackPanel.Children.Add(scrollViewer);
                    mainStackPanel.Children.Add(button);

                    border.Child = mainStackPanel;
                    panel.Children.Add(border);
                }
            }
        }
        
        private Dictionary<string, DataRow> Cities_Dictionary = new Dictionary<string, DataRow>();
        private void IniCities()
        {
            Cities_Dictionary.Clear();
            city_id.Items.Clear();
            foreach (DataRow warehouse in Tables.cities.database.Rows)
            {
                if (Tables.warehouses.getOrders(warehouse).Length != 0)
                {
                    Cities_Dictionary.Add(warehouse["city_name"].ToString(), warehouse);
                    city_id.Items.Add(warehouse["city_name"].ToString());
                }
            }
        }
    }
}
