using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WH_APP_GUI.Employee;
using WH_APP_GUI.Staff;

namespace WH_APP_GUI
{
    abstract class FeatureCheckbox : CheckBox
    {
        public string FeatureNameInDataBase { get; set; }
        public bool CanBeCreated { get; set; }
        public FeatureCheckbox(string FeatureName)
        {
            this.FeatureNameInDataBase = FeatureName;
            Update();
        }
        public virtual void Update()
        {
            if (SQL.Tables().Contains("feature"))
            {
                if (bool.Parse(SQL.FindOneDataFromQuery($"SELECT in_use FROM feature WHERE name = '{this.FeatureNameInDataBase}'")))
                {
                    this.IsChecked = true;
                    CanBeCreated = false;
                }
                else
                {
                    this.CanBeCreated = true;
                }
            }
            else
            {
                this.IsEnabled = false;
            }
        }
        abstract public void ActivateFeature();
        abstract public void DisableFeature();
    }
    #region Feature Classes
    class DateLogFeature : FeatureCheckbox
    {
        public DateLogFeature(string FeatureName) : base(FeatureName) { }
        public override void ActivateFeature()
        {
            if (this.IsChecked == true)
            {
                Controller.DateLog();
                this.CanBeCreated = false;
            }
        }
        public override void DisableFeature()
        {
            if (this.IsChecked != true)
            {
                Controller.DateLogOff();
                this.CanBeCreated = true;
            }
        }
    }

    class FleetFeature : FeatureCheckbox
    {
        public FleetFeature(string FeatureName) : base(FeatureName) { }

        public override void ActivateFeature()
        {
            if (this.IsChecked == true)
            {
                Controller.Fleet();
                this.CanBeCreated = false;
            }
        }
        public override void DisableFeature()
        {
            if (this.IsChecked != true)
            {
                Controller.FleetOff();
                this.CanBeCreated = true;
            }
        }
    }

    class CityFeature : FeatureCheckbox
    {
        public CityFeature(string FeatureName) : base(FeatureName) { }

        public override void ActivateFeature()
        {
            if (this.IsChecked == true)
            {
                Controller.City();
                this.CanBeCreated = false;
                Tables.warehouses.Refresh();
                Tables.cities.Refresh();
            }
        }

        public override void DisableFeature()
        {
            if (this.IsChecked != true)
            {
                Controller.CityOff();
                this.CanBeCreated = true;
            }
        }
    }
    class LogFeature : FeatureCheckbox
    {
        public LogFeature(string FeatureName) : base(FeatureName) { }

        public override void ActivateFeature()
        {
            if (this.IsChecked == true)
            {
                Controller.Log();
                this.CanBeCreated = false;
            }
        }

        public override void DisableFeature()
        {
            if (this.IsChecked != true)
            {
                Controller.LogOff();
                this.CanBeCreated = true;
            }
        }
    }
    class ActivityFeature : FeatureCheckbox
    {
        public ActivityFeature(string FeatureName) : base(FeatureName) { }

        public override void ActivateFeature()
        {
            if (this.IsChecked == true)
            {
                Controller.Activity();
                this.CanBeCreated = false;
            }
        }

        public override void DisableFeature()
        {
            if (this.IsChecked != true)
            {
                Controller.ActivityOff();
                this.CanBeCreated = true;
            }
        }
    }

    class RevenueFeature : FeatureCheckbox
    {
        public RevenueFeature(string FeatureName) : base(FeatureName) { }

        public override void ActivateFeature()
        {
            if (this.IsChecked == true)
            {
                Controller.Revenue();
                this.CanBeCreated = false;
            }
        }

        public override void DisableFeature()
        {
            if (this.IsChecked != true)
            {
                Controller.RevenueOff();
                this.CanBeCreated = true;
            }
        }
    }

    class StorageFeature : FeatureCheckbox
    {
        public StorageFeature(string FeatureName) : base(FeatureName) { }

