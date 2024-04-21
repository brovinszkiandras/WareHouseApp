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
        private Map terkep = new Map();
        private Type PreviousPageType;
        private DataRow Warehouse;
        public InspectWarehouse(Page previousPage, DataRow warehouse)
        {
            PreviousPageType = previousPage.GetType();
            Warehouse = warehouse;
            InitializeComponent();

            if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'City'"))
            {
                terkep.IsEnabled = true;
                MapDisplay.Children.Add(terkep);
                terkep.CredentialsProvider = new ApplicationIdCredentialsProvider("I28YbqAL3vpfFHWSLW5x~bGccdfvqXsmwkAA8zHurUw~Apx4iHJNCNHKm28KE8CDvxw6wAeIp4-8Yz1DDnwyIa81h9Obx4dD-xlgWz3mrIq8");

                MapPolyline polyline = new MapPolyline();
                polyline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
                polyline.StrokeThickness = 5;
                polyline.Opacity = 0.7;

                double lat = double.Parse(Tables.warehouses.getCity(Warehouse)["latitude"].ToString());
                double lon = double.Parse(Tables.warehouses.getCity(Warehouse)["longitude"].ToString());

                terkep.Center = new Location(lat, lon);
                terkep.ZoomLevel = 10;

                terkep.Children.Add(polyline);
            }

            if (User.currentUser.Table == Tables.staff.database)
            {
                User.tempWarehouse = Warehouse;
            }

            List<string[]> revenue_a_day = SQL.SqlQuery($"SELECT * FROM `revenue_a_day` WHERE `warehouse_id` = {Warehouse["id"]};");
            if (revenue_a_day.Count() > 0)
            {
                RevenueBorder.Visibility = Visibility.Visible;
                NoRevenue.Visibility = Visibility.Collapsed;
                Ini_Revnue_A_Day();
            }
            else
            {
                RevenueBorder.Visibility = Visibility.Collapsed;
                NoRevenue.Visibility = Visibility.Visible;
            }
        }
        private void Ini_Revnue_A_Day()
        {
            List<string[]> revenue_a_day = SQL.SqlQuery($"SELECT `date`, `total_expenditure`, `total_income` FROM `revenue_a_day` WHERE `warehouse_id` = {Warehouse["id"]};");

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

        }

        private void OrdersInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProductsInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {
           WarehouseProductsPage page = new WarehouseProductsPage();
           Navigation.content2.Navigate(page);
        }

        private void FleetInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DocksInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ForkliftInspectToWarehouse_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
