using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
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
using WH_APP_GUI.Product;

namespace WH_APP_GUI.Staff
{
    public partial class CreateStaffPage : Page
    {
        private static Type PreviousPageType;
        public CreateStaffPage(Page previousPage)
        {
            PreviousPageType = previousPage.GetType();

            InitializeComponent();

            name.ValueDataType = typeof(string);
            email.ValueDataType = typeof(string);
            Ini_role_id();
        }
        private Dictionary<string, DataRow> role_id_Dictionary = new Dictionary<string, DataRow>();
        private void Ini_role_id()
        {
            role_id.Items.Clear();
            role_id_Dictionary.Clear();

            foreach (DataRow role in Tables.roles.database.Rows)
            {
                if (!(bool)role["in_warehouse"])
                {
                    role_id.Items.Add(role["role"].ToString());
                    role_id_Dictionary.Add(role["role"].ToString(), role);
                }
            }
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

                    profile_picture.Tag = fileName;
                    profile_picture.Background = new ImageBrush(bitmap);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during the Image browsing: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            DataRow staff = Tables.products.database.NewRow();

            if (!Validation.ValidateTextbox(name, staff) && !Validation.ValidateTextbox(email, staff))
            {
                staff["name"] = name.Text;
                staff["email"] = email.Text;
                staff["profile_picture"] = profile_picture.Tag != null ? profile_picture.Tag.ToString() : "DefaultStaffProfilePicture.png";
                staff["role_id"] = role_id_Dictionary[role_id.SelectedItem.ToString()]["id"];

                string password = Hash.GenerateRandomPassword(); //TODO: Ez kell majd az emailbe
                string HashedPassword = Hash.HashPassword(password);
                staff["password"] = HashedPassword;
                Tables.employees.updateChanges();

                /*DEBUG*/
                MessageBox.Show("Create Staff Page: " + password);
                MessageBox.Show("Create Staff Page: " + HashedPassword);
                /*DEBUG*/

                Tables.staff.database.Rows.Add(staff);
                Tables.staff.updateChanges();

                Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has been created {staff["name"]} staff.");

                MessageBox.Show("Staff created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                //this.Close();
                //StaffPage staffPage = new StaffPage();
                //Navigation.content2.Navigate(staffPage);
                Page previousPage = (Page)Activator.CreateInstance(PreviousPageType);
                Navigation.content2.Navigate(previousPage);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //StaffPage staffPage = new StaffPage();
            //Navigation.content2.Navigate(staffPage);
            Page previousPage = (Page)Activator.CreateInstance(PreviousPageType);
            Navigation.content2.Navigate(previousPage);
        }
    }
}
