﻿using Microsoft.Win32;
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
    public partial class EditStaffPage : Page
    {
        private DataRow staff;
        public EditStaffPage(DataRow Staff)
        {
            this.staff = Staff;
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

            if (staff["role_id"] != DBNull.Value)
            {
                role_id.SelectedItem = Tables.staff.getRole(staff)["role"];
            } 
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

                        ImageBrush brush = new ImageBrush(bitmap);

                        profile_picture.Background = brush;
                        staff["profile_picture"] = fileName;
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
            
            if (staff != null)
            {
                if (!Validation.ValidateTextbox(name, staff) && !Validation.validateEmail(email.Text))
                {
                    staff["name"] = name.Text;
                    staff["email"] = email.Text;
                    staff["role_id"] = role_id_Dictionary[role_id.SelectedItem.ToString()]["id"];

                    Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has been modified {staff["name"]} staff.");
                    MessageBox.Show("Staff updated");
                    
                    Tables.staff.updateChanges();

                    Navigation.OpenPage(Navigation.PreviousPage.GetType());
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.PreviousPage.GetType());
        }

        private void PasswordReset_Click(object sender, RoutedEventArgs e)
        {
            DataRow staff = (sender as Button).Tag as DataRow;
            if (staff != null)
            {
                string password = Hash.GenerateRandomPassword();
                string HashedPassword = Hash.HashPassword(password);

                staff["password"] = HashedPassword;
                Tables.staff.updateChanges();

                string text = $"Subject: Your Password Has Been Reset\r\n\r\n" +
                 $"Dear {staff["name"]},\r\n\r\nYour password has been successfully reset." +
                 $" Please find your updated login credentials below:\r\n\r\n" +
                 $"Username/Email: {staff["email"]}\r\nNew Password: {password}\r\n" +
                 $"Please keep this information secure and do not share it with anyone.\r\n" +
                 $"If you have any questions or concerns, feel free to reach out to us.\r\n" +
                 $"Best regards,\r\n[Your Company Name] Team";

                Email.send($"{staff["email"]}", "Password Reset Confirmation", text);

                MessageBox.Show("Password has been reseted for the staff!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void EditStaff_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
        }
    }
}
