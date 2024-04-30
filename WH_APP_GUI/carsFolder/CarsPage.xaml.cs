using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
using WH_APP_GUI;
using WH_APP_GUI.carsFolder;
using WH_APP_GUI.transport;
using WH_APP_GUI.Warehouse;

namespace WH_APP_GUI.carsFolder
{
    public partial class CarsPage : Page
    {
        public CarsPage()
        {
            InitializeComponent();
            Back.Visibility = Visibility.Collapsed;
            DisplayCars();

            if (!User.DoesHavePermission("Modify all Car"))
            {
                Create.Visibility = Visibility.Collapsed;
            }
        }
        private DataRow Warehouse = null;
        public CarsPage(DataRow warehouse)
        {
            InitializeComponent();
            Warehouse = warehouse;
            DisplayCars();

            if (!User.DoesHavePermission("Modify all Car") || !User.DoesHavePermission("Modify Car"))
            {
                Create.Visibility = Visibility.Collapsed;
            }

            InspectWarehouse inspectWarehouse = new InspectWarehouse(warehouse);
            Navigation.PreviousPage = inspectWarehouse;
        }
        private void DisplayOneCar(DataRow car, int lastRow)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(2);
            border.Background = Brushes.White;
            border.Margin = new Thickness(5);

            StackPanel mainStackPanel = new StackPanel();
            
            Image image = new Image();
            image.Width = 80;
            image.Height = 80;

