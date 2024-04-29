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
using WH_APP_GUI.Forklift;

namespace WH_APP_GUI.Product
{
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
            DisplayAllProducts(ProductsDiaplayStackPanel);

            if (!User.DoesHavePermission("Add Products"))
            {
                AddProducts.Visibility = Visibility.Collapsed;
            }
        }
        private DataRow WarehouseFromPage;
        public ProductsPage(DataRow warehouseFromPage)
        {
            InitializeComponent();
            DisplayAllProducts(ProductsDiaplayStackPanel);

            if (!User.DoesHavePermission("Add Products"))
            {
                AddProducts.Visibility = Visibility.Collapsed;
            }

            WarehouseFromPage = warehouseFromPage;
        }

        private void DisplayOneProduct(Panel panel, DataRow product)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.Background = new SolidColorBrush(Color.FromArgb(255, 0x39, 0x52, 0x50));
            border.CornerRadius = new CornerRadius(15);
            border.BorderThickness = new Thickness(2);
            border.Margin = new Thickness(5);

            StackPanel outerStack = new StackPanel();
            if (Tables.features.isFeatureInUse("Date Log"))
            {
                Label dateLog = new Label();
                dateLog.Content = $"Created at: {product["created_at"]} \tUpdated at: {product["updated_at"]}";
                dateLog.Style = (Style)this.Resources["labelstyle"];
                dateLog.HorizontalContentAlignment = HorizontalAlignment.Center;
                outerStack.Children.Add(dateLog);
            }

            Grid mainStackPanel = new Grid();
            mainStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            mainStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            mainStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

            StackPanel imageStackpanel = new StackPanel();

            Image productImage = new Image();
            productImage.Margin = new Thickness(5);
            productImage.Height = 100;
            productImage.Width = 100;
            imageStackpanel.Children.Add(productImage);

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
                    productImage.Source = bitmap;
                }
            }

            Label productName = new Label();
            productName.Content = $"{product["name"]}";
            productName.Style = (Style)this.Resources["labelstyle"];
            productName.HorizontalContentAlignment = HorizontalAlignment.Center;
            imageStackpanel.Children.Add(productName);

            Grid.SetColumn(imageStackpanel, 0);
            mainStackPanel.Children.Add(imageStackpanel);

            Grid grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            Label nameLabel = new Label();
            nameLabel.Content = $"Product Name: {product["name"]}";
            nameLabel.Style = (Style)this.Resources["labelstyle"];
            Grid.SetColumn(nameLabel, 0);
            Grid.SetRow(nameLabel, 0);

            Label buyingPriceLabel = new Label();
            buyingPriceLabel.Content = $"Buying Price: {product["buying_price"]}";
            buyingPriceLabel.Style = (Style)this.Resources["labelstyle"];
            Grid.SetColumn(buyingPriceLabel, 0);
            Grid.SetRow(buyingPriceLabel, 1);

            Label sellingPriceLabel = new Label();
            sellingPriceLabel.Content = $"Selling Price: {product["selling_price"]}";
            sellingPriceLabel.Style = (Style)this.Resources["labelstyle"];
            Grid.SetColumn(sellingPriceLabel, 0);
            Grid.SetRow(sellingPriceLabel, 2);

            TextBlock descriptionLabel = new TextBlock();
            descriptionLabel.TextWrapping = TextWrapping.Wrap;
            descriptionLabel.Style = (Style)this.Resources["textBlock"];
            descriptionLabel.Text = $"{product["description"]}";

            Border borderDescription = new Border();
            borderDescription.Child = descriptionLabel;
            Grid.SetColumn(borderDescription, 0);
            Grid.SetRow(borderDescription, 3);
            Grid.SetRowSpan(borderDescription, 2);

            if (Tables.features.isFeatureInUse("Storage"))
            {
                Label weightLabel = new Label();
                weightLabel.Content = $"Weight: {product["weight"]}";
                weightLabel.Style = (Style)this.Resources["labelstyle"];
                Grid.SetColumn(weightLabel, 1);
                Grid.SetRow(weightLabel, 0);

                Label volumeLabel = new Label();
                volumeLabel.Content = $"Volume: {product["volume"]}";
                volumeLabel.Style = (Style)this.Resources["labelstyle"];
                Grid.SetColumn(volumeLabel, 1);
                Grid.SetRow(volumeLabel, 1);

                Label widthLabel = new Label();
                widthLabel.Content = $"Width: {product["width"]}";
                widthLabel.Style = (Style)this.Resources["labelstyle"];
                Grid.SetColumn(widthLabel, 1);
                Grid.SetRow(widthLabel, 2);

                Label heightLabel = new Label();
                heightLabel.Content = $"Height: {product["heigth"]}";
                heightLabel.Style = (Style)this.Resources["labelstyle"];
                Grid.SetColumn(heightLabel, 1);
                Grid.SetRow(heightLabel, 3);

                Label lengthLabel = new Label();
                lengthLabel.Content = $"Length: {product["length"]}";
                lengthLabel.Style = (Style)this.Resources["labelstyle"];
                Grid.SetColumn(lengthLabel, 1);
                Grid.SetRow(lengthLabel, 4);

                grid.Children.Add(nameLabel);
                grid.Children.Add(buyingPriceLabel);
                grid.Children.Add(sellingPriceLabel);
                grid.Children.Add(borderDescription);
                grid.Children.Add(weightLabel);
                grid.Children.Add(volumeLabel);
                grid.Children.Add(widthLabel);
                grid.Children.Add(heightLabel);
                grid.Children.Add(lengthLabel);

                Grid.SetColumn(grid, 1);
                mainStackPanel.Children.Add(grid);
            }

            StackPanel buttonsStackPanel = new StackPanel();

            if (User.DoesHavePermission("Change Products"))
            {
                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.Click += DeleteProduct_Click;
                deleteButton.Tag = product;
                deleteButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                buttonsStackPanel.Children.Add(deleteButton);

                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                editButton.Click += EditProduct_Click;
                editButton.Tag = product;
                buttonsStackPanel.Children.Add(editButton);
            }


            Grid.SetColumn(buttonsStackPanel, 2);
            mainStackPanel.Children.Add(buttonsStackPanel);

            outerStack.Children.Add(mainStackPanel);
            border.Child = outerStack;
            panel.Children.Add(border);
        }

        public void DisplayAllProducts(Panel panel)
        {
            panel.Children.Clear();
            ProductsDisplay.Visibility = Visibility.Visible;
            panel.Visibility = Visibility.Visible;

            foreach (DataRow product in Tables.products.database.Rows)
            {
                DisplayOneProduct(ProductsDiaplayStackPanel, product);
            }
        }
        void DeleteProduct_Click(object sender, EventArgs e)
        {
            DataRow product = (sender as Button).Tag as DataRow;
            if (product != null)
            {
                product.Delete();
                Tables.products.updateChanges();
                DisplayAllProducts(ProductsDiaplayStackPanel);

                MessageBox.Show("Product deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        void EditProduct_Click(object sender, EventArgs e)
        {
            DataRow product = (sender as Button).Tag as DataRow;
            if (product != null)
            {
                if (WarehouseFromPage != null)
                {
                    Navigation.OpenPage(Navigation.GetTypeByName("EditProductPage"), product);
                    Navigation.ReturnParam = WarehouseFromPage;
                }
                else
                {
                    Navigation.OpenPage(Navigation.GetTypeByName("EditProductPage"), product);
                }          
            }
        }
        void CloseAndDisplay(object sender, EventArgs e)
        {
            DisplayAllProducts(ProductsDiaplayStackPanel);
        }
        private void AllProducts_Click(object sender, RoutedEventArgs e)
        {
            DisplayAllProducts(ProductsDiaplayStackPanel);
        }
        private void AddProducts_Click(object sender, RoutedEventArgs e)
        {
            if (WarehouseFromPage != null)
            {
                Navigation.OpenPage(Navigation.GetTypeByName("CreateProduct"));
                Navigation.ReturnParam = WarehouseFromPage;
            }
            else
            {
                Navigation.OpenPage(Navigation.GetTypeByName("CreateProduct"));
            }
        }
    }
}
