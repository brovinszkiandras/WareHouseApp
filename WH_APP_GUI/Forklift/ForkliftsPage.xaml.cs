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

namespace WH_APP_GUI
{
    public partial class ForkliftsPage : Page
    {
        public ForkliftsPage()
        {
            InitializeComponent();
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
            for (int i = 0; i < Tables.warehouses.database.Rows.Count; i++)
            {
                DisplayForkliftsInWarehouse(panel, Tables.warehouses.database.Rows[i]);
            }
        }
        public void DisplayForkliftsInWarehouse(Panel panel, DataRow warehouse)
        {
            for (int i = 0; i < Tables.warehouses.getForklifts(warehouse).Length; i++)
            {
                StackPanel mainStackPanel = new StackPanel();
                mainStackPanel.Height = 100;
                mainStackPanel.Orientation = Orientation.Horizontal;

                Image image = new Image();
                image.Width = 100;
                image.Height = 100;
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
                type.Content = "Type: " + Tables.warehouses.getForklifts(warehouse)[i]["type"];
                type.BorderBrush = System.Windows.Media.Brushes.Black;
                type.BorderThickness = new Thickness(0, 0, 0, 1);

                Label status = new Label();
                status.Content = "Status: " + Tables.warehouses.getForklifts(warehouse)[i]["status"];
                status.BorderBrush = System.Windows.Media.Brushes.Black;
                status.BorderThickness = new Thickness(0, 0, 0, 1);

                Label warehouseName = new Label();
                warehouseName.Content = "Warehouse Name: " + warehouse["name"];
                warehouseName.BorderBrush = System.Windows.Media.Brushes.Black;
                warehouseName.BorderThickness = new Thickness(0, 0, 0, 1);

                Label operating_hours = new Label();
                operating_hours.Content = "Operating hours: " + Tables.warehouses.getForklifts(warehouse)[i]["operating_hours"];
                operating_hours.BorderBrush = System.Windows.Media.Brushes.Black;
                operating_hours.BorderThickness = new Thickness(0, 0, 0, 1);

                leftStackPanel.Children.Add(type);
                leftStackPanel.Children.Add(status);
                leftStackPanel.Children.Add(warehouseName);
                leftStackPanel.Children.Add(operating_hours);

                StackPanel rightStackPanel = new StackPanel();
                rightStackPanel.Orientation = Orientation.Vertical;
                rightStackPanel.Width = 130;

                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.Tag = Tables.warehouses.getForklifts(warehouse)[i];
                deleteButton.Click += deleteForklift_Click;

                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.Tag = Tables.warehouses.getForklifts(warehouse)[i];
                editButton.Click += editForklift_Click;

                rightStackPanel.Children.Add(deleteButton);
                rightStackPanel.Children.Add(editButton);

                mainStackPanel.Children.Add(image);
                mainStackPanel.Children.Add(leftStackPanel);
                mainStackPanel.Children.Add(rightStackPanel);

                panel.Children.Add(mainStackPanel);
            }
        }

        private void AddNewforklift_Click(object sender, RoutedEventArgs e)
        {
            CreateFrokliftPage createFrokliftPage = new CreateFrokliftPage(new ForkliftsPage());
            ForkliftContent.Content = null;
            ForkliftContent.Navigate(createFrokliftPage);
            ForkliftContent.Visibility = Visibility.Visible;
        }
        
        private void editForklift_Click(object sender, RoutedEventArgs e)
        {
            DataRow forklift = (sender as Button).Tag as DataRow;
            if (forklift != null)
            {
                EditForkliftPage editForkliftPage = new EditForkliftPage(new ForkliftsPage(), forklift);
                ForkliftContent.Content = null;
                ForkliftContent.Navigate(editForkliftPage);
                ForkliftContent.Visibility = Visibility.Visible;
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

        void CloseAndDisplay(object sender, EventArgs e)
        {
            DisplayForkliftsStackPanel.Children.Clear();
            DisplayAllForklifts(DisplayForkliftsStackPanel);
        }
        private void Allforklift_Click(object sender, RoutedEventArgs e)
        {
            DisplayForkliftsStackPanel.Children.Clear();
            DisplayAllForklifts(DisplayForkliftsStackPanel);
        }
        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            //HomePage homePage = new HomePage();
            //this.Hide();
            //homePage.Show();
        }
        private void forkliftFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (forkliftFilter.SelectedIndex != -1)
            {
                DisplayForkliftsStackPanel.Children.Clear();
                DisplayForkliftsInWarehouse(DisplayForkliftsStackPanel, warehouse_id_Dictionary[forkliftFilter.SelectedItem.ToString()]);
            }
        }
    }
}
