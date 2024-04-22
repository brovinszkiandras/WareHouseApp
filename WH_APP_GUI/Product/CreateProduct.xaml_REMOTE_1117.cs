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
                product["buying_price"] = double.Parse(buying_price.Text);
                product["selling_price"] = double.Parse(selling_price.Text);
                product["width"] = double.Parse(width.Text);
                product["heigth"] = double.Parse(heigth.Text);
                product["length"] = double.Parse(length.Text);
                product["description"] = description.Text.ToString();
                product["created_at"] = DateTime.Now;
                product["updated_at"] = DateTime.Now;
                File.WriteAllText("datetimeValue", product["created_at"].ToString());
                string dateTimeString = product["created_at"].ToString();
                string updatedString = product["updated_at"].ToString();

                if (DateTime.TryParseExact(dateTimeString, "yyyy. MM. dd. H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeValue))
                {
                    string formattedDateTime = dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss");
                    product["created_at"] = formattedDateTime;
                }

                if (DateTime.TryParseExact(updatedString, "yyyy. MM. dd. H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeValue2))
                {
                    string formattedDateTime = dateTimeValue2.ToString("yyyy-MM-dd HH:mm:ss");
                    product["updated_at"] = formattedDateTime;
                }


                if (SQL.BoolQuery("SElECT in_use FROM feature WHERE name = 'Storage'"))
                {
                    if (!Validation.ValidateTextbox(weight, product))
                    {
                        product["weight"] = double.Parse(weight.Text);
                        product["volume"] = double.Parse(width.Text) * double.Parse(heigth.Text) * double.Parse(length.Text);
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
    }
}
