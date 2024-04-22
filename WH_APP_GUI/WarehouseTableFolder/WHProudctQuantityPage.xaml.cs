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

namespace WH_APP_GUI.WarehouseTableFolder
{
    /// <summary>
    /// Interaction logic for WHProudctQuantityPage.xaml
    /// </summary>
    public partial class WHProudctQuantityPage : Window
    {
        DataRow warehouseProduct;
        public WHProudctQuantityPage(DataRow WarehouseProduct)
        {
            InitializeComponent();
            this.warehouseProduct = WarehouseProduct;

            this.DataContext = warehouseProduct;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
