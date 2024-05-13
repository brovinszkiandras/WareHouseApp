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
using WH_APP_GUI.Staff;
using WH_APP_GUI.Warehouse;

namespace WH_APP_GUI
{
    public partial class EmployeesPage : Page
    {
        private DataRow WarehouseFromPage;
        public EmployeesPage()
        {
            InitializeComponent();

            Back.Visibility = Visibility.Collapsed;

            Ini_warehouse_id();

            if (User.DoesHavePermission("Modify all employees"))
            {
                AddNewEmployee.Visibility = Visibility.Visible;
            }

            DisplayEmployeesStackpanel.Children.Clear();
            InitializeAllEmployees();
        }
        public EmployeesPage(DataRow warehouse)
        {
            InitializeComponent();

            if (User.DoesHavePermission("Modify all employees"))
            {
                AddNewEmployee.Visibility = Visibility.Visible;
            }
            else if (User.DoesHavePermission("Modify employees"))
            {
                if (User.currentUser.Table.TableName == "employees")
                {
                    if ((int)User.currentUser["warehouse"] == (int)warehouse["id"])
                    {
                        AddNewEmployee.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        AddNewEmployee.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                AddNewEmployee.Visibility = Visibility.Collapsed;
            }

            DisplayEmployeesStackpanel.Children.Clear();
            AllEmployees.Visibility = Visibility.Collapsed;
            AddNewEmployee.Visibility = Visibility.Visible;

            WarehouseFromPage = warehouse;
            InitializeEmployeesInWarehouse(warehouse);
        }
        #region Display
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
        public void InitializeAllEmployees()
        {
            foreach (DataRow employee in Tables.employees.database.Rows)
            {
                DisplayOneEmployee(employee);
            }
        }
        private void DisplayOneEmployee(DataRow employee)
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
                dateLog.Content = $"Created at: {employee["created_at"]} \tUpdated at: {employee["updated_at"]}";
                dateLog.HorizontalContentAlignment = HorizontalAlignment.Center;
                dateLog.Style = (Style)this.Resources["labelstyle"];
                outerStack.Children.Add(dateLog);
            }

            Grid mainGrid = new Grid();
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

            StackPanel imageStack = new StackPanel(); 
            imageStack.Orientation = Orientation.Vertical;
            Grid.SetColumn(imageStack, 0);
            mainGrid.Children.Add(imageStack);

            Image image = new Image();
            image.Margin = new Thickness(5);
            image.MaxHeight = 100;
            image.MaxWidth = 100;
            image.Stretch = Stretch.Fill;
            imageStack.Children.Add(image);

            if (Tables.features.isFeatureInUse("Activity"))
            {
                Border onlineBorder = new Border();
                onlineBorder.BorderBrush = Brushes.Black;
                onlineBorder.Background = new SolidColorBrush(Color.FromArgb(255, 0x39, 0x52, 0x50));
                onlineBorder.CornerRadius = new CornerRadius(15);
                onlineBorder.BorderThickness = new Thickness(2);
                onlineBorder.Margin = new Thickness(5);

                Label online = new Label();
                online.VerticalAlignment = VerticalAlignment.Bottom;
                online.HorizontalContentAlignment = HorizontalAlignment.Center;
                online.Foreground = Brushes.White;
                online.Style = (Style)this.Resources["labelstyle"];
                onlineBorder.Child = online;
                imageStack.Children.Add(onlineBorder);

                if ((bool)employee["is_loggedin"])
                {
                    online.Content = "Online";
                    onlineBorder.Background = Brushes.Green;
                }
                else
                {
                    online.Content = "Offline";
                    onlineBorder.Background = Brushes.Red;
                }             
            }  

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
                else
                {
                    imageFileName = "DefaultEmployeeProfile.png";
                    string fileName = Path.GetFileName(imagePath);
                    string targetFilePath = Path.Combine(targetDirectory, fileName);

                    BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));
                    image.Source = bitmap;
                }
            }

            StackPanel leftStackPanel = new StackPanel();
            leftStackPanel.Orientation = Orientation.Vertical;

            Grid.SetColumn(leftStackPanel, 1);
            mainGrid.Children.Add(leftStackPanel);

            Label nameLabel = new Label();
            nameLabel.Content = employee["name"];
            nameLabel.Style = (Style)this.Resources["labelstyle"];

            Label emailLabel = new Label();
            emailLabel.Content = employee["email"];
            emailLabel.Style = (Style)this.Resources["labelstyle"];

            Label warehouseLBL = new Label();
            warehouseLBL.Style = (Style)this.Resources["labelstyle"];

            if (employee["warehouse_id"] != DBNull.Value)
            {
                warehouseLBL.Content = Tables.employees.getWarehouse(employee)["name"];
            }
            else
            {
                warehouseLBL.Content = "Not in warehouse.";
            }

            Label roleLabel = new Label();
            roleLabel.Style = (Style)this.Resources["labelstyle"];

            if (employee["role_id"] != DBNull.Value)
            {
                roleLabel.Content = Tables.employees.getRole(employee)["role"];
            }
            else
            {
                roleLabel.Content = "This employee does not have a role";
            }

            leftStackPanel.Children.Add(nameLabel);
            leftStackPanel.Children.Add(emailLabel);
            leftStackPanel.Children.Add(warehouseLBL);
            leftStackPanel.Children.Add(roleLabel);

            StackPanel rightStackPanel = new StackPanel();
            rightStackPanel.Orientation = Orientation.Vertical;
            Grid.SetColumn(rightStackPanel, 2);
            mainGrid.Children.Add(rightStackPanel);

            if (User.DoesHavePermission("Modify employees"))
            {
                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.Tag = employee;
                deleteButton.Click += deleteEmployee_Click;
                deleteButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                rightStackPanel.Children.Add(deleteButton);

                Button resetPasswordButton = new Button();
                resetPasswordButton.Content = "Reset Password";
                resetPasswordButton.Click += resetPassword_Click;
                resetPasswordButton.Tag = employee;
                resetPasswordButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                rightStackPanel.Children.Add(resetPasswordButton);

                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.Click += EditEmployee_Click;
                editButton.Tag = employee;
                editButton.Style = (Style)this.Resources["GoldenButtonStyle"];

                rightStackPanel.Children.Add(editButton);
            }
            else if (User.DoesHavePermission("Modify all employees"))
            {
                if (User.currentUser.Table.TableName == "employees" && WarehouseFromPage != null)
                {
                    if (User.currentUser["warehouse"].ToString() == WarehouseFromPage["id"].ToString())
                    {
                        Button deleteButton = new Button();
                        deleteButton.Content = "Delete";
                        deleteButton.Tag = employee;
                        deleteButton.Click += deleteEmployee_Click;
                        deleteButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                        rightStackPanel.Children.Add(deleteButton);

                        Button resetPasswordButton = new Button();
                        resetPasswordButton.Content = "Reset Password";
                        resetPasswordButton.Click += resetPassword_Click;
                        resetPasswordButton.Tag = employee;
                        resetPasswordButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                        rightStackPanel.Children.Add(resetPasswordButton);

                        Button editButton = new Button();
                        editButton.Content = "Edit";
                        editButton.Click += EditEmployee_Click;
                        editButton.Tag = employee;
                        editButton.Style = (Style)this.Resources["GoldenButtonStyle"];

                        rightStackPanel.Children.Add(editButton);
                    }
                }
            }

            if (User.currentUser == employee)
            {
                Button modifyPassword = new Button();
                modifyPassword.Content = "Change Password";
                modifyPassword.Click += ModifyPassword;
                modifyPassword.Tag = employee;
                modifyPassword.Style = (Style)this.Resources["GoldenButtonStyle"];
                rightStackPanel.Children.Add(modifyPassword);
            }

            outerStack.Children.Add(mainGrid);
            border.Child = outerStack;
            DisplayEmployeesStackpanel.Children.Add(border);
        }
        public void InitializeEmployeesInWarehouse(DataRow warehouse)
        {
            DisplayEmployeesStackpanel.Children.Clear();
            DisplayEmployeesStackpanel.Visibility = Visibility.Visible;

            foreach (DataRow employee in Tables.warehouses.getEmployees(warehouse))
            {
                DisplayOneEmployee(employee);
            }
        }
        private void EmployePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
        }
        #endregion
        #region Methods
        private void ModifyPassword(object sender, RoutedEventArgs e)
        {
            PasswordChangeForEmployee passwordChangeForEmployee = new PasswordChangeForEmployee();
            passwordChangeForEmployee.ShowDialog();
        }
        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            DataRow employee = (sender as Button).Tag as DataRow;
            if (employee != null)
            {
                if (WarehouseFromPage != null)
                {
                    Navigation.ReturnParam = null;
                    Navigation.OpenPage(Navigation.GetTypeByName("EditEmployeePage"), employee);
                    Navigation.ReturnParam = WarehouseFromPage;
                }
                else
                {
                    if (Navigation.GetTypeByName("EditEmployeePage") != null)
                    {
                        Navigation.OpenPage(Navigation.GetTypeByName("EditEmployeePage"), employee);
                    }
                }
            }
        }
        private void AddNewEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (WarehouseFromPage != null)
            {
                Navigation.ReturnParam = null;
                Navigation.OpenPage(Navigation.GetTypeByName("CreateEmployee"));
                Navigation.ReturnParam = WarehouseFromPage;
            }
            else
            {
                Navigation.OpenPage(Navigation.GetTypeByName("CreateEmployee"));
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
                InitializeAllEmployees();

                MessageBox.Show("Employee deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void resetPassword_Click(object sender, RoutedEventArgs e)
        {
            DataRow employee = (sender as Button).Tag as DataRow;
            if (employee != null)
            {
                string password = Hash.GenerateRandomPassword();
                string HashedPassword = Hash.HashPassword(password);

                employee["password"] = HashedPassword;
                Tables.employees.updateChanges();

                string text = $"Subject: Your Password Has Been Reset\r\n\r\n" +
                     $"Dear {employee["name"]},\r\n\r\nYour password has been successfully reset." +
                     $" Please find your updated login credentials below:\r\n\r\n" +
                     $"Username/Email: {employee["email"]}\r\nNew Password: {password}\r\n" +
                     $"Please keep this information secure and do not share it with anyone.\r\n" +
                     $"If you have any questions or concerns, feel free to reach out to us.\r\n";

                Email.send($"{employee["email"]}", "Password Reset Confirmation", text);

                MessageBox.Show("Password has been reseted for the employee!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void EmployeeWarehouses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeWarehouses.SelectedIndex != -1)
            {
                DisplayEmployeesStackpanel.Children.Clear();
                InitializeEmployeesInWarehouse(warehouse_id_Dictionary[EmployeeWarehouses.SelectedItem.ToString()]);
            }
        }
        private void AllEmployees_Click(object sender, RoutedEventArgs e)
        {
            EmployeeWarehouses.SelectedIndex = -1;
            DisplayEmployeesStackpanel.Children.Clear();
            InitializeAllEmployees();
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
        #endregion
    }
}