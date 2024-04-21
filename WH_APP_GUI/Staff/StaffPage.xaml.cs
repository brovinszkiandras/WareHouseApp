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
using WH_APP_GUI.Employee;

namespace WH_APP_GUI.Staff
{
    public partial class StaffPage : Page
    {
        public StaffPage()
        {
            InitializeComponent();
            DisplayStaffsStackpanel.Children.Clear();
            InitializeAllStaffs(DisplayStaffsStackpanel);
            Ini_role_id();
            Back.Visibility = Visibility.Collapsed;
        }
        private DataRow WarehouseFromPage;
        public StaffPage(DataRow warehouseFromPage)
        {
            InitializeComponent();
            DisplayStaffsStackpanel.Children.Clear();
            InitializeAllStaffs(DisplayStaffsStackpanel);
            Ini_role_id();
            Back.Visibility = Visibility.Collapsed;
            WarehouseFromPage = warehouseFromPage;
        }
        private static Type PreviousPageType;
        public StaffPage(Page previousPage)
        {
            InitializeComponent();
            DisplayStaffsStackpanel.Children.Clear();
            InitializeAllStaffs(DisplayStaffsStackpanel);
            Ini_role_id();
            Back.Visibility = Visibility.Visible;

            PreviousPageType = previousPage.GetType();
        }