            string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
            if (Directory.Exists(targetDirectory))
            {
                string imageFileName = "DefaultCarPicture.png";
                string imagePath = Path.Combine(targetDirectory, imageFileName);
                if (File.Exists(imagePath))
                {
                    string fileName = Path.GetFileName(imagePath);
                    string targetFilePath = Path.Combine(targetDirectory, fileName);

                    BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));
                    image.Source = bitmap;
                }
            }

            Label plateNumber = new Label();
            plateNumber.Content = "Platenumber: " + car["plate_number"];
            plateNumber.HorizontalContentAlignment = HorizontalAlignment.Center;
            plateNumber.Margin = new Thickness(5);
            plateNumber.BorderBrush = Brushes.Black;
            plateNumber.BorderThickness = new Thickness(1,0,1,1);

            mainStackPanel.Children.Add(image);
            mainStackPanel.Children.Add(plateNumber);

            Expander datas = new Expander();
            datas.Header = "Datas";
            datas.Margin = new Thickness(5);

            StackPanel grids = new StackPanel();

            UniformGrid defaultDatas = new UniformGrid();
            defaultDatas.Rows = 3;
            defaultDatas.Margin = new Thickness(5);

            Label type = new Label();
            type.Content = $"Type: {car["type"]}";
            type.BorderBrush = Brushes.Black;
            type.BorderThickness = new Thickness(1, 0, 0, 1);
            defaultDatas.Children.Add(type);

            Label ready = new Label();
            ready.Content = $"Ready: " + ((bool)car["ready"] ? "Ready" : "Not ready");
            ready.BorderBrush = Brushes.Black;
            ready.BorderThickness = new Thickness(1, 0, 0, 1);
            defaultDatas.Children.Add(ready);

            Label km = new Label();
            km.Content = $"Km: {car["km"]}";
            km.BorderBrush = Brushes.Black;
            km.BorderThickness = new Thickness(1, 0, 0, 1);
            defaultDatas.Children.Add(km);

            Label lastService = new Label();
            lastService.Content = $"Last service: {car["last_service"]}";
            lastService.BorderBrush = Brushes.Black;
            lastService.BorderThickness = new Thickness(1, 0, 0, 1);
            defaultDatas.Children.Add(lastService);

            Label lastexam = new Label();
            lastexam.Content = $"Last exam: {car["last_exam"]}";
            lastexam.BorderBrush = Brushes.Black;
            lastexam.BorderThickness = new Thickness(1, 0, 0, 1);
            defaultDatas.Children.Add(lastexam);

            Label warehouse = new Label();
            warehouse.Content = $"Warehouse: {Tables.cars.getWarehouse(car)["name"]}";
            warehouse.BorderBrush = Brushes.Black;
            warehouse.BorderThickness = new Thickness(1, 0, 0, 1);
            defaultDatas.Children.Add(warehouse);

            grids.Children.Add(defaultDatas);

            if (Tables.features.isFeatureInUse("Storage"))
            {
                UniformGrid storageFeature = new UniformGrid();
                storageFeature.Columns = 2;
                storageFeature.Margin = new Thickness(5);

                Label storage = new Label();
                storage.Content = $"Storage: {car["storage"]}";
                storage.BorderBrush = Brushes.Black;
                storage.BorderThickness = new Thickness(1, 0, 0, 1);
                storageFeature.Children.Add(storage);

                Label carrying_capacity = new Label();
                carrying_capacity.Content = $"Carrying capacity: {car["carrying_capacity"]}";
                carrying_capacity.BorderBrush = Brushes.Black;
                carrying_capacity.BorderThickness = new Thickness(1, 0, 0, 1);
                storageFeature.Children.Add(carrying_capacity);

                grids.Children.Add(storageFeature);
            }

            if (Tables.features.isFeatureInUse("Fuel"))
            {
                UniformGrid fuelFeature = new UniformGrid();
                fuelFeature.Columns = 2;
                fuelFeature.Margin = new Thickness(5);

                Label consumption = new Label();
                consumption.Content = $"Consumption: {car["consumption"]}";
                consumption.BorderBrush = Brushes.Black;
                consumption.BorderThickness = new Thickness(1, 0, 0, 1);
                fuelFeature.Children.Add(consumption);

                Label gas_tank_size = new Label();
                gas_tank_size.Content = $"Gas tank size: {car["gas_tank_size"]}";
                gas_tank_size.BorderBrush = Brushes.Black;
                gas_tank_size.BorderThickness = new Thickness(1, 0, 0, 1);
                fuelFeature.Children.Add(gas_tank_size);

                grids.Children.Add(fuelFeature);
            }

            datas.Content = grids;
            mainStackPanel.Children.Add(datas);

            if (User.DoesHavePermission("Modify Car") || User.DoesHavePermission("Modify all Car"))
            {
                Button edit = new Button();
                edit.Content = "Edit";
                edit.FontSize = 15;
                edit.Foreground = Brushes.White;
                edit.Background = Brushes.Black;
                edit.Click += Edit_Click;
                edit.Tag = car["id"];

                Grid.SetRow(edit, lastRow);
                Grid.SetColumn(edit, 4);

                mainStackPanel.Children.Add(edit);

                Button delete = new Button();
                delete.Content = "Delete";
                delete.FontSize = 15;
                delete.Foreground = Brushes.White;
                delete.Background = Brushes.Black;
                delete.Tag = car["id"];
                delete.Click += Delete_Click;
                Grid.SetRow(delete, lastRow);
                Grid.SetColumn(delete, 5);

                mainStackPanel.Children.Add(delete);
            }
            else
            {
                Create.Visibility = Visibility.Collapsed;
            }

            border.Child = mainStackPanel;
            CarsDisplay.Children.Add(border);
        }
        public void DisplayCars()
        {
            CarsDisplay.Children.Clear();
            if (Warehouse != null)
            {
                int lastRow = 0;
                foreach (DataRow car in Tables.warehouses.getCars(Warehouse))
                {
                    DisplayOneCar(car, lastRow);
                    lastRow++;
                }
            }
            else
            {
                int lastRow = 0;
                foreach (DataRow car in Tables.cars.database.Rows)
                {
                    DisplayOneCar(car, lastRow);
                    lastRow++;
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Do you want to delete this car?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DataRow car = Tables.cars.database.Select($"id = {button.Tag}")[0];
                if (car != null)
                {
                    car.Delete();
                    Tables.cars.updateChanges();

                    DisplayCars();
                }
            }
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            DataRow car = Tables.cars.database.Select($"id = {button.Tag}")[0];

            Navigation.OpenPage(Navigation.GetTypeByName("UpdateCarWindow"), car);
            if (Warehouse != null)
            {
                Navigation.ReturnParam = Warehouse;
            }

            //UpdateCarWindow updateCarWindow = new UpdateCarWindow(car);
            //Navigation.content2.Navigate(updateCarWindow);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            //CreateCarWindow createCar = new CreateCarWindow();
            //Navigation.content2.Navigate(createCar);

            Navigation.SkipParam = true;
            Navigation.OpenPage(Navigation.GetTypeByName("CreateCarWindow"));
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

        private void CarsPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
        }
    }
}
