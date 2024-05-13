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

namespace WH_APP_GUI.warehouseTableFolder
{
    public partial class WHProudctQuantityPage : Window
    {
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
            warehouseTable.updateChanges();
            MessageBox.Show($"The products quantity has changed", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            WarehouseProductsPage page = new WarehouseProductsPage(warehouseTable);
            Navigation.content2.Navigate(page);

            this.Close();
        }

        DataRow warehouseProduct;
        warehouse warehouseTable;
        public WHProudctQuantityPage(DataRow WarehouseProduct)
        {
            InitializeComponent();
            this.warehouseProduct = WarehouseProduct;

            warehouseTable = Tables.getWarehosue(warehouseProduct.Table.TableName);

            this.DataContext = warehouseProduct;

            Product_name.Text = warehouseTable.getProduct(warehouseProduct)["name"].ToString();

            qty.ValueDataType = typeof(int);
            qty.MinValue = 1;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.ValidateTextbox(qty, warehouseProduct) == false)
            {
                if (Tables.features.isFeatureInUse("Storage") == true && (bool)warehouseProduct["is_in_box"] == true)
                {
                    CheckifProductsFitInbox();
                }
                else
                {
                    updateDatabase();
                }
            }
        }
    }
}