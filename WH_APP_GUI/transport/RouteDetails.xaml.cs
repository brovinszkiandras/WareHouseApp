using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HtmlAgilityPack;
using Microsoft.Maps.MapControl.WPF;

namespace WH_APP_GUI.transport
{
    public partial class RouteDetails : Window
    {
        DataRow Transport = null;
        private Map terkep = new Map();
        public RouteDetails(DataRow transport)
        {
            InitializeComponent();
            Transport = transport;
            DisplayAllOrders();
            Ini_GasPrices();
            Ini_Map();
        }
        private void DisplayAllOrders()
        {
            Orders.Children.Clear();
            foreach (DataRow order in Tables.transports.getOrders(Transport))
            {
                DisplayOneOrder(order);
            }
        }   
        private void DisplayOneOrder(DataRow order)
        {
            Expander OneOrder = new Expander();
            OneOrder.Margin = new Thickness(5);
            OneOrder.Header = "Order";

            Border border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Black;
            StackPanel stackPanel = new StackPanel();

            Label username = new Label();
            username.Content = $"Username: {order["user_name"]}";
            stackPanel.Children.Add(username);

            Label address = new Label();
            address.Content = $"Address: {order["address"]}";
            stackPanel.Children.Add(address);

            Label city = new Label();
            city.Content = $"Address: {Tables.orders.getCity(order)["city_name"]}";
            stackPanel.Children.Add(city);

            Button viewOnMap = new Button();
            viewOnMap.Content = "View On Map";
            viewOnMap.Margin = new Thickness(5);
            viewOnMap.Tag = order;
            viewOnMap.Click += ViewOnMapClick;
            stackPanel.Children.Add(viewOnMap);

            border.Child = stackPanel;
            OneOrder.Content = border;
            Orders.Children.Add(OneOrder);
        }

        private void Ini_Map()
        {
            terkep.IsEnabled = false;
            MapDisplay.Children.Add(terkep);
            terkep.CredentialsProvider = new ApplicationIdCredentialsProvider("I28YbqAL3vpfFHWSLW5x~bGccdfvqXsmwkAA8zHurUw~Apx4iHJNCNHKm28KE8CDvxw6wAeIp4-8Yz1DDnwyIa81h9Obx4dD-xlgWz3mrIq8");

            MapPolyline polyline = new MapPolyline();
            polyline.Stroke = new SolidColorBrush(Colors.Black);
            polyline.StrokeThickness = 5;
            polyline.Opacity = 0.7;

            double lat = double.Parse(Tables.warehouses.getCity(Tables.transports.getWarehouse(Transport))["latitude"].ToString());
            double lon = double.Parse(Tables.warehouses.getCity(Tables.transports.getWarehouse(Transport))["longitude"].ToString());

            terkep.Center = new Location(lat, lon);
            terkep.ZoomLevel = 8;

            terkep.Children.Add(polyline);
        }

        private void ViewOnMapClick(object sender, RoutedEventArgs e)
        {
            DataRow order = (sender as Button).Tag as DataRow;
            if (order != null) 
            {
                double lat = double.Parse(Tables.orders.getCity(order)["latitude"].ToString());
                double lon = double.Parse(Tables.orders.getCity(order)["longitude"].ToString());

                terkep.Center = new Location(lat, lon);
                terkep.ZoomLevel = 8;
            }
        }

        private void Ini_GasPrices()
        {
            GasPrices.Children.Clear();

            //Web scraping
            string url = "https://holtankoljak.hu/index.php";

            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(url);

                var priceNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'price')]");
                if (priceNodes != null)
                {
                    string[] types = { "95 - E10", "D - B7", "100 - E5" };
                    int index = 0;
                    int count = 0;

                    foreach (var node in priceNodes)
                    {
                        var typeNode = node.SelectSingleNode(".//text()[not(parent::span)]");
                        var priceNode = node.SelectSingleNode(".//span[@class='ar']");
                        if (typeNode != null && priceNode != null)
                        {
                            StackPanel stackPanel = new StackPanel();
                            stackPanel.Orientation = Orientation.Vertical;

                            if (count == 0)
                            {
                                Label label = new Label();
                                label.Content = types[index];
                                stackPanel.Children.Add(label);

                                index++;
                                count = 3;
                            }

                            string uzemanyagTipusa = typeNode.InnerText.Trim();
                            string ar = priceNode.InnerText.Trim();

                            Button button = new Button();
                            button.Content = $"{uzemanyagTipusa} - {ar} Ft";
                            stackPanel.Children.Add(button);

                            GasPrices.Children.Add(stackPanel);

                            count--;
                        }
                    }
                }
                else
                {
                    TextBlock error = new TextBlock();
                    error.Text = "The gas prices can not be loaded!";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
                TextBlock error = new TextBlock();
                error.TextWrapping = TextWrapping.Wrap;
                error.Text = "The gas prices can not be loaded!";
            }
        }
    }
}