        public override void ActivateFeature()
        {
            if (this.IsChecked == true)
            {
                Controller.Storage();
                this.CanBeCreated = false;
            }
        }

        public override void DisableFeature()
        {
            if (this.IsChecked != true)
            {
                Controller.StorageOff();
                this.CanBeCreated = true;
            }
        }

        public override void Update()
        {
            if (SQL.Tables().Contains("feature"))
            {
                if (bool.Parse(SQL.FindOneDataFromQuery($"SELECT in_use FROM feature WHERE name = '{this.FeatureNameInDataBase}'")))
                {
                    this.IsChecked = true;
                    CanBeCreated = false;
                }
                else if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Fleet';"))
                {
                    CanBeCreated = true;
                }
                else
                {
                    this.IsChecked = false;
                    this.CanBeCreated = true;
                    this.IsEnabled = false;
                }
            }
            else
            {
                this.IsEnabled = false;
            }
        }
    }

    class FuelFeature : FeatureCheckbox
    {
        public FuelFeature(string FeatureName) : base(FeatureName) { }

        public override void ActivateFeature()
        {
            if (this.IsChecked == true)
            {
                Controller.Fuel();
                this.CanBeCreated = false;
            }
        }
        public override void DisableFeature()
        {
            if (this.IsChecked != true)
            {
                Controller.FuelOff();
                this.CanBeCreated = true;
            }
        }
        public override void Update()
        {
            if (SQL.Tables().Contains("feature"))
            {
                if (bool.Parse(SQL.FindOneDataFromQuery($"SELECT in_use FROM feature WHERE name = '{this.FeatureNameInDataBase}'")))
                {
                    this.IsChecked = true;
                    CanBeCreated = false;
                }
                else if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Fleet';"))
                {
                    CanBeCreated = true;
                }
                else
                {
                    this.IsChecked = false;
                    this.CanBeCreated = true;
                    this.IsEnabled = false;
                }
            }
            else
            {
                this.IsEnabled = false;
            }
        }
    }

    class DockFeature : FeatureCheckbox
    {
        public DockFeature(string FeatureName) : base(FeatureName) { }

        public override void ActivateFeature()
        {
            if (this.IsChecked == true)
            {
                Controller.Dock();
                this.CanBeCreated = false;
            }
        }
        public override void DisableFeature()
        {
            if (this.IsChecked != true)
            {
                Controller.DockOff();
                this.CanBeCreated = true;
            }
        }
        public override void Update()
        {
            if (SQL.Tables().Contains("feature"))
            {
                if (bool.Parse(SQL.FindOneDataFromQuery($"SELECT in_use FROM feature WHERE name = '{this.FeatureNameInDataBase}'")))
                {
                    this.IsChecked = true;
                    CanBeCreated = false;
                }
                else if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Fleet';"))
                {
                    CanBeCreated = true;
                }
                else
                {
                    this.IsChecked = false;
                    this.CanBeCreated = true;
                    this.IsEnabled = false;
                }
            }
            else
            {
                this.IsEnabled = false;
            }
        }
    }

