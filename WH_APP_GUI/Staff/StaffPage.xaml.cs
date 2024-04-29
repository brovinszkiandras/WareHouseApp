using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
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
            staffLabel.Style = (Style)this.Resources["labelstyle"];
            panel.Children.Add(staffLabel);

            foreach (DataRow staff in Tables.staff.database.Rows)
            {
                DisplayOneStaff(panel, staff);
            }
        }

        private void DisplayOneStaff(Panel panel, DataRow staff)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.Background = new SolidColorBrush(Color.FromArgb(255, 0x39, 0x52, 0x50));
            border.CornerRadius = new CornerRadius(15);
            border.BorderThickness = new Thickness(2);
            border.Margin = new Thickness(5);

            Grid mainStackPanel = new Grid();
            mainStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            mainStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            mainStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

            Image image = new Image();
            image.Width = 100;
            image.Height = 100;
            image.Margin = new Thickness(5);

            Grid.SetColumn(image, 0);
            mainStackPanel.Children.Add(image);

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

                    image.Source = bitmap;
                }
                else
                {
                    imageFileName = "DefaultStaffProfilePicture.png";
                    imagePath = Path.Combine(targetDirectory, imageFileName);
                    string fileName = Path.GetFileName(imagePath);
                    string targetFilePath = Path.Combine(targetDirectory, fileName);

                    BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));

                    image.Source = bitmap;
                }
            }

            StackPanel leftStackPanel = new StackPanel();
            leftStackPanel.Orientation = Orientation.Vertical;

            Grid.SetColumn(leftStackPanel, 1);
            mainStackPanel.Children.Add(leftStackPanel);

            Label nameLabel = new Label();
            nameLabel.Content = "Name: " + staff["name"];
            nameLabel.Style = (Style)this.Resources["labelstyle"];

            Label emailLabel = new Label();
            emailLabel.Content = "Email: " + staff["email"];
            emailLabel.Style = (Style)this.Resources["labelstyle"];

            Label roleLabel = new Label();
            roleLabel.Style = (Style)this.Resources["labelstyle"];
            roleLabel.Content = "Role: " + Tables.staff.getRole(staff)["role"];

            if (staff["role_id"] != DBNull.Value)
            {
                roleLabel.Content = "Role: " + Tables.staff.getRole(staff)["role"];
            }
            else
            {
                roleLabel.Content = "This staff does not have a role";
            }

            leftStackPanel.Children.Add(nameLabel);
            leftStackPanel.Children.Add(emailLabel);
            leftStackPanel.Children.Add(roleLabel);

            StackPanel rightStackPanel = new StackPanel();
            rightStackPanel.Orientation = Orientation.Vertical;
            Grid.SetColumn(rightStackPanel, 2);
            mainStackPanel.Children.Add(rightStackPanel);

            if (User.currentUser != staff)
            {
                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.Click += deleteStaff_Click;
                deleteButton.Tag = staff;
                deleteButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                rightStackPanel.Children.Add(deleteButton);

                Button resetPasswordButton = new Button();
                resetPasswordButton.Content = "Reset Password";
                resetPasswordButton.Click += resetPassword_Click;
                resetPasswordButton.Tag = staff;
                resetPasswordButton.Style = (Style)this.Resources["GoldenButtonStyle"];
                rightStackPanel.Children.Add(resetPasswordButton);
            }
            else
            {
                Button changePassword = new Button();
                changePassword.Content = "Change Password";
                changePassword.Tag = staff;
                changePassword.Click += ModifyPassword;
                changePassword.Style = (Style)this.Resources["GoldenButtonStyle"];
                rightStackPanel.Children.Add(changePassword);
            }


            Button editButton = new Button();
            editButton.Content = "Edit Staff";
            editButton.Click += editStaff_Click;
            editButton.Tag = staff;
            editButton.Style = (Style)this.Resources["GoldenButtonStyle"];

            rightStackPanel.Children.Add(editButton);
            
            border.Child = mainStackPanel;
            panel.Children.Add(border);
        }

        public void InitializeStaffsByRole(Panel panel, DataRow role)
        {
            if (Tables.roles.getStaff(role).Length != 0)
            {
                StaffsDisplay.Visibility = Visibility.Visible;
                panel.Visibility = Visibility.Visible;
                panel.Children.Clear();

                Label stafflabel = new Label();
                stafflabel.Content = role["role"] + ":";
                stafflabel.Style = (Style)this.Resources["labelstyle"];
                panel.Children.Add(stafflabel);

                foreach (DataRow staff in Tables.roles.getStaff(role))
                {
                    DisplayOneStaff(panel, staff);
                }
            }
        }

        private void ModifyPassword(object sender, RoutedEventArgs e)
        {
            PasswordChangeForStaff passwordChangeForStaff = new PasswordChangeForStaff();
            passwordChangeForStaff.ShowDialog();
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
                InitializeStaffsByRole(DisplayStaffsStackpanel, Tables.roles.database.Select($"id = {role_id_Dictionary[StaffRoles.SelectedItem.ToString()]["id"]}")[0]);
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

        private void Staff_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
        }
    }
}
