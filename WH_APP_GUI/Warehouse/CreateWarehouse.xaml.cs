using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using System.Windows.Navigation;

namespace WH_APP_GUI.Warehouse
{
    public partial class CreateWarehouse : Page
    {
        private static Type PreviousPageType;
        public CreateWarehouse(Page previousPage)
        {
            PreviousPageType = previousPage.GetType();
            InitializeComponent();

            name.ValueDataType = typeof(string);
            length.ValueDataType = typeof(double);
            width.ValueDataType = typeof(double);
            height.ValueDataType = typeof(double);

            IniCities();
            CityLBL.Visibility = Visibility.Visible;
            city_id.Visibility = Visibility.Visible;


            string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
            if (Directory.Exists(targetDirectory))
            {
                string imageFileName = "WarehouseDefaultPicture.png";
                string imagePath = Path.Combine(targetDirectory, imageFileName);
                if (File.Exists(imagePath))
                {
                    string fileName = Path.GetFileName(imagePath);
                    string targetFilePath = Path.Combine(targetDirectory, fileName);

                    BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));
                    WarehouseImage.Source = bitmap;
                }
            }
        }

        private Dictionary<string, DataRow> Cities_Dictionary = new Dictionary<string, DataRow>();
        private void IniCities()
        {
            Cities_Dictionary.Clear();
            city_id.Items.Clear();
            foreach (DataRow warehouse in Tables.cities.database.Rows)
            {
                Cities_Dictionary.Add(warehouse["city_name"].ToString(), warehouse);
                city_id.Items.Add(warehouse["city_name"].ToString());
            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            DataRow warehouse = Tables.warehouses.database.NewRow();

            if (Validation.ValidateSQLNaming(name.Text, "Warehouse name") == true && !Validation.ValidateTextbox(name, warehouse) && !Validation.ValidateTextbox(length, warehouse) && !Validation.ValidateTextbox(width, warehouse) && !Validation.ValidateTextbox(height, warehouse))
            {
                warehouse["name"] = name.Text;
                warehouse["length"] = length.Text;
                warehouse["width"] = width.Text;
                warehouse["height"] = height.Text;
                warehouse["city_id"] = Cities_Dictionary[city_id.SelectedItem.ToString()]["id"];

                Tables.warehouses.database.Rows.Add(warehouse);
                Tables.warehouses.updateChanges();
                Controller.CreateWarehouse(warehouse["name"].ToString());

                Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has been created {warehouse["name"]} warehouse.");

                MessageBox.Show("Warehouse created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                GoBack();
            }
        }
        private void GoBack()
        {
            if (PreviousPageType != null)
            {
                Page previousPage = (Page)Activator.CreateInstance(PreviousPageType);
                Navigation.content2.Navigate(previousPage);
            }
            else
            {
                WarehousesPage warehousesPage = new WarehousesPage();
                Navigation.content2.Navigate(warehousesPage);
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }
    }
}
