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
using WH_APP_GUI.Employee;
using WH_APP_GUI.Forklift;
using WH_APP_GUI.Warehouse;

namespace WH_APP_GUI
{
    public partial class ForkliftsPage : Page
    {
        public ForkliftsPage()
        {
            InitializeComponent();
            Ini_warehouse_id();
            DisplayAllForklifts(DisplayForkliftsStackPanel);
            Back.Visibility = Visibility.Collapsed;
        }
        private DataRow WarehouseFromPage;
        public ForkliftsPage(DataRow warehouse)
        {
            WarehouseFromPage = warehouse;
            InitializeComponent();
            forkliftFilter.Visibility = Visibility.Collapsed;
            Allforklift.Visibility = Visibility.Collapsed;
            Ini_warehouse_id();
            DisplayAllForklifts(DisplayForkliftsStackPanel);
        }
        private Dictionary<string, DataRow> warehouse_id_Dictionary = new Dictionary<string, DataRow>();
        private void Ini_warehouse_id()
        {
            warehouse_id_Dictionary.Clear();
            forkliftFilter.Items.Clear();

            foreach (DataRow warehouse in Tables.warehouses.database.Rows)
            {
                forkliftFilter.Items.Add(warehouse["name"].ToString());
                warehouse_id_Dictionary.Add(warehouse["name"].ToString(), warehouse);
            }
        }
        public void DisplayAllForklifts(Panel panel)
        {
            if (WarehouseFromPage != null)
            {
                foreach (DataRow forklift in Tables.warehouses.getForklifts(WarehouseFromPage))
                {
                    DisplayOneForklift(DisplayForkliftsStackPanel, forklift);
                }
            }
            else
            {
                foreach (DataRow forklift in Tables.forklifts.database.Rows)
                {
                    DisplayOneForklift(DisplayForkliftsStackPanel, forklift);
                }
            }
        }

        private void DisplayOneForklift(Panel panel, DataRow forklift)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(2);
            border.Margin = new Thickness(5);
            border.Width = 600;

            StackPanel mainStackPanel = new StackPanel();
            mainStackPanel.MaxHeight = 250;
            mainStackPanel.Orientation = Orientation.Horizontal;
            mainStackPanel.Background = Brushes.White;

            Image image = new Image();
            image.Width = 100;
            image.Height = 100;
            image.Margin = new Thickness(5);
            image.HorizontalAlignment = HorizontalAlignment.Left;
            image.SetValue(Grid.RowSpanProperty, 3);

            string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
            if (Directory.Exists(targetDirectory))
            {
                string imageFileName = "ForkliftPicture.png";
                string imagePath = Path.Combine(targetDirectory, imageFileName);

                if (File.Exists(imagePath))
                {
                    string fileName = Path.GetFileName(imagePath);
                    string targetFilePath = Path.Combine(targetDirectory, fileName);

                    BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));

