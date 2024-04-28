﻿using System;
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
            foreach (DataRow employee in Tables.employees.database.Rows)
            {
                DisplayOneEmployee(panel, employee);
            }
        }

        private void DisplayOneEmployee(Panel panel, DataRow employee)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(2);
            border.Margin = new Thickness(5);
            border.MinHeight = 100;
            border.MaxHeight = 170;

            StackPanel mainStackPanel = new StackPanel();
            mainStackPanel.Orientation = Orientation.Horizontal;
            mainStackPanel.Background = Brushes.White;

            Image image = new Image();
            image.Margin = new Thickness(5);
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
            leftStackPanel.Width = 550;

            Label nameLabel = new Label();
            nameLabel.Content = employee["name"];
            nameLabel.BorderBrush = Brushes.Black;
            nameLabel.BorderThickness = new Thickness(0, 0, 0, 1);

            Label emailLabel = new Label();
            emailLabel.Content = employee["email"];
            emailLabel.BorderBrush = Brushes.Black;
            emailLabel.BorderThickness = new Thickness(0, 0, 0, 1);

            Label warehouseLBL = new Label();
            warehouseLBL.BorderBrush = Brushes.Black;
            warehouseLBL.BorderThickness = new Thickness(0, 0, 0, 1);

            if (employee["warehouse_id"] != DBNull.Value)
            {
                warehouseLBL.Content = Tables.employees.getWarehouse(employee)["name"];
            }
            else
            {
                warehouseLBL.Content = "This emplyee does not belongs to a warehouse";
            }

            Label roleLabel = new Label(); 
            roleLabel.BorderBrush = Brushes.Black;

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
            rightStackPanel.VerticalAlignment = VerticalAlignment.Center;
            rightStackPanel.Width = 140;

            if (User.currentUser != employee)
            {
                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.Tag = employee;
                deleteButton.Click += deleteEmployee_Click;
                deleteButton.Margin = new Thickness(5);
                rightStackPanel.Children.Add(deleteButton);

                Button resetPasswordButton = new Button();
                resetPasswordButton.Content = "Reset Password";
                resetPasswordButton.Click += resetPassword_Click;
                resetPasswordButton.Tag = employee;
                resetPasswordButton.Margin = new Thickness(5);
                rightStackPanel.Children.Add(resetPasswordButton);
            }
            else
            {
                Button modifyPassword = new Button();
                modifyPassword.Content = "Reset Password";
                modifyPassword.Click += ModifyPassword;
                modifyPassword.Tag = employee;
                modifyPassword.Margin = new Thickness(5);
                rightStackPanel.Children.Add(modifyPassword);
            }

            Button editButton = new Button();
            editButton.Content = "Edit";
            editButton.Click += EditEmployee_Click;
            editButton.Tag = employee;
            editButton.Margin = new Thickness(5);

            rightStackPanel.Children.Add(editButton);

            mainStackPanel.Children.Add(image);
            mainStackPanel.Children.Add(leftStackPanel);
            mainStackPanel.Children.Add(rightStackPanel);

            border.Child = mainStackPanel;
            panel.Children.Add(border);
        }

        private void ModifyPassword(object sender, RoutedEventArgs e)
        {
            PasswordChangeForStaff passwordChangeForStaff = new PasswordChangeForStaff();
            passwordChangeForStaff.ShowDialog();
        }

        public void InitializeEmployeesInWarehouse(Panel panel, DataRow warehouse)
        {
            DisplayEmployeesStackpanel.Children.Clear();
            panel.Visibility = Visibility.Visible;

            foreach (DataRow emplyee in Tables.warehouses.getEmployees(warehouse))
            {
                DisplayOneEmployee(DisplayEmployeesStackpanel, emplyee);
            }
        }
        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            DataRow employee = (sender as Button).Tag as DataRow;
            if (employee != null)
            {
                if (WarehouseFromPage != null)
                {
                    MessageBox.Show("ez is lefutott");
                    Navigation.OpenPage(Navigation.GetTypeByName("EditEmployeePage"), employee);
                    Navigation.ReturnParam = WarehouseFromPage;
                }
                else
                {
                    if(Navigation.GetTypeByName("EditEmployeePage") != null)
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
