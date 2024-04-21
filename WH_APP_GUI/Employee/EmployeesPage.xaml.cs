using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WH_APP_GUI.Employee;
using WH_APP_GUI.Warehouse;

namespace WH_APP_GUI
{
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();

            Back.Visibility = Visibility.Collapsed;

            Ini_warehouse_id();
            Ini_role_id();

            DisplayEmployeesStackpanel.Children.Clear();
            InitializeAllEmployees(DisplayEmployeesStackpanel);
        }
        private DataRow WarehouseFromPage;
        //itt nem kell bekérni az előző oldalt mert elég csak a raktárat átadni, abból meg lehet mondani hogy az egy inspect warehouse page
        public EmployeesPage(DataRow warehouse)
        {
            InitializeComponent();

            DisplayEmployeesStackpanel.Children.Clear();
            AllEmployees.Visibility = Visibility.Collapsed;
            AddNewEmployee.Visibility = Visibility.Visible;

            WarehouseFromPage = warehouse;
            InitializeEmployeesInWarehouse(DisplayEmployeesStackpanel, warehouse);
        }

        private Dictionary<string, DataRow> warehouse_id_Dictionary = new Dictionary<string, DataRow>();
        private void Ini_warehouse_id()
        {
            EmployeeWarehouses.Visibility = Visibility.Visible;
            EmployeeWarehouses.Items.Clear();
            warehouse_id_Dictionary.Clear();

            foreach (DataRow warehouse in Tables.warehouses.database.Rows)
            {
                EmployeeWarehouses.Items.Add(warehouse["name"].ToString());
                warehouse_id_Dictionary.Add(warehouse["name"].ToString(), warehouse);
            }
        }
        private Dictionary<string, DataRow> role_id_Dictionary = new Dictionary<string, DataRow>();
        private void Ini_role_id()
        {
            role_id_Dictionary.Clear();

            foreach (DataRow role in Tables.roles.database.Rows)
            {
                if ((bool)role["in_warehouse"])
                {
                    role_id_Dictionary.Add(role["role"].ToString(), role);
                }
            }
        }
        public void InitializeAllEmployees(Panel panel)
        {
            for (int i = 0; i < Tables.warehouses.database.Rows.Count; i++)
            {
                InitializeEmployeesInWarehouse(panel, Tables.warehouses.database.Rows[i]);
            }
        }
        public void InitializeEmployeesInWarehouse(Panel panel, DataRow warehouse)
        {
            EmployeesDisplay.Visibility = Visibility.Visible;
            panel.Visibility = Visibility.Visible;

            if (Tables.warehouses.getEmployees(warehouse).Length > 0)
            {
                Label employeelabel = new Label();
                employeelabel.Content = $"Employees in {warehouse["name"]}:";
                employeelabel.BorderBrush = Brushes.Black;
                employeelabel.BorderThickness = new Thickness(0, 0, 0, 1);
                panel.Children.Add(employeelabel);
            }

            foreach (DataRow employee in Tables.warehouses.getEmployees(warehouse))
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
                    string imageFileName = employee["profile_picture"].ToString();
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
                nameLabel.Content = employee["name"];
                nameLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                nameLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                Label emailLabel = new Label();
                emailLabel.Content = employee["email"];
                emailLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                emailLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                Label roleLabel = new Label();
                roleLabel.Content = Tables.employees.getRole(employee)["role"];
                roleLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                roleLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                leftStackPanel.Children.Add(nameLabel);
                leftStackPanel.Children.Add(emailLabel);
                leftStackPanel.Children.Add(roleLabel);

                StackPanel rightStackPanel = new StackPanel();
                rightStackPanel.Orientation = Orientation.Vertical;
                rightStackPanel.Width = 130;

                if (User.currentUser != employee)
                {
                    Button deleteButton = new Button();
                    deleteButton.Content = "Delete";
                    deleteButton.Tag = employee;
                    deleteButton.Click += deleteEmployee_Click;
                    rightStackPanel.Children.Add(deleteButton);
                }

                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.Click += EditEmployee_Click;
                editButton.Tag = employee;

                Button resetPasswordButton = new Button();
                resetPasswordButton.Content = "Reset Password";
                resetPasswordButton.Click += resetPassword_Click;
                resetPasswordButton.Tag = employee;

                rightStackPanel.Children.Add(editButton);
                rightStackPanel.Children.Add(resetPasswordButton);

                mainStackPanel.Children.Add(image);
                mainStackPanel.Children.Add(leftStackPanel);
                mainStackPanel.Children.Add(rightStackPanel);

                panel.Children.Add(mainStackPanel);
            }
        }
        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            DataRow employee = (sender as Button).Tag as DataRow;
            if (employee != null)
            {
                if (WarehouseFromPage != null)
                {
                    Navigation.OpenPage(Navigation.GetTypeByName("EditEmployeePage"), employee);
                    Navigation.ReturnParam = WarehouseFromPage;
                }
                else
                {
                    Navigation.OpenPage(Navigation.GetTypeByName("EditEmployeePage"), employee);   
                }
            }
        }
        private void AddNewEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (WarehouseFromPage != null)
            {
                Navigation.OpenPage(Navigation.GetTypeByName("CreateEmployee"));   
                Navigation.ReturnParam = WarehouseFromPage;
            }
            else
            {
                Navigation.ReturnParam = WarehouseFromPage;
            }
        }

        private void deleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            DataRow employee = (sender as Button).Tag as DataRow;
            if (employee != null)
            {
                employee.Delete();
                Tables.employees.updateChanges();
                EmployeeWarehouses.SelectedIndex = -1;
                DisplayEmployeesStackpanel.Children.Clear();
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

        private void EmployeeWarehouses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeWarehouses.SelectedIndex != -1)
            {
                DisplayEmployeesStackpanel.Children.Clear();
                InitializeEmployeesInWarehouse(DisplayEmployeesStackpanel, warehouse_id_Dictionary[EmployeeWarehouses.SelectedItem.ToString()]);
            }
        }

        private void AllEmployees_Click(object sender, RoutedEventArgs e)
        {
            EmployeeWarehouses.SelectedIndex = -1;
            DisplayEmployeesStackpanel.Children.Clear();
            InitializeAllEmployees(DisplayEmployeesStackpanel);
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
