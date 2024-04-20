using Microsoft.Win32;
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
using Xceed.Wpf.Toolkit.Primitives;
using MySqlConnector;

namespace WH_APP_GUI.Product
{
    public partial class EditProductPage : Window
    {
        public DataRow product;
        public EditProductPage(DataRow product)
        {
            InitializeComponent();

            MessageBox.Show(product["id"].ToString());
            

            this.product = product;

            this.DataContext = product;


            foreach (DataColumn column in Tables.products.database.Columns)
            {
                Type columnType = column.DataType.GetType();
                if (!Equals(product[column, DataRowVersion.Original], product[column, DataRowVersion.Current]))
                {
                    // If any column's original value is different from the current value, return false
                    MessageBox.Show("Not equal: " + column.ColumnName);
                }
            }

            //string message = "";
            //foreach (DataColumn column in Tables.products.database.Columns)
            //{
            //    message += product[column.ColumnName, DataRowVersion.Original] + "\n";
            //}
            //MessageBox.Show(message);

            //string message2 = "";
            //foreach (DataColumn column in Tables.products.database.Columns)
            //{
            //    message2 += product[column.ColumnName, DataRowVersion.Current] + "\n";
            //}
            //MessageBox.Show(message2);

           


            //string message4 = "";
            //foreach (DataColumn column in Tables.products.database.Columns)
            //{
            //    if (product[column.ColumnName, DataRowVersion.Default] != null)
            //    {
            //        message4 += product[column.ColumnName, DataRowVersion.Default] + "\n";
            //    }
            //}
            //MessageBox.Show(message4);

            

            name.ValueDataType = typeof(string);
            buying_price.ValueDataType = typeof(double);
            selling_price.ValueDataType = typeof(double);
            width.ValueDataType = typeof(double);
            heigth.ValueDataType = typeof(double);
            length.ValueDataType = typeof(double);
            description.ValueDataType = typeof(string);

           

            if (SQL.BoolQuery("SElECT in_use FROM feature WHERE name = 'Storage'"))
            {
                weight.Visibility = Visibility.Visible;
                weight.ValueDataType = typeof(double);
               
            }
            else
            {
                weight.Visibility = Visibility.Collapsed;
            }

            string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
            if (Directory.Exists(targetDirectory))
            {
                string imageFileName = product["image"].ToString();
                string imagePath = Path.Combine(targetDirectory, imageFileName);
                if (File.Exists(imagePath))
                {
                    string fileName = Path.GetFileName(imagePath);
                    string targetFilePath = Path.Combine(targetDirectory, fileName);

                    BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));
                    ImageBrush brush = new ImageBrush(bitmap);

                    image.Background = brush;
                }
            }
        }

        private void image_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose Image";
            openFileDialog.Filter = "Image|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All File|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string selectedFilePath = openFileDialog.FileName;
                    string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");

                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    string fileName = Path.GetFileName(selectedFilePath);
                    string targetFilePath = Path.Combine(targetDirectory, fileName);

                    File.Copy(selectedFilePath, targetFilePath, true);

                    BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));

                    image.Tag = fileName;
                    image.Background = new ImageBrush(bitmap);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during the Image browsing: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            bool hasError = false;
            foreach (object element in DatasgGRID.Children)
            {
                if (element.GetType() == typeof(ValueRangeTextBox))
                {
                    ValueRangeTextBox valueRangeTextBox = (ValueRangeTextBox)element;
                    hasError = Validation.ValidateTextbox(valueRangeTextBox, product);
                }

                if (hasError == true)
                {
                    break;
                }
            }
            if (hasError == false)
            {

               








                Tables.products.updateChanges();

                Xceed.Wpf.Toolkit.MessageBox.Show($"You have succesfully created a new sector");

               this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CalculateVolume(string widthStr, string heightStr, string lengthStr)
        {
            if (double.TryParse(widthStr, out double width) && double.TryParse(heightStr, out double height) && double.TryParse(lengthStr, out double length))
            {
                volume.Text = (width * height * length).ToString();
            }
        }



        private void width_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateVolume(width.Text, heigth.Text, length.Text);
        }

        private void heigth_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateVolume(width.Text, heigth.Text, length.Text);
        }

        private void length_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateVolume(width.Text, heigth.Text, length.Text);
        }
    }
}
