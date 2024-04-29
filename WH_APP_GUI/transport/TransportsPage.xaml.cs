using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Shapes;
using WH_APP_GUI.carsFolder;
using WH_APP_GUI.transport;

namespace WH_APP_GUI.transport
{
    public partial class TransportsPage : Page
    {
        public TransportsPage()
        {
            InitializeComponent();
            IniTransports();
            Back.Visibility = Visibility.Collapsed;
        }

        private DataRow Warehouse = null;
        public TransportsPage(DataRow warehouseFromPage)
        {
            InitializeComponent();
            IniTransports();
            Warehouse = warehouseFromPage;
        }

        private void IniTransports()
        {
            if (User.DoesHavePermission("Inspect own Transport"))
            {
                if (Tables.employees.database.Select($"email = '{User.currentUser["email"]}'").Length != 0)
                {
                    int lastRow = 0;
                    foreach (DataRow transport in Tables.transports.database.Select($"employee_id = {User.currentUser["id"]}"))
                    {
                        DisplayOneTransport(transport, lastRow);
                        lastRow++;
                    }
                }
                else
                {
                    displayTransports();
                }
            }
        }

        private void DisplayOneTransport(DataRow transport, int lastRow)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(2);
            border.Background = Brushes.White;
            border.Margin = new Thickness(5);

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
            car.Margin = new Thickness(0,0,5,0);
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

            if (User.DoesHavePermission("Modify Transport") || User.DoesHavePermission("Modify all Transport"))
            {
                Button update = new Button();
                update.Content = "Update";
                update.MinWidth = 150;
                update.Margin = new Thickness(5);
                update.Tag = transport["id"];
                update.Click += Edit_Click;
                buttons.Children.Add(update);

                Button delete = new Button();
                delete.Content = "Delete";
                delete.MinWidth = 150;
                delete.Margin = new Thickness(5);
                delete.Tag = transport["id"];
                delete.Click += Delete_Click;
                buttons.Children.Add(delete);
            }
            else
            {
                Create.Visibility = Visibility.Collapsed;
            }

            if (User.DoesHavePermission("Assign to transport") || User.DoesHavePermission("Inspect own Transport"))
            {
                Button inspect = new Button();
                inspect.Content = "Inspect";
                inspect.MinWidth = 150;
                inspect.Tag = transport;
                inspect.Click += InspectTransport;
                inspect.Margin = new Thickness(5);
                buttons.Children.Add(inspect);
            }

            mainStackPanel.Children.Add(buttons);
            border.Child = mainStackPanel;
            transportDisplay.Children.Add(border);
        }

        private void InspectTransport(object sender, RoutedEventArgs e)
        {
            DataRow transport = (sender as Button).Tag as DataRow;
            if (transport != null)
            {
                Navigation.OpenPage(Navigation.GetTypeByName("InspectTransportPage"), transport);
            }
        }

        public void displayTransports()
        {
            transportDisplay.Children.Clear();
            if (Warehouse != null)
            {
                int lastRow = 0;
                foreach (DataRow transport in Tables.warehouses.getTransports(Warehouse))
                {
                    DisplayOneTransport(transport, lastRow);
                    lastRow++;
                }
            }
            else
            {
                int lastRow = 0;
                foreach (DataRow transport in Tables.transports.database.Rows)
                {
                    DisplayOneTransport(transport, lastRow);
                    lastRow++;
                }
            }

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Do you want to delete this transport?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                DataRow transport = Tables.transports.database.Select($"id = '{button.Tag}'")[0];
                if (transport != null)
                {
                    Tables.transports.getCar(transport)["ready"] = true;
                    Tables.transports.getDock(transport)["free"] = true;

                    transport.Delete();
                    Tables.transports.updateChanges();

                    displayTransports();
                }
            }  
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            DataRow transport = Tables.transports.database.Select($"id = '{button.Tag}'")[0];
            UpdateTransport updateTransport = new UpdateTransport(transport);

            Navigation.content2.Navigate(updateTransport);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("CreateTransportPage"));
            if (Warehouse != null)
            {
                Navigation.ReturnParam = Warehouse;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Navigation.PreviousPage != null)
            {
                if (Warehouse != null)
                {
                    Navigation.OpenPage(Navigation.PreviousPage.GetType(), Warehouse);
                }
                else
                {
                    Navigation.OpenPage(Navigation.PreviousPage.GetType());
                }
            }
            else
            {
                Navigation.OpenPage(Navigation.GetTypeByName("InspectWarehouse"));
            }
        }
    }
}
