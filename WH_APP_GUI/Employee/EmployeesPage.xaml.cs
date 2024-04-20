using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

namespace WH_APP_GUI
{
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();

            Ini_warehouse_id();

            DisplayEmployeesStackpanel.Children.Clear();
            InitializeAllEmployees(DisplayEmployeesStackpanel);
        }

        private Dictionary<string, DataRow> warehouse_id_Dictionary = new Dictionary<string, DataRow>();
        private void Ini_warehouse_id()
        {
            warehouse_id.Items.Clear();
            EmployeeWarehouses.Items.Clear();
            warehouse_id_Dictionary.Clear();

            foreach (DataRow warehouse in Tables.warehouses.database.Rows)
            {
                warehouse_id.Items.Add(warehouse["name"].ToString());
                EmployeeWarehouses.Items.Add(warehouse["name"].ToString());
                warehouse_id_Dictionary.Add(warehouse["name"].ToString(), warehouse);
            }
        }
        private Dictionary<string, DataRow> role_id_Dictionary = new Dictionary<string, DataRow>();
        private void Ini_role_id()
        {
            role_id.Items.Clear();
            role_id_Dictionary.Clear();

            foreach (DataRow role in Tables.roles.database.Rows)
            {
                if ((bool)role["in_warehouse"])
                {
                    role_id.Items.Add(role["role"].ToString());
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

            for (int i = 0; i < Tables.warehouses.getEmployees(warehouse).Length; i++)
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
                    string imageFileName = Tables.warehouses.getEmployees(warehouse)[i]["profile_picture"].ToString();
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
                nameLabel.Content = Tables.warehouses.getEmployees(warehouse)[i]["name"];
                nameLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                nameLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                Label emailLabel = new Label();
                emailLabel.Content = Tables.warehouses.getEmployees(warehouse)[i]["email"];
                emailLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                emailLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                Label roleLabel = new Label();
                roleLabel.Content = Tables.employees.getRole(Tables.warehouses.getEmployees(warehouse)[i])["role"];
                roleLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                roleLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                leftStackPanel.Children.Add(nameLabel);
                leftStackPanel.Children.Add(emailLabel);
                leftStackPanel.Children.Add(roleLabel);

                StackPanel rightStackPanel = new StackPanel();
                rightStackPanel.Orientation = Orientation.Vertical;
                rightStackPanel.Width = 130;

                if (User.currentUser != Tables.warehouses.getEmployees(warehouse)[i])
                {
                    Button deleteButton = new Button();
                    deleteButton.Content = "Delete";
                    deleteButton.Tag = Tables.employees.database.Rows[i];
                    deleteButton.Click += deleteEmployee_Click;
                    rightStackPanel.Children.Add(deleteButton);
                }

                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.Click += EditEmployee_Click;
                editButton.Tag = Tables.employees.database.Rows[i];

                Button resetPasswordButton = new Button();
                resetPasswordButton.Content = "Reset Password";
                resetPasswordButton.Click += resetPassword_Click;
                resetPasswordButton.Tag = Tables.employees.database.Rows[i];

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
            Button btn = sender as Button;
            if (btn.Tag != null)
            {
                EditEmployeePage editEmployeePage = new EditEmployeePage(btn.Tag as DataRow);
                editEmployeePage.Show();
                editEmployeePage.Closing += CloseEditWindow;
            }
        }
        void CloseEditWindow(object sender, EventArgs e)
        {
            DisplayEmployeesStackpanel.Children.Clear();
            InitializeAllEmployees(DisplayEmployeesStackpanel);
        }
        private void AddNewEmployee_Click(object sender, RoutedEventArgs e)
        {
            EmployeesDisplay.Visibility = Visibility.Collapsed;

            name.ValueDataType = typeof(string);
            email.ValueDataType = typeof(string);

            Ini_role_id();
            Ini_warehouse_id();

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

        private void RegisterEmployeeWithDatas_Click(object sender, RoutedEventArgs e)
        {
            if (Tables.staff.database.Select($"email = '{email.Text}'").Length == 0 && Tables.employees.database.Select($"email = '{email.Text}'").Length == 0)
            {
                string password = Hash.GenerateRandomPassword(); //TODO: Ez kell majd az emailbe
                string HashedPassword = Hash.HashPassword(password);
                /*DEBUG*/
                MessageBox.Show("Employees Page: " + password);
                MessageBox.Show("Employees Page: " + HashedPassword);
                /*DEBUG*/

                DataRow employee = Tables.employees.database.NewRow();
                if (!Validation.ValidateTextbox(name, employee) && !Validation.ValidateTextbox(email, employee) && role_id.SelectedIndex != -1 && warehouse_id.SelectedIndex != -1)
                {
                    string imageSrc;
                    if (profile_picture.Tag != null)
                    {
                        imageSrc = profile_picture.Tag.ToString();
                    }
                    else
                    {
                        imageSrc = "DefaultEmployeeProfile.png";
                    }
                    employee["name"] = name.Text;
                    employee["email"] = email.Text;
                    employee["password"] = HashedPassword;
                    MessageBox.Show(role_id_Dictionary[role_id.SelectedItem.ToString()]["id"].ToString());
                    employee["role_id"] = role_id_Dictionary[role_id.SelectedItem.ToString()]["id"];
                    MessageBox.Show(warehouse_id_Dictionary[warehouse_id.SelectedItem.ToString()]["id"].ToString());
                    employee["warehouse_id"] = warehouse_id_Dictionary[warehouse_id.SelectedItem.ToString()]["id"];
                    employee["profile_picture"] = imageSrc;

                    Tables.employees.database.Rows.Add(employee);
                    Tables.employees.updateChanges();
                    

                    MessageBox.Show("New employees has been added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    EmployeesDisplay.Visibility = Visibility.Visible;
                    DisplayEmployeesStackpanel.Children.Clear();
                    InitializeAllEmployees(DisplayEmployeesStackpanel);

                    CancelF();
                }
            }
            else
            {
                MessageBox.Show("A person with this email alreday exist!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void CancelF()
        {
            RegisterEmployeDatas.Visibility = Visibility.Collapsed;
            name.Text = string.Empty;
            email.Text = string.Empty;
            role_id.SelectedIndex = -1;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CancelF();

            EmployeesDisplay.Visibility = Visibility.Visible;
            DisplayEmployeesStackpanel.Children.Clear();
            InitializeAllEmployees(DisplayEmployeesStackpanel);
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
            CancelF();
            DisplayEmployeesStackpanel.Children.Clear();
            InitializeAllEmployees(DisplayEmployeesStackpanel);
        }

        private void profile_picture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose Image";
            openFileDialog.Filter = "Image|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All File|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string selectedFilePath = openFileDialog.FileName;
                    string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");

                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    string fileName = Path.GetFileName(selectedFilePath);
                    string targetFilePath = Path.Combine(targetDirectory, fileName);

                    File.Copy(selectedFilePath, targetFilePath, true);

                    BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));
                    ImageDisplay.Source = bitmap;

                    profile_picture.Tag = fileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during the Image browsing: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }
    }
}
