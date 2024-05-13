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
    //TODO: Buttons like: Delete employee, Inspect Employee, Reset password for employee waiting for to be binded, add an inspect all worker button too
    //TODO: kell egy olyan gomb ami vissza adja az összes dolgozót, mert ha rá nyomsz pl az inspect employees-ra akkor utána nem tudsz vissza menni egy olyan view-ra ahol meg tudod nézni megint együtt a staff és az employees-t
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();
            DisplayEmployeesStackpanel.Children.Clear();
            InitializeAllEmployees(DisplayEmployeesStackpanel);

            EmployeeWarehouses.Items.Clear();
            WarehouseNames_Id = new Dictionary<string, int>();
            for (int i = 0; i < Tables.warehouses.database.Rows.Count; i++)
            {
                WarehouseNames_Id.Add(Tables.warehouses.database.Rows[i]["name"].ToString(), int.Parse(Tables.warehouses.database.Rows[i]["id"].ToString()));
                EmployeeWarehouses.Items.Add(Tables.warehouses.database.Rows[i]["name"]);
            }
        }
        public static Dictionary<string, int> WarehouseNames_Id;
        public void InitializeAllEmployees(Panel panel)
        {
            EmployeesDisplay.Visibility = Visibility.Visible;
            panel.Visibility = Visibility.Visible;
            panel.Children.Clear();

            Label employeelabel = new Label();
            employeelabel.Content = "Employees:";
            employeelabel.BorderBrush = Brushes.Black;
            employeelabel.BorderThickness = new Thickness(0, 0, 0, 1);
            panel.Children.Add(employeelabel);

            for (int i = 0; i < Tables.employees.database.Rows.Count; i++)
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
                deleteButton.Tag = Tables.employees.database.Rows[i];
                deleteButton.Click += deleteEmployee_Click;

                Button resetPasswordButton = new Button();
                resetPasswordButton.Content = "Reset Password";
                resetPasswordButton.Click += resetPassword_Click;
                resetPasswordButton.Tag = Tables.employees.database.Rows[i];

                rightStackPanel.Children.Add(deleteButton);
                rightStackPanel.Children.Add(resetPasswordButton);

                mainStackPanel.Children.Add(image);
                mainStackPanel.Children.Add(leftStackPanel);
                mainStackPanel.Children.Add(rightStackPanel);

                panel.Children.Add(mainStackPanel);
            }
        }
        //TODO: warehouse class need
        public void InitializeEmployeesInWarehouse(Panel panel, int WarehouseID)
        {
            DataRow THISwarehouse = Tables.warehouses.findById(WarehouseID);

            EmployeesDisplay.Visibility = Visibility.Visible;
            panel.Children.Clear();
            panel.Visibility = Visibility.Visible;

            Label employeelabel = new Label();
            employeelabel.Content = $"Employees in {THISwarehouse["name"]}:";
            employeelabel.BorderBrush = Brushes.Black;
            employeelabel.BorderThickness = new Thickness(0, 0, 0, 1);
            panel.Children.Add(employeelabel);

            for (int i = 0; i < Tables.warehouses.getEmployees(THISwarehouse).Length; i++)
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
                nameLabel.Content = Tables.warehouses.getEmployees(THISwarehouse)[i]["name"];
                nameLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                nameLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                Label emailLabel = new Label();
                emailLabel.Content = Tables.warehouses.getEmployees(THISwarehouse)[i]["email"];
                emailLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                emailLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                Label roleLabel = new Label();
                roleLabel.Content = Tables.employees.getRole(Tables.warehouses.getEmployees(THISwarehouse)[i])["role"];
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
                deleteButton.Tag = Tables.employees.database.Rows[i];
                deleteButton.Click += deleteEmployee_Click;

                Button resetPasswordButton = new Button();
                resetPasswordButton.Content = "Reset Password";
                resetPasswordButton.Click += resetPassword_Click;
                resetPasswordButton.Tag = Tables.employees.database.Rows[i];

                rightStackPanel.Children.Add(deleteButton);
                //rightStackPanel.Children.Add(inspectButton);
                rightStackPanel.Children.Add(resetPasswordButton);

                mainStackPanel.Children.Add(image);
                mainStackPanel.Children.Add(leftStackPanel);
                mainStackPanel.Children.Add(rightStackPanel);

                panel.Children.Add(mainStackPanel);
            }
        }
        public static Dictionary<string, int> Role_Id;
        public static Dictionary<string, int> Warehouse_Id;
        private void AddNewEmployee_Click(object sender, RoutedEventArgs e)
        {
            EmployeesDisplay.Visibility = Visibility.Collapsed;

            Role_Id = new Dictionary<string, int>();
            EmployeeRole.Items.Clear();
            for (int i = 0; i < Tables.roles.database.Rows.Count; i++)
            {
                if (! SQL.BoolQuery($"SELECT in_warehouse FROM {Tables.roles.actual_name} WHERE role = '{Tables.roles.database.Rows[i]["role"]}'"))
                {
                    Role_Id.Add(Tables.roles.database.Rows[i]["role"].ToString(), (int)Tables.roles.database.Rows[i]["id"]);
                    EmployeeRole.Items.Add(Tables.roles.database.Rows[i]["role"].ToString());
                }
            }

            Warehouse_Id = new Dictionary<string, int>();
            EmployeesWarehouse.Items.Clear();
            for (int i = 0; i < Tables.warehouses.database.Rows.Count; i++)
            {
                Warehouse_Id.Add(Tables.warehouses.database.Rows[i]["name"].ToString(), (int)Tables.warehouses.database.Rows[i]["id"]);
                EmployeesWarehouse.Items.Add(Tables.warehouses.database.Rows[i]["name"].ToString());
            }

            RegisterEmployeDatas.Visibility = Visibility.Visible;
        }

        private void deleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            DataRow employee = (sender as Button).Tag as DataRow;
            if (employee != null)
            {
                employee.Delete();
                Tables.employees.updateChanges();
                EmployeeWarehouses.SelectedIndex = -1;
                InitializeAllEmployees(DisplayEmployeesStackpanel);

                MessageBox.Show("Employee deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void resetPassword_Click(object sender, RoutedEventArgs e)
        {
            DataRow employee = (sender as Button).Tag as DataRow;
            if (employee != null)
            {
                string password = Hash.GenerateRandomPassword(); //TODO: Ez kell majd az emailbe
                string HashedPassword = Hash.HashPassword(password);

                employee["password"] = HashedPassword;
                Tables.employees.updateChanges();

                /*DEBUG*/
                MessageBox.Show("Employees Page at Password reset: " + password);
                MessageBox.Show("Employees Page at Password reste: " + HashedPassword);
                /*DEBUG*/

                MessageBox.Show("Password has been reseted for the employee!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RegisterEmployeeWithDatas_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeRole.SelectedIndex != -1 && EmployeeName.Text != string.Empty && EmployeeEmail.Text != string.Empty)
            {
                if (Tables.staff.database.Select($"email = '{EmployeeEmail.Text}'").Length == 0 && Tables.employees.database.Select($"email = '{EmployeeEmail.Text}'").Length == 0)
                {
                    string password = Hash.GenerateRandomPassword(); //TODO: Ez kell majd az emailbe
                    string HashedPassword = Hash.HashPassword(password);
                    /*DEBUG*/
                    MessageBox.Show("Employees Page: " + password);
                    MessageBox.Show("Employees Page: " + HashedPassword);
                    /*DEBUG*/

                    SQL.SqlCommand($"INSERT INTO `{Tables.employees.actual_name}`(`name`, `email`, `password`, `role_id`, `warehouse_id`) VALUES ('{EmployeeName.Text}', '{EmployeeEmail.Text}', '{HashedPassword}' , {Role_Id[EmployeeRole.SelectedItem.ToString()]}, {Warehouse_Id[EmployeesWarehouse.SelectedItem.ToString()]})");
                    Tables.employees.Refresh();
                    
                    MessageBox.Show("New employees has been added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    EmployeesDisplay.Visibility = Visibility.Visible;
                    DisplayEmployeesStackpanel.Children.Clear();
                    InitializeAllEmployees(DisplayEmployeesStackpanel);

                    CancelF();
                }
                else
                {
                    MessageBox.Show("A person with this email alreday exist!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Empty input fileds!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void CancelF()
        {
            RegisterEmployeDatas.Visibility = Visibility.Collapsed;
            EmployeeName.Text = string.Empty;
            EmployeeEmail.Text = string.Empty;
            EmployeeRole.SelectedIndex = -1;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CancelF();

            EmployeesDisplay.Visibility = Visibility.Visible;
            DisplayEmployeesStackpanel.Children.Clear();
            InitializeAllEmployees(DisplayEmployeesStackpanel);
        }

        private void InsperctEmployees_Click(object sender, RoutedEventArgs e)
        {
            EmployeesDisplay.Visibility = Visibility.Visible;
            DisplayEmployeesStackpanel.Children.Clear();
            InitializeAllEmployees(DisplayEmployeesStackpanel);

            CancelF();
        }

        private void EmployeeWarehouses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeWarehouses.SelectedIndex != -1)
            {
                InitializeEmployeesInWarehouse(DisplayEmployeesStackpanel, WarehouseNames_Id[EmployeeWarehouses.SelectedItem.ToString()]);
            }
        }

        private void AllEmployees_Click(object sender, RoutedEventArgs e)
        {
            EmployeeWarehouses.SelectedIndex = -1;
            CancelF();
            InitializeAllEmployees(DisplayEmployeesStackpanel);
        }
    }
}
