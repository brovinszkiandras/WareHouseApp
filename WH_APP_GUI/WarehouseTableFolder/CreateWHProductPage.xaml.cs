﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
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
using Xceed.Wpf.Toolkit.Primitives;

namespace WH_APP_GUI.warehouseTableFolder
{
    public partial class CreateWHProductPage : Page
    {
        private DataRow warehouseProduct;

        private void CheckifProductsFitInbox()
        {

            double boxvolume = (double)warehouseProduct["width"]
           * (double)warehouseProduct["height"]
           * (double)warehouseProduct["length"];

            double productsFullVolume = (double)warehouseTable.getProduct(warehouseProduct)["volume"]
                * (int)warehouseProduct["qty"];

            if (boxvolume < productsFullVolume)
            {
                MessageBoxResult result = MessageBox.Show("The prouducts dont fit inside the box\n" +
                    $"Full volume of products: {productsFullVolume} cm3\n" +
                    $"Volume of the box: {boxvolume} cm3\n" +
                    $"Are you sure you want to proceed?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    updateDatabase();
                }
            }
            else
            {
                updateDatabase();
            }
        }

        private void updateDatabase()
        {
            warehouseTable.database.Rows.Add(warehouseProduct);
            warehouseTable.updateChanges();
            MessageBox.Show($"A new product has been added to the warehouse!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            if (Tables.features.isFeatureInUse("Revenue"))
            {
                DataRow Warehouse = Tables.warehouses.database.Select($"name = '{warehouseTable.database.TableName}'")[0];
                if (Warehouse != null)
                {
                    double buyingPrice = 0;
                    double sellingPrice = 0;

                    if (warehouseTable.getProduct(warehouseProduct)["buying_price"] != DBNull.Value && warehouseProduct["qty"] != DBNull.Value)
                    {
                        buyingPrice = Convert.ToDouble(warehouseTable.getProduct(warehouseProduct)["buying_price"]) * Convert.ToDouble(warehouseProduct["qty"]);
                    }

                    if (warehouseTable.getProduct(warehouseProduct)["selling_price"] != DBNull.Value && warehouseProduct["qty"] != DBNull.Value)
                    {
                        sellingPrice = Convert.ToDouble(warehouseTable.getProduct(warehouseProduct)["selling_price"]) * Convert.ToDouble(warehouseProduct["qty"]);
                    }

                    double warehouseTotalValue = Warehouse["total_value"] != DBNull.Value ? (double)Warehouse["total_value"] : 0;
                    double warehouseTotalSpending = Warehouse["total_spending"] != DBNull.Value ? (double)Warehouse["total_spending"] : 0;

                    Warehouse["total_value"] = warehouseTotalValue + sellingPrice;
                    Warehouse["total_spending"] = warehouseTotalSpending + buyingPrice;

                    Tables.warehouses.updateChanges();
                    Controller.AddToRevnue_A_Day_Expenditure(Warehouse, buyingPrice);
                }
            }
            
            WarehouseProductsPage page = new WarehouseProductsPage(warehouseTable);
            Navigation.content2.Navigate(page);
        }
        private warehouse warehouseTable;
        public CreateWHProductPage(warehouse WarehouseTable)
        {
            InitializeComponent();

            this.warehouseTable = WarehouseTable;

            warehouseProduct = warehouseTable.database.NewRow();
            warehouseProduct["is_in_box"] = true;

            this.DataContext = warehouseProduct;

            product_id.ItemsSource = Tables.products.database.DefaultView;

            List<DataRow> shelfs = new List<DataRow>();

            DataRow warehosue = Tables.warehouses.database.Select($"name = {warehouseTable.database.TableName}")[0];

            foreach (DataRow sector in Tables.warehouses.getSectors(warehosue))
            {
                foreach (DataRow shelf in Tables.sector.getShelfs(sector))
                {
                    shelfs.Add(shelf);
                }
            }

            shelf_id.ItemsSource = shelfs;

            if (Tables.features.isFeatureInUse("Storage") == false)
            {
                width.Visibility = Visibility.Collapsed;
                height.Visibility = Visibility.Collapsed;
                length.Visibility = Visibility.Collapsed;

                widhtLabel.Visibility = Visibility.Collapsed;
                heightLabel.Visibility = Visibility.Collapsed;
                lengthLabel.Visibility = Visibility.Collapsed;
            }
            else
            {

                Binding widthBinding = new Binding("[width]");
                widthBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                width.SetBinding(TextBox.TextProperty, widthBinding);
                Binding lengthBinding = new Binding("[length]");
                lengthBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                length.SetBinding(TextBox.TextProperty, lengthBinding);
                Binding heightBinding = new Binding("[height]");
                heightBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                height.SetBinding(TextBox.TextProperty, heightBinding);
            }

            qty.ValueDataType = typeof(string);
            width.ValueDataType = typeof(double);
            height.ValueDataType = typeof(double);
            length.ValueDataType = typeof(double);
        }

        private void shelf_id_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (shelf_id.SelectedIndex > -1)
            {
                on_shelf_level.IsEnabled = true;
                DataRow shelf = shelf_id.SelectedItem as DataRow;

                for (int i = 0; i < (int)shelf["number_of_levels"]; i++)
                {
                    on_shelf_level.Items.Add(i + 1);
                }
                warehouseProduct["shelf_id"] = shelf["id"];
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded == true)
            {
                if (Tables.features.isFeatureInUse("Storage") == true)
                {
                    widhtLabel.Content = "Box width (cm)";
                    heightLabel.Content = "Box height (cm)";
                    lengthLabel.Content = "Box length (cm)";

                    width.IsEnabled = true;
                    height.IsEnabled = true;
                    length.IsEnabled = true;
                    if (product_id.SelectedIndex > -1)
                    {
                        DataRowView selectedItem = product_id.SelectedItem as DataRowView;
                        DataRow product = selectedItem.Row;

                        width.Clear();
                        height.Clear();
                        length.Clear();
                    }
                }
            }
        }

