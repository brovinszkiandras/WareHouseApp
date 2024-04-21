using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WH_APP_GUI.Employee
{
    public partial class CreateEmployee : Page
    {
        private static Type PreviousPageType;
        public CreateEmployee(Page previousPage)
        {
            PreviousPageType = previousPage.GetType();

            InitializeComponent();
            IniWarehouses();
            IniRoles();

            profile_picture.Height = this.Height * 0.25;
            profile_picture.Width = this.Height * 0.25;

            name.ValueDataType = typeof(string);
            email.ValueDataType = typeof(string);
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

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            DataRow employee = Tables.employees.database.NewRow();

            if (! Validation.ValidateTextbox(name, employee) && ! Validation.ValidateTextbox(email, employee) && role_id.SelectedIndex != -1 && warehouse_id.SelectedIndex != -1)
            {
                string password = Hash.GenerateRandomPassword(); //TODO: Ez kell majd az emailbe
                string HashedPassword = Hash.HashPassword(password);

                /*DEBUG*/
                MessageBox.Show("Employees Edit Page at Password reset: " + password);
                MessageBox.Show("Employees Edit Page at Password reste: " + HashedPassword);
                /*DEBUG*/

                employee["name"] = name.Text;
                employee["email"] = email.Text;
                employee["role_id"] = Roles[role_id.SelectedItem.ToString()]["id"];
                employee["warehouse_id"] = Warehouses[warehouse_id.SelectedItem.ToString()]["id"];
                employee["password"] = HashedPassword;
                employee["profile_picture"] = profile_picture.Tag != null ? profile_picture.Tag : "DefaultEmployeeProfile.png";

                Tables.employees.database.Rows.Add(employee);
                Tables.employees.updateChanges();

                Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has created {employee["name"]} employee.");
                MessageBox.Show("Employee has successfully created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                //EmployeesPage employeesPage = new EmployeesPage();
                //Navigation.content2.Navigate(employeesPage);

                //EmployeesPage employeesPage = new EmployeesPage();

                Page previousPage = (Page)Activator.CreateInstance(PreviousPageType);
                Navigation.content2.Navigate(previousPage);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //EmployeesPage employeesPage = new EmployeesPage();
            //Navigation.content2.Navigate(employeesPage);

            Page previousPage = (Page)Activator.CreateInstance(PreviousPageType);
            Navigation.content2.Navigate(previousPage);
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

                    profile_picture.Background = new ImageBrush(bitmap);
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
