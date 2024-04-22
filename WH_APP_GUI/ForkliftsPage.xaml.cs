using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace WH_APP_GUI
{
    public partial class ForkliftsPage : Page
    {
        public ForkliftsPage()
        {
            InitializeComponent();

            forkliftFilter.Items.Clear();
            CreateDatawarehouse_id.Items.Clear();
            for (int i = 0; i < Tables.warehouses.database.Rows.Count; i++)
            {
                WarehouseNames_Id.Add(Tables.warehouses.database.Rows[i]["name"].ToString(), int.Parse(Tables.warehouses.database.Rows[i]["id"].ToString()));
                forkliftFilter.Items.Add(Tables.warehouses.database.Rows[i]["name"]);
                CreateDatawarehouse_id.Items.Add(Tables.warehouses.database.Rows[i]["name"]);
            }
        }
        public Dictionary<string, int> WarehouseNames_Id = new Dictionary<string, int>();
        
        public void DisplayForkliftsInWarehouse(Panel panel, DataRow warehouse)
        {
            //Foklift class need
            for (int i = 0; i < 5 ; i++)
            {
                StackPanel mainStackPanel = new StackPanel();
                mainStackPanel.Height = 100;
                mainStackPanel.Orientation = Orientation.Horizontal;

                Image image = new Image();
                image.Width = 100;
                image.Height = 100;
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.SetValue(Grid.RowSpanProperty, 3);

                StackPanel leftStackPanel = new StackPanel();
                leftStackPanel.Orientation = Orientation.Vertical;
                leftStackPanel.Width = 350;

                Label nameLabel = new Label();
                nameLabel.Content = Tables.employees.database.Rows[i]["name"];
                nameLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                nameLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                Label emailLabel = new Label();
                emailLabel.Content = Tables.employees.database.Rows[i]["email"];
                emailLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                emailLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                Label roleLabel = new Label();
                roleLabel.Content = Tables.employees.getRole(Tables.employees.database.Rows[i])["role"];
                roleLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                roleLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                leftStackPanel.Children.Add(nameLabel);
                leftStackPanel.Children.Add(emailLabel);
                leftStackPanel.Children.Add(roleLabel);

                StackPanel rightStackPanel = new StackPanel();
                rightStackPanel.Orientation = Orientation.Vertical;
                rightStackPanel.Width = 130;

                Button deleteButton = new Button();
                deleteButton.Content = "Delete";

                Button inspectButton = new Button();
                inspectButton.Content = "Inspect";

                Button resetPasswordButton = new Button();
                resetPasswordButton.Content = "Reset Password";

                rightStackPanel.Children.Add(deleteButton);
                rightStackPanel.Children.Add(inspectButton);
                rightStackPanel.Children.Add(resetPasswordButton);

                mainStackPanel.Children.Add(image);
                mainStackPanel.Children.Add(leftStackPanel);
                mainStackPanel.Children.Add(rightStackPanel);

                panel.Children.Add(mainStackPanel);
            }
        }

        private void AddNewforklift_Click(object sender, RoutedEventArgs e)
        {
            CreateforkliftDisplay.Visibility = Visibility.Visible;
        }
        private void Allforklift_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            //HomePage homePage = new HomePage();
            //this.Hide();
            //homePage.Show();
        }
        private void forkliftFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void CancelM()
        {
            CreateforkliftDisplay.Visibility = Visibility.Collapsed;
            CreateDatawarehouse_id.Text = string.Empty;
            CreateDatatype.Text = string.Empty;
            CreateDatastatus.Text = string.Empty;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CancelM();
        }
        private void CreateforkliftWithDatas_Click(object sender, RoutedEventArgs e)
        {
            if (CreateDatawarehouse_id.Text != string.Empty && CreateDatatype.Text != string.Empty && CreateDatastatus.Text != string.Empty)
            {
                SQL.SqlCommand($"INSERT INTO `forklift` (`warehouse_id`,`type`,`status`) VALUES ({WarehouseNames_Id[CreateDatawarehouse_id.SelectedItem.ToString()]},'{CreateDatatype.Text}','{CreateDatastatus.Text}');");
                MessageBox.Show("New Forklift has been added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CancelM();

                CreateDatawarehouse_id.Text = string.Empty;
                CreateDatatype.Text = string.Empty;
                CreateDatastatus.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("Empty input fileds!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
