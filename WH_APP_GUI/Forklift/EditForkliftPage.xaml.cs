﻿using System;
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
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using WH_APP_GUI.Warehouse;

namespace WH_APP_GUI.Forklift
{
    public partial class EditForkliftPage : Page
    {
        public EditForkliftPage(DataRow forklift)
        {
            InitializeComponent();
            IniWarehouses();
            IniStatuses();

            type.ValueDataType = typeof(string);
            operating_hours.ValueDataType = typeof(int);

            type.Text = forklift["type"].ToString();
            status.SelectedItem = forklift["status"].ToString();
            warehouse_id.SelectedItem = Tables.forklifts.getWarehouse(forklift)["name"];
            operating_hours.Text = forklift["operating_hours"].ToString();

            Done.Tag = forklift;

            if (! User.DoesHavePermission("Change status of Forklift"))
            {
                status.IsEnabled = false;
            }

            string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
            if (Directory.Exists(targetDirectory))
            {
                string imageFileName = "ForkliftPicture.png";
                string imagePath = Path.Combine(targetDirectory, imageFileName);
                if (File.Exists(imagePath))
                {
                    string fileName = Path.GetFileName(imagePath);
                    string targetFilePath = Path.Combine(targetDirectory, fileName);

                    BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));
                    ForkliftImage.Source = bitmap;
                }
            }
        }

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
        private void IniStatuses()
        {
            status.Items.Clear();

            status.Items.Add("Free");
            status.Items.Add("On duty");
            status.Items.Add("Under Maintenance");
            status.Items.Add("Faulty");
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            DataRow forklift = (sender as Button).Tag as DataRow;
            if (forklift != null)
            {
                if (! Validation.ValidateTextbox(type, forklift) && ! Validation.ValidateTextbox(operating_hours, forklift) && status.SelectedIndex != -1 && warehouse_id.SelectedIndex != -1)
                {
                    forklift["warehouse_id"] = Warehouses[warehouse_id.SelectedItem.ToString()]["id"];
                    forklift["type"] = type.Text;
                    forklift["status"] = status.SelectedItem;
                    forklift["operating_hours"] = operating_hours.Text;

                    Tables.forklifts.updateChanges();
                    Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has been modified {forklift["type"]}[{forklift["id"]}] forklift.");

                    MessageBox.Show("Product updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    Navigation.OpenPage(Navigation.PreviousPage.GetType());
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.PreviousPage.GetType());
        }

        private void EditForkliftPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
            ForkliftImage.Width = ForkliftImage.Height;
        }
    }
}
