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
using System.Windows.Navigation;

namespace WH_APP_GUI.Order
{
    public partial class AllOrdersPage : Page
    {
        public AllOrdersPage()
        {
            InitializeComponent();
            IniCities();
        }
        private void IniOrders(Panel panel)
        {
            foreach (DataRow order in Tables.warehouses.database.Rows)
            {
                //StackPanel mainStackPanel = new StackPanel();
                //mainStackPanel.MinHeight = 100;
                //mainStackPanel.Orientation = Orientation.Horizontal;

                //Image image = new Image();
                //image.Width = 80;
                //image.Height = 80;

                //string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
                //if (Directory.Exists(targetDirectory))
                //{
                //    string imageFileName = "OrdersDefaultImage.png";
                //    string imagePath = Path.Combine(targetDirectory, imageFileName);

                //    if (File.Exists(imagePath))
                //    {
                //        string fileName = Path.GetFileName(imagePath);
                //        string targetFilePath = Path.Combine(targetDirectory, fileName);

                //        BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));

                //        image.Source = bitmap;
                //    }
                //}

                //StackPanel leftStackPanel = new StackPanel();
                //leftStackPanel.Orientation = Orientation.Vertical;
                //leftStackPanel.Width = 350;

                //Label idLabel = new Label();
                //idLabel.Content = "Order ID: " + order["id"];
                //idLabel.BorderBrush = Brushes.Black;
                //idLabel.BorderThickness = new Thickness(0, 0, 0, 1);


                //Label orderDateLabel = new Label();
                //orderDateLabel.Content = order["order_date"];
                //orderDateLabel.BorderBrush = Brushes.Black;
                //orderDateLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                //Label addressAndName = new Label();
                //addressAndName.Content = $"{order["username"]} : {order["address"]}({Tables.orders.ge})";
                //addressAndName.BorderBrush = Brushes.Black;
                //addressAndName.BorderThickness = new Thickness(0, 0, 0, 1);

                ////foreach (DataRow product in Tables.orders.getProduct(order))
                ////{
                ////  GROUP BY
                ////}

                //leftStackPanel.Children.Add(idLabel);
                //leftStackPanel.Children.Add(orderDateLabel);
                //leftStackPanel.Children.Add(roleLabel);

                //StackPanel rightStackPanel = new StackPanel();
                //rightStackPanel.Orientation = Orientation.Vertical;
                //rightStackPanel.Width = 130;

                //Button editButton = new Button();
                //editButton.Content = "Edit";
                //editButton.Click += EditEmployee_Click;
                //editButton.Tag = employee;

                //Button resetPasswordButton = new Button();
                //resetPasswordButton.Content = "Reset Password";
                //resetPasswordButton.Click += resetPassword_Click;
                //resetPasswordButton.Tag = employee;

                //rightStackPanel.Children.Add(editButton);
                //rightStackPanel.Children.Add(resetPasswordButton);

                //mainStackPanel.Children.Add(image);
                //mainStackPanel.Children.Add(leftStackPanel);
                //mainStackPanel.Children.Add(rightStackPanel);

                //panel.Children.Add(mainStackPanel);
            }
        }
        private Dictionary<string, DataRow> Cities_Dictionary = new Dictionary<string, DataRow>();
        private void IniCities()
        {
            Cities_Dictionary.Clear();
            city_id.Items.Clear();
            foreach (DataRow warehouse in Tables.cities.database.Rows)
            {
                if (Tables.warehouses.getOrders(warehouse).Length != 0)
                {
                    Cities_Dictionary.Add(warehouse["city_name"].ToString(), warehouse);
                    city_id.Items.Add(warehouse["city_name"].ToString());
                }
            }
        }
    }
}
