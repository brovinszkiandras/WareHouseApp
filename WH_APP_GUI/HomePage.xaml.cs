using System;
using System.Collections.Generic;
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

namespace WH_APP_GUI
{
    //TODO: a frissitésel kezdeni kell valamit, amikor létrehozok pl egy új felhasználót(ez lehet akár raktár is) akkor utána nem frissiti a show oldalt, még akkor sem amikor bezárom majd vissza lépek, csak a program ujrainditásakor frissit
    public partial class HomePage : Window
    {
        public HomePage()
        {
            InitializeComponent();

            string[] inspectionItems = new string[]
            {
                "Inspect all Warehouses",
                "Inspect all Employees",
                "Inspect all Orders",
                "Inspect Products",
                "Inspect Staff",
                "Inspect all Car",
                "Inspect all Transport",
                "Inspect all Forklift",
                "Access to Database"
            };            

            #region Show Permission btns
            int indexOfGrid = 1;
            if (User.DoesHavePermission(inspectionItems[0]))
            {
                Button btn = new Button();
                btn.Content = inspectionItems[0];
                btn.Click += InspectAllWarehouses_Click;
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            if (User.DoesHavePermission(inspectionItems[1]))
            {
                Button btn = new Button();
                btn.Content = inspectionItems[1];
                btn.Click += InspectAllEmployees_Click;
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            if (User.DoesHavePermission(inspectionItems[2]))
            {
                Button btn = new Button();
                btn.Content = inspectionItems[2];
                btn.Click += InspectAllOrders_Click;
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            if (User.DoesHavePermission(inspectionItems[3]))
            {
                Button btn = new Button();
                btn.Content = inspectionItems[3];
                btn.Click += InspectProducts_Click;
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            if (User.DoesHavePermission(inspectionItems[4]))
            {
                Button btn = new Button();
                btn.Content = inspectionItems[4];
                btn.Click += InspectAllStaff_Click;
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            if (User.DoesHavePermission(inspectionItems[5]) && SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Fleet'"))
            {
                Button btn = new Button();
                btn.Content = inspectionItems[5];
                btn.Click += InspectAllCars_Click;
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            if (User.DoesHavePermission(inspectionItems[6]) && SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Fleet'"))
            {
                Button btn = new Button();
                btn.Content = inspectionItems[6];
                btn.Click += InspectAllTransport_Click;
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);
                indexOfGrid++;
            }

            if (User.DoesHavePermission(inspectionItems[7]) && SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Forklift'"))
            {
                Button btn = new Button();
                btn.Content = inspectionItems[7];
                btn.Click += InspectAllForkliftst_Click;
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            if (User.DoesHavePermission(inspectionItems[8]))
            {
                Button btn = new Button();
                btn.Content = inspectionItems[8];
                btn.Click += Database_Click;
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            #endregion
        }

        private void InspectAllWarehouses_Click(object sender, RoutedEventArgs e)
        {
            WarehousesPage warehousesPage = new WarehousesPage();
            this.Close();
            warehousesPage.Show();
        }

        private void InspectAllEmployees_Click(object sender, RoutedEventArgs e)
        {
            EmployeesPage employeesPage = new EmployeesPage();
            this.Close();
            employeesPage.Show();
        }

        private void InspectAllOrders_Click(object sender, RoutedEventArgs e)
        {
            OrdersPage ordersPage = new OrdersPage();
            this.Close();
            ordersPage.Show();
        }

        private void InspectProducts_Click(object sender, RoutedEventArgs e)
        {
            ProductsPage productsPage = new ProductsPage();
            this.Close();
            productsPage.Show();
        }

        private void InspectAllStaff_Click(object sender, RoutedEventArgs e)
        {
            StaffPage staffPage = new StaffPage();
            this.Close();
            staffPage.Show();
        }

        private void InspectAllCars_Click(object sender, RoutedEventArgs e)
        {
            CarsPage carsPage = new CarsPage();
            this.Close();
            carsPage.Show();
        }

        private void InspectAllTransport_Click(object sender, RoutedEventArgs e)
        {
            TransportsPage transportsPage = new TransportsPage();
            this.Close();
            transportsPage.Show();
        }

        private void InspectAllForkliftst_Click(object sender, RoutedEventArgs e)
        {
            ForkliftsPage forkliftsPage = new ForkliftsPage();
            this.Close();
            forkliftsPage.Show();
        }

        private void Database_Click(object sender, RoutedEventArgs e)
        {
            AdminHomePage adminHomePage = new AdminHomePage();
            this.Close();
            adminHomePage.Show();
        }
    }
}