        private void is_in_a_box_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded == true)
            {
                if (Tables.features.isFeatureInUse("Storage") == true)
                {
                    widhtLabel.Content = "Product's width";
                    heightLabel.Content = "Product's height";
                    lengthLabel.Content = "Product's length";

                    width.IsEnabled = false;
                    height.IsEnabled = false;
                    length.IsEnabled = false;

                    if (product_id.SelectedIndex > -1)
                    {
                        DataRowView selectedItem = product_id.SelectedItem as DataRowView;
                        DataRow product = selectedItem.Row;


                        width.Text = product["width"].ToString();
                        height.Text = product["heigth"].ToString();
                        length.Text = product["length"].ToString();
                    }
                }
            }
        }

        private void product_id_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (product_id.SelectedIndex > -1)
            {
                DataRowView selectedItem = product_id.SelectedItem as DataRowView;
                DataRow product = selectedItem.Row;
                if (is_in_a_box.IsChecked == false && Tables.features.isFeatureInUse("Storage") == true)
                {
                    width.Text = product["width"].ToString();
                    height.Text = product["heigth"].ToString();
                    length.Text = product["length"].ToString();
                }

                warehouseProduct["product_id"] = product["id"];
            }
        }

        private void on_shelf_level_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedItem = (int)on_shelf_level.SelectedItem;

            warehouseProduct["on_shelf_level"] = selectedItem;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            bool thereIsAnError = false;

            foreach (var gridElement in productGrid.Children)
            {
                if (gridElement.GetType() == typeof(StackPanel))
                {
                    StackPanel stackPanel = (StackPanel)gridElement;
                    foreach (var element in stackPanel.Children)
                    {
                        if (element.GetType() == typeof(ValueRangeTextBox))
                        {
                            ValueRangeTextBox VTextBox = (ValueRangeTextBox)element;

                            thereIsAnError = Validation.ValidateTextbox(VTextBox, warehouseProduct);
                        }
                        else if (element.GetType() == typeof(ComboBox))
                        {
                            ComboBox comboBox = (ComboBox)element;

                            thereIsAnError = Validation.ValidateCombobox(comboBox, warehouseProduct);
                        }
                    }
                }
                if (thereIsAnError == true)
                {
                    break;
                }
            }

            if (thereIsAnError == false)
            {
                if (Tables.features.isFeatureInUse("Storage") && (bool)warehouseProduct["is_in_box"] == true)
                {
                    CheckifProductsFitInbox();
                }
                else
                {
                    updateDatabase();
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Navigation.PreviousPage != null)
            {
                Navigation.OpenPage(Navigation.PreviousPage.GetType());
            }
        }
    }
}