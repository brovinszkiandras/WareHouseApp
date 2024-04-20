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
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
            DisplayAllProducts(ProductsDiaplayStackPanel);
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
                EditProductPage editProductPage = new EditProductPage(product);
                editProductPage.Show();
                editProductPage.Closing += CloseAndDisplay;

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

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            //HomePage homePage = new HomePage();
            //this.Hide();
            //homePage.Show();
        }
        private void AddProducts_Click(object sender, RoutedEventArgs e)
        {
            CreateProduct createProductPage = new CreateProduct();
            createProductPage.Show();
            createProductPage.Closing += CloseAndDisplay;
        }
    }
}
