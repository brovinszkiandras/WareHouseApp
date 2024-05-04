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


namespace WH_APP_GUI
{
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
            Navigation.RemoveParent();

            Loaded += HomeLoaded;

            Grid.SetColumn(Navigation.content2, 1);
            Grid.SetRow(Navigation.content2, 1);

            alapgrid.Children.Add(Navigation.content2);

            UserNameDisplay.Content = User.currentUser["name"].ToString();
            UserEmailDisplay.Content = User.currentUser["email"].ToString();

            Navigation.ReturnParam = null;
            Navigation.OpenPage(Navigation.GetTypeByName("WarehousesPage"));

            Navigation.content2.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            #region Show Permission btns

            Button WarehouseBtn = new Button();
            WarehouseBtn.Content = "Warehouses";
            WarehouseBtn.Click += InspectAllWarehouses_Click;
            WarehouseBtn.Style = (Style)this.Resources["GoldenButtonStyle"];
            Menu.Children.Add(WarehouseBtn);

            if (User.DoesHavePermission("Inspect all Employees"))
            {
                Button btn = new Button();
                btn.Content = "Employees";
                btn.Click += InspectAllEmployees_Click;
                btn.Style = (Style)this.Resources["GoldenButtonStyle"];
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Inspect all Orders"))
            {
                Button btn = new Button();  
                
                btn.Content = "Orders";
                btn.Click += InspectAllOrders_Click;
                btn.Style = (Style)this.Resources["GoldenButtonStyle"];
                btn.Tag = "Orders";
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Inspect Products"))
            {
                Button btn = new Button();
                btn.Content = "Products";
                btn.Click += InspectProducts_Click;
                btn.Style = (Style)this.Resources["GoldenButtonStyle"];
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Inspect Staff"))
            {
                Button btn = new Button();
                btn.Content = "Staffs";
                btn.Click += InspectAllStaff_Click;
                btn.Style = (Style)this.Resources["GoldenButtonStyle"];
                Menu.Children.Add(btn);
            }

            if (Tables.features.isFeatureInUse("Fleet"))
            {
                if (User.DoesHavePermission("Inspect all Transport"))
                {
                    Button btn = new Button();
                    btn.Content = "Transports";
                    btn.Click += InspectAllTransport_Click;
                    btn.Style = (Style)this.Resources["GoldenButtonStyle"];
                    Menu.Children.Add(btn);
                }
                else if (User.DoesHavePermission("Handle own Transport"))
                {
                    Button btn = new Button();
                    btn.Content = "Own Transports";
                    btn.Click += InspectAllTransport_Click;
                    btn.Style = (Style)this.Resources["GoldenButtonStyle"];
                    Menu.Children.Add(btn);
                }
            }

            if (Tables.features.isFeatureInUse("Fleet"))
            {
                if (User.DoesHavePermission("Inspect all Car") && Tables.features.isFeatureInUse("Fleet") == true)
                {
                    Button btn = new Button();
                    btn.Content = "Cars";
                    btn.Click += InspectAllCars_Click;
                    btn.Style = (Style)this.Resources["GoldenButtonStyle"];
                    Menu.Children.Add(btn);
                }
            }

            if (User.DoesHavePermission("Inspect all Forklift") && Tables.features.isFeatureInUse("Forklift") == true)
            {
                Button btn = new Button();
                btn.Content = "Forklifts";
                btn.Click += InspectAllForkliftst_Click;
                btn.Style = (Style)this.Resources["GoldenButtonStyle"];
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Inspect Log") && Tables.features.isFeatureInUse("Log") == true)
            {
                Button btn = new Button();
                btn.Content = "Log";
                btn.Click += InspectLog_Click;
                btn.Style = (Style)this.Resources["GoldenButtonStyle"];
                Menu.Children.Add(btn);
            }

            if (User.DoesHavePermission("Access to Database"))
            {
                Button btn = new Button();
                btn.Content = "Settings";
                btn.Click += Database_Click;
                btn.Style = (Style)this.Resources["GoldenButtonStyle"];
                Menu.Children.Add(btn);
            }

            #endregion
        }
        private void HomeLoaded(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);

            if (parentWindow != null)
            {
                parentWindow.Closed += UserLogOut;
            }
        }

        private void UserLogOut(object sender, EventArgs e)
        {
            if (User.currentUser != null)
            {
                if (Tables.features.isFeatureInUse("Activity"))
                {
                    if (Tables.employees.database.Select($"email = '{User.currentUser["email"]}'").Length != 0)
                    {
                        Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has been logged out from the application.");
                        User.currentUser["is_loggedin"] = false;
                        Tables.employees.updateChanges();
                    }
                }
            }
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
            logout.FontSize = e.NewSize.Height * 0.02;

        }
        private void InspectAllWarehouses_Click(object sender, RoutedEventArgs e)
        {
            Navigation.ReturnParam = null;
            Navigation.OpenPage(Navigation.GetTypeByName("WarehousesPage"));
        }

        private void InspectAllEmployees_Click(object sender, RoutedEventArgs e)
        {
            Navigation.ReturnParam = null;
            Navigation.OpenPage(Navigation.GetTypeByName("EmployeesPage"));
        }

        private void InspectAllOrders_Click(object sender, RoutedEventArgs e)
        {
            Navigation.ReturnParam = null;
            Navigation.OpenPage(Navigation.GetTypeByName("AllOrdersPage"));
        }

        private void InspectProducts_Click(object sender, RoutedEventArgs e)
        {
            Navigation.ReturnParam = null;
            Navigation.OpenPage(Navigation.GetTypeByName("ProductsPage"));
        }

        private void InspectAllStaff_Click(object sender, RoutedEventArgs e)
        {
            Navigation.ReturnParam = null;
            Navigation.OpenPage(Navigation.GetTypeByName("StaffPage"));
        }

        private void InspectAllCars_Click(object sender, RoutedEventArgs e)
        {
            Navigation.ReturnParam = null;
            Navigation.OpenPage(Navigation.GetTypeByName("CarsPage"));
        }

        private void InspectAllTransport_Click(object sender, RoutedEventArgs e)
        {
            Navigation.ReturnParam = null;
            Navigation.OpenPage(Navigation.GetTypeByName("TransportsPage"));
        }

        private void InspectAllForkliftst_Click(object sender, RoutedEventArgs e)
        {
            Navigation.ReturnParam = null;
            Navigation.OpenPage(Navigation.GetTypeByName("ForkliftsPage"));
        }

        private void InspectLog_Click(object sender, RoutedEventArgs e)
        {
            Navigation.ReturnParam = null;
            Navigation.OpenPage(Navigation.GetTypeByName("LogDisplay"));
        }

        private void Database_Click(object sender, RoutedEventArgs e)
        {
            Navigation.ReturnParam = null;
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
        private void menubutton_Click(object sender, RoutedEventArgs e)
        {
            if (Menu.IsVisible)
            {
                Menu.Visibility = Visibility.Collapsed;
            }
            else
            {
                Menu.Visibility = Visibility.Visible;
            }
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            if (User.currentUser != null)
            {
                Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has been logged out from the application.");
                if (User.currentUser.Table.TableName == "employees")
                {
                    if (Tables.features.isFeatureInUse("Activity"))
                    {
                        User.currentUser["is_loggedin"] = false;
                        Tables.employees.updateChanges();
                    }
                }

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                User.MainWindow.Close();
                User.currentUser = null;
                User.MainWindow = mainWindow;
            }
        }
    }
}
