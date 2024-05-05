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

            if (!User.DoesHavePermission("Modify all Forklift"))
            {
                AddNewforklift.Visibility = Visibility.Collapsed;
            }
        }
        private DataRow WarehouseFromPage;
        public ForkliftsPage(DataRow warehouse)
        {
            WarehouseFromPage = warehouse;
            InitializeComponent();
            forkliftFilter.Visibility = Visibility.Collapsed;
            Allforklift.Visibility = Visibility.Collapsed;

            if (!User.DoesHavePermission("Modify all Forklift") || !User.DoesHavePermission("Modify Forklift"))
            {
                AddNewforklift.Visibility = Visibility.Collapsed;
            }

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
            border.Background = new SolidColorBrush(Color.FromArgb(255, 0x39, 0x52, 0x50));
            border.CornerRadius = new CornerRadius(15);
            border.BorderThickness = new Thickness(2);
            border.Margin = new Thickness(5);

            Grid mainGrid = new Grid();
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });


            Image image = new Image();
            image.Width = 100;
            image.Height = 100;
            image.Margin = new Thickness(5);

            Grid.SetColumn(image, 0);
            mainGrid.Children.Add(image);

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
            Grid.SetColumn(leftStackPanel, 1);
            mainGrid.Children.Add(leftStackPanel);

            Label type = new Label();
            type.Content = "Type: " + forklift["type"];
            type.Style = (Style)this.Resources["labelstyle"];

            ComboBox statusCBX = new ComboBox();
            statusCBX.VerticalContentAlignment = VerticalAlignment.Center;

            statusCBX.FontFamily = new FontFamily("Baskerville Old Face");
            statusCBX.Foreground = Brushes.Black;
            statusCBX.Background = new SolidColorBrush(Color.FromRgb(0x39, 0x52, 0x50));
            statusCBX.BorderBrush = Brushes.Black;
            statusCBX.Margin = new Thickness(5);
            statusCBX.Items.Add("Free");
            statusCBX.Items.Add("On duty");
            statusCBX.Items.Add("Under Maintenance");
            statusCBX.Items.Add("Faulty");
            statusCBX.IsEnabled = false;
            statusCBX.Tag = forklift;
            if (forklift["status"] != DBNull.Value)
            {
                statusCBX.SelectedItem = forklift["status"].ToString();
            }
            statusCBX.SelectionChanged += StatusCBX_SelectionChanged;

            Label warehouseName = new Label();
            warehouseName.Style = (Style)this.Resources["labelstyle"];

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
            operating_hours.Style = (Style)this.Resources["labelstyle"];

            leftStackPanel.Children.Add(type);
            leftStackPanel.Children.Add(statusCBX);
            leftStackPanel.Children.Add(warehouseName);
            leftStackPanel.Children.Add(operating_hours);

            StackPanel rightStackPanel = new StackPanel();
            rightStackPanel.Orientation = Orientation.Vertical;
            Grid.SetColumn(rightStackPanel, 2);
            mainGrid.Children.Add(rightStackPanel);

            if (User.DoesHavePermission("Modify all Forklift"))
            {
                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.Tag = forklift;
                deleteButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                deleteButton.Click += deleteForklift_Click;

                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.Tag = forklift;
                editButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                editButton.Click += editForklift_Click;

                statusCBX.IsEnabled = true;

                rightStackPanel.Children.Add(deleteButton);
                rightStackPanel.Children.Add(editButton);
            }
            else if (User.DoesHavePermission("Modify Forklift"))
            {
                if (User.currentUser.Table.TableName == "employees" && WarehouseFromPage != null)
                {
                    if ((int)User.currentUser["warehouse_id"] == (int)WarehouseFromPage["id"])
                    {
                        Button deleteButton = new Button();
                        deleteButton.Content = "Delete";
                        deleteButton.Tag = forklift;
                        deleteButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                        deleteButton.Click += deleteForklift_Click;

                        Button editButton = new Button();
                        editButton.Content = "Edit";
                        editButton.Tag = forklift;
                        editButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                        editButton.Click += editForklift_Click;

                        statusCBX.IsEnabled = true;

                        rightStackPanel.Children.Add(deleteButton);
                        rightStackPanel.Children.Add(editButton);
                    }
                }
            }
            else if (User.DoesHavePermission("Change status of Forklift"))
            {
                if (User.currentUser.Table.TableName == "employees" && WarehouseFromPage != null)
                {
                    if ((int)User.currentUser["warehouse_id"] == (int)WarehouseFromPage["id"])
                    {
                        statusCBX.IsEnabled = true;
                    }
                }
            }

            border.Child = mainGrid;
            panel.Children.Add(border);
        }
        public void StatusCBX_SelectionChanged(object sender, RoutedEventArgs e)
        {
            DataRow forklift = (sender as ComboBox).Tag as DataRow;
            if (forklift != null && (sender as ComboBox).SelectedIndex != -1)
            {
                forklift["status"] = (sender as ComboBox).SelectedItem.ToString();
                Tables.forklifts.updateChanges();
                MessageBox.Show("Forklift has been updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
