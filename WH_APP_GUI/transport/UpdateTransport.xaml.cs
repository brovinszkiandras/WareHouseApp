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
    /// <summary>
    /// Interaction logic for UpdateTransport.xaml
    /// </summary>
    public partial class UpdateTransport : Page
    {

        public DataRow transport;
        public UpdateTransport(DataRow Transport)
        {
            InitializeComponent();
            this.transport = Transport;

            this.DataContext = transport;


            foreach (DataRow row in Tables.employees.database.Rows)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = row["name"];
                item.Tag = row["id"];

                EmployeesCBX.Items.Add(item);

                if ((int)row["id"] == (int)transport["employee_id"])
                {

                    EmployeesCBX.SelectedItem = item;
                }
            }


            foreach (DataRow row in Tables.cars.database.Rows)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = row["type"] + " - " + row["plate_number"];
                item.Tag = row["id"];

                CarsCBX.Items.Add(item);

                if ((int)row["id"] == (int)transport["car_id"])
                {

                    CarsCBX.SelectedItem = item;
                }
            }

            if(Tables.features.isFeatureInUse("Dock") == true)
            {
                foreach (DataRow row in Tables.docks.database.Rows)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = row["name"];
                    item.Tag = row["id"];

                    DocksCBX.Items.Add(item);

                    if ((int)row["id"] == (int)transport["dock_id"])
                    {

                        DocksCBX.SelectedItem = item;
                    }
                }
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
            else
            {

            }
        }

        private void EmployeesCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem comboBoxItem = EmployeesCBX.SelectedItem as ComboBoxItem;

            transport["employee_id"] = comboBoxItem.Tag;
        }

        private void CarsCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem comboBoxItem = CarsCBX.SelectedItem as ComboBoxItem;

            Tables.transports.getCar(transport)["ready"] = true;

            // Tables.cars.database.Select(comboBoxItem.Tag.ToString())[0]["ready"] = false;
            transport["car_id"] = comboBoxItem.Tag;
        }

        private void DocksCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem comboBoxItem = DocksCBX.SelectedItem as ComboBoxItem;
            if (Tables.features.isFeatureInUse("Dock"))
            {

                Tables.transports.getDock(transport)["free"] = true;

                // Tables.docks.database.Select(comboBoxItem.Tag.ToString())[0]["free"] = false;
                transport["dock_id"] = comboBoxItem.Tag;
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {



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
    }
}
