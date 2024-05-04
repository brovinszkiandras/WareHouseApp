using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace WH_APP_GUI
{
    public partial class ManageRolePage : Page
    {
        public ManageRolePage()
        {
            InitializeComponent();
            DisplayAllRoles();
        }
        private DataRow role = null;
        private void DisplayOneRole(DataRow Role)
        {
            Button roleBTN = new Button();
            roleBTN.Tag = Role;
            roleBTN.Margin = new Thickness(5);
            roleBTN.Content = Role["role"];
            roleBTN.Click += RoleClick;

            Roles.Children.Add(roleBTN);
        }
        private void RoleClick(object sender, RoutedEventArgs e)
        {
            DataRow Role = (sender as Button).Tag as DataRow;
            if (Role != null)
            {
                role = Role;
                if (Buttons.Children.Count == 3)
                {
                    Buttons.Children.RemoveAt(2);
                }
                Permissions.Children.Clear();
                IsInWarehouse.Content = (bool)role["in_warehouse"] ? "This role belongs to a warehouse" : "This role does not belongs to a warehouse";
                IsInWarehouse.Visibility = Visibility.Visible;
                Description.Text = role["description"].ToString();
                DisplayPermissionsForRole();
            }
        }
        private void DisplayAllRoles()
        {
            Roles.Children.Clear();
            if (Buttons.Children.Count == 3)
            {
                Buttons.Children.RemoveAt(2);
            }

            Description.Text = string.Empty;
            IsInWarehouse.Content = string.Empty;
            IsInWarehouse.Visibility = Visibility.Collapsed;
            Permissions.Children.Clear();

            foreach (DataRow role in Tables.roles.database.Rows)
            {
                DisplayOneRole(role);
            }
        }
        private void DisplayOnePermission(DataRow permission)
        {
            Border border = new Border();
            border.Background = Brushes.White;
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(2);
            border.Margin = new Thickness(5);

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            StackPanel stackPanel = new StackPanel();
            Grid.SetColumn(stackPanel, 0);
            grid.Children.Add(stackPanel);

            Label label1 = new Label();
            label1.Content = permission["name"];
            stackPanel.Children.Add(label1);

            TextBlock description = new TextBlock();
            description.Text = permission["description"].ToString();
            description.TextWrapping = TextWrapping.Wrap;
            description.Margin = new Thickness(5);
            stackPanel.Children.Add(description);

            if (role != null)
            {
                if (Tables.roles.getPermission(role).Contains(permission))
                {
                    Button on = new Button();
                    on.Tag = permission;
                    on.Click += btnOff_Click;
                    on.Content = "On";
                    on.Background = Brushes.Green;
                    on.Foreground = Brushes.Black;
                    on.BorderThickness = new Thickness(1);
                    on.BorderBrush = Brushes.Black;
                    on.HorizontalAlignment = HorizontalAlignment.Right;
                    on.Margin = new Thickness(5);
                    stackPanel.Children.Add(on);
                }
                else
                {
                    Button off = new Button();
                    off.Tag = permission;
                    off.Click += btnOn_Click;
                    off.Content = "Off";
                    off.Background = Brushes.Red;
                    off.Foreground = Brushes.Black;
                    off.BorderThickness = new Thickness(1);
                    off.BorderBrush = Brushes.Black;
                    off.HorizontalAlignment = HorizontalAlignment.Right;
                    off.Margin = new Thickness(5);
                    stackPanel.Children.Add(off);
                }
            }        
            border.Child = grid;
            Permissions.Children.Add(border);
        }
        private void DisplayPermissionsForRole()
        {
            if (role != null)
            {
                Permissions.Children.Clear();
                if (Buttons.Children.Count == 3)
                {
                    Buttons.Children.RemoveAt(2);
                }

                Button deleteRole = new Button();
                deleteRole.Content = $"Delete \n{role["role"]}";
                deleteRole.Click += DeleteRole_Click;
                deleteRole.Margin = new Thickness(5);
                Buttons.Children.Add(deleteRole);

                foreach (DataRow permission in Tables.permissions.database.Rows)
                {
                    DisplayOnePermission(permission);
                }
            }
        }

        private void DeleteRole_Click(object sender, RoutedEventArgs e)
        {
            if (role != null)
            {
                try
                {
                    if ((bool)role["in_warehouse"])
                    {
                        foreach (DataRow employee in Tables.employees.database.Select($"role_id = {role["id"]}"))
                        {
                            employee["role_id"] = null;
                        }
                        Tables.employees.updateChanges();

                        role.Delete();
                        role = null;
                        Tables.roles.updateChanges();
                        DisplayAllRoles();
                        Permissions.Children.Clear();

                        MessageBox.Show("Role successfully deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        foreach (DataRow staff in Tables.staff.database.Select($"role_id = {role["id"]}"))
                        {
                            staff["role_id"] = null;
                        }
                        Tables.staff.Refresh();
                        role.Delete();
                        role = null;
                        Tables.roles.updateChanges();
                        DisplayAllRoles();
                        Permissions.Children.Clear();

                        MessageBox.Show("Role successfully deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }

        private void btnOff_Click(object sender, RoutedEventArgs e)
        {
            if (role != null)
            {
                Button clickedButton = sender as Button;
                DataRow permission = clickedButton.Tag as DataRow;
                try
                {
                    SQL.SqlCommand($"DELETE FROM `role_permission` WHERE `role_id` = '{role["id"]}' AND `permission_id` = '{permission["id"]}'");

                    Tables.roles.Refresh();
                    Tables.permissions.Refresh();
                    DisplayPermissionsForRole();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }

        private void btnOn_Click(object sender, RoutedEventArgs e)
        {
            if (role != null)
            {
                Button clickedButton = sender as Button;
                DataRow permission = clickedButton.Tag as DataRow;
                try
                {
                    SQL.SqlCommand($"INSERT INTO `role_permission`(`role_id`, `permission_id`) VALUES ('{role["id"]}','{permission["id"]}')");

                    Tables.roles.Refresh();
                    Tables.permissions.Refresh();
                    DisplayPermissionsForRole();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            AdminHomePage adminHomePage = new AdminHomePage();
            Navigation.content2.Navigate(adminHomePage);
        }

        private void AddNewRole_Click(object sender, RoutedEventArgs e)
        {
            CreateRoleWindow createRoleWindow = new CreateRoleWindow();
            createRoleWindow.ShowDialog();
            DisplayAllRoles();
        }
    }
}