                    image.Source = bitmap;
                }
            }

            StackPanel leftStackPanel = new StackPanel();
            leftStackPanel.Orientation = Orientation.Vertical;
            leftStackPanel.Width = 350;

            Label type = new Label();
            type.Content = "Type: " + forklift["type"];
            type.BorderBrush = Brushes.Black;
            type.BorderThickness = new Thickness(0, 0, 0, 1);

            Label status = new Label();
            status.Content = "Status: " + forklift["status"];
            status.BorderBrush = Brushes.Black;
            status.BorderThickness = new Thickness(0, 0, 0, 1);

            Label warehouseName = new Label();
            warehouseName.BorderBrush = Brushes.Black;
            warehouseName.BorderThickness = new Thickness(0, 0, 0, 1);

            if (forklift["warehouse_id"] != DBNull.Value)
            {
                warehouseName.Content = "Warehouse Name: " + Tables.forklifts.getWarehouse(forklift)["name"];
            }
            else
            {
                warehouseName.Content = "This forklift does not belongs to a warehouse";
            }

            Label operating_hours = new Label();
            operating_hours.Content = "Operating hours: " + forklift["operating_hours"];
            operating_hours.BorderBrush = Brushes.Black;

            leftStackPanel.Children.Add(type);
            leftStackPanel.Children.Add(status);
            leftStackPanel.Children.Add(warehouseName);
            leftStackPanel.Children.Add(operating_hours);

            StackPanel rightStackPanel = new StackPanel();
            rightStackPanel.Orientation = Orientation.Vertical;
            rightStackPanel.Width = 130;
            rightStackPanel.VerticalAlignment = VerticalAlignment.Center;

            Button deleteButton = new Button();
            deleteButton.Content = "Delete";
            deleteButton.Margin = new Thickness(5);
            deleteButton.Tag = forklift;
            deleteButton.Click += deleteForklift_Click;

            Button editButton = new Button();
            editButton.Content = "Edit";
            editButton.Margin = new Thickness(5);
            editButton.Tag = forklift;
            editButton.Click += editForklift_Click;

            rightStackPanel.Children.Add(deleteButton);
            rightStackPanel.Children.Add(editButton);

            mainStackPanel.Children.Add(image);
            mainStackPanel.Children.Add(leftStackPanel);
            mainStackPanel.Children.Add(rightStackPanel);

            border.Child = mainStackPanel;
            panel.Children.Add(border);
        }
        public void DisplayForkliftsInWarehouse(Panel panel, DataRow warehouse)
        {
            DisplayForkliftsStackPanel.Children.Clear();
            foreach (DataRow forklift in Tables.warehouses.getForklifts(warehouse))
            {
                DisplayOneForklift(DisplayForkliftsStackPanel, forklift);
            }
        }

        private void AddNewforklift_Click(object sender, RoutedEventArgs e)
        {
            if (WarehouseFromPage != null)
            {
                Navigation.OpenPage(Navigation.GetTypeByName("CreateFrokliftPage"));
                Navigation.ReturnParam = WarehouseFromPage;
            }
            else
            {
                Navigation.OpenPage(Navigation.GetTypeByName("CreateFrokliftPage"));
            }
        }
        
        private void editForklift_Click(object sender, RoutedEventArgs e)
        {
            DataRow forklift = (sender as Button).Tag as DataRow;
            if (forklift != null)
            {
                if (WarehouseFromPage != null)
                {
                    Navigation.OpenPage(Navigation.GetTypeByName("EditForkliftPage"), forklift);
                    Navigation.ReturnParam = WarehouseFromPage;
                }
                else
                {
                    Navigation.OpenPage(Navigation.GetTypeByName("EditForkliftPage"), forklift);
                }
            }
        }
        private void deleteForklift_Click(object sender, RoutedEventArgs e)
        {
            DataRow forklift = (sender as Button).Tag as DataRow;
            if (forklift != null)
            {
                forklift.Delete();
                Tables.forklifts.updateChanges();

                DisplayForkliftsStackPanel.Children.Clear();
                DisplayAllForklifts(DisplayForkliftsStackPanel);

                MessageBox.Show("Product deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void Allforklift_Click(object sender, RoutedEventArgs e)
        {
            DisplayForkliftsStackPanel.Children.Clear();
            DisplayAllForklifts(DisplayForkliftsStackPanel);
        }
        private void forkliftFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (forkliftFilter.SelectedIndex != -1)
            {
                DisplayForkliftsStackPanel.Children.Clear();
                DisplayForkliftsInWarehouse(DisplayForkliftsStackPanel, warehouse_id_Dictionary[forkliftFilter.SelectedItem.ToString()]);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Navigation.PreviousPage != null)
            {
                if (WarehouseFromPage != null)
                {
                    Navigation.PreviousPage = new InspectWarehouse(WarehouseFromPage);
                    Navigation.OpenPage(Navigation.PreviousPage.GetType(), WarehouseFromPage);
                }
                else
                {
                    Navigation.OpenPage(Navigation.PreviousPage.GetType());
                }
            }
        }
    }
}
