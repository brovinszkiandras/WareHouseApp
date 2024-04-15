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

namespace WH_APP_GUI
{
    /// <summary>
    /// Interaction logic for OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            InitializeComponent();
            FillAndClearWarehousesComboBox(OrdersInWarehouse);
            DisplayAllOrders(OrdersDisplayStackPanel);
        }
        //TODO: Order auto assign to wh
        private static void FillAndClearWarehousesComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            for (int i = 0; i < Tables.warehouses.database.Rows.Count; i++)
            {
                comboBox.Items.Add(Tables.warehouses.database.Rows[i]["name"].ToString());
            }
        }
        private void DisplayOrdersInWarehouse(Panel panel, DataRow warehouse)
        {
            Label employeelabel = new Label();
            employeelabel.Content = "Orders in {TODO}:";
            employeelabel.BorderBrush = Brushes.Black;
            employeelabel.BorderThickness = new Thickness(0, 0, 0, 1);
            panel.Children.Add(employeelabel);

            //for (int i = 0; i < Tables.warehouses.getOrders(warehouse).Count(); i++)
            for (int i = 0; i < 5; i++)
            {
                Grid grid = new Grid();
                grid.Height = 100;

                Border borderInsideGrid = new Border();
                borderInsideGrid.BorderBrush = Brushes.Black;
                borderInsideGrid.BorderThickness = new Thickness(0, 0, 0, 2);
                grid.Children.Add(borderInsideGrid);

                StackPanel stackPanelInsideGrid = new StackPanel();
                stackPanelInsideGrid.Orientation = Orientation.Horizontal;
                grid.Children.Add(stackPanelInsideGrid);

                StackPanel firstStackPanel = new StackPanel();
                firstStackPanel.Orientation = Orientation.Vertical;
                stackPanelInsideGrid.Children.Add(firstStackPanel);

                Border borderInsideFirstStackPanel = new Border();
                borderInsideFirstStackPanel.BorderBrush = Brushes.Black;
                borderInsideFirstStackPanel.BorderThickness = new Thickness(1);
                firstStackPanel.Children.Add(borderInsideFirstStackPanel);

                Image image = new Image();
                image.Width = 80;
                image.Height = 80;
                borderInsideFirstStackPanel.Child = image;

                Label label = new Label();
                label.Content = "Prod name";
                label.Height = 20;
                label.FontSize = 8;
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.FontStyle = FontStyles.Italic;
                firstStackPanel.Children.Add(label);

                StackPanel secondStackPanel = new StackPanel();
                secondStackPanel.Width = 190;
                secondStackPanel.Orientation = Orientation.Vertical;
                stackPanelInsideGrid.Children.Add(secondStackPanel);

                secondStackPanel.Children.Add(new Label { Content = "Order Quantity: " });
                secondStackPanel.Children.Add(new Label { Content = "Order Date: " });
                secondStackPanel.Children.Add(new Label { Content = "Warehouse: " });

                StackPanel thirdStackPanel = new StackPanel();
                thirdStackPanel.Width = 190;
                thirdStackPanel.Orientation = Orientation.Vertical;
                stackPanelInsideGrid.Children.Add(thirdStackPanel);

                thirdStackPanel.Children.Add(new Label { Content = "Username: " });
                thirdStackPanel.Children.Add(new Label { Content = "Payment method: " });
                thirdStackPanel.Children.Add(new Label { Content = "Delivery Addres: " });

                StackPanel fourthStackPanel = new StackPanel();
                fourthStackPanel.Orientation = Orientation.Vertical;
                fourthStackPanel.Width = 120;
                stackPanelInsideGrid.Children.Add(fourthStackPanel);

                fourthStackPanel.Children.Add(new Button { Content = "Delete" });
                fourthStackPanel.Children.Add(new Button { Content = "Edit" });
                if (SQL.BoolQuery($"SElECT in_use FROM feature WHERE name = 'City'"))
                {
                    fourthStackPanel.Children.Add(new Button { Content = "View On Map" });
                }

                panel.Children.Add(grid);
            }
        }

        private void DisplayAllOrders(Panel panel)
        {
            for (int i = 0; i < Tables.warehouses.database.Rows.Count; i++)
            {
                DisplayOrdersInWarehouse(panel, Tables.warehouses.database.Rows[i]);
            }
        }
        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            AddOrderDisplay.Visibility = Visibility.Collapsed;
            DisplayAllOrders(OrdersDisplayStackPanel);
        }

        private void OrdersInWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CancelEvent();
            OrdersDisplay.Visibility = Visibility.Visible;
            //DisplayOrdersInWarehouse(OrdersDisplayStackPanel, Tables.warehouses.database.Select($"WHERE name = '{OrdersInWarehouse.SelectedItem}'")[0]);
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void WarehouseId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductsID.Items.Clear();
            //TODO: we need the warehouse class(no not the warehouses) and only after we can fill the ProductsID combobox
            //But it have to look like somehow like this:
            /*
            for (int i = 0; i < Tables.warehouses.database.Rows.Count; i++)
            {
                ProductsID.Items.Add(//Relation ide//);
            }
            */
        }
        private void CancelEvent()
        {
            AddOrderDisplay.Visibility = Visibility.Collapsed;
            ProductsInOrder.Items.Clear();
            UserName.Text = string.Empty;
            Address.Text = string.Empty;
            QuantityOfOrder.Text = string.Empty;
            ProductsID.SelectedIndex = -1;
            WarehouseId.SelectedIndex = -1;
            CityID.SelectedIndex = -1;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CancelEvent();
        }

        private void AllOrders_Click(object sender, RoutedEventArgs e)
        {
            CancelEvent();
            OrdersDisplay.Visibility = Visibility.Visible;
            DisplayAllOrders(OrdersDisplayStackPanel);
        }

        //TODO: kell vagy sem manuálisan order hozzá adás?
        private void AddProductToOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SQL.BoolQuery("SELECT in_use FROM feture WHERE name = 'City'"))
                {
                    if (UserName.Text != string.Empty && Address.Text != string.Empty && QuantityOfOrder.Text != string.Empty && PaymentMethod.Text != string.Empty && ProductsID.SelectedIndex != -1 && WarehouseId.SelectedIndex != -1 && CityID.SelectedIndex != -1)
                    {
                        int warehouseId = int.Parse(Tables.warehouses.database.Select($"WHERE name = '{WarehouseId.SelectedItem}'")[0]["id"].ToString());
                        int prodId = int.Parse(Tables.products.database.Select($"WHERE name = '{ProductsID.SelectedItem}")[0]["id"].ToString());
                        int cityId = int.Parse(Tables.cities.database.Select($"WHERE name = '{CityID.SelectedItem}")[0]["id"].ToString());
                        if (SQL.BoolQuery("SELECT in_use FROM feture WHERE name = 'Storage'"))
                        {
                            double Sum_Volume = double.Parse(Tables.products.database.Select($"WHERE id = '{prodId}'")[0]["volume"].ToString()) * int.Parse(QuantityOfOrder.Text);

                            MessageBox.Show($"INSERT INTO `orders`(`qty`, `order_date`, `address`, `warehouse_id`, `product_id`, `user_name`, `payment_method`, `sum_volume`, `city_id`) VALUES ({QuantityOfOrder.Text}, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', '{Address.Text}', '{warehouseId}', '{prodId}', '{UserName.Text}', '{PaymentMethod.Text}', {Sum_Volume}, {cityId})");
                        }
                        else
                        {
                            SQL.SqlCommand($"INSERT INTO `orders`(`qty`, `order_date`, `address`, `warehouse_id`, `product_id`, `user_name`, `payment_method`, `city_id`) VALUES ({QuantityOfOrder.Text}, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', '{Address.Text}', '{warehouseId}', '{prodId}', '{UserName.Text}', '{PaymentMethod.Text}', {cityId})");
                        }
                        ProductsInOrder.Items.Add($"{ProductsID.SelectedItem} x {QuantityOfOrder}");
                        MessageBox.Show("Order successfully added'", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Empty input field!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    if (UserName.Text != string.Empty && Address.Text != string.Empty && QuantityOfOrder.Text != string.Empty && PaymentMethod.Text != string.Empty && ProductsID.SelectedIndex != -1 && WarehouseId.SelectedIndex != -1)
                    {
                        int warehouseId = int.Parse(Tables.warehouses.database.Select($"WHERE name = '{WarehouseId.SelectedItem}'")[0]["id"].ToString());
                        int prodId = int.Parse(Tables.products.database.Select($"WHERE name = '{ProductsID.SelectedItem}")[0]["id"].ToString());
                        if (SQL.BoolQuery("SELECT in_use FROM feture WHERE name = 'Storage'"))
                        {
                            double Sum_Volume = double.Parse(Tables.products.database.Select($"WHERE id = '{prodId}'")[0]["volume"].ToString()) * int.Parse(QuantityOfOrder.Text);

                            MessageBox.Show($"INSERT INTO `orders`(`qty`, `order_date`, `address`, `warehouse_id`, `product_id`, `user_name`, `payment_method`, `sum_volume`) VALUES ({QuantityOfOrder.Text}, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', '{Address.Text}', '{warehouseId}', '{prodId}', '{UserName.Text}', '{PaymentMethod.Text}', {Sum_Volume})");
                        }
                        else
                        {
                            SQL.SqlCommand($"INSERT INTO `orders`(`qty`, `order_date`, `address`, `warehouse_id`, `product_id`, `user_name`, `payment_method`) VALUES ({QuantityOfOrder.Text}, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', '{Address.Text}', '{warehouseId}', '{prodId}', '{UserName.Text}', '{PaymentMethod.Text}')");
                        }
                        ProductsInOrder.Items.Add($"{ProductsID.SelectedItem} x {QuantityOfOrder}");
                        MessageBox.Show("Order successfully added'", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Empty input field!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
                throw;
            }
        }
    }
}
