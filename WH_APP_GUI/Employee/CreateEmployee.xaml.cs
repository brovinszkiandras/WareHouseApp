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
using WH_APP_GUI.Warehouse;

namespace WH_APP_GUI.Employee
{
    public partial class CreateEmployee : Page
    {
        public CreateEmployee()
        {
            InitializeComponent();
            IniWarehouses();
            IniRoles();
            IniPicture();

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
        private void IniPicture()
        {
            string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
            if (Directory.Exists(targetDirectory))
            {
                string imageFileName = "DefaultEmployeeProfile.png";
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

            if (! Validation.ValidateTextbox(name, employee) && ! Validation.validateEmail(email.Text) && role_id.SelectedIndex != -1 && warehouse_id.SelectedIndex != -1)
            {
                string password = Hash.GenerateRandomPassword(); //TODO: Ez kell majd az emailbe
                string HashedPassword = Hash.HashPassword(password);

                employee["name"] = name.Text;
                employee["email"] = email.Text;
                employee["role_id"] = Roles[role_id.SelectedItem.ToString()]["id"];
                employee["warehouse_id"] = Warehouses[warehouse_id.SelectedItem.ToString()]["id"];
                employee["password"] = HashedPassword;
                employee["profile_picture"] = profile_picture.Tag != null ? profile_picture.Tag : "DefaultEmployeeProfile.png";

                if (Tables.features.isFeatureInUse("Activity"))
                {
                    employee["activity"] = true;
                }

                Tables.employees.database.Rows.Add(employee);
                Tables.employees.updateChanges();

                Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has created {employee["name"]} employee.");
                MessageBox.Show("Employee has successfully created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                //EmployeesPage employeesPage = new EmployeesPage();

                //string text = $"Subject: Welcome to the company! Your Login Credentials Inside\r\n\r\n" +
                //    $"Dear {employee["name"]},\r\n\r\nWe are thrilled to welcome you to the company." +
                //    $" We are excited to have you on board and look forward to your contributions to our company." +
                //    $"\r\n\r\nAs a new member of our team, you will need access to our company's applications. " +
                //    $"Below, you will find your login credentials:\r\n\r\n" +
                //    $"Username/Email: {employee["email"]}\r\nPassword: {password}\r\n";

                //Email.send($"{employee["email"]}","Welcome to the company",text);

                Navigation.OpenPage(Navigation.PreviousPage.GetType());
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.PreviousPage.GetType());
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
                    string pngFilename =  Path.ChangeExtension(fileName, "png");
                    
                    string targetFilePath = Path.Combine(targetDirectory, pngFilename);

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
        private void CreateEmploye_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
            profile_picture.Height = alapgrid.Height;
            profile_picture.Width = alapgrid.Height;
        }
    }
}
