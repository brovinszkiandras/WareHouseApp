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
using WH_APP_GUI.Warehouse;

namespace WH_APP_GUI.warehouseTableFolder
{
    public partial class WarehouseProductsPage : Page
    {
        warehouse warehouseTable;
        public WarehouseProductsPage(warehouse WarehouseTable)
        {
            InitializeComponent();

            this.warehouseTable = WarehouseTable;

            Displayproducts();
        }
        public void Displayproducts()
        {
            if (Tables.features.isFeatureInUse("Storage") == true)
            {
                MessageBox.Show("Lefutott");
                addStorageFeautoreElementsToDisplay();
            }

            productGrid.Children.Clear();
            int lastRow = 0;


            foreach (DataRow product in warehouseTable.database.Rows)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                productGrid.RowDefinitions.Add(rowDefinition);


                TextBlock name = new TextBlock();
                name.Text = warehouseTable.getProduct(product)["name"].ToString();
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

                CheckBox is_in_box = new CheckBox();
                is_in_box.IsChecked = (bool)product["is_in_box"];
                is_in_box.HorizontalAlignment = HorizontalAlignment.Center;
                is_in_box.IsEnabled = false;
                Grid.SetRow(is_in_box, lastRow);
                Grid.SetColumn(is_in_box, 2);

                productGrid.Children.Add(qty);
                if (product["shelf_id"] != DBNull.Value)
                {
                    #region shelf
                    TextBlock shelf = new TextBlock();
                    shelf.Text = warehouseTable.getShelf(product)["name"].ToString();
                    shelf.FontSize = 15;
                    shelf.Foreground = Brushes.White;
                    shelf.TextWrapping = TextWrapping.Wrap;
                    shelf.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(shelf, lastRow);
                    Grid.SetColumn(shelf, 3);

                    productGrid.Children.Add(shelf);
                    #endregion


                    #region shelf_level
                    TextBlock on_shelf_level = new TextBlock();
                    on_shelf_level.Text = product["on_shelf_level"].ToString();
                    on_shelf_level.FontSize = 15;
                    on_shelf_level.Foreground = Brushes.White;
                    on_shelf_level.TextWrapping = TextWrapping.Wrap;
                    on_shelf_level.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(on_shelf_level, lastRow);
                    Grid.SetColumn(on_shelf_level, 4);

                    productGrid.Children.Add(on_shelf_level);

                    #region StorageElements
                    if (Tables.features.isFeatureInUse("Storage"))
                    {
                        TextBlock width = new TextBlock();
                        width.Text = product["width"].ToString();
                        width.FontSize = 15;
                        width.Foreground = Brushes.White;
                        width.TextWrapping = TextWrapping.Wrap;
                        width.HorizontalAlignment = HorizontalAlignment.Center;
                        Grid.SetRow(width, lastRow);
                        Grid.SetColumn(width, 5);

                        productGrid.Children.Add(width);


                        TextBlock length = new TextBlock();
                        length.Text = product["length"].ToString();
                        length.FontSize = 15;
                        length.Foreground = Brushes.White;
                        length.TextWrapping = TextWrapping.Wrap;
                        length.HorizontalAlignment = HorizontalAlignment.Center;
                        Grid.SetRow(length, lastRow);
                        Grid.SetColumn(length, 6);

                        productGrid.Children.Add(length);

                        TextBlock height = new TextBlock();
                        height.Text = product["height"].ToString();
                        height.FontSize = 15;
                        height.Foreground = Brushes.White;
                        height.TextWrapping = TextWrapping.Wrap;
                        height.HorizontalAlignment = HorizontalAlignment.Center;
                        Grid.SetRow(height, lastRow);
                        Grid.SetColumn(height, 7);

                        productGrid.Children.Add(height);
                    }
                    #endregion

                    #endregion
                }
                else
                {
                    MessageBox.Show("alma");
                    Label notPlacedLabel = new Label();
                    notPlacedLabel.Content = "Not placed yet";
                    notPlacedLabel.FontSize = 20;
                    notPlacedLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    notPlacedLabel.Foreground = Brushes.White;
                    Grid.SetRow(notPlacedLabel, lastRow);
                    Grid.SetColumn(notPlacedLabel, 3);
                    if (Tables.features.isFeatureInUse("Storage") == true)
                    {
                        Grid.SetColumnSpan(notPlacedLabel, 5);
                    }
                    else
                    {
                        Grid.SetColumnSpan(notPlacedLabel, 2);
                    }
                    productGrid.Children.Add(notPlacedLabel);
                }


               

                productGrid.Children.Add(is_in_box);

                Button add = new Button();
                add.Content = "Quantity";
                add.FontSize = 15;
                add.Foreground = Brushes.White;
                add.Background = Brushes.Green;
                add.Tag = product["id"];
                add.Click += Add_Click;
                Grid.SetRow(add, lastRow);
                Grid.SetColumn(add, productGrid.ColumnDefinitions.Count - 3);

                productGrid.Children.Add(add);


                if (User.DoesHavePermission("Modify Warehouse") || User.DoesHavePermission("Modify all Warehouse"))
                {
                    Button edit = new Button();
                    edit.Content = "Edit";
                    edit.FontSize = 15;
                    edit.Foreground = Brushes.White;
                    edit.Background = Brushes.Green;
                    edit.Click += Edit_Click;
                    edit.Tag = product["id"];

                    Grid.SetRow(edit, lastRow);
                    Grid.SetColumn(edit, productGrid.ColumnDefinitions.Count - 2);

                    productGrid.Children.Add(edit);

                    Button delete = new Button();
                    delete.Content = "Delete";
                    delete.FontSize = 15;
                    delete.Foreground = Brushes.White;
                    delete.Background = Brushes.Green;
                    delete.Tag = product["id"];
                    //delete.Click += Delete_Click;
                    Grid.SetRow(delete, lastRow);
                    Grid.SetColumn(delete, productGrid.ColumnDefinitions.Count - 1);

                    productGrid.Children.Add(delete);
                }
                else
                {
                    //Create.Visibility = Visibility.Collapsed;
                }

                lastRow++;
            }
        }

