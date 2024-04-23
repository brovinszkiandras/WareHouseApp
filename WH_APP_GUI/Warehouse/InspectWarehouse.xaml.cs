using Microsoft.Maps.MapControl.WPF;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WH_APP_GUI.sectors;
using WH_APP_GUI.WarehouseTableFolder;

namespace WH_APP_GUI.Warehouse
{
    public partial class InspectWarehouse : Page
    {
        private void inspectWarehouse_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MapDisplay.Height = e.NewSize.Height * 0.4;
        }
        private Map terkep = new Map();
        private Type PreviousPageType;
        private DataRow Warehouse;
        public InspectWarehouse(DataRow warehouse)
        {
            InitializeComponent();
            Warehouse = warehouse;

            WarehouseNameDisplay.Content = $"{warehouse["name"]} - {Tables.warehouses.getCity(warehouse)["city_name"]}";

            if (User.currentUser.Table == Tables.staff.database)
            {
                User.tempWarehouse = Warehouse;
            }
            Ini_City();
            Ini_Revnue_A_Day();
        }
        private void Ini_City()
        {
            if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'City'"))
            {
                terkep.IsEnabled = false;
                MapDisplay.Children.Add(terkep);
                terkep.CredentialsProvider = new ApplicationIdCredentialsProvider("I28YbqAL3vpfFHWSLW5x~bGccdfvqXsmwkAA8zHurUw~Apx4iHJNCNHKm28KE8CDvxw6wAeIp4-8Yz1DDnwyIa81h9Obx4dD-xlgWz3mrIq8");

                MapPolyline polyline = new MapPolyline();
                polyline.Stroke = new SolidColorBrush(Colors.Black);
                polyline.StrokeThickness = 5;
                polyline.Opacity = 0.7;

                double lat = double.Parse(Tables.warehouses.getCity(Warehouse)["latitude"].ToString());
                double lon = double.Parse(Tables.warehouses.getCity(Warehouse)["longitude"].ToString());

                terkep.Center = new Location(lat, lon);
                terkep.ZoomLevel = 10;

                terkep.Children.Add(polyline);
                
            }
        }
        private void Ini_Revnue_A_Day()
        {
            List<string[]> revenue_a_day = SQL.SqlQuery($"SELECT `date`, `total_expenditure`, `total_income` FROM `revenue_a_day` WHERE `warehouse_id` = {Warehouse["id"]} GROUP BY `date`;");
            if (revenue_a_day.Count() > 0)
            {
                NoRevenue.Visibility = Visibility.Collapsed;

                string MaxValue = Warehouse["total_value"].ToString();
                string SellingPrice = SQL.FindOneDataFromQuery($"SELECT SUM(products.selling_price) FROM {Warehouse["name"]} INNER JOIN {Tables.products.actual_name} ON {Warehouse["name"]}.product_id = {Tables.products.actual_name}.id");
                string BuyingPrice = SQL.FindOneDataFromQuery($"SELECT SUM(products.buying_price) FROM {Warehouse["name"]} INNER JOIN {Tables.products.actual_name} ON {Warehouse["name"]}.product_id = {Tables.products.actual_name}.id");

                double WarehouseMaxValue = MaxValue != "" ? double.Parse(MaxValue) : 0;
                double AllSellingPrice = SellingPrice != "" ? double.Parse(SellingPrice) : 0;
                double AllBuyingPrice = BuyingPrice != "" ? double.Parse(BuyingPrice) : 0;

                WarehouseTotalSpending.Maximum = WarehouseMaxValue;
                WarehouseTotalSpendingLBL.Content = Warehouse["total_spending"] + " - Ft";
                bool ValidateWarehouseTotalSpending = Warehouse["total_spending"].ToString() != "" ? true : false;
                WarehouseTotalSpending.Value = ValidateWarehouseTotalSpending ? double.Parse(Warehouse["total_spending"].ToString()) : 0;

                WarehouseTotalIncome.Maximum = WarehouseMaxValue;
                WarehouseTotalIncomeLBL.Content = Warehouse["total_income"] + " - Ft";
                bool ValidateWarehouseTotalIncome = Warehouse["total_income"].ToString() != "" ? true : false;
                WarehouseTotalIncome.Value = ValidateWarehouseTotalIncome ? double.Parse(Warehouse["total_income"].ToString()) : 0;

                ProductsTotalSellingPrice.Maximum = WarehouseMaxValue;
                ProductsTotalSellingPrice.Value = AllSellingPrice;
                ProductsTotalSellingPriceLBL.Content = AllSellingPrice + " - Ft";

                ProductsTotalBuyingPrice.Maximum = WarehouseMaxValue;
                ProductsTotalBuyingPrice.Value = AllBuyingPrice;
                ProductsTotalBuyingPriceLBL.Content = AllBuyingPrice + " - Ft";

                int index = 0;
                int Dayindex = 0;
                for (int i = 0; i < 7; i++)
                {
                    if (i >= revenue_a_day.Count)
                    {
                        break;
                    }
                    else
                    {
                        double total_expenditure = double.Parse(revenue_a_day[i][1]);
                        double total_income = double.Parse(revenue_a_day[i][2]);
                        double max = total_expenditure + total_income;

                        ProgressBar Spending = new ProgressBar();
                        Spending.MinHeight = 150;
                        Spending.Value = total_expenditure;
                        Spending.Maximum = max;
                        Spending.Foreground = Brushes.Red;
                        Spending.Orientation = Orientation.Vertical;
                        Grid.SetColumn(Spending, index);
                        Grid.SetRow(Spending, 3);
                        RevenueDisplay.Children.Add(Spending);
                        index++;

                        ProgressBar Income = new ProgressBar();
                        Income.MinHeight = 150;
                        Income.Value = total_income;
                        Income.Maximum = max;
                        Spending.Foreground = Brushes.Green;
                        Income.Orientation = Orientation.Vertical;
                        Grid.SetColumn(Income, index);
                        Grid.SetRow(Income, 3);
                        RevenueDisplay.Children.Add(Income);
                        index += 2;

                        Label day = new Label();
                        day.Content = DateTime.Parse(revenue_a_day[i][0]).DayOfWeek.ToString();
                        Grid.SetRow(day, 4);
                        Grid.SetColumn(day, Dayindex);
                        Dayindex += 3;
                        Grid.SetColumnSpan(day, 3);
                        RevenueDisplay.Children.Add(day);
                    }
                }
            }
            else
            {
                NoRevenue.Visibility = Visibility.Visible;
            }
        }
        private void InspectWarehousePage_Unloaded(object sender, RoutedEventArgs e)
        {
            MapDisplay.Children.Remove(terkep);
            terkep.Dispose();
        }