        private Dictionary<string, DataRow> role_id_Dictionary = new Dictionary<string, DataRow>();
        private void Ini_role_id()
        {
            role_id_Dictionary.Clear();
            StaffRoles.Items.Clear();

            foreach (DataRow role in Tables.roles.database.Rows)
            {
                if (! (bool)role["in_warehouse"])
                {
                    role_id_Dictionary.Add(role["role"].ToString(), role);
                    StaffRoles.Items.Add(role["role"].ToString());
                }
            }
        }
        public void InitializeAllStaffs(Panel panel)
        {
            panel.Children.Clear();
            StaffsDisplay.Visibility = Visibility.Visible;
            panel.Visibility = Visibility.Visible;
            Label staffLabel = new Label();
            staffLabel.Content = "Staffs:";
            staffLabel.BorderBrush = Brushes.Black;
            staffLabel.BorderThickness = new Thickness(0, 0, 0, 1);
            panel.Children.Add(staffLabel);

            foreach (DataRow role in Tables.roles.database.Rows)
            {
                if (!(bool)role["in_warehouse"])
                {
                    InitializeStaffsByRole(panel, role);
                }
            }
        }
        public void InitializeStaffsByRole(Panel panel, DataRow role)
        {
            if (Tables.roles.getStaff(role).Length != 0)
            {
                StaffsDisplay.Visibility = Visibility.Visible;
                panel.Visibility = Visibility.Visible;

                Label stafflabel = new Label();
                stafflabel.Content = role["role"] + ":";
                stafflabel.BorderBrush = Brushes.Black;
                stafflabel.BorderThickness = new Thickness(0, 0, 0, 1);
                panel.Children.Add(stafflabel);

                for (int i = 0; i < Tables.roles.getStaff(role).Length; i++)
                {
                    StackPanel mainStackPanel = new StackPanel();
                    mainStackPanel.Height = 100;
                    mainStackPanel.Orientation = Orientation.Horizontal;

                    Image image = new Image();
                    image.Width = 100;
                    image.Height = 100;
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.SetValue(Grid.RowSpanProperty, 3);

                    string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
                    if (Directory.Exists(targetDirectory))
                    {
                        string imageFileName = Tables.roles.getStaff(role)[i]["profile_picture"].ToString();
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
                    nameLabel.Content = "Name: " + Tables.roles.getStaff(role)[i]["name"];
                    nameLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                    nameLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                    Label emailLabel = new Label();
                    emailLabel.Content = "Email: " + Tables.roles.getStaff(role)[i]["email"];
                    emailLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                    emailLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                    Label roleLabel = new Label();
                    roleLabel.Content = "Role: " + role["role"];
                    roleLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                    roleLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                    leftStackPanel.Children.Add(nameLabel);
                    leftStackPanel.Children.Add(emailLabel);
                    leftStackPanel.Children.Add(roleLabel);

                    StackPanel rightStackPanel = new StackPanel();
                    rightStackPanel.Orientation = Orientation.Vertical;
                    rightStackPanel.Width = 130;

                    if (User.currentUser != Tables.roles.getStaff(role)[i])
                    {
                        Button deleteButton = new Button();
                        deleteButton.Content = "Delete";
                        deleteButton.Click += deleteStaff_Click;
                        deleteButton.Tag = Tables.roles.getStaff(role)[i];
                        rightStackPanel.Children.Add(deleteButton);
                    }

                    Button resetPasswordButton = new Button();
                    resetPasswordButton.Content = "Reset Password";
                    resetPasswordButton.Click += resetPassword_Click;
                    resetPasswordButton.Tag = Tables.roles.getStaff(role)[i];

                    Button editButton = new Button();
                    editButton.Content = "Edit Staff";
                    editButton.Click += editStaff_Click;
                    editButton.Tag = Tables.roles.getStaff(role)[i];

                    rightStackPanel.Children.Add(resetPasswordButton);
                    rightStackPanel.Children.Add(editButton);

                    mainStackPanel.Children.Add(image);
                    mainStackPanel.Children.Add(leftStackPanel);
                    mainStackPanel.Children.Add(rightStackPanel);

                    panel.Children.Add(mainStackPanel);
                }
            }
        }
        public Dictionary<string, int> Role_Id = new Dictionary<string, int>();
        private void AddNewStaff_Click(object sender, RoutedEventArgs e)
        {
            if (WarehouseFromPage != null)
            {
                Navigation.OpenPage(Navigation.GetTypeByName("CreateStaffPage"));
                Navigation.ReturnParam = WarehouseFromPage;
            }
            else
            {
                Navigation.OpenPage(Navigation.GetTypeByName("CreateStaffPage"));
            }
        }

        void CloseAndDisplay(object sender, EventArgs e)
        {
            InitializeAllStaffs(DisplayStaffsStackpanel);
        }

        private void deleteStaff_Click(object sender, RoutedEventArgs e)
        {
            DataRow staff = (sender as Button).Tag as DataRow;
            if (staff != null)
            {
                staff.Delete();
                Tables.staff.updateChanges();
                StaffRoles.SelectedIndex = -1;
                InitializeAllStaffs(DisplayStaffsStackpanel);

                MessageBox.Show("Staff deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void editStaff_Click(object sender, RoutedEventArgs e)
        {
            DataRow staff = (sender as Button).Tag as DataRow;
            if (staff != null)
            {
                if (WarehouseFromPage != null)
                {
                    Navigation.OpenPage(Navigation.GetTypeByName("EditStaffPage"), staff);
                    Navigation.ReturnParam = WarehouseFromPage;
                }
                else
                {
                    Navigation.OpenPage(Navigation.GetTypeByName("EditStaffPage"), staff);
                }
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
                Tables.staff.updateChanges();

                /*DEBUG*/
                MessageBox.Show("Staff Page at Password reset: " + password);
                MessageBox.Show("Staff Page at Password reste: " + HashedPassword);
                /*DEBUG*/

                MessageBox.Show("Password has been reseted for the employee!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            StaffsDisplay.Visibility = Visibility.Visible;
            DisplayStaffsStackpanel.Children.Clear();
            InitializeAllStaffs(DisplayStaffsStackpanel);
        }

        private void InsperctStaffs_Click(object sender, RoutedEventArgs e)
        {
            StaffsDisplay.Visibility = Visibility.Visible;
            DisplayStaffsStackpanel.Children.Clear();
            InitializeAllStaffs(DisplayStaffsStackpanel);
        }

        private void AllStaffs_Click(object sender, RoutedEventArgs e)
        {
            StaffRoles.SelectedIndex = -1;
            InitializeAllStaffs(DisplayStaffsStackpanel);
        }

        private void StaffRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StaffRoles.SelectedIndex != -1)
            {
                DisplayStaffsStackpanel.Children.Clear();
                InitializeStaffsByRole(DisplayStaffsStackpanel, Tables.roles.database.Select($"id = {role_id_Dictionary [StaffRoles.SelectedItem.ToString()]}")[0]);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (PreviousPageType != null)
            {
                Page previousPage = (Page)Activator.CreateInstance(PreviousPageType);
                Navigation.content2.Navigate(previousPage);
            }
        }
    }
}
