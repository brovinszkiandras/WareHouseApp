using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace WH_APP_GUI.Employee
{
    public partial class EditEmployeePage : Page
    {
        private DataRow Employee = null;
        public EditEmployeePage(DataRow employee)
        {
            InitializeComponent();
            Employee = employee;

            IniWarehouses();
            IniRoles();

            name.ValueDataType = typeof(string);
            email.ValueDataType = typeof(string);

            name.Text = employee["name"].ToString();
            email.Text = employee["email"].ToString();
            if (employee["role_id"] != DBNull.Value)
            {
                role_id.SelectedItem = Tables.employees.getRole(employee)["role"];
            }

            if (employee["warehouse_id"] != DBNull.Value)
            {
                warehouse_id.SelectedItem = Tables.employees.getWarehouse(employee)["name"];
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
                    ImageBrush brush = new ImageBrush(bitmap);

                    profile_picture.Background = brush;
                }
            }

            if (Tables.features.isFeatureInUse("Activity"))
            {
                activity.Content = (bool)Employee["activity"] ? "ACTIVE" : "INACTIVE";
                activity.Background = (bool)Employee["activity"] ? Brushes.Green : Brushes.Red;

                activity.Visibility = Visibility.Visible;
                activity.Content = SQL.BoolQuery($"SELECT activity FROM {Tables.employees.actual_name} WHERE email = '{employee["email"]}'") ? "ACTIVE" : "INACTIVE";
            }
            else
            {
                activity.Visibility = Visibility.Collapsed;
            }
        }
        #region Display
        private Dictionary<string, DataRow> Warehouses = new Dictionary<string, DataRow>();
        private void IniWarehouses()
        {
            Warehouses.Clear();
            warehouse_id.Items.Clear();

            if (User.currentUser.Table.TableName != "employees")
            {
                foreach (DataRow warehouse in Tables.warehouses.database.Rows)
                {
                    Warehouses.Add(warehouse["name"].ToString(), warehouse);
                    warehouse_id.Items.Add(warehouse["name"].ToString());
                }
            }
            else
            {
                DataRow warehouse = Tables.employees.getWarehouse(User.currentUser);
                Warehouses.Add(warehouse["name"].ToString(), warehouse);
                warehouse_id.Items.Add(warehouse["name"].ToString());
                warehouse_id.SelectedItem = warehouse["name"].ToString();
            }
        }
        private Dictionary<string, DataRow> Roles = new Dictionary<string, DataRow>();
        private void IniRoles()
        {
            role_id.Items.Clear();
            Roles.Clear();

            foreach (DataRow role in Tables.roles.database.Rows)
            {
                if ((bool)role["in_warehouse"])
                {
                    role_id.Items.Add(role["role"].ToString());
                    Roles.Add(role["role"].ToString(), role);
                }
            }
        }
        private void EditEmploye_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
        }
        #endregion
        #region Methods
        private void PasswordReset_Click(object sender, RoutedEventArgs e)
        {
            if (Employee != null)
            {
                string password = Hash.GenerateRandomPassword();
                string HashedPassword = Hash.HashPassword(password);

                Employee["password"] = HashedPassword;
                Tables.employees.updateChanges();

                string text = $"Subject: Your Password Has Been Reset\r\n\r\n" +
                     $"Dear {Employee["name"]},\r\n\r\nYour password has been successfully reset." +
                     $" Please find your updated login credentials below:\r\n\r\n" +
                     $"Username/Email: {Employee["email"]}\r\nNew Password: {password}\r\n" +
                     $"Please keep this information secure and do not share it with anyone.\r\n" +
                     $"If you have any questions or concerns, feel free to reach out to us.\r\n" +
                     $"Best regards,\r\n[Your Company Name] Team";

                Email.send($"{Employee["email"]}", "Password Reset Confirmation", text);

                Tables.employees.updateChanges();
                MessageBox.Show("Password has been reseted for the employee!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void activity_Click(object sender, RoutedEventArgs e)
        {
            if (Employee != null)
            {
                Employee["activity"] = (bool)Employee["activity"] ? false : true;
                Tables.employees.updateChanges();

                activity.Content = (bool)Employee["activity"] ? "ACTIVE" : "INACTIVE";
                activity.Background = (bool)Employee["activity"] ? Brushes.Green : Brushes.Red;
                Tables.employees.updateChanges();

                MessageBox.Show("Activity changed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void profile_picture_Click(object sender, RoutedEventArgs e)
        {
            if (Employee != null)
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

                        profile_picture.Tag = fileName;
                        profile_picture.Background = new ImageBrush(bitmap);

                        Employee["profile_picture"] = fileName;
                        Tables.employees.updateChanges();
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
        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (Employee != null)
            {
                if (!Validation.ValidateTextbox(name, Employee) && !Validation.ValidateTextbox(email, Employee))
                {
                    Employee["name"] = name.Text;
                    Employee["email"] = email.Text;
                    Employee["role_id"] = Roles[role_id.SelectedItem.ToString()]["id"];
                    Employee["warehouse_id"] = Warehouses[warehouse_id.SelectedItem.ToString()]["id"];

                    Tables.employees.updateChanges();
                    Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has been modified {Employee["name"]} employee.");
                    MessageBox.Show("The employee has been updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    Navigation.OpenPage(Navigation.PreviousPage.GetType());
                }
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.PreviousPage.GetType());
        }
        #endregion
    }
}
