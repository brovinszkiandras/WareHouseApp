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

            if (!User.DoesHavePermission("Handle Products"))
            {
                Create.Visibility = Visibility.Collapsed;
            }

            Displayproducts();
        }
        public void Displayproducts()
        {
            addStorageFeautoreElementsToDisplay();

            productGrid.Children.Clear();
            int lastRow = 0;

            foreach (DataRow product in warehouseTable.database.Rows)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                productGrid.RowDefinitions.Add(rowDefinition);

                TextBlock name = new TextBlock();
                name.Text = warehouseTable.getProduct(product)["name"].ToString();
                name.Style = (Style)this.Resources["textblockstyle"];
                name.TextWrapping = TextWrapping.Wrap;
                name.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(name, lastRow);
                Grid.SetColumn(name, 0);

                productGrid.Children.Add(name);

                TextBlock qty = new TextBlock();
                qty.Text = product["qty"].ToString();
                qty.Style = (Style)this.Resources["textblockstyle"];
                qty.TextWrapping = TextWrapping.Wrap;
                qty.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(qty, lastRow);
                Grid.SetColumn(qty, 1);

                productGrid.Children.Add(qty);

                TextBlock shelf = new TextBlock();
                if (product["shelf_id"] != DBNull.Value)
                {
                    shelf.Text = warehouseTable.getShelf(product)["name"].ToString();
                }
                else
                {
                    shelf.Text = "";
                }
                shelf.Style = (Style)this.Resources["textblockstyle"];
                shelf.TextWrapping = TextWrapping.Wrap;
                shelf.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(shelf, lastRow);
                Grid.SetColumn(shelf, 2);

                productGrid.Children.Add(shelf);

                TextBlock width = new TextBlock();

                #region StorageElements
                if (Tables.features.isFeatureInUse("Storage"))
                {
                    width.Text = product["width"].ToString();
                    width.Style = (Style)this.Resources["textblockstyle"];
                    width.TextWrapping = TextWrapping.Wrap;
                    width.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(width, lastRow);
                    Grid.SetColumn(width, 5);

                    productGrid.Children.Add(width);


                    TextBlock length = new TextBlock();
                    length.Text = product["length"].ToString();
                    length.Style = (Style)this.Resources["textblockstyle"];
                    length.TextWrapping = TextWrapping.Wrap;
                    length.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(length, lastRow);
                    Grid.SetColumn(length, 6);

                    productGrid.Children.Add(length);

                    TextBlock height = new TextBlock();
                    height.Text = product["height"].ToString();
                    height.Style = (Style)this.Resources["textblockstyle"];
                    height.TextWrapping = TextWrapping.Wrap;
                    height.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(height, lastRow);
                    Grid.SetColumn(height, 7);

                    productGrid.Children.Add(height);
                }
                #endregion

                TextBlock on_shelf_level = new TextBlock();
                if (product["on_shelf_level"] != DBNull.Value)
                {
                    on_shelf_level.Text = product["on_shelf_level"].ToString();
                }
                else
                {
                    on_shelf_level.Text = "";
                }
                on_shelf_level.Style = (Style)this.Resources["textblockstyle"];
                on_shelf_level.TextWrapping = TextWrapping.Wrap;
                on_shelf_level.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(on_shelf_level, lastRow);
                Grid.SetColumn(on_shelf_level, 3);

                productGrid.Children.Add(on_shelf_level);

                CheckBox is_in_box = new CheckBox();
                is_in_box.IsChecked = (bool)product["is_in_box"];
                is_in_box.HorizontalAlignment = HorizontalAlignment.Center;
                is_in_box.IsEnabled = false;
                Grid.SetRow(is_in_box, lastRow);
                Grid.SetColumn(is_in_box, 4);

                productGrid.Children.Add(is_in_box);

                if (User.DoesHavePermission("Handle Products"))
                {
                    Button add = new Button();
                    add.Content = "Quantity";
                    add.Style = (Style)this.Resources["GoldenButtonStyle"];
                    add.Tag = product["id"];
                    add.Click += Add_Click;
                    Grid.SetRow(add, lastRow);
                    Grid.SetColumn(add, productGrid.ColumnDefinitions.Count - 3);

                    productGrid.Children.Add(add);

                    Button edit = new Button();
                    edit.Content = "Edit";
                    edit.Style = (Style)this.Resources["GoldenButtonStyle"];
                    edit.Click += Edit_Click;
                    edit.Tag = product["id"];

                    Grid.SetRow(edit, lastRow);
                    Grid.SetColumn(edit, productGrid.ColumnDefinitions.Count - 2);

                    productGrid.Children.Add(edit);

                    Button delete = new Button();
                    delete.Content = "Delete";
                    delete.Style = (Style)this.Resources["GoldenButtonStyle"];
                    delete.Tag = product["id"];
                    delete.Click += Delete_Click;
                    Grid.SetRow(delete, lastRow);
                    Grid.SetColumn(delete, productGrid.ColumnDefinitions.Count - 1);

                    productGrid.Children.Add(delete);
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
            widthLabel.Style = (Style)this.Resources["labelstyle"];
            widthLabel.HorizontalAlignment = HorizontalAlignment.Center;
            widthLabel.VerticalAlignment = VerticalAlignment.Center;
            widthLabel.Content = "Width";
            Grid.SetColumn(widthLabel, 5);
            labelsGrid.Children.Add(widthLabel);

            Label heightLabel = new Label();
            heightLabel.Style = (Style)this.Resources["labelstyle"];
            heightLabel.HorizontalAlignment = HorizontalAlignment.Center;
            heightLabel.VerticalAlignment = VerticalAlignment.Center;
            heightLabel.Content = "Height";
            Grid.SetColumn(heightLabel, 6);
            labelsGrid.Children.Add(heightLabel);

            Label lengthLabel = new Label();
            lengthLabel.Style = (Style)this.Resources["labelstyle"];
            lengthLabel.HorizontalAlignment = HorizontalAlignment.Center;
            lengthLabel.VerticalAlignment = VerticalAlignment.Center;
            lengthLabel.Content = "Length";
            Grid.SetColumn(lengthLabel, 7);
            labelsGrid.Children.Add(lengthLabel);
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            MessageBoxResult result = MessageBox.Show("Do you want to delete this car?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
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