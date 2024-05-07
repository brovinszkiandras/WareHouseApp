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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WH_APP_GUI.Order
{
    public partial class AddOrderToTransport : Window
    {
        DataRow[] orders = null;
        public AddOrderToTransport(DataRow[] orders)
        {
            InitializeComponent();

            Ini_Transports();
            this.orders = orders;
        }

        private Dictionary<string, DataRow> TransportsDataRow = new Dictionary<string, DataRow>();
        private void Ini_Transports()
        {
            if (User.currentUser.Table.TableName == "employees")
            {
                DataRow warehouse = Tables.employees.getWarehouse(User.currentUser);
                foreach (DataRow transport in Tables.warehouses.getTransports(warehouse))
                {
                    if (int.Parse(transport["warehouse_id"].ToString()) == int.Parse(User.Warehouse()["id"].ToString()) && transport["status"].ToString() == "Docking")
                    {
                        string format = $"{Tables.transports.getEmployee(transport)["name"]} - {Tables.transports.getCar(transport)["type"]}\n" +
                            $"{transport["end_date"]}";
                        if (! TransportsDataRow.ContainsKey(format))
                        {
                            Transports.Items.Add(format);
                            TransportsDataRow.Add(format, transport);
                        }
                    }
                }
            }
            else
            {
                foreach (DataRow transport in Tables.transports.database.Rows)
                {

                    string format = $"{Tables.transports.getEmployee(transport)["name"]} - {Tables.transports.getCar(transport)["type"]}\n" +
                            $"{transport["end_date"]}";
                    if (!TransportsDataRow.ContainsKey(format))
                    {
                        Transports.Items.Add(format);
                        TransportsDataRow.Add(format, transport);
                    }
                }
            }
        }
        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (Transports.SelectedIndex != -1)
            {
                foreach (DataRow order in orders)
                {
                    order["transport_id"] = TransportsDataRow[Transports.SelectedItem.ToString()]["id"];
                    Tables.orders.updateChanges();
                }
                MessageBox.Show("Orders has been added to the transport!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }
    }
}
