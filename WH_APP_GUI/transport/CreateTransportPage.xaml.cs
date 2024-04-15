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
    /// Interaction logic for CreateTransportPage.xaml
    /// </summary>
    public partial class CreateTransportPage : Page
    {
        DataRow transport = Tables.transports.database.NewRow();
        public CreateTransportPage()
        {
            InitializeComponent();
            transport["start_date"] = DateTime.Now;
           // transport["is_transported"] = false;



            this.DataContext = transport;

            foreach (DataRow row in Tables.employees.database.Rows)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = row["name"];
                item.Tag = row["id"];

                EmployeesCBX.Items.Add(item);


            }


            foreach (DataRow row in Tables.cars.database.Rows)
            {
                if ((bool)row["ready"] != false)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = row["type"] + " - " + row["plate_number"];
                    item.Tag = row["id"];

                    CarsCBX.Items.Add(item);
                }



            }

            foreach (DataRow row in Tables.docks.database.Rows)
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
            if (CarsCBX.Items.Count == 0)
            {

                CarsCBX.IsEnabled = false;
            }

            if (DocksCBX.Items.Count == 0)
            {

                DocksCBX.IsEnabled = false;
            }


            //start_date.NullValue = DateTime.Now;

            //start_date.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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

            if (transport["car_id"] != DBNull.Value)
            {
                Tables.transports.getCar(transport)["ready"] = true;
            }

            Tables.cars.database.Select($"id = {comboBoxItem.Tag}")[0]["ready"] = false;
            transport["car_id"] = comboBoxItem.Tag;
        }

        private void DocksCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem comboBoxItem = DocksCBX.SelectedItem as ComboBoxItem;

            if (transport["dock_id"] != DBNull.Value)
            {
                Tables.transports.getDock(transport)["free"] = true;
            }
           
            Tables.docks.database.Select($"id = {comboBoxItem.Tag}")[0]["free"] = false;
            transport["dock_id"] = comboBoxItem.Tag;
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
            else if (DocksCBX.SelectedIndex < 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"A dock mus be selected");

            }
            else
            {
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
    }
}