        public void addStorageFeautoreElementsToDisplay()
        {
            for (int i = 1; i <= 3; i++)
            {
                ColumnDefinition labelColumn = new ColumnDefinition();
                labelColumn.Width = new GridLength(1, GridUnitType.Star);
                labelsGrid.ColumnDefinitions.Add(labelColumn);

                ColumnDefinition valueColumn = new ColumnDefinition();
                valueColumn.Width = new GridLength(1, GridUnitType.Star);
                productGrid.ColumnDefinitions.Add(valueColumn);
            }

            Label widthLabel = new Label();
            widthLabel.FontSize = 15;
            widthLabel.Foreground = Brushes.White;
            widthLabel.HorizontalAlignment = HorizontalAlignment.Center;
            widthLabel.VerticalAlignment = VerticalAlignment.Center;
            widthLabel.Content = "Width";
            Grid.SetColumn(widthLabel, 5);
            labelsGrid.Children.Add(widthLabel);

            Label heightLabel = new Label();
            heightLabel.FontSize = 15;
            heightLabel.Foreground = Brushes.White;
            heightLabel.HorizontalAlignment = HorizontalAlignment.Center;
            heightLabel.VerticalAlignment = VerticalAlignment.Center;
            heightLabel.Content = "Height";
            Grid.SetColumn(heightLabel, 6);
            labelsGrid.Children.Add(heightLabel);

            Label lengthLabel = new Label();
            lengthLabel.FontSize = 15;
            lengthLabel.Foreground = Brushes.White;
            lengthLabel.HorizontalAlignment = HorizontalAlignment.Center;
            lengthLabel.VerticalAlignment = VerticalAlignment.Center;
            lengthLabel.Content = "Length";
            Grid.SetColumn(lengthLabel, 7);
            labelsGrid.Children.Add(lengthLabel);
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Do you want to delete this car?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DataRow product = warehouseTable.database.Select($"id = {button.Tag}")[0];
                if (product != null)
                {
                    product.Delete();
                    warehouseTable.updateChanges();
                    Displayproducts();
                }
            }
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            DataRow product = warehouseTable.database.Select($"id = {button.Tag}")[0];
            EditWHProductPage page = new EditWHProductPage(product);

            Navigation.content2.Navigate(page);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            //CreateWHProductPage page = new CreateWHProductPage(warehouseTable);
            //Navigation.content2.Navigate(page);

            Navigation.OpenPage(Navigation.GetTypeByName("CreateWHProductPage"), warehouseTable);
            Navigation.ReturnParam = warehouseTable;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            DataRow product = warehouseTable.database.Select($"id = {button.Tag}")[0];
            WHProudctQuantityPage page = new WHProudctQuantityPage(product);

            page.ShowDialog();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Navigation.PreviousPage != null)
            {
                if (warehouseTable.database.TableName != null)
                {
                    DataRow warehouse = Tables.warehouses.database.Select($"name = '{warehouseTable.database.TableName}'")[0];

                    InspectWarehouse inspectWarehouse = new InspectWarehouse(warehouse);
                    Navigation.PreviousPage = inspectWarehouse;

                    Navigation.OpenPage(Navigation.PreviousPage.GetType(), warehouse);
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