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
    public partial class EditProductPage : Page
    {
        public DataRow product;
        public EditProductPage(DataRow product)
        {
            InitializeComponent();
            
            this.product = product;
            this.DataContext = product;

            foreach (DataColumn column in Tables.products.database.Columns)
            {
                Type columnType = column.DataType.GetType();
                //if (!Equals(product[column, DataRowVersion.Original], product[column, DataRowVersion.Current]))
                //{
                //    MessageBox.Show("Not equal: " + column.ColumnName);
                //}
            }

            name.ValueDataType = typeof(string);
            buying_price.ValueDataType = typeof(double);
            selling_price.ValueDataType = typeof(double);
            width.ValueDataType = typeof(double);
            heigth.ValueDataType = typeof(double);
            length.ValueDataType = typeof(double);
            description.ValueDataType = typeof(string);

            if (Tables.features.isFeatureInUse("Storage"))
            {
                width.Text = product["width"].ToString();
                heigth.Text = product["heigth"].ToString();
                length.Text = product["length"].ToString();
                weight.Text = product["weight"].ToString();
                volume.Text = product["volume"].ToString();

                WidthLBL.Visibility = Visibility.Visible;
                HeigthLBL.Visibility = Visibility.Visible;
                LengthLBL.Visibility = Visibility.Visible;
                WeightLBL.Visibility = Visibility.Visible;
                VolumeLBL.Visibility = Visibility.Visible;
            }
            else
            {
                WidthLBL.Visibility = Visibility.Collapsed;
                HeigthLBL.Visibility = Visibility.Collapsed;
                LengthLBL.Visibility = Visibility.Collapsed;
                WeightLBL.Visibility = Visibility.Collapsed;
                VolumeLBL.Visibility = Visibility.Collapsed;

                width.Visibility = Visibility.Collapsed;
                heigth.Visibility = Visibility.Collapsed;
                length.Visibility = Visibility.Collapsed;
                weight.Visibility = Visibility.Collapsed;
                volume.Visibility = Visibility.Collapsed;
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
                    product["image"] = fileName;
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

                Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has been modified {product["name"]} product.");
                MessageBox.Show($"You have succesfully updated the product", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                Navigation.OpenPage(Navigation.PreviousPage.GetType());
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.PreviousPage.GetType());
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
        private void EditProductsPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
        }
    }
}
