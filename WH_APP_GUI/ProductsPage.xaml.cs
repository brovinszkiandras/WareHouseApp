using MahApps.Metro.IconPacks;
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


namespace WH_APP_GUI
{
    //TODO: meg kéne csinálni a kép kiválasztását úgy hogy rendesen egy fáljt lehessen kiválasztani


    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();

            DisplayAllProducts(ProductsDiaplayStackPanel);
        }
        public string ImageName = string.Empty;

        public void DisplayAllProducts(Panel panel)
        {
            panel.Children.Clear();
            ProductsDisplay.Visibility = Visibility.Visible;
            panel.Visibility = Visibility.Visible;

            for (int i = 0; i < Tables.products.database.Rows.Count; i++)
            {
                Grid grid = new Grid();

                Border borderInsideGrid = new Border();
                borderInsideGrid.BorderBrush = Brushes.Black;
                borderInsideGrid.BorderThickness = new Thickness(0, 0, 0, 2);
                grid.Children.Add(borderInsideGrid);

                StackPanel stackPanelInsideGrid = new StackPanel();
                stackPanelInsideGrid.Orientation = Orientation.Horizontal;
                grid.Children.Add(stackPanelInsideGrid);

                StackPanel firstStackPanel = new StackPanel();
                firstStackPanel.Orientation = Orientation.Vertical;
                stackPanelInsideGrid.Children.Add(firstStackPanel);

                Border borderInsideFirstStackPanel = new Border();
                borderInsideFirstStackPanel.BorderBrush = Brushes.Black;
                borderInsideFirstStackPanel.BorderThickness = new Thickness(1);
                firstStackPanel.Children.Add(borderInsideFirstStackPanel);

                string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
                if (Directory.Exists(targetDirectory))
                {
                    string imageFileName = Tables.products.database.Rows[i]["image"].ToString();
                    string imagePath = Path.Combine(targetDirectory, imageFileName);

                    if (File.Exists(imagePath))
                    {
                        Image image = new Image();
                        image.Width = 80;
                        image.Height = 80;

                        string fileName = Path.GetFileName(imagePath);
                        string targetFilePath = Path.Combine(targetDirectory, fileName);

                        BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));

                        image.Source = bitmap;

                        borderInsideFirstStackPanel.Child = image;
                    }
                    else
                    {
                        //work in progress
                    }
                }



                Label label = new Label();
                label.Content = Tables.products.database.Rows[i]["name"].ToString();
                label.Height = 20;
                label.FontSize = 8;
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.FontStyle = FontStyles.Italic;
                firstStackPanel.Children.Add(label);

                StackPanel secondStackPanel = new StackPanel();
                secondStackPanel.Width = 190;
                secondStackPanel.Orientation = Orientation.Vertical;
                stackPanelInsideGrid.Children.Add(secondStackPanel);

                secondStackPanel.Children.Add(new Label { Content = Tables.products.database.Rows[i]["name"].ToString() });
                secondStackPanel.Children.Add(new Label { Content = Tables.products.database.Rows[i]["buying_price"].ToString() });
                secondStackPanel.Children.Add(new Label { Content = Tables.products.database.Rows[i]["selling_price"].ToString() });
                secondStackPanel.Children.Add(new Label { Content = Tables.products.database.Rows[i]["description"].ToString() });

                StackPanel thirdStackPanel = new StackPanel();
                thirdStackPanel.Width = 190;
                thirdStackPanel.Orientation = Orientation.Vertical;
                stackPanelInsideGrid.Children.Add(thirdStackPanel);

                thirdStackPanel.Children.Add(new Label { Content = Tables.products.database.Rows[i]["width"].ToString() });
                thirdStackPanel.Children.Add(new Label { Content = Tables.products.database.Rows[i]["heigth"].ToString() });
                thirdStackPanel.Children.Add(new Label { Content = Tables.products.database.Rows[i]["length"].ToString() });
                if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Storage'"))
                {
                    thirdStackPanel.Children.Add(new Label { Content = Tables.products.database.Rows[i]["weight"].ToString() });
                    thirdStackPanel.Children.Add(new Label { Content = Tables.products.database.Rows[i]["volume"].ToString() });
                }

                StackPanel fourthStackPanel = new StackPanel();
                fourthStackPanel.Orientation = Orientation.Vertical;
                fourthStackPanel.Width = 120;
                stackPanelInsideGrid.Children.Add(fourthStackPanel);

                fourthStackPanel.Children.Add(new Button { Content = "Delete" });
                fourthStackPanel.Children.Add(new Button { Content = "Edit" });

                panel.Children.Add(grid);
            }
        }

        private void AddProducts_Click(object sender, RoutedEventArgs e)
        {
            ProductsDisplay.Visibility = Visibility.Collapsed;

            if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Storage'"))
            {
                WeightLBL.Visibility = Visibility.Visible;
                Weight.Visibility = Visibility.Visible;
            }
            AddProductsDisplay.Visibility = Visibility.Visible;
        }

        private void AllProducts_Click(object sender, RoutedEventArgs e)
        {
            CancelM();
            DisplayAllProducts(ProductsDiaplayStackPanel);
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            //HomePage homePage = new HomePage();
            //this.Hide();
            //homePage.Show();
        }
        private void CancelM()
        {
            AddProductsDisplay.Visibility = Visibility.Collapsed;
            DisplayAllProducts(ProductsDiaplayStackPanel);
            ProductName.Text = string.Empty;
            BuyingPrice.Text = string.Empty;
            SellingPrice.Text = string.Empty;
            ImageName = string.Empty;
            Description.Text = string.Empty;
            Width.Text = string.Empty;
            Heigth.Text = string.Empty;
            Length.Text = string.Empty;
            Weight.Text = string.Empty;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CancelM();
        }

        private void CreateProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Storage'"))
                {
                    if (ProductName.Text != string.Empty && BuyingPrice.Text != string.Empty && SellingPrice.Text != string.Empty && ImageName != string.Empty && Description.Text != string.Empty && Width.Text != string.Empty && Heigth.Text != string.Empty && Length.Text != string.Empty && Weight.Text != string.Empty)
                    {
                        double volume = double.Parse(Width.Text) * double.Parse(Heigth.Text) * double.Parse(Length.Text);
                        SQL.SqlCommand($"INSERT INTO `{Tables.products.actual_name}`(`name`, `buying_price`, `selling_price`, `width`, `heigth`, `length`, `description`, `image`, `weight`, `volume`) VALUES ('{ProductName.Text}', {BuyingPrice.Text}, {SellingPrice.Text}, {Width.Text}, {Heigth.Text}, {Length.Text}, '{Description.Text}', '{ImageName}', {Weight.Text}, {volume});");
                        Tables.products.Refresh();
                        MessageBox.Show("Product creation was successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        CancelM();
                    }
                    else
                    {
                        MessageBox.Show("Missing input filed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    if (ProductName.Text != string.Empty && BuyingPrice.Text != string.Empty && SellingPrice.Text != string.Empty && ImageName != string.Empty && Description.Text != string.Empty && Width.Text != string.Empty && Heigth.Text != string.Empty && Length.Text != string.Empty)
                    {
                        SQL.SqlCommand($"INSERT INTO `{Tables.products.actual_name}`(`name`, `price`, `width`, `heigth`, `length`, `description`, `image`) VALUES ('{ProductName.Text}', {BuyingPrice.Text}, {SellingPrice.Text}, {Width.Text}, {Heigth.Text}, {Length.Text}, '{Description.Text}', '{ImageName}');");
                        Tables.products.Refresh();
                        MessageBox.Show("Product creation was successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        CancelM();
                    }
                    else
                    {
                        MessageBox.Show("Missing input filed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
                throw;
            }
        }

        private void Image_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choos Image";
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
                    DisplayAddImage.Source = bitmap;

                    ImageName = fileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during the Image browsing: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }
    }
}
