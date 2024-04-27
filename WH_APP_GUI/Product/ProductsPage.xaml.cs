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
            //DisplayAllProducts(ProductsDiaplayStackPanel);
            foreach (DataRow product in Tables.products.database.Rows)
            {
                DisplayOneProduct(ProductsDiaplayStackPanel, product);
            }
        }
        private DataRow WarehouseFromPage;
        public ProductsPage(DataRow warehouseFromPage)
        {
            InitializeComponent();
            DisplayAllProducts(ProductsDiaplayStackPanel);
            WarehouseFromPage = warehouseFromPage;
        }

        private void DisplayOneProduct(Panel panel, DataRow product)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(2);
            border.Background = Brushes.White;
            //border.MaxHeight = 150;
            //border.MaxWidth = 600;
            border.Margin = new Thickness(5);

            StackPanel mainStackpanel = new StackPanel();
            mainStackpanel.Orientation = Orientation.Horizontal;

            StackPanel imageStackpanel = new StackPanel();
            imageStackpanel.Margin = new Thickness(5);
            imageStackpanel.VerticalAlignment = VerticalAlignment.Center;

            Image productImage = new Image();
            productImage.Height = 80;
            productImage.Width = 80;
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
            productName.BorderBrush = Brushes.Black;
            productName.BorderThickness = new Thickness(0, 0, 0, 1);
            productName.Width = 80;
            productName.FontStyle = FontStyles.Italic;
            productName.FontSize = 8;
            productName.HorizontalContentAlignment = HorizontalAlignment.Center;
            imageStackpanel.Children.Add(productName);

            mainStackpanel.Children.Add(imageStackpanel);

            Grid grid = new Grid();
            grid.MinWidth = 400;
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            Label nameLabel = new Label();
            nameLabel.Content = $"Product Name: {product["name"]}";
            nameLabel.BorderBrush = Brushes.Black;
            nameLabel.BorderThickness = new Thickness(0, 0, 1, 1);
            Grid.SetColumn(nameLabel, 0);
            Grid.SetRow(nameLabel, 0);

            Label buyingPriceLabel = new Label();
            buyingPriceLabel.Content = $"Buying Price: {product["buying_price"]}";
            buyingPriceLabel.BorderBrush = Brushes.Black;
            buyingPriceLabel.BorderThickness = new Thickness(0, 0, 1, 1);
            Grid.SetColumn(buyingPriceLabel, 0);
            Grid.SetRow(buyingPriceLabel, 1);

            Label sellingPriceLabel = new Label();
            sellingPriceLabel.Content = $"Selling Price: {product["selling_price"]}";
            sellingPriceLabel.BorderBrush = Brushes.Black;
            sellingPriceLabel.BorderThickness = new Thickness(0, 0, 1, 1);
            Grid.SetColumn(sellingPriceLabel, 0);
            Grid.SetRow(sellingPriceLabel, 2);

            Label descriptionLabel = new Label();
            descriptionLabel.Content = $"{product["description"]}";
            descriptionLabel.BorderBrush = Brushes.Black;
            descriptionLabel.BorderThickness = new Thickness(0, 0, 1, 0);
            Grid.SetColumn(descriptionLabel, 0);
            Grid.SetRow(descriptionLabel, 3);
            Grid.SetRowSpan(descriptionLabel, 2);

            if (Tables.features.isFeatureInUse("Storage"))
            {
                Label weightLabel = new Label();
                weightLabel.Content = $"Weight: {product["weight"]}";
                weightLabel.BorderBrush = Brushes.Black;
                weightLabel.BorderThickness = new Thickness(0, 0, 0, 1);
                Grid.SetColumn(weightLabel, 1);
                Grid.SetRow(weightLabel, 0);

                Label volumeLabel = new Label();
                volumeLabel.Content = $"Volume: {product["volume"]}";
                volumeLabel.BorderBrush = Brushes.Black;
                volumeLabel.BorderThickness = new Thickness(0, 0, 0, 1);
                Grid.SetColumn(volumeLabel, 1);
                Grid.SetRow(volumeLabel, 1);

                Label widthLabel = new Label();
                widthLabel.Content = $"Width: {product["width"]}";
                widthLabel.BorderBrush = Brushes.Black;
                widthLabel.BorderThickness = new Thickness(0, 0, 0, 1);
                Grid.SetColumn(widthLabel, 1);
                Grid.SetRow(widthLabel, 2);

                Label heightLabel = new Label();
                heightLabel.Content = $"Height: {product["heigth"]}";
                heightLabel.BorderBrush = Brushes.Black;
                heightLabel.BorderThickness = new Thickness(0, 0, 0, 1);
                Grid.SetColumn(heightLabel, 1);
                Grid.SetRow(heightLabel, 3);

                Label lengthLabel = new Label();
                lengthLabel.Content = $"Length: {product["length"]}";
                lengthLabel.BorderBrush = Brushes.Black;
                Grid.SetColumn(lengthLabel, 1);
                Grid.SetRow(lengthLabel, 4);

                grid.Children.Add(nameLabel);
                grid.Children.Add(buyingPriceLabel);
                grid.Children.Add(sellingPriceLabel);
                grid.Children.Add(descriptionLabel);
                grid.Children.Add(weightLabel);
                grid.Children.Add(volumeLabel);
                grid.Children.Add(widthLabel);
                grid.Children.Add(heightLabel);
                grid.Children.Add(lengthLabel);

                mainStackpanel.Children.Add(grid);
            }

            StackPanel buttonsStackPanel = new StackPanel();
            buttonsStackPanel.Width = 100;
            buttonsStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            buttonsStackPanel.VerticalAlignment = VerticalAlignment.Center;

            Button deleteButton = new Button();
            deleteButton.Content = "Delete";
            deleteButton.Margin = new Thickness(5);
            buttonsStackPanel.Children.Add(deleteButton);

            Button editButton = new Button();
            editButton.Content = "Edit";
            editButton.Margin = new Thickness(5);
            buttonsStackPanel.Children.Add(editButton);

            mainStackpanel.Children.Add(buttonsStackPanel);

            border.Child = mainStackpanel;
            panel.Children.Add(border);
        }

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

                
                if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Storage'"))
                {
                    thirdStackPanel.Children.Add(new Label { Content = Tables.products.database.Rows[i]["weight"].ToString() });
                    thirdStackPanel.Children.Add(new Label { Content = Tables.products.database.Rows[i]["volume"].ToString() });
                    thirdStackPanel.Children.Add(new Label { Content = Tables.products.database.Rows[i]["width"].ToString() });
                    thirdStackPanel.Children.Add(new Label { Content = Tables.products.database.Rows[i]["heigth"].ToString() });
                    thirdStackPanel.Children.Add(new Label { Content = Tables.products.database.Rows[i]["length"].ToString() });
                }

                StackPanel fourthStackPanel = new StackPanel();
                fourthStackPanel.Orientation = Orientation.Vertical;
                fourthStackPanel.Width = 120;
                stackPanelInsideGrid.Children.Add(fourthStackPanel);

                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.Click += DeleteProduct_Click;
                deleteButton.Tag = Tables.products.database.Rows[i];

                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.Click += EditProduct_Click;
                editButton.Tag = Tables.products.database.Rows[i];

                fourthStackPanel.Children.Add(deleteButton);
                fourthStackPanel.Children.Add(editButton);

                panel.Children.Add(grid);
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
