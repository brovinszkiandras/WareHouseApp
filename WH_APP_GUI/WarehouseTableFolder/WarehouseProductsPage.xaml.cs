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
using WH_APP_GUI.carsFolder;

namespace WH_APP_GUI.WarehouseTableFolder
{
    /// <summary>
    /// Interaction logic for WarehouseProductsPage.xaml
    /// </summary>
    public partial class WarehouseProductsPage : Page
    {
        public void Displayproducts()
        {
            productGrid.Children.Clear();
            int lastRow = 0;

            foreach (DataRow product in User.WarehouseTable().database.Rows)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                productGrid.RowDefinitions.Add(rowDefinition);


                TextBlock name = new TextBlock();
                name.Text = User.WarehouseTable().getProduct(product)["name"].ToString();
                name.FontSize = 15;
                name.TextWrapping = TextWrapping.Wrap;
                name.Foreground = Brushes.White;
                name.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(name, lastRow);
                Grid.SetColumn(name, 0);

                productGrid.Children.Add(name);

                TextBlock qty = new TextBlock();
                qty.Text = product["qty"].ToString();
                qty.FontSize = 15;
                qty.Foreground = Brushes.White;
                qty.TextWrapping = TextWrapping.Wrap;
                qty.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(qty, lastRow);
                Grid.SetColumn(qty, 1);

                productGrid.Children.Add(qty);

                TextBlock shelf = new TextBlock();
                shelf.Text = User.WarehouseTable().getShelf(product)["name"].ToString();
                shelf.FontSize = 15;
                shelf.Foreground = Brushes.White;
                shelf.TextWrapping = TextWrapping.Wrap;
                shelf.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(shelf, lastRow);
                Grid.SetColumn(shelf, 2);

                productGrid.Children.Add(shelf);

                TextBlock width = new TextBlock();

                width.Text = product["width"].ToString();
                width.FontSize = 15;
                width.Foreground = Brushes.White;
                width.TextWrapping = TextWrapping.Wrap;
                width.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(width, lastRow);
                Grid.SetColumn(width, 3);

                productGrid.Children.Add(width);


                TextBlock length = new TextBlock();
                length.Text = product["length"].ToString();
                length.FontSize = 15;
                length.Foreground = Brushes.White;
                length.TextWrapping = TextWrapping.Wrap;
                length.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(length, lastRow);
                Grid.SetColumn(length, 4);

                productGrid.Children.Add(length);

                TextBlock height = new TextBlock();
                height.Text = product["height"].ToString();
                height.FontSize = 15;
                height.Foreground = Brushes.White;
                height.TextWrapping = TextWrapping.Wrap;
                height.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(height, lastRow);
                Grid.SetColumn(height, 5);

                productGrid.Children.Add(height);

                TextBlock on_shelf_level = new TextBlock();
                on_shelf_level.Text = product["on_shelf_level"].ToString();
                on_shelf_level.FontSize = 15;
                on_shelf_level.Foreground = Brushes.White;
                on_shelf_level.TextWrapping = TextWrapping.Wrap;
                on_shelf_level.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(on_shelf_level, lastRow);
                Grid.SetColumn(on_shelf_level, 6);

                productGrid.Children.Add(on_shelf_level);

                CheckBox is_in_box = new CheckBox();
                is_in_box.IsChecked = (bool)product["is_in_box"];
                is_in_box.HorizontalAlignment = HorizontalAlignment.Center;
                is_in_box.IsEnabled = false;
                Grid.SetRow(is_in_box, lastRow);
                Grid.SetColumn(is_in_box, 7);

                productGrid.Children.Add(is_in_box);

                Button inspect = new Button();
                inspect.Content = "Inspect";
                inspect.FontSize = 15;
                inspect.Foreground = Brushes.White;
                inspect.Background = Brushes.Green;
                inspect.Tag = product["id"];
                //inspect.Click += Inspect_Click;
                Grid.SetRow(inspect, lastRow);
                Grid.SetColumn(inspect, 8);

                productGrid.Children.Add(inspect);


                if (User.DoesHavePermission("Modify Warehouse") || User.DoesHavePermission("Modify all Warehouse"))
                {
                    Button edit = new Button();
                    edit.Content = "Edit";
                    edit.FontSize = 15;
                    edit.Foreground = Brushes.White;
                    edit.Background = Brushes.Green;
                    //edit.Click += Edit_Click;
                    edit.Tag = product["id"];

                    Grid.SetRow(edit, lastRow);
                    Grid.SetColumn(edit, 9);

                    productGrid.Children.Add(edit);

                    Button delete = new Button();
                    delete.Content = "Delete";
                    delete.FontSize = 15;
                    delete.Foreground = Brushes.White;
                    delete.Background = Brushes.Green;
                    delete.Tag = product["id"];
                    //delete.Click += Delete_Click;
                    Grid.SetRow(delete, lastRow);
                    Grid.SetColumn(delete, 10);

                    productGrid.Children.Add(delete);
                }
                else
                {
                    //Create.Visibility = Visibility.Collapsed;
                }

                lastRow++;
            }
        }


        public WarehouseProductsPage()
        {
            InitializeComponent();

            Displayproducts();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Do you want to delete this car?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DataRow product = User.WarehouseTable().database.Select($"id = {button.Tag}")[0];
                if (product != null)
                {

                    product.Delete();
                    User.WarehouseTable().updateChanges();



                    Displayproducts();
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
            CreateWHProductPage page = new CreateWHProductPage();
            Navigation.content2.Navigate(page);
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
