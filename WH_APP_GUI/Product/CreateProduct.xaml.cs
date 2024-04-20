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

namespace WH_APP_GUI.Product
{
    public partial class CreateProduct : Page
    {
        private static Type PreviousPageType;
        public CreateProduct(Page previousPage)
        {
            PreviousPageType = previousPage.GetType();

            InitializeComponent();

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

            Tables.products.database.Columns["width"].AllowDBNull = false;
            Tables.products.database.Columns["heigth"].AllowDBNull=false;
            Tables.products.database.Columns["length"].AllowDBNull = false;
            Tables.products.database.Columns["weight"].AllowDBNull = false;

            if (!Validation.ValidateTextbox(name, product) && !Validation.ValidateTextbox(buying_price, product) && !Validation.ValidateTextbox(selling_price, product) && !Validation.ValidateTextbox(width, product) && !Validation.ValidateTextbox(heigth, product) && !Validation.ValidateTextbox(length, product) && !Validation.ValidateTextbox(description, product))
            {
                product["name"] = name.Text;
                product["image"] = image.Tag != null ? image.Tag.ToString() : "DefaultProductImage.png";
                product["buying_price"] = buying_price.Text;
                product["selling_price"] = selling_price.Text;
                product["width"] = width.Text;
                product["heigth"] = heigth.Text;
                product["length"] = length.Text;
                product["description"] = description.Text;

                if (SQL.BoolQuery("SElECT in_use FROM feature WHERE name = 'Storage'"))
                {
                    if (!Validation.ValidateTextbox(weight, product))
                    {
                        product["weight"] = weight.Text;
                        product["volume"] = double.Parse(width.Text) * double.Parse(heigth.Text) * double.Parse(length.Text);
                    }
                }

                Tables.products.database.Rows.Add(product);
                Tables.products.updateChanges();
                Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has been created {product["name"]} product.");

                MessageBox.Show("Product created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                //this.Close();

                //ProductsPage productsPage = new ProductsPage();
                //Navigation.content2.Navigate(productsPage);

                Page previousPage = (Page)Activator.CreateInstance(PreviousPageType);
                Navigation.content2.Navigate(previousPage);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();

            //ProductsPage productsPage = new ProductsPage();
            //Navigation.content2.Navigate(productsPage);

            Page previousPage = (Page)Activator.CreateInstance(PreviousPageType);
            Navigation.content2.Navigate(previousPage);

        }
    }
}
