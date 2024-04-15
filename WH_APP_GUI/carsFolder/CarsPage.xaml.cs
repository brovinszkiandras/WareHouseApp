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
using System.Windows.Shapes;
using WH_APP_GUI;
using WH_APP_GUI.carsFolder;
using WH_APP_GUI.transport;

namespace WH_APP_GUI.carsFolder
{
    /// <summary>
    /// Interaction logic for CarsPage.xaml
    /// </summary>
    public partial class CarsPage : Page
    {
        public void DisplayCars()
        {
            carsGrid.Children.Clear();
            
            int lastRow = 0;
            foreach (DataRow car in Tables.cars.database.Rows)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                carsGrid.RowDefinitions.Add(rowDefinition);

                TextBlock plate_number = new TextBlock();
                plate_number.Text = car["plate_number"].ToString();
                plate_number.FontSize = 15;
                plate_number.TextWrapping = TextWrapping.Wrap;
                plate_number.Foreground = Brushes.White;
                plate_number.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(plate_number, lastRow);
                Grid.SetColumn(plate_number, 0);

                TextBlock type = new TextBlock();
                type.Text = car["type"].ToString();
                type.FontSize = 15;
                type.TextWrapping = TextWrapping.Wrap;
                type.Foreground = Brushes.White;
                type.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(type, lastRow);
                Grid.SetColumn(type, 1);

                CheckBox ready = new CheckBox();
                ready.HorizontalContentAlignment = HorizontalAlignment.Center;
                ready.VerticalContentAlignment = VerticalAlignment.Center;
                ready.HorizontalAlignment = HorizontalAlignment.Center;
                ready.Foreground = Brushes.White;
                ready.IsEnabled = false;
                ready.IsChecked = (bool)car["ready"];
                Grid.SetRow(ready, lastRow);
                Grid.SetColumn(ready, 2);

                carsGrid.Children.Add(plate_number);
                carsGrid.Children.Add(type);
                carsGrid.Children.Add(ready);


                Button inspect = new Button();
                inspect.Content = "Details";
                inspect.FontSize = 15;
                inspect.Foreground = Brushes.White;
                inspect.Background = Brushes.Green;
                inspect.Tag = car["id"];
                inspect.Click += Details_Click;
                Grid.SetRow(inspect, lastRow);
                Grid.SetColumn(inspect, 3);

                carsGrid.Children.Add(inspect);

                Button edit = new Button();
                edit.Content = "Edit";
                edit.FontSize = 15;
                edit.Foreground = Brushes.White;
                edit.Background = Brushes.Green;
                edit.Click += Edit_Click;
                edit.Tag = car["id"];

                Grid.SetRow(edit, lastRow);
                Grid.SetColumn(edit, 4);

                carsGrid.Children.Add(edit);

                Button delete = new Button();
                delete.Content = "Delete";
                delete.FontSize = 15;
                delete.Foreground = Brushes.White;
                delete.Background = Brushes.Green;
                delete.Tag = car["id"];
                delete.Click += Delete_Click;
                Grid.SetRow(delete, lastRow);
                Grid.SetColumn(delete, 5);

               carsGrid.Children.Add(delete);

                lastRow++;
            }
        }
        public CarsPage()
        {
            InitializeComponent();

            DisplayCars();
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
            UpdateCarWindow updateCarWindow = new UpdateCarWindow(car);

            Navigation.content2.Navigate(updateCarWindow);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
           CreateCarWindow createCar = new CreateCarWindow();
            Navigation.content2.Navigate(createCar);
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            DataRow car = Tables.cars.database.Select($"id = {button.Tag}")[0];
            InspectCarWindow inspectCarWindow = new InspectCarWindow(car);
            inspectCarWindow.ShowDialog();
        }
    }
}
