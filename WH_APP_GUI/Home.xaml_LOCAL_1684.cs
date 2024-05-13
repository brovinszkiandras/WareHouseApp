using MahApps.Metro.IconPacks;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WH_APP_GUI.carsFolder;
using WH_APP_GUI.transport;
using Xceed.Wpf.Toolkit;

namespace WH_APP_GUI
{
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
            Navigation.RemoveParent();
            Grid.SetColumn(Navigation.content2, 1);
            Grid.SetRow(Navigation.content2, 1);
            Navigation.content2.Background = Brushes.WhiteSmoke;
            alapgrid.Children.Add(Navigation.content2);

            UserNameDisplay.Content = User.currentUser["name"].ToString();
            UserEmailDisplay.Content = User.currentUser["email"].ToString();
           
            string[] inspectionItems = new string[]
            {
                "Inspect all Warehouses",
                "Inspect all Employees",
                "Inspect all Orders",
                "Inspect Products",
                "Inspect Staff",
                "Inspect all Transport",
                "Inspect all Car",
                "Inspect all Forklift",
                "Access to Database"
            };


            #region Show Permission btns
            int indexOfGrid = 1;

            if (User.DoesHavePermission("Inspect all Warehouses"))
            {
                Button btn = new Button();
                btn.Content = "Warehouses";
                btn.Click += InspectAllWarehouses_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);
                indexOfGrid++;
            }

            if (User.DoesHavePermission("Inspect all Employees"))
            {
                Button btn = new Button();
                btn.Content = "Employees";
                btn.Click += InspectAllEmployees_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            if (User.DoesHavePermission("Inspect all Orders"))
            {

                PackIconMaterial icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Warehouse };
                IconButton btn = new IconButton();
                btn.Content = icon + "alma";
                btn.Click += InspectAllOrders_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                btn.Tag = "Alma";
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            if (User.DoesHavePermission("Inspect Products"))
            {
                Button btn = new Button();
                btn.Content = "Products";
                btn.Click += InspectProducts_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            if (User.DoesHavePermission("Inspect Staff"))
            {
                Button btn = new Button();
                btn.Content = "Staffs";
                btn.Click += InspectAllStaff_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            if (User.DoesHavePermission("Inspect all Transport") && Tables.features.isFeatureInUse("Fleet") == true)
            {
                Button btn = new Button();
                btn.Content = "Transports";
                btn.Click += InspectAllTransport_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            if (User.DoesHavePermission("Inspect all Car") && Tables.features.isFeatureInUse("Fleet") == true)
            {
                Button btn = new Button();
                btn.Content = "Cars";
                btn.Click += InspectAllCars_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            if (User.DoesHavePermission("Inspect all Forklift") && Tables.features.isFeatureInUse("Forklift") == true)
            {
                Button btn = new Button();
                btn.Content = "Forklifts";
                btn.Click += InspectAllForkliftst_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Inspect Log") && Tables.features.isFeatureInUse("Log") == true)
            {
                Button btn = new Button();
                btn.Content = "Log";
                btn.Click += InspectLog_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Access to Database"))
            {
                Button btn = new Button();
                btn.Content = "Settings";
                btn.Click += Database_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Grid.SetRow(btn, indexOfGrid);
                Menu.Children.Add(btn);

                indexOfGrid++;
            }

            #endregion
        }
        private void Home_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.025;
            }
        }
        private void InspectAllWarehouses_Click(object sender, RoutedEventArgs e)
        {
            // WarehousesPage warehousesPage = new WarehousesPage();
            // this.Hide();
            // warehousesPage.Show();
            Navigation.content2.Content = null;
            Navigation.content2.Navigate(new Uri("Warehouse/WarehousesPage.xaml", UriKind.Relative));
        }

        private void InspectAllEmployees_Click(object sender, RoutedEventArgs e)
        {
            //EmployeesPage employeesPage = new EmployeesPage();
            //this.Hide();
            //employeesPage.Show();
            Navigation.content2.Content = null;
            Navigation.content2.Navigate(new Uri("Employee/EmployeesPage.xaml", UriKind.Relative));
        }

        private void InspectAllOrders_Click(object sender, RoutedEventArgs e)
        {
            //OrdersPage ordersPage = new OrdersPage();
            //this.Hide();
            //ordersPage.Show();
            Navigation.content2.Content = null;
            Navigation.content2.Navigate(new Uri("OrdersPage.xaml", UriKind.Relative));
        }

        private void InspectProducts_Click(object sender, RoutedEventArgs e)
        {
            //    ProductsPage productsPage = new ProductsPage();
            //    this.Hide();
            //    productsPage.Show();
            Navigation.content2.Content = null;
            Navigation.content2.Navigate(new Uri("Product/ProductsPage.xaml", UriKind.Relative));
        }

        private void InspectAllStaff_Click(object sender, RoutedEventArgs e)
        {
            //StaffPage staffPage = new StaffPage();
            //this.Hide();
            //staffPage.Show();
           // StaffPage staffPage = new StaffPage();
            Navigation.content2.Content = null;
            Navigation.content2.Navigate(new Uri("Staff/StaffPage.xaml", UriKind.Relative));
        }

        private void InspectAllCars_Click(object sender, RoutedEventArgs e)
        {
            //CarsPage carsPage = new CarsPage();
            //this.Hide();
            //carsPage.Show();
            Navigation.content2.Content = null;
            CarsPage carsPage = new CarsPage();
            Navigation.content2.Navigate(carsPage);
       
        }

        private void InspectAllTransport_Click(object sender, RoutedEventArgs e)
        {
            //TransportsPage transportsPage = new TransportsPage();
            //this.Hide();
            //transportsPage.Show();
            TransportsPage transportsPage = new TransportsPage();
            Navigation.content2.Content = null;
            Navigation.content2.Navigate(transportsPage);
        }

        private void InspectAllForkliftst_Click(object sender, RoutedEventArgs e)
        {
            //ForkliftsPage forkliftsPage = new ForkliftsPage();
            //this.Hide();
            //forkliftsPage.Show();
            Navigation.content2.Content = null;
            Navigation.content2.Navigate(new Uri("Forklift/ForkliftsPage.xaml", UriKind.Relative));
        }

        private void InspectLog_Click(object sender, RoutedEventArgs e)
        {
            //ForkliftsPage forkliftsPage = new ForkliftsPage();
            //this.Hide();
            //forkliftsPage.Show();
            Navigation.content2.Content = null;
            Navigation.content2.Navigate(new Uri("LogDisplay.xaml", UriKind.Relative));
        }

        private void Database_Click(object sender, RoutedEventArgs e)
        {
            //AdminHomePage adminHomePage = new AdminHomePage();
            //this.Hide();
            //adminHomePage.Show();
            Navigation.content2.Content = null;
            Navigation.content2.Navigate(new Uri("AdminHomePage.xaml", UriKind.Relative));
        }

        private void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            foreach (Button children in Menu.Children)
            {
                if (children.Tag != null)
                {

                    children.Content += children.Tag.ToString();

                }
            }
        }

        private void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            foreach (Button children in Menu.Children)
            {
                if (children.Tag != null)
                {
                    string originalString = children.Content.ToString();
                    string stringtToRemove = children.Tag.ToString();

                    children.Content = originalString.Replace(stringtToRemove, "");
                }
            }
        }
    }
}
