using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

namespace WH_APP_GUI
{
    public partial class EditEmployeePage : Page
    {
        private static Type PreviousPageType;
        public EditEmployeePage(Page previousPage, DataRow employee)
        {
            PreviousPageType = previousPage.GetType();

            InitializeComponent();
            IniWarehouses();
            IniRoles();

            name.ValueDataType = typeof(string);
            email.ValueDataType = typeof(string);

            name.Text = employee["name"].ToString();
            email.Text = employee["email"].ToString();
            role_id.SelectedItem = Tables.employees.getRole(employee)["role"];
            warehouse_id.SelectedItem = Tables.employees.getWarehouse(employee)["name"];
            PasswordReset.Tag = employee;
            activity.Tag = employee;
            DoneEmployeeUpdate.Tag = employee;
            profile_picture.Tag = employee;

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

            if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Activity';"))
            {
                activity.Visibility = Visibility.Visible;
                activity.Content = SQL.BoolQuery($"SELECT activity FROM {Tables.employees.actual_name} WHERE email = '{employee["email"]}'") ? "ACTIVE" : "INACTIVE";
            }
            else
            {
                activity.Visibility = Visibility.Collapsed;
            }

            if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Revenue';"))
            {
                payment.Visibility = Visibility.Visible;
                payment.ValueDataType = typeof(double);
                payment.Text = employee["payment"].ToString();
                paymentLBL.Visibility = Visibility.Visible;
            }
            else
            {
                payment.Visibility = Visibility.Collapsed;
                paymentLBL.Visibility = Visibility.Collapsed;
            }
        }

        private Dictionary<string, DataRow> Warehouses = new Dictionary<string, DataRow>();
        private void IniWarehouses()
        {
            Warehouses.Clear();
            warehouse_id.Items.Clear();
            foreach (DataRow warehouse in Tables.warehouses.database.Rows)
            {
                Warehouses.Add(warehouse["name"].ToString(), warehouse);
                warehouse_id.Items.Add(warehouse["name"].ToString());
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //EmployeesPage employeesPage = new EmployeesPage();
            //Navigation.content2.Navigate(employeesPage);

            Page previousPage = (Page)Activator.CreateInstance(PreviousPageType);
            Navigation.content2.Navigate(previousPage);
        }

        private void PasswordReset_Click(object sender, RoutedEventArgs e)
        {
            DataRow employee = (sender as Button).Tag as DataRow;
            if (employee != null)
            {
                string password = Hash.GenerateRandomPassword(); //TODO: Ez kell majd az emailbe
                string HashedPassword = Hash.HashPassword(password);

                employee["password"] = HashedPassword;
                Tables.employees.updateChanges();

                /*DEBUG*/
                MessageBox.Show("Employees Edit Page at Password reset: " + password);
                MessageBox.Show("Employees Edit Page at Password reste: " + HashedPassword);
                /*DEBUG*/

                Tables.employees.updateChanges();
                MessageBox.Show("Password has been reseted for the employee!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void activity_Click(object sender, RoutedEventArgs e)
        {
            DataRow employee = (sender as Button).Tag as DataRow;
            if (employee != null)
            {
                employee["activity"] = (bool)employee["activity"] ? false : true;
                Tables.employees.updateChanges();

                activity.Content = (bool)employee["activity"] ? "ACTIVE" : "INACTIVE";
                Tables.employees.updateChanges();

                MessageBox.Show("Activity changed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            DataRow employee = (sender as Button).Tag as  DataRow;
            if (employee != null)
            {
                if (! Validation.ValidateTextbox(name, employee) && ! Validation.ValidateTextbox(email, employee))
                {
                    employee["name"] = name.Text;
                    employee["email"] = email.Text;
                    employee["role_id"] = Roles[role_id.SelectedItem.ToString()]["id"];
                    employee["warehouse_id"] = Warehouses[warehouse_id.SelectedItem.ToString()]["id"];

                    if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Revenue';"))
                    {
                        if (! Validation.ValidateTextbox(payment, employee))
                        {
                            employee["payment"] = payment.Text != string.Empty ? payment.Text : "0";
                        }
                    }

                    Tables.employees.updateChanges();
                    Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has been modified {employee["name"]} employee.");
                    MessageBox.Show("The employee has been updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    //EmployeesPage employeesPage = new EmployeesPage();
                    //Navigation.content2.Navigate(employeesPage);
                    Page previousPage = (Page)Activator.CreateInstance(PreviousPageType);
                    Navigation.content2.Navigate(previousPage);
                }
            }
        }

        private void profile_picture_Click(object sender, RoutedEventArgs e)
        {
            DataRow employee = (sender as Button).Tag as DataRow;
            if (employee != null)
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

                        employee["profile_picture"] = fileName;
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
    }
}
