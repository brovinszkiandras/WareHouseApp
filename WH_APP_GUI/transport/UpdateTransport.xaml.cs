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
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace WH_APP_GUI.transport
{
    public partial class UpdateTransport : Page
    {
        public DataRow transport;
        public UpdateTransport(DataRow Transport)
        {
            InitializeComponent();
            this.transport = Transport;
            this.DataContext = transport;

            Ini_warehouse_id();
            IniEmployees();
            IniCars();
            IniDocks();

            if (transport["employee_id"] != DBNull.Value)
            {
                EmployeesCBX.SelectedItem = Tables.employees.database.Select($"id = {transport["employee_id"]}")[0]["name"];
            }

            if (transport["car_id"] != DBNull.Value)
            {
                CarsCBX.SelectedItem = Tables.cars.database.Select($"id = {transport["car_id"]}")[0]["type"];
            }

            if (transport["dock_id"] != DBNull.Value)
            {
                DocksCBX.SelectedItem = Tables.docks.database.Select($"id = {transport["dock_id"]}")[0]["name"];
            }

            switch (transport["status"].ToString())
            {
                case "Docking":
                    StatusCBX.SelectedIndex = 0;
                    break;
                case "On route":
                    StatusCBX.SelectedIndex = 1;
                    break;
                case "Finished":
                    StatusCBX.SelectedIndex = 2;
                    break;
                default:
                    break;
            }

        }
        private Dictionary<string, DataRow> employees = new Dictionary<string, DataRow>(); 
        private void IniEmployees()
        {
            EmployeesCBX.Items.Clear();
            employees.Clear();
            if (transport["warehouse_id"] != DBNull.Value)
            {
                foreach (DataRow employee in Tables.warehouses.getEmployees(warehouse_id_Dictionary[WarehouseCBX.SelectedItem.ToString()]))
                {
                    EmployeesCBX.Items.Add(employee["name"].ToString());  
                    employees.Add(employee["name"].ToString(), employee);
                }
            }
            else
            {
                foreach (DataRow employee in Tables.employees.database.Rows)
                {
                    EmployeesCBX.Items.Add(employee["name"].ToString());
                    employees.Add(employee["name"].ToString(), employee);
                }
            }
        }

        private Dictionary<string, DataRow> cars = new Dictionary<string, DataRow>();
        private void IniCars()
        {
            cars.Clear();
            CarsCBX.Items.Clear();
            if (transport["warehouse_id"] != DBNull.Value)
            {
                foreach (DataRow car in Tables.warehouses.getCars(warehouse_id_Dictionary[WarehouseCBX.SelectedItem.ToString()]))
                {
                    CarsCBX.Items.Add(car["type"].ToString());
                    cars.Add(car["type"].ToString(), car);
                }
            }
            else
            {
                foreach (DataRow car in Tables.cars.database.Rows)
                {
                    CarsCBX.Items.Add(car["type"].ToString());
                    cars.Add(car["type"].ToString(), car);
                }
            }
        }

        private Dictionary<string, DataRow> docks = new Dictionary<string, DataRow>();
        private void IniDocks()
        {
            docks.Clear();
            DocksCBX.Items.Clear();
            if (transport["warehouse_id"] != DBNull.Value)
            {
                foreach (DataRow dock in Tables.warehouses.getDocks(warehouse_id_Dictionary[WarehouseCBX.SelectedItem.ToString()]))
                {
                    DocksCBX.Items.Add(dock["name"].ToString());
                    docks.Add(dock["name"].ToString(), dock);
                }
            }
            else
            {
                foreach (DataRow dock in Tables.docks.database.Rows)
                {
                    DocksCBX.Items.Add(dock["name"].ToString());
                    docks.Add(dock["name"].ToString(), dock);
                }
            }
        }

        private Dictionary<string, DataRow> warehouse_id_Dictionary = new Dictionary<string, DataRow>();
        private void Ini_warehouse_id()
        {
            WarehouseCBX.Visibility = Visibility.Visible;
            WarehouseCBX.Items.Clear();
            warehouse_id_Dictionary.Clear();

            foreach (DataRow warehouse in Tables.warehouses.database.Rows)
            {
                WarehouseCBX.Items.Add(warehouse["name"].ToString());
                warehouse_id_Dictionary.Add(warehouse["name"].ToString(), warehouse);
            }

            if (transport["warehouse_id"] != DBNull.Value)
            {
                WarehouseCBX.SelectedItem = Tables.warehouses.database.Select($"id = {transport["warehouse_id"]}")[0]["name"].ToString();
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
                transport["employee_id"] = employees[EmployeesCBX.SelectedItem.ToString()]["id"];
            }
        }

        private void CarsCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CarsCBX.SelectedItem != null)
            {
                transport["car_id"] = cars[CarsCBX.SelectedItem.ToString()]["id"];
                Tables.transports.getCar(transport)["ready"] = true;
            }         
        }

        private void DocksCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DocksCBX.SelectedItem != null)
            {
                if (Tables.features.isFeatureInUse("Dock"))
                {
                    transport["dock_id"] = docks[DocksCBX.SelectedItem.ToString()]["id"];
                    Tables.transports.getDock(transport)["free"] = true;
                }
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            transport["start_date"] = SQL.convertDateToCorrectFormat((DateTime)transport["start_date"]);
            if (transport["end_date"] != DBNull.Value)
            {
                transport["end_date"] = SQL.convertDateToCorrectFormat((DateTime)transport["end_date"]);
            }

            Tables.transports.updateChanges();
            Xceed.Wpf.Toolkit.MessageBox.Show($"You have succesfully updated transport number {transport["id"]}");
            TransportsPage transportsPage = new TransportsPage();
            Navigation.content2.Navigate( transportsPage );
        }


        private void start_date_InputValidationError(object sender, Xceed.Wpf.Toolkit.Core.Input.InputValidationErrorEventArgs e)
        {
            Xceed.Wpf.Toolkit.MessageBox.Show($"You can only input a date");
        }

        private void StatusCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem SelectedItem = StatusCBX.SelectedItem as ComboBoxItem;
            if(StatusCBX.SelectedIndex == 2)
            {
                if (transport["status"].ToString() != "Finished")
                {
                    end_date.Value = DateTime.Now;
                }
                
            }
           if(this.IsLoaded == true)
           {
                transport["status"] = SelectedItem.Content.ToString();
           }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (DataColumn column in Tables.transports.database.Columns)
            {
                if (transport[column, DataRowVersion.Original] != null)
                {
                    transport[column] = transport[column, DataRowVersion.Original];
                }
            }
        }

        private void WarehouseCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EmployeesCBX.SelectedIndex = -1;
            DocksCBX.SelectedIndex = -1;
            CarsCBX.SelectedIndex = -1;

            transport["warehouse_id"] = warehouse_id_Dictionary[WarehouseCBX.SelectedItem.ToString()]["id"];
            IniEmployees();
            IniCars();
            IniDocks();
        }

        private void UpdateTransportPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
        }
    }
}
