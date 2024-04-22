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
using System.Windows.Navigation;
using WH_APP_GUI.Employee;
using WH_APP_GUI.Warehouse;

namespace WH_APP_GUI.Dock
{
    public partial class DockPage : Page
    {
        public DockPage()
        {
            InitializeComponent();
            IniWarehouses();
            InitializeAllDocks(DockDisplaySTACK);
            Back.Visibility = Visibility.Collapsed;
            DocksByWarehouses.Visibility = Visibility.Visible;
            AllDocks.Visibility = Visibility.Visible;
        }
        private DataRow WarehouseFromPage = null;
        public DockPage(DataRow warehouseFromPage)
        {
            InitializeComponent();
            IniWarehouses();
            InitializeDocksInWarehouse(DockDisplaySTACK, warehouseFromPage);

            WarehouseFromPage = warehouseFromPage;
            DocksByWarehouses.Visibility = Visibility.Collapsed;
            Back.Visibility = Visibility.Visible;
            AllDocks.Visibility = Visibility.Collapsed;
        }
        private Dictionary<string, DataRow> Warehouses = new Dictionary<string, DataRow>();
        private void IniWarehouses()
        {
            Warehouses.Clear();
            DocksByWarehouses.Items.Clear();
            foreach (DataRow warehouse in Tables.warehouses.database.Rows)
            {
                Warehouses.Add(warehouse["name"].ToString(), warehouse);
                DocksByWarehouses.Items.Add(warehouse["name"].ToString());
            }
        }
        public void InitializeAllDocks(Panel panel)
        {
            panel.Children.Clear();
            for (int i = 0; i < Tables.warehouses.database.Rows.Count; i++)
            {
                InitializeDocksInWarehouse(panel, Tables.warehouses.database.Rows[i]);
            }
        }
        public void InitializeDocksInWarehouse(Panel panel, DataRow warehouse)
        {
            DockDisplaySTACK.Visibility = Visibility.Visible;
            panel.Visibility = Visibility.Visible;

            if (Tables.warehouses.getDocks(warehouse).Length > 0)
            {
                Label employeelabel = new Label();
                employeelabel.Content = $"Docks in {warehouse["name"]}:";
                employeelabel.BorderBrush = Brushes.Black;
                employeelabel.BorderThickness = new Thickness(0, 0, 0, 1);
                panel.Children.Add(employeelabel);
                foreach (DataRow dock in Tables.warehouses.getDocks(warehouse))
                {
                    StackPanel mainStackPanel = new StackPanel();
                    mainStackPanel.Height = 100;
                    mainStackPanel.Orientation = Orientation.Horizontal;

                    Image image = new Image();
                    image.Width = 80;
                    image.Height = 80;

                    string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
                    if (Directory.Exists(targetDirectory))
                    {
                        string imageFileName = "DockDefaultImage.png";
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

                    Label nameLabel = new Label();
                    nameLabel.Content = dock["name"];
                    nameLabel.BorderBrush = Brushes.Black;
                    nameLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                    Label roleLabel = new Label();
                    roleLabel.Content = warehouse["name"];
                    roleLabel.BorderBrush = Brushes.Black;
                    roleLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                    leftStackPanel.Children.Add(nameLabel);
                    leftStackPanel.Children.Add(roleLabel);

                    StackPanel rightStackPanel = new StackPanel();
                    rightStackPanel.Orientation = Orientation.Vertical;
                    rightStackPanel.Width = 130;

                    Button deleteButton = new Button();
                    deleteButton.Content = "Delete";
                    deleteButton.Click += deleteDock_Click;
                    deleteButton.Tag = dock;

                    Button editButton = new Button();
                    editButton.Content = "Edit";
                    editButton.Click += EditDock_Click;
                    editButton.Tag = dock;

                    Button isDockFree = new Button();
                    isDockFree.Content = bool.Parse(dock["free"].ToString()) ? "Free" : "In use";
                    isDockFree.Background = bool.Parse(dock["free"].ToString()) ? Brushes.Blue : Brushes.Red;
                    isDockFree.Click += isDockFree_Click;
                    isDockFree.Tag = dock;

                    rightStackPanel.Children.Add(deleteButton);
                    rightStackPanel.Children.Add(editButton);
                    rightStackPanel.Children.Add(isDockFree);

                    mainStackPanel.Children.Add(image);
                    mainStackPanel.Children.Add(leftStackPanel);
                    mainStackPanel.Children.Add(rightStackPanel);

                    panel.Children.Add(mainStackPanel);
                }
            }
        }

        private void deleteDock_Click(object sender, RoutedEventArgs e)
        {
            DataRow dock = (sender as Button).Tag as DataRow;
            if (dock != null)
            {
                dock.Delete();
                Tables.docks.updateChanges();
                DocksByWarehouses.SelectedIndex = -1;
                DockDisplaySTACK.Children.Clear();
                InitializeAllDocks(DockDisplaySTACK);

                MessageBox.Show("Dock deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void EditDock_Click(object sender, RoutedEventArgs e)
        {
            DataRow dock = (sender as Button).Tag as DataRow;
            if (dock != null)
            {
                if (WarehouseFromPage != null)
                {
                    Navigation.OpenPage(Navigation.GetTypeByName("EditDockPage"), dock);
                    Navigation.ReturnParam = WarehouseFromPage;
                }
                else
                {
                    Navigation.OpenPage(Navigation.GetTypeByName("EditDockPage"), dock);
                }
            }
        }
        private void AddNewDock_Click(object sender, RoutedEventArgs e)
        {
            if (WarehouseFromPage != null)
            {
                Navigation.OpenPage(Navigation.GetTypeByName("CreateDockPage"));
                Navigation.ReturnParam = WarehouseFromPage;
            }
            else
            {
                Navigation.OpenPage(Navigation.GetTypeByName("CreateDockPage"));
            }
        }
        private void isDockFree_Click(object sender, RoutedEventArgs e)
        {
            DataRow dock = (sender as Button).Tag as DataRow;
            if (dock != null)
            {
                dock["free"] = bool.Parse(dock["free"].ToString()) ? false : true;
                Tables.docks.updateChanges();
                InitializeAllDocks(DockDisplaySTACK);
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

        private void DockByWarehouses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DocksByWarehouses.SelectedIndex != -1)
            {
                DockDisplaySTACK.Children.Clear();
                InitializeDocksInWarehouse(DockDisplaySTACK, Warehouses[DocksByWarehouses.SelectedItem.ToString()]);
            }
        }

        private void AllDocks_Click(object sender, RoutedEventArgs e)
        {
            DockDisplaySTACK.Children.Clear();
            InitializeAllDocks(DockDisplaySTACK);
        }
    }
}
