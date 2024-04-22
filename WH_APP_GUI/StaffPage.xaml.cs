using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;

namespace WH_APP_GUI
{
    public partial class StaffPage : Page
    {
        public StaffPage()
        {
            InitializeComponent();
            DisplayStaffsStackpanel.Children.Clear();
            InitializeAllStaffs(DisplayStaffsStackpanel);

            for (int i = 0; i < Tables.roles.database.Rows.Count; i++)
            {
                if (SQL.BoolQuery($"SELECT in_warehouse FROM {Tables.roles.actual_name} WHERE role = '{Tables.roles.database.Rows[i]["role"]}'"))
                {
                    Role_Id.Add(Tables.roles.database.Rows[i]["role"].ToString(), (int)Tables.roles.database.Rows[i]["id"]);
                    StaffRole.Items.Add(Tables.roles.database.Rows[i]["role"].ToString());
                    StaffRoles.Items.Add(Tables.roles.database.Rows[i]["role"].ToString());
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

            for (int i = 0; i < Tables.staff.database.Rows.Count; i++)
            {
                StackPanel mainStackPanel = new StackPanel();
                mainStackPanel.Height = 100;
                mainStackPanel.Orientation = Orientation.Horizontal;

                //Image image = new Image();
                //image.Width = 100;
                //image.Height = 100;
                //image.HorizontalAlignment = HorizontalAlignment.Left;
                //image.Source = 
                //image.SetValue(Grid.RowSpanProperty, 3);

                PackIconCodicons icon = new PackIconCodicons() { Kind = PackIconCodiconsKind.Account };
                icon.Height = 100;
                icon.Width = 100;
                icon.SetValue(Grid.RowSpanProperty, 3);

                StackPanel leftStackPanel = new StackPanel();
                leftStackPanel.Orientation = Orientation.Vertical;
                leftStackPanel.Width = 350;

                Label nameLabel = new Label();
                nameLabel.Content = "Name: " + Tables.staff.database.Rows[i]["name"];
                nameLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                nameLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                Label emailLabel = new Label();
                emailLabel.Content = "Email: " + Tables.staff.database.Rows[i]["email"];
                emailLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                emailLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                Label roleLabel = new Label();
                roleLabel.Content = "Role: " + Tables.staff.getRole(Tables.staff.database.Rows[i])["role"];
                roleLabel.BorderBrush = System.Windows.Media.Brushes.Black;
                roleLabel.BorderThickness = new Thickness(0, 0, 0, 1);

                leftStackPanel.Children.Add(nameLabel);
                leftStackPanel.Children.Add(emailLabel);
                leftStackPanel.Children.Add(roleLabel);

                StackPanel rightStackPanel = new StackPanel();
                rightStackPanel.Orientation = Orientation.Vertical;
                rightStackPanel.Width = 130;

                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.Click += deleteStaff_Click;
                deleteButton.Tag = Tables.staff.database.Rows[i];

                Button resetPasswordButton = new Button();
                resetPasswordButton.Content = "Reset Password";
                resetPasswordButton.Click += resetPassword_Click;
                resetPasswordButton.Tag = Tables.staff.database.Rows[i];

                rightStackPanel.Children.Add(deleteButton);
                rightStackPanel.Children.Add(resetPasswordButton);

                mainStackPanel.Children.Add(icon);
                mainStackPanel.Children.Add(leftStackPanel);
                mainStackPanel.Children.Add(rightStackPanel);

                panel.Children.Add(mainStackPanel);
            }
        }
        public void InitializeStaffsByRole(Panel panel, DataRow role)
        {
            panel.Children.Clear();
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

                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.Click += deleteStaff_Click;
                deleteButton.Tag = Tables.roles.getStaff(role)[i];

                Button resetPasswordButton = new Button();
                resetPasswordButton.Content = "Reset Password";
                resetPasswordButton.Click += resetPassword_Click;
                resetPasswordButton.Tag = Tables.roles.getStaff(role)[i];

                rightStackPanel.Children.Add(deleteButton);
                rightStackPanel.Children.Add(resetPasswordButton);

                mainStackPanel.Children.Add(image);
                mainStackPanel.Children.Add(leftStackPanel);
                mainStackPanel.Children.Add(rightStackPanel);

                panel.Children.Add(mainStackPanel);
            }
        }
        public static Dictionary<string, int> Role_Id = new Dictionary<string, int>();
        private void AddNewStaff_Click(object sender, RoutedEventArgs e)
        {
            StaffsDisplay.Visibility = Visibility.Collapsed;
            RegisterStaffDatas.Visibility = Visibility.Visible;
        }

        private void deleteStaff_Click(object sender, RoutedEventArgs e)
        {
            DataRow employee = (sender as Button).Tag as DataRow;
            if (employee != null)
            {
                employee.Delete();
                Tables.staff.updateChanges();
                StaffRoles.SelectedIndex = -1;
                InitializeAllStaffs(DisplayStaffsStackpanel);

                MessageBox.Show("Staff deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
        private void RegisterStaffWithDatas_Click(object sender, RoutedEventArgs e)
        {
            if (StaffRole.SelectedIndex != -1 && StaffName.Text != string.Empty && StaffEmail.Text != string.Empty)
            {
                if (Tables.staff.database.Select($"email = '{StaffEmail.Text}'").Length == 0 && Tables.employees.database.Select($"email = '{StaffEmail.Text}'").Length == 0)
                {
                    string password = Hash.GenerateRandomPassword(); //TODO: Ez kell majd az emailbe
                    string HashedPassword = Hash.HashPassword(password);
                    /*DEBUG*/
                    MessageBox.Show("Staffs Page: " + password);
                    MessageBox.Show("Staffs Page: " + HashedPassword);
                    /*DEBUG*/

                    SQL.SqlCommand($"INSERT INTO `{Tables.staff.actual_name}`(`name`, `email`, `password`, `role_id`) VALUES ('{StaffName.Text}', '{StaffEmail.Text}', '{HashedPassword}', {Role_Id[StaffRole.SelectedItem.ToString()]});");
                    Tables.staff.Refresh();

                    MessageBox.Show("New staffs has been added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    StaffsDisplay.Visibility = Visibility.Visible;
                    DisplayStaffsStackpanel.Children.Clear();
                    InitializeAllStaffs(DisplayStaffsStackpanel);

                    CancelF();
                }
                else
                {
                    MessageBox.Show("A person with this email alreday exist!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Empty input fileds!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void CancelF()
        {
            RegisterStaffDatas.Visibility = Visibility.Collapsed;
            StaffName.Text = string.Empty;
            StaffEmail.Text = string.Empty;
            StaffRole.SelectedIndex = -1;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CancelF();

            StaffsDisplay.Visibility = Visibility.Visible;
            DisplayStaffsStackpanel.Children.Clear();
            InitializeAllStaffs(DisplayStaffsStackpanel);
        }

        private void InsperctStaffs_Click(object sender, RoutedEventArgs e)
        {
            StaffsDisplay.Visibility = Visibility.Visible;
            DisplayStaffsStackpanel.Children.Clear();
            InitializeAllStaffs(DisplayStaffsStackpanel);

            CancelF();
        }

        private void AllStaffs_Click(object sender, RoutedEventArgs e)
        {
            StaffRoles.SelectedIndex = -1;
            CancelF();
            InitializeAllStaffs(DisplayStaffsStackpanel);
        }

        private void StaffRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StaffRoles.SelectedIndex != -1)
            {
                InitializeStaffsByRole(DisplayStaffsStackpanel, Tables.roles.database.Select($"id = {Role_Id[StaffRoles.SelectedItem.ToString()]}")[0]);
            }
        }
    }
}