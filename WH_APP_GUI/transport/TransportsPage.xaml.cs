using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
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
using WH_APP_GUI.carsFolder;
using WH_APP_GUI.transport;

namespace WH_APP_GUI.transport
{
    public partial class TransportsPage : Page
    {
        public TransportsPage()
        {
            InitializeComponent();
            displayTransports();
            Back.Visibility = Visibility.Collapsed;
        }

        private DataRow Warehouse = null;
        public TransportsPage(DataRow warehouseFromPage)
        {
            InitializeComponent();
            displayTransports();
            Warehouse = warehouseFromPage;
        }
        private void DisplayOneTransport(DataRow transport, int lastRow)
        {
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = GridLength.Auto;
            transportGrid.RowDefinitions.Add(rowDefinition);

            TextBlock driver = new TextBlock();
            driver.Text = Tables.transports.getEmployee(transport)["name"].ToString();
            driver.FontSize = 15;
            driver.TextWrapping = TextWrapping.Wrap;
            Grid.SetRow(driver, lastRow);
            Grid.SetColumn(driver, 0);

            transportGrid.Children.Add(driver);

            TextBlock car = new TextBlock();
            car.Text = Tables.transports.getCar(transport)["type"].ToString();
            car.FontSize = 15;
            car.TextWrapping = TextWrapping.Wrap;
            Grid.SetRow(car, lastRow);
            Grid.SetColumn(car, 1);

            transportGrid.Children.Add(car);

            TextBlock status = new TextBlock();
            status.Text = transport["status"].ToString();
            status.FontSize = 15;
            status.TextWrapping = TextWrapping.Wrap;

            Grid.SetRow(status, lastRow);
            Grid.SetColumn(status, 2);

            transportGrid.Children.Add(status);

            TextBlock start_date = new TextBlock();
            DateTime startDate = (DateTime)transport["start_date"];
            start_date.Text = startDate.ToString("yyyy-MM-dd HH:mm:ss");
            start_date.FontSize = 15;
            start_date.TextWrapping = TextWrapping.Wrap;
            Grid.SetRow(start_date, lastRow);
            Grid.SetColumn(start_date, 3);

            transportGrid.Children.Add(start_date);

            TextBlock end_date = new TextBlock();
            if (transport["end_date"] != DBNull.Value)
            {
                DateTime endDate = (DateTime)transport["end_date"];
                end_date.Text = endDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            end_date.FontSize = 15;
            end_date.TextWrapping = TextWrapping.Wrap;
            Grid.SetRow(end_date, lastRow);
            Grid.SetColumn(end_date, 4);

            transportGrid.Children.Add(end_date);


            if (Tables.features.isFeatureInUse("Dock") == true)
            {
                TextBlock dock = new TextBlock();
                dock.Text = Tables.transports.getDock(transport)["name"].ToString();
                dock.FontSize = 15;
                dock.TextWrapping = TextWrapping.Wrap;
                Grid.SetRow(dock, lastRow);
                Grid.SetColumn(dock, 5);

                transportGrid.Children.Add(dock);
            }


            if (User.DoesHavePermission("Assign to transport"))
            {
                Button inspect = new Button();
                inspect.Content = "Inspect";
                inspect.FontSize = 15;
                inspect.Background = Brushes.Green;
                inspect.Tag = transport["id"];
                Grid.SetRow(inspect, lastRow);
                Grid.SetColumn(inspect, 6);

                transportGrid.Children.Add(inspect);
            }

            if (User.DoesHavePermission("Modify Transport") || User.DoesHavePermission("Modify all Transport"))
            {
                Button edit = new Button();
                edit.Content = "Edit";
                edit.FontSize = 15;
                edit.Background = Brushes.Green;
                edit.Click += Edit_Click;
                edit.Tag = transport["id"];

                Grid.SetRow(edit, lastRow);
                Grid.SetColumn(edit, 7);

                transportGrid.Children.Add(edit);

                Button delete = new Button();
                delete.Content = "Delete";
                delete.FontSize = 15;
                delete.Background = Brushes.Green;
                delete.Tag = transport["id"];
                delete.Click += Delete_Click;
                Grid.SetRow(delete, lastRow);
                Grid.SetColumn(delete, 8);

                transportGrid.Children.Add(delete);
            }
            else
            {
                Create.Visibility = Visibility.Collapsed;
            }
        }
        public void displayTransports()
        {
            transportGrid.Children.Clear();
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
            CreateTransportPage createTransportPage = new CreateTransportPage();


            Navigation.content2.Navigate(createTransportPage);
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
