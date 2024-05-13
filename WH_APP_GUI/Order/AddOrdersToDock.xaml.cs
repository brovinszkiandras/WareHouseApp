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

namespace WH_APP_GUI.Order
{
    public partial class AddOrdersToDock : Window
    {
        DataRow[] orders = null;
        public AddOrdersToDock(DataRow[] orders)
        {
            InitializeComponent();
            this.orders = orders;
            Ini_Docks();
        }

        private Dictionary<string, DataRow> DocksDataRow = new Dictionary<string, DataRow>();
        private void Ini_Docks()
        {
            if (User.currentUser.Table.TableName == "employees")
            {
                DataRow warehouse = Tables.employees.getWarehouse(User.currentUser);
                foreach (DataRow dock in Tables.warehouses.getDocks(warehouse))
                {
                    if (!DocksDataRow.ContainsKey(dock["name"].ToString()))
                    {
                        Docks.Items.Add(dock["name"].ToString());
                        DocksDataRow.Add(dock["name"].ToString(), dock);
                    }
                }
            }
            else
            {
                foreach (DataRow dock in Tables.warehouses.database.Rows)
                {
                    if (!DocksDataRow.ContainsKey(dock["name"].ToString()))
                    {
                        Docks.Items.Add(dock["name"].ToString());
                        DocksDataRow.Add(dock["name"].ToString(), dock);
                    }
                }
            }
            
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (Docks.SelectedIndex != -1)
            {
                foreach (DataRow order in orders)
                {
                    order["transport_id"] = DocksDataRow[Docks.SelectedItem.ToString()]["id"];
                    Tables.orders.updateChanges();
                }
                MessageBox.Show("Orders has been added to the transport!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }
    }
}
