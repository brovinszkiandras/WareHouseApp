using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

namespace WH_APP_GUI.WarehouseTableFolder
{
    /// <summary>
    /// Interaction logic for CreateWHProductPage.xaml
    /// </summary>
    public partial class CreateWHProductPage : Page
    {
        DataRow warehouseProduct;
        public CreateWHProductPage()
        {
            InitializeComponent();

            warehouseProduct = User.WarehouseTable().database.NewRow();

            this.DataContext = warehouseProduct;

            product_id.ItemsSource = Tables.products.database.DefaultView;

            List<DataRow> shelfs = new List<DataRow>();
            foreach (DataRow sector in Tables.warehouses.getSectors(User.Warehouse()))
            {
               foreach (DataRow shelf in Tables.sector.getShelfs(sector)) 
                {
                  shelfs.Add(shelf);
                }
            }

            shelf_id.ItemsSource = shelfs;
        }

        private void shelf_id_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(shelf_id.SelectedIndex > -1)
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
            if(this.IsLoaded == true)
            {
                widhtLabel.Content = "Box width";
                heightLabel.Content = "Box height";
                lengthLabel.Content = "Box length";

                if (product_id.SelectedIndex > -1)
                {
                    DataRowView selectedItem = product_id.SelectedItem as DataRowView;
                    DataRow product = selectedItem.Row;

                    width.IsEnabled = true;
                    height.IsEnabled = true;
                    length.IsEnabled = true;


                    width.Clear();
                    height.Clear();
                    length.Clear();

                }
            }
           
        }

        private void is_in_a_box_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded == true)
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

        private void product_id_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (product_id.SelectedIndex > -1)
            {
                DataRowView selectedItem = product_id.SelectedItem as DataRowView;
                DataRow product = selectedItem.Row;
                if (is_in_a_box.IsChecked == false) {
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
                        else if(element.GetType() == typeof(ComboBox))
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
                User.WarehouseTable().database.Rows.Add(warehouseProduct);
                User.WarehouseTable().updateChanges();
                Xceed.Wpf.Toolkit.MessageBox.Show($"A new product has been added to the warehouse");
               

                WarehouseProductsPage page = new WarehouseProductsPage();
                Navigation.content2.Navigate( page );
            }
        }
    }
}