    class ForkliftFeature : FeatureCheckbox
    {
        public ForkliftFeature(string FeatureName) : base(FeatureName) { }
        public override void ActivateFeature()
        {
            if (this.IsChecked == true)
            {
                Controller.Forklift();
                this.CanBeCreated = false;
            }
        }
        public override void DisableFeature()
        {
            if (this.IsChecked != true)
            {
                Controller.ForkliftOff();
                this.CanBeCreated = true;
            }
        }
    }
    #endregion
    public partial class AdminHomePage : Page
    {
        public static string SAdminName = string.Empty;
        public static string SAdminEmail = string.Empty;
        public static string SAdminPassword = string.Empty;
        public AdminHomePage(string AdminName, string AdminEmail, string AdminPassword)
        {
            InitializeComponent();

            SAdminName = AdminName;
            SAdminEmail = AdminEmail;
            SAdminPassword = AdminPassword;
            ToTheApp.Visibility = Visibility.Visible;

            try
            {
                if (Controller.IsMigrationContainsAllDefaultTables())
                {
                    CreateRequiredTablesBTN.IsEnabled = false;
                    CreateCheckBoxes(FeaturesDisplayG);
                }
                else
                {
                    RegisterEmployee.IsEnabled = false;
                    ImportEmployees.IsEnabled = false;
                    manageRoles.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid database datas!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteError(ex);
                throw;
            }
        }
        public AdminHomePage()
        {
            InitializeComponent();
            ToTheApp.Visibility = Visibility.Collapsed;

            try
            {
                if (Controller.IsMigrationContainsAllDefaultTables())
                {
                    CreateRequiredTablesBTN.IsEnabled = false;
                    CreateCheckBoxes(FeaturesDisplayG);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid database datas!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteError(ex);
                throw;
            }

        }
        public static void CreateCheckBoxes(Panel Display)
        {
            Display.Children.Clear();
            #region Checkboxes
            DateLogFeature DateLogCBXexe = new DateLogFeature("Date Log");
            DateLogCBXexe.Content = "Include DateLog feature";
            Display.Children.Add(DateLogCBXexe);

            FleetFeature FleetCBXexe = new FleetFeature("Fleet");
            FleetCBXexe.Content = "Include Fleet feature";
            Display.Children.Add(FleetCBXexe);

            CityFeature CityCBXexe = new CityFeature("City");
            CityCBXexe.Content = "Include City feature";
            Display.Children.Add(CityCBXexe);

            LogFeature LogCBXexe = new LogFeature("Log");
            LogCBXexe.Content = "Include Log feature";
            Display.Children.Add(LogCBXexe);

            ActivityFeature ActivityCBXexe = new ActivityFeature("Activity");
            ActivityCBXexe.Content = "Include Activity feature";
            Display.Children.Add(ActivityCBXexe);

            RevenueFeature RevenueCBXexe = new RevenueFeature("Revenue");
            RevenueCBXexe.Content = "Include Revenue feature";
            Display.Children.Add(RevenueCBXexe);

            StorageFeature StorageCBXexe = new StorageFeature("Storage");
            StorageCBXexe.Content = "Include Storage feature";
            Display.Children.Add(StorageCBXexe);

            FuelFeature FuelCBXexe = new FuelFeature("Fuel");
            FuelCBXexe.Content = "Include Fuel feature";
            Display.Children.Add(FuelCBXexe);

            DockFeature DockCBXexe = new DockFeature("Dock");
            DockCBXexe.Content = "Include Dock feature";
            Display.Children.Add(DockCBXexe);

            ForkliftFeature ForkliftCBXexe = new ForkliftFeature("Forklift");
            ForkliftCBXexe.Content = "Include Forklift feature";
            Display.Children.Add(ForkliftCBXexe);
            #endregion
        }

        public static Dictionary<string, int> Role_Id;
        public void CloseBeforeOpen()
        {
            foreach (UIElement panel in Display.Children)
            {
                panel.Visibility = Visibility.Collapsed;
            }
        }
        private void CreateRequiredTablesBTN_Click(object sender, RoutedEventArgs e)
        {
            RegisterEmployee.IsEnabled = true;
            ImportEmployees.IsEnabled = true;
            manageRoles.IsEnabled = true;

            Controller.CreateMigration();
            Controller.CreateFeature();

            /*Required*/
            Controller.CreateDefaultTablesWithMigrationInsert();

            if (SQL.Tables().Contains("migrations") && SQL.Tables().Contains(Tables.staff.actual_name))
            {
                SQL.SqlCommand($"INSERT INTO `{Tables.staff.actual_name}`(`name`, `email`, `password`, `role_id`) VALUES ('{SAdminName}', '{SAdminEmail}', '{SAdminPassword}', 1)");
                MessageBox.Show("You have been registered as an Admin", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                User.SetCurrentUser(SAdminEmail, SAdminPassword);
            }

            if (Controller.IsMigrationContainsAllDefaultTables())
            {
                CreateRequiredTablesBTN.IsEnabled = false;
                CreateCheckBoxes(FeaturesDisplayG);
            }

            MessageBox.Show("Requierd tables and Feature tables created and filled with the datas.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void IncludeFeture_Click(object sender, RoutedEventArgs e)
        {
            string message = string.Empty;
            foreach (FeatureCheckbox cbx in FeaturesDisplayG.Children)
            {
                if (cbx.CanBeCreated)
                {
                    if (cbx.IsChecked == true)
                    {
                        message += cbx.FeatureNameInDataBase + ", ";
                        cbx.ActivateFeature();
                        cbx.Update();
                    }
                }
                else
                {
                    cbx.DisableFeature();
                    cbx.Update();
                }
            }

            Tables.features.updateChanges();
            CreateCheckBoxes(FeaturesDisplayG);

            if (message.Length > 0)
            {
                message = message.Substring(0, message.Length - 2);
                MessageBox.Show("This features has been created successfully: " + message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("The actions has been successfully completed", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            if (! FirstOpen())
            {
                MainWindow currentWindow = Window.GetWindow(this) as MainWindow;
                Home newHome = new Home();
                currentWindow.content.Navigate(newHome);
            }
        }

        private void RegisterEmployee_Click(object sender, RoutedEventArgs e)
        {
            CreateEmployee createEmployee = new CreateEmployee(new AdminHomePage());
            AdminContent.Content = null;
            AdminContent.Navigate(createEmployee);
            AdminContent.Visibility = Visibility.Visible;
        }

        private void RegisterStaff_Click(object sender, RoutedEventArgs e)
        {
            CreateStaffPage createStaffPage = new CreateStaffPage(new AdminHomePage());
            AdminContent.Content = null;
            AdminContent.Navigate(createStaffPage);
            AdminContent.Visibility = Visibility.Visible;
        }
        private void ImportEmployees_Click(object sender, RoutedEventArgs e)
        {
            CloseBeforeOpen();
            MessageBox.Show("Work in progress", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RegisterEmployeeWithDatas_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeStatus.SelectedIndex != -1 && EmployeeRole.SelectedIndex != -1 && EmployeeName.Text != string.Empty && EmployeeEmail.Text != string.Empty)
            {
                if (Tables.staff.database.Select($"email = '{EmployeeEmail.Text}'").Length == 0 && Tables.employees.database.Select($"email = '{EmployeeEmail.Text}'").Length == 0)
                {
                    string password = Hash.GenerateRandomPassword(); //TODO: Ez kell majd az emailbe
                    string HashedPassword = Hash.HashPassword(password);
                    /*DEBUG*/
                    MessageBox.Show("Admin Page: " + password);
                    MessageBox.Show("Admin Page: " + HashedPassword);
                    /*DEBUG*/

                    if (EmployeeStatus.SelectedIndex == 0)
                    {
                        SQL.SqlCommand($"INSERT INTO `{Tables.staff.actual_name}`(`name`, `email`, `password`, `role_id`) VALUES ('{EmployeeName.Text}', '{EmployeeEmail.Text}', '{HashedPassword}', {Role_Id[EmployeeRole.SelectedItem.ToString()]})");
                    }
                    else if (EmployeeStatus.SelectedIndex == 1)
                    {
                        SQL.SqlCommand($"INSERT INTO `{Tables.employees.actual_name}`(`name`, `email`, `password`, `role_id`) VALUES ('{EmployeeName.Text}', '{EmployeeEmail.Text}', '{HashedPassword}' , {Role_Id[EmployeeRole.SelectedItem.ToString()]})");
                    }
                    MessageBox.Show("New employees has been added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    RegisterEmployeDatas.Visibility = Visibility.Collapsed;
                    EmployeeStatus.SelectedIndex = -1;
                    EmployeeName.Text = string.Empty;
                    EmployeeEmail.Text = string.Empty;
                    EmployeeRole.SelectedIndex = -1;
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

        private void ManageDatabase_Click(object sender, RoutedEventArgs e)
        {
            CloseBeforeOpen();

            RegisterEmployeDatas.Visibility = Visibility.Collapsed;
            EmployeeStatus.SelectedIndex = -1;
            EmployeeName.Text = string.Empty;
            EmployeeEmail.Text = string.Empty;
            EmployeeRole.SelectedIndex = -1;

            ManagedatabaseGrid.Visibility = Visibility.Visible;

            DataSourceFU.Text = SQL.datasource;
            portFU.Text = SQL.port.ToString();
            usernameFU.Text = SQL.username;
            passwrdFU.Text = SQL.password;
            DatabaseNameFU.Text = SQL.database;
        }

        private void ConfirmDBdatas_Click(object sender, RoutedEventArgs e)
        {
            if (DataSourceFU.Text != string.Empty && portFU.Text != string.Empty && usernameFU.Text != string.Empty && DatabaseNameFU.Text != string.Empty)
            {
                SQL.CreateDatabaseConnectionDatas(DataSourceFU.Text, int.Parse(portFU.Text), usernameFU.Text, passwrdFU.Text, DatabaseNameFU.Text);
                SQL.FillStaticDatabaseValues();
                ManagedatabaseGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Empty input field", "Missing data", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EmployeeStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EmployeeRole.IsEnabled = true;

            if (EmployeeStatus.SelectedIndex == 0)
            {
                EmployeeRole.Items.Clear();
                for (int i = 0; i < Tables.roles.database.Rows.Count; i++)
                {
                    if (! (bool)Tables.roles.database.Rows[i]["in_warehouse"])
                    {
                        EmployeeRole.Items.Add(Tables.roles.database.Rows[i]["role"]);
                    }
                }
            }
            else if (EmployeeStatus.SelectedIndex == 1)
            {
                EmployeeRole.Items.Clear();
                for (int i = 0; i < Tables.roles.database.Rows.Count; i++)
                {
                    if ((bool)Tables.roles.database.Rows[i]["in_warehouse"])
                    {
                        EmployeeRole.Items.Add(Tables.roles.database.Rows[i]["role"]);
                    }
                }
            }
        }
        private void AdminHome_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.02;

            }
            foreach (var child in FeaturesDisplayG.Children)
            {
                FontSize = e.NewSize.Height * 0.03;

            }

            RegisterEmployee.FontSize = e.NewSize.Height * 0.02;
            RegisterStaff.FontSize = e.NewSize.Height * 0.02;
            ImportEmployees.FontSize = e.NewSize.Height * 0.02;
            ManageDatabase.FontSize = e.NewSize.Height * 0.02;
            manageRoles.FontSize = e.NewSize.Height * 0.02;
            ToTheApp.FontSize = e.NewSize.Height * 0.02;
            AddRole.FontSize = e.NewSize.Height * 0.02;
            CancelRoles.FontSize = e.NewSize.Height * 0.02;
            AddNewRoleBTN.FontSize = e.NewSize.Height * 0.02;
            CancelRoleCreation.FontSize = e.NewSize.Height * 0.02;
            CreateRequiredTablesBTN.FontSize = e.NewSize.Height * 0.02;
            IncludeFeture.FontSize = e.NewSize.Height * 0.02;
            
        }

        private static DataRow SelectedRole = null;
        private void DisplayRoles(Panel panel)
        {
            ManageRoleGrid.Visibility = Visibility.Visible;
            RolesScrollViewer.Visibility = Visibility.Visible;
            PermmissionsScrollViewer.Visibility = Visibility.Visible;
            panel.Visibility = Visibility.Visible;
            panel.Children.Clear();
            for (int i = 0; i < Tables.roles.database.Rows.Count; i++)
            {
                Button btn = new Button();
                btn.Content = Tables.roles.database.Rows[i]["role"];
                btn.Tag = Tables.roles.database.Rows[i];

                btn.Background = Brushes.White;
                btn.Foreground = Brushes.Black;
                btn.BorderThickness = new Thickness(1);
                btn.BorderBrush = Brushes.Black;
                btn.Click += RoleButton_Click;
                btn.Margin = new Thickness(5);
                panel.Children.Add(btn);
            }
        }
        private void manageRoles_Click(object sender, RoutedEventArgs e)
        {
            CloseBeforeOpen();
            DisplayRoles(RolesStackpanel);
        }

        private void PermissionDisplayToRole(DataRow role)
        {
            if (role != null)
            {
                PermissonsStackpanel.Children.Clear();
                for (int i = 0; i < Tables.permissions.database.Rows.Count; i++)
                {
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Horizontal;
                    stackPanel.HorizontalAlignment = HorizontalAlignment.Right;
                    stackPanel.VerticalAlignment = VerticalAlignment.Top;

                    Label content = new Label();
                    content.Content = Tables.permissions.database.Rows[i]["name"];
                    content.Background = Brushes.White;
                    content.Foreground = Brushes.Black;
                    content.BorderThickness = new Thickness(1);
                    content.HorizontalAlignment = HorizontalAlignment.Left;
                    content.Margin = new Thickness(5);
                    stackPanel.Children.Add(content);

                    if (Tables.roles.getPermission(role).Contains(Tables.permissions.database.Rows[i]))
                    {
                        Button on = new Button();
                        on.Tag = Tables.permissions.database.Rows[i];
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
                        off.Tag = Tables.permissions.database.Rows[i];
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

                    PermissonsStackpanel.Children.Add(stackPanel);
                }
            }
        }

        private void RoleButton_Click(object sender, RoutedEventArgs e)
        {
            PermissonsStackpanel.Children.Clear();
            Button clickedButton = sender as Button;
            DataRow role = clickedButton.Tag as DataRow;
            SelectedRole = role;

            Button deleteRole = new Button();
            deleteRole.Content = $"Delete {role["role"]}";
            deleteRole.Click += DeleteRole_Click;
            PermissonsStackpanel.Children.Add(deleteRole);;

            for (int i = 0; i < Tables.permissions.database.Rows.Count; i++)
            {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.HorizontalAlignment = HorizontalAlignment.Right;
                stackPanel.VerticalAlignment = VerticalAlignment.Top;

                Label content = new Label();
                content.Content = Tables.permissions.database.Rows[i]["name"];
                content.Background = Brushes.White;
                content.Foreground = Brushes.Black;
                content.BorderThickness = new Thickness(1);
                content.HorizontalAlignment = HorizontalAlignment.Left;
                content.Margin = new Thickness(5);
                stackPanel.Children.Add(content);

                if (Tables.roles.getPermission(role).Contains(Tables.permissions.database.Rows[i]))
                {
                    Button on = new Button();
                    on.Tag = Tables.permissions.database.Rows[i];
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
                    off.Tag = Tables.permissions.database.Rows[i];
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

                PermissonsStackpanel.Children.Add(stackPanel);
            }
        }

        private void btnOff_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRole != null)
            {
                Button clickedButton = sender as Button;
                DataRow permission = clickedButton.Tag as DataRow;
                try
                {
                    SQL.SqlCommand($"DELETE FROM `role_permission` WHERE `role_id` = '{SelectedRole["id"]}' AND `permission_id` = '{permission["id"]}'");

                    Tables.roles.Refresh();
                    Tables.permissions.Refresh();

                    MessageBox.Show("Status updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    PermissionDisplayToRole(SelectedRole);
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }

        private void DeleteRole_Click(object sender, RoutedEventArgs e)
        {
            //UPDATE `staff` SET `role_id`= null WHERE `role_id`= 13;
            if (SelectedRole != null)
            {
                try
                {
                    if ((bool)SelectedRole["in_warehouse"])
                    {
                        foreach (DataRow employee in Tables.employees.database.Select($"role_id = {SelectedRole["id"]}"))
                        {
                            employee["role_id"] = null;
                        }
                        Tables.employees.updateChanges();

                        SelectedRole.Delete();

                        SelectedRole = null;
                        Tables.roles.updateChanges();
                        DisplayRoles(RolesStackpanel);

                        MessageBox.Show("Role successfully deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        foreach (DataRow staff in Tables.staff.database.Select($"role_id = {SelectedRole["id"]}"))
                        {
                            staff["role_id"] = null;
                        }
                        Tables.staff.Refresh();

                        SelectedRole.Delete();

                        SelectedRole = null;
                        Tables.roles.updateChanges();
                        DisplayRoles(RolesStackpanel);

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

        private void btnOn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRole != null)
            {
                Button clickedButton = sender as Button;
                DataRow permission = clickedButton.Tag as DataRow;
                try
                {
                    SQL.SqlCommand($"INSERT INTO `role_permission`(`role_id`, `permission_id`) VALUES ('{SelectedRole["id"]}','{permission["id"]}')");

                    Tables.roles.Refresh();
                    Tables.permissions.Refresh();

                    MessageBox.Show("Status updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    PermissionDisplayToRole(SelectedRole);
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }

        private void CancelRoles_Click(object sender, RoutedEventArgs e)
        {
            SelectedRole = null;
            ManageRoleGrid.Visibility = Visibility.Collapsed;
            RolesScrollViewer.Visibility = Visibility.Collapsed;
            PermmissionsScrollViewer.Visibility = Visibility.Collapsed;
        }

        private void AddRole_Click(object sender, RoutedEventArgs e)
        {
            RolesScrollViewer.Visibility = Visibility.Collapsed;
            PermmissionsScrollViewer.Visibility = Visibility.Collapsed;
            AddRoleGrid.Visibility = Visibility.Visible;
        }

        private void AddNewRoleBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NewRoleName.Text != string.Empty && NewRoleDescription.Text != string.Empty)
                {
                    SQL.SqlCommand($"INSERT INTO `{Tables.roles.actual_name}`(`role`, `in_warehouse`, `description`) VALUES ('{NewRoleName.Text}', {Is_Belongst_To_Warehouse.IsChecked}, '{NewRoleDescription.Text}');");
                    Tables.roles.Refresh();
                    CancelRoleCreationM();
                    DisplayRoles(RolesStackpanel);
                    MessageBox.Show("Role hase been added, now you can set the permmsions to it!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Missing datas!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
                throw;
            }

            RolesScrollViewer.Visibility = Visibility.Visible;
            PermmissionsScrollViewer.Visibility = Visibility.Visible;
        }

        private void CancelRoleCreationM()
        {
            AddRoleGrid.Visibility = Visibility.Collapsed;
            NewRoleName.Text = string.Empty;
            NewRoleDescription.Text = string.Empty;
            Is_Belongst_To_Warehouse.IsChecked = false;

            RolesScrollViewer.Visibility = Visibility.Collapsed;
            PermmissionsScrollViewer.Visibility = Visibility.Collapsed;
        }

        private void CancelRoleCreation_Click(object sender, RoutedEventArgs e)
        {
            CloseBeforeOpen();
            CancelRoleCreationM();
        }

        private void ToTheApp_Click(object sender, RoutedEventArgs e)
        {
            SAdminName = string.Empty;
            SAdminEmail = string.Empty;
            SAdminPassword = string.Empty;

            MainWindow currentWindow = Window.GetWindow(this) as MainWindow;
            Home newHome = new Home();
            currentWindow.content.Navigate(newHome);
        }

        private bool FirstOpen()
        {
            if (SAdminName != string.Empty && SAdminEmail != string.Empty && SAdminPassword != string.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}