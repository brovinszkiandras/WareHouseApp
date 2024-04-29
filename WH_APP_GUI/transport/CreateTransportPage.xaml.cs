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
using Xceed.Wpf.Toolkit;

namespace WH_APP_GUI.transport
{
    public partial class CreateTransportPage : Page
    {
        DataRow transport = Tables.transports.database.NewRow();
        public CreateTransportPage()
        {
            InitializeComponent();
            Ini_warehouse_id();

            DocksCBX.IsEnabled = false;
            CarsCBX.IsEnabled = false;
            EmployeesCBX.IsEnabled = false;

            transport["status"] = "Docking";
            transport["start_date"] = SQL.convertDateToCorrectFormat(DateTime.Now);

            this.DataContext = transport;
           
            if (CarsCBX.Items.Count == 0)
            {
                CarsCBX.IsEnabled = false;
            }

            if (DocksCBX.Items.Count == 0)
            {
                DocksCBX.IsEnabled = false;
            }
        }

        private DataRow Warehouse = null;
        public CreateTransportPage(DataRow WarehouseFromPage)
        {
            InitializeComponent();
            Ini_warehouse_id();

            DocksCBX.IsEnabled = false;
            CarsCBX.IsEnabled = false;
            EmployeesCBX.IsEnabled = false;

            transport["status"] = "Docking";
            transport["start_date"] = SQL.convertDateToCorrectFormat(DateTime.Now);

            this.DataContext = transport;
            Warehouse = WarehouseFromPage;

            if (CarsCBX.Items.Count == 0)
            {
                CarsCBX.IsEnabled = false;
            }

            if (DocksCBX.Items.Count == 0)
            {
                DocksCBX.IsEnabled = false;
            }
        }

        private Dictionary<string, DataRow> warehouse_id_Dictionary = new Dictionary<string, DataRow>();
        private void Ini_warehouse_id()
        {
            WarehouseCBX.Items.Clear();
            warehouse_id_Dictionary.Clear();

            if (Warehouse != null)
            {
                WarehouseCBX.Visibility = Visibility.Collapsed;
                WarehouseCBX.Items.Add(Warehouse);
                warehouse_id_Dictionary.Add(Warehouse["name"].ToString(), Warehouse);
            }
            else
            {
                WarehouseCBX.Visibility = Visibility.Visible;
                foreach (DataRow warehouse in Tables.warehouses.database.Rows)
                {
                    WarehouseCBX.Items.Add(warehouse["name"].ToString());
                    warehouse_id_Dictionary.Add(warehouse["name"].ToString(), warehouse);
                }
            }
        }

        private void maskedTextbox_LostFocus(object sender, RoutedEventArgs e)
        {

            MaskedTextBox maskedTextBox = e.Source as MaskedTextBox;

            if (maskedTextBox.IsMaskCompleted == false)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"{maskedTextBox.Name} must be in a YYYY-MM-DD HH-mm-ss format and cant be empty");

                maskedTextBox.Text = transport[maskedTextBox.Name].ToString();
            }
            else if (maskedTextBox.HasParsingError == true)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"{maskedTextBox.Name} must be a valid {maskedTextBox.ValueDataType.Name}");

                maskedTextBox.Text = transport[maskedTextBox.Name].ToString();
            }
        }

        private void EmployeesCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeesCBX.SelectedItem != null)
            {
                ComboBoxItem comboBoxItem = EmployeesCBX.SelectedItem as ComboBoxItem;

                transport["employee_id"] = comboBoxItem.Tag;
            }
        }

        private void CarsCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CarsCBX.SelectedItem != null)
            {
                ComboBoxItem comboBoxItem = CarsCBX.SelectedItem as ComboBoxItem;

                if (transport["car_id"] != DBNull.Value)
                {
                    Tables.transports.getCar(transport)["ready"] = true;
                }

                Tables.cars.database.Select($"id = {comboBoxItem.Tag}")[0]["ready"] = false;
                transport["car_id"] = comboBoxItem.Tag;
            }
        }

        private void DocksCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DocksCBX.SelectedItem != null)
            {
                ComboBoxItem comboBoxItem = DocksCBX.SelectedItem as ComboBoxItem;
                if (Tables.features.isFeatureInUse("Dock"))
                {
                    if (transport["dock_id"] != DBNull.Value)
                    {
                        Tables.transports.getDock(transport)["free"] = true;
                    }

                    Tables.docks.database.Select($"id = {comboBoxItem.Tag}")[0]["free"] = false;
                    transport["dock_id"] = comboBoxItem.Tag;
                }
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesCBX.SelectedIndex < 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"An employee mus be selected");

            }
            else if (CarsCBX.SelectedIndex < 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"A car mus be selected");

            }
            else if (DocksCBX.SelectedIndex < 0 && Tables.features.isFeatureInUse("Dock") == true)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"A dock mus be selected");
            }
            else
            {
                transport["start_date"] = SQL.convertDateToCorrectFormat((DateTime)transport["start_date"]);
                if (transport["end_date"] != DBNull.Value)
                {
                    transport["end_date"] = SQL.convertDateToCorrectFormat((DateTime)transport["end_date"]);
                }
                transport["warehouse_id"] = warehouse_id_Dictionary[WarehouseCBX.SelectedItem.ToString()]["id"];

                Tables.transports.database.Rows.Add(transport);
                Tables.transports.updateChanges();
                Xceed.Wpf.Toolkit.MessageBox.Show($"You have succesfully created a new transport");
                TransportsPage transports = new TransportsPage();

                Navigation.content2.Navigate( transports );
            }
        }

        private void start_date_InputValidationError(object sender, Xceed.Wpf.Toolkit.Core.Input.InputValidationErrorEventArgs e)
        {
            Xceed.Wpf.Toolkit.MessageBox.Show($"You can only input a date");
        }

        private void WarehouseCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WarehouseCBX.SelectedIndex != -1)
            {
                CarsCBX.IsEnabled = true;
                DocksCBX.IsEnabled = true;
                EmployeesCBX.IsEnabled = true;

                CarsCBX.Items.Clear();
                DocksCBX.Items.Clear();
                EmployeesCBX.Items.Clear();

                CarsCBX.SelectedIndex = -1;
                DocksCBX.SelectedIndex = -1;
                EmployeesCBX.SelectedIndex = -1;

                foreach (DataRow row in Tables.warehouses.getEmployees(warehouse_id_Dictionary[WarehouseCBX.SelectedItem.ToString()]))
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = row["name"];
                    item.Tag = row["id"];

                    EmployeesCBX.Items.Add(item);
                }

                foreach (DataRow row in Tables.warehouses.getCars(warehouse_id_Dictionary[WarehouseCBX.SelectedItem.ToString()]))
                {
                    if ((bool)row["ready"] != false)
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = row["type"] + " - " + row["plate_number"];
                        item.Tag = row["id"];

                        CarsCBX.Items.Add(item);
                    }
                }

                if (Tables.features.isFeatureInUse("Dock") == true)
                {
                    foreach (DataRow row in Tables.warehouses.getDocks(warehouse_id_Dictionary[WarehouseCBX.SelectedItem.ToString()]))
                    {
                        if ((bool)row["free"] != false)
                        {
                            ComboBoxItem item = new ComboBoxItem();
                            item.Content = row["name"];
                            item.Tag = row["id"];
                            Xceed.Wpf.Toolkit.MessageBox.Show(item.Tag.ToString());
                            DocksCBX.Items.Add(item);
                        }
                    }
                }
            }
        }
    }
}
