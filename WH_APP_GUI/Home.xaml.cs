using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            Navigation.OpenPage(Navigation.GetTypeByName("WarehousesPage"));
            Navigation.content2.Background = new SolidColorBrush(Color.FromArgb(255, 66, 71, 105));
            Navigation.content2.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            #region Show Permission btns

            if (User.DoesHavePermission("Inspect all Warehouses"))
            {
                Button btn = new Button();
                btn.Content = "Warehouses";
                btn.Click += InspectAllWarehouses_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Inspect all Employees"))
            {
                Button btn = new Button();
                btn.Content = "Employees";
                btn.Click += InspectAllEmployees_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Inspect all Orders"))
            {
                Button btn = new Button();  
                
                btn.Content = "Orders";
                btn.Click += InspectAllOrders_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                btn.Tag = "Orders";
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Inspect Products"))
            {
                Button btn = new Button();
                btn.Content = "Products";
                btn.Click += InspectProducts_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Inspect Staff"))
            {
                Button btn = new Button();
                btn.Content = "Staffs";
                btn.Click += InspectAllStaff_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Inspect all Transport") && Tables.features.isFeatureInUse("Fleet") == true)
            {
                Button btn = new Button();
                btn.Content = "Transports";
                btn.Click += InspectAllTransport_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Inspect all Car") && Tables.features.isFeatureInUse("Fleet") == true)
            {
                Button btn = new Button();
                btn.Content = "Cars";
                btn.Click += InspectAllCars_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Inspect all Forklift") && Tables.features.isFeatureInUse("Forklift") == true)
            {
                Button btn = new Button();
                btn.Content = "Forklifts";
                btn.Click += InspectAllForkliftst_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
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

            if (User.DoesHavePermission("Modify all Dock") && Tables.features.isFeatureInUse("Dock") == true)
            {
                Button btn = new Button();
                btn.Content = "Dock";
                btn.Click += InspectDock_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Access to Database"))
            {
                Button btn = new Button();
                btn.Content = "Settings";
                btn.Click += Database_Click;
                btn.Style = (Style)this.Resources["ElegantButtonStyle"];
                Menu.Children.Add(btn);
            }

            #endregion
        }

        private void menucolum_MouseEnter(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            foreach (Button child in Menu.Children)
            {
                child.Content += btn.Tag.ToString();
            }
        }
        private void Home_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
        }
        private void InspectAllWarehouses_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("WarehousesPage"));
        }

        private void InspectAllEmployees_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("EmployeesPage"));
        }

        private void InspectAllOrders_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("AllOrdersPage"));
        }

        private void InspectProducts_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("ProductsPage"));
        }

        private void InspectAllStaff_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("StaffPage"));
        }

        private void InspectAllCars_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("CarsPage"));
        }

        private void InspectAllTransport_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("TransportsPage"));
        }

        private void InspectAllForkliftst_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("ForkliftsPage"));
        }

        private void InspectLog_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("LogDisplay"));
        }

        private void InspectDock_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("DockPage"));
        }

        private void Database_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("AdminHomePage"));
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