        private void SectorsInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {
            sectorIndexWindow page = new sectorIndexWindow();
            Navigation.content2.Navigate(page);
        }

        private void EmployeesInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("EmployeesPage"), Warehouse);
        }

        private void OrdersInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProductsInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {
            WarehouseProductsPage page = new WarehouseProductsPage();
            Navigation.content2.Navigate(page);
            //Navigation.OpenPage(Navigation.GetTypeByName("ProductsPage"), Warehouse);
        }

        private void FleetInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DocksInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("DockPage"), Warehouse);
        }

        private void ForkliftInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("ForkliftsPage"), Warehouse);
        }

        private void Back_AllWarehouse_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }
        private void GoBack()
        {
            if (PreviousPageType != null)
            {
                Page previousPage = (Page)Activator.CreateInstance(PreviousPageType);
                Navigation.content2.Navigate(previousPage);
            }
            else
            {
                WarehousesPage warehousesPage = new WarehousesPage();
                Navigation.content2.Navigate(warehousesPage);
            }
        }

        private void MapDisplay_MouseEnter(object sender, MouseEventArgs e)
        {
            mainScrollviewer.CanContentScroll = false;
        }

        private void MapDisplay_MouseLeave(object sender, MouseEventArgs e)
        {
            mainScrollviewer.CanContentScroll = true;
        }
    }
}
