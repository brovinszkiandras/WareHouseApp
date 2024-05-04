using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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

namespace WH_APP_GUI.transport
{
    public partial class SetGasPrices : Window
    {
        class Gas
        {
            public string Type { get; set; }
            public double Price { get; set; }
            public Gas(string line)
            {
                string[] dataArray = line.Split(';');

                Type = dataArray[0];
                Price = double.Parse(dataArray[1], CultureInfo.InvariantCulture);
            }
        }
        DataRow Transport = null;
        private double GasPrice = 0;
        private double AllKm = 0;
        public SetGasPrices(DataRow transport)
        {
            InitializeComponent();
            Transport = transport;
            Ini_GasPrices();
            Ini_TransportDetails();
        }

        private void Ini_TransportDetails()
        {
            Dictionary<DataRow, int> CityCount = new Dictionary<DataRow, int>();

            foreach (DataRow order in Tables.transports.getOrders(Transport))
            {
                DataRow city = Tables.orders.getCity(order);
                if (!CityCount.ContainsKey(city))
                {
                    CityCount.Add(city, 1);
                }
                else
                {
                    CityCount[city]++;
                }
            }

            double TotalKM = 0;
            DataRow PreviousCity = null;
            foreach (var city in CityCount)
            {
                if (PreviousCity == null)
                {
                    double lat1 = double.Parse(Tables.warehouses.getCity(Tables.transports.getWarehouse(Transport))["latitude"].ToString());
                    double lon1 = double.Parse(Tables.warehouses.getCity(Tables.transports.getWarehouse(Transport))["longitude"].ToString());
                    double lat2 = double.Parse(city.Key["latitude"].ToString());
                    double lon2 = double.Parse(city.Key["longitude"].ToString());

                    double distance = CalculateDistance(lat1, lon1, lat2, lon2);
                    TotalKM += distance;
                }
                else
                {
                    double lat1 = double.Parse(PreviousCity["latitude"].ToString());
                    double lon1 = double.Parse(PreviousCity["longitude"].ToString());
                    double lat2 = double.Parse(city.Key["latitude"].ToString());
                    double lon2 = double.Parse(city.Key["longitude"].ToString());

                    double distance = CalculateDistance(lat1, lon1, lat2, lon2);
                    TotalKM += distance;
                }
                PreviousCity = city.Key;
            }

            double lastlat1 = double.Parse(PreviousCity["latitude"].ToString());
            double lastlon1 = double.Parse(PreviousCity["longitude"].ToString());

            double warehouselat1 = double.Parse(Tables.warehouses.getCity(Tables.transports.getWarehouse(Transport))["latitude"].ToString());
            double warehouselon1 = double.Parse(Tables.warehouses.getCity(Tables.transports.getWarehouse(Transport))["longitude"].ToString());

            TotalKM = TotalKM + CalculateDistance(lastlat1, lastlon1, warehouselat1, warehouselon1);

            Details.Text = $"The transport included a total of {CityCount.Count()} cities for {Tables.transports.getOrders(Transport).Length} separate orders." +
                           $" Starting from and returning from the {Tables.transports.getWarehouse(Transport)["name"]} warehouse, the estimated kilometer is {Math.Round(TotalKM, 2)}km." +
                           " Choose a gas rate from below.";

            AllKm = Math.Round(TotalKM, 2);
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371;

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;

            return distance;
        }

        private double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private void Ini_GasPrices()
        {
            GasPrices.Children.Clear();

            List<Gas> gasPrices = new List<Gas>();

            string url = "https://holtankoljak.hu/index.php";

            var web = new HtmlWeb();
            var doc = web.Load(url);

            var priceNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'price')]");
            if (priceNodes != null)
            {
                foreach (var node in priceNodes)
                {
                    var typeNode = node.SelectSingleNode(".//text()[not(parent::span)]");
                    var priceNode = node.SelectSingleNode(".//span[@class='ar']");
                    if (typeNode != null && priceNode != null)
                    {
                        string uzemanyagTipusa = typeNode.InnerText.Trim();
                        string ar = priceNode.InnerText.Trim();
                        Gas gas = new Gas($"{uzemanyagTipusa};{ar}");
                        gasPrices.Add(gas);
                    }
                }
            }

            for (int i = 0; i < gasPrices.Count; i++)
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = Brushes.Black;
                border.Margin = new Thickness(5);

                StackPanel stackPanel = new StackPanel();

                if (i <= 2)
                {
                    Label label = new Label();
                    label.Content = $"100-E5: ";
                    label.Margin = new Thickness(5);
                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                    stackPanel.Children.Add(label);
                }
                else if (i <= 5)
                {
                    Label label = new Label();
                    label.Content = $"D-B7";
                    label.Margin = new Thickness(5);
                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                    stackPanel.Children.Add(label);
                }
                else if (i <= 8)
                {
                    Label label = new Label();
                    label.Content = $"95-E10";
                    label.Margin = new Thickness(5);
                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                    stackPanel.Children.Add(label);
                }
                    
                Button price = new Button();
                price.Content = $"{gasPrices[i].Type} : {gasPrices[i].Price}";
                price.Margin = new Thickness(5);
                price.Tag = gasPrices[i].Price;
                price.Click += GetGasPriceClick;
                stackPanel.Children.Add(price);

                border.Child = stackPanel;
                GasPrices.Children.Add(border);
            }
        }

        private void GetGasPriceClick(object sender, RoutedEventArgs e)
        {
            double gasPrice = (double)(sender as Button).Tag;
            if (gasPrice != 0)
            {
                GasPrice = gasPrice;
            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (GasPrice != 0)
            {
                Controller.AddToRevnue_A_Day_Expenditure(Tables.transports.getWarehouse(Transport), AllKm * GasPrice);
                DataRow Warehouse = Tables.transports.getWarehouse(Transport);
                Warehouse["total_spending"] = Warehouse["total_spending"] != DBNull.Value ? (double)Warehouse["total_spending"] + AllKm * GasPrice : AllKm * GasPrice;

                Transport.Delete();
                Tables.warehouses.updateChanges();
                Tables.transports.updateChanges();
                Tables.transports.Refresh();

                this.Close();
            }
            else
            {
                MessageBox.Show("There is no any gas price selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
