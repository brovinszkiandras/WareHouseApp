using Microsoft.Win32;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

namespace WH_APP_GUI.Product
{
    public partial class CreateProduct : Page
    {
        public CreateProduct()
        {
            InitializeComponent();

            name.ValueDataType = typeof(string);
            buying_price.ValueDataType = typeof(double);
            selling_price.ValueDataType = typeof(double);

            if (Tables.features.isFeatureInUse("Storage") == true)
            {
                width.ValueDataType = typeof(double);
                heigth.ValueDataType = typeof(double);
                length.ValueDataType = typeof(double);
                weight.ValueDataType = typeof(double);
            }
            else
            {
                width.Visibility = Visibility.Collapsed;
                heigth.Visibility = Visibility.Collapsed;
                length.Visibility = Visibility.Collapsed;
                weight.Visibility = Visibility.Collapsed;

                widthLabel.Visibility = Visibility.Collapsed;
                heigthLabel.Visibility = Visibility.Collapsed;
                lengthLabel.Visibility = Visibility.Collapsed;
                WeightLBL.Visibility = Visibility.Collapsed;
            }
            IniPicture();
        }
        private void IniPicture()
        {
            string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
            if (Directory.Exists(targetDirectory))
            {
                string imageFileName = "DefaultProductImage.png";
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
            DataRow product = Tables.products.database.NewRow();

            if (!Validation.ValidateTextbox(name, product) && !Validation.ValidateTextbox(buying_price, product) && !Validation.ValidateTextbox(selling_price, product))
            {
                product["name"] = name.Text;
                product["image"] = image.Tag != null ? image.Tag.ToString() : "DefaultProductImage.png";
                product["buying_price"] = double.Parse(buying_price.Text);
                product["selling_price"] = double.Parse(selling_price.Text);
                product["description"] = description.Text;
               
                product["description"] = description.Text.ToString();

                if (SQL.BoolQuery("SElECT in_use FROM feature WHERE name = 'Storage'"))
                {
                    if (!Validation.ValidateTextbox(weight, product) && !Validation.ValidateTextbox(width, product) && !Validation.ValidateTextbox(heigth, product) && !Validation.ValidateTextbox(length, product))
                    {
                        product["weight"] = double.Parse(weight.Text);
                        product["volume"] = double.Parse(width.Text) * double.Parse(heigth.Text) * double.Parse(length.Text);
                        product["width"] = double.Parse(width.Text);
                        product["heigth"] = double.Parse(heigth.Text);
                        product["length"] = double.Parse(length.Text);
                    }
                }

                Tables.products.database.Rows.Add(product);
                Tables.products.updateChanges();
                Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has been created {product["name"]} product.");
                MessageBox.Show("Product created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                Navigation.OpenPage(Navigation.PreviousPage.GetType());
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.PreviousPage.GetType());
        }
        private void CreateProductsPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
        }
    }
}
