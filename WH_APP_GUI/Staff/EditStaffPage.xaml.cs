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

namespace WH_APP_GUI.Staff
{
    public partial class EditStaffPage : Window
    {
        public EditStaffPage(DataRow staff)
        {
            InitializeComponent();

            PasswordReset.Tag = staff;
            Done.Tag = staff;
            profile_picture.Tag = staff;

            name.ValueDataType = typeof(string);
            email.ValueDataType = typeof(string);

            name.Text = staff["name"].ToString();
            email.Text = staff["email"].ToString();

            Ini_role_id();

            string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
            if (Directory.Exists(targetDirectory))
            {
                string imageFileName = staff["profile_picture"].ToString();
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

            role_id.SelectedItem = Tables.staff.getRole(staff)["role"];
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
            DataRow staff = (sender as Button).Tag as DataRow;
            if (staff != null)
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

                        staff["password"] = fileName;
                        Tables.staff.updateChanges();
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
            DataRow staff = (sender as Button).Tag as DataRow;
            if (staff != null)
            {
                if (!Validation.ValidateTextbox(name, staff) && Validation.ValidateTextbox(email, staff))
                {
                    staff["name"] = name.Text;
                    staff["email"] = email.Text;
                    staff["role_id"] = role_id_Dictionary[role_id.SelectedItem.ToString()]["id"];

                    MessageBox.Show("Staff updated");
                    this.Close();
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PasswordReset_Click(object sender, RoutedEventArgs e)
        {
            DataRow staff = (sender as Button).Tag as DataRow;
            if (staff != null)
            {
                string password = Hash.GenerateRandomPassword(); //TODO: Ez kell majd az emailbe
                string HashedPassword = Hash.HashPassword(password);

                staff["password"] = HashedPassword;
                Tables.staff.updateChanges();

                /*DEBUG*/
                MessageBox.Show("Staff Edit Page at Password reset: " + password);
                MessageBox.Show("Staff Edit Page at Password reste: " + HashedPassword);
                /*DEBUG*/

                MessageBox.Show("Password has been reseted for the employee!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
