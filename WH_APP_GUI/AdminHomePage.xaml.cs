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
                if (Tables.databases.Tables.Count != 0)
                {
                    CreateRequiredTablesBTN.IsEnabled = false;
                    CreateCheckBoxes(FeaturesDisplayG);
                }
                else
                {
                    
                    RegisterEmployee.IsEnabled = false;
                    manageRoles.IsEnabled = false;
                    RegisterStaff.IsEnabled = false;
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
                if (Tables.databases.Tables.Count != 0)
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

            if (! User.DoesHavePermission("Access to Database"))
            {
                CreateRequiredTablesBTN.Visibility = Visibility.Collapsed;
                FeaturesDisplayG.Visibility = Visibility.Collapsed;
                ModifyDatabase.Visibility = Visibility.Collapsed;
            }
        }
        public static void CreateCheckBoxes(Panel Display)
        {
            Display.Children.Clear();
            #region Checkboxes
            DateLogFeature DateLogCBXexe = new DateLogFeature("Date Log");
            DateLogCBXexe.Content = "DateLog feature";
            DateLogCBXexe.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCEA2"));
            DateLogCBXexe.FontFamily = new FontFamily("Baskerville Old Face");
            Display.Children.Add(DateLogCBXexe);

            FleetFeature FleetCBXexe = new FleetFeature("Fleet");
            FleetCBXexe.Content = "Fleet feature";
            FleetCBXexe.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCEA2"));
            FleetCBXexe.FontFamily = new FontFamily("Baskerville Old Face");
            Display.Children.Add(FleetCBXexe);

            LogFeature LogCBXexe = new LogFeature("Log");
            LogCBXexe.Content = "Log feature";
            LogCBXexe.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCEA2"));
            LogCBXexe.FontFamily = new FontFamily("Baskerville Old Face");
            Display.Children.Add(LogCBXexe);

            ActivityFeature ActivityCBXexe = new ActivityFeature("Activity");
            ActivityCBXexe.Content = "Activity feature";
            ActivityCBXexe.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCEA2"));
            ActivityCBXexe.FontFamily = new FontFamily("Baskerville Old Face");
            Display.Children.Add(ActivityCBXexe);

            RevenueFeature RevenueCBXexe = new RevenueFeature("Revenue");
            RevenueCBXexe.Content = "Revenue feature";
            RevenueCBXexe.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCEA2"));
            RevenueCBXexe.FontFamily = new FontFamily("Baskerville Old Face");
            Display.Children.Add(RevenueCBXexe);

            StorageFeature StorageCBXexe = new StorageFeature("Storage");
            StorageCBXexe.Content = "Storage feature";
            StorageCBXexe.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCEA2"));
            StorageCBXexe.FontFamily = new FontFamily("Baskerville Old Face");
            Display.Children.Add(StorageCBXexe);

            FuelFeature FuelCBXexe = new FuelFeature("Fuel");
            FuelCBXexe.Content = "Fuel feature";
            FuelCBXexe.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCEA2"));
            FuelCBXexe.FontFamily = new FontFamily("Baskerville Old Face");
            Display.Children.Add(FuelCBXexe);

            DockFeature DockCBXexe = new DockFeature("Dock");
            DockCBXexe.Content = "Dock feature";
            DockCBXexe.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCEA2"));
            DockCBXexe.FontFamily = new FontFamily("Baskerville Old Face");
            Display.Children.Add(DockCBXexe);

            ForkliftFeature ForkliftCBXexe = new ForkliftFeature("Forklift");
            ForkliftCBXexe.Content = "Forklift feature";
            ForkliftCBXexe.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCEA2"));
            ForkliftCBXexe.FontFamily = new FontFamily("Baskerville Old Face");
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
            manageRoles.IsEnabled = true;

            Controller.CreateFeature();

            /*Required*/
            Controller.CreateDefaultTables();

            if (SQL.Tables().Contains(Tables.staff.actual_name))
            {
                SQL.SqlCommand($"INSERT INTO `{Tables.staff.actual_name}`(`name`, `email`, `password`, `role_id`) VALUES ('{SAdminName}', '{SAdminEmail}', '{SAdminPassword}', 1)");
                MessageBox.Show("You have been registered as an Admin", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                User.SetCurrentUser(SAdminEmail, SAdminPassword);
            }

            CreateRequiredTablesBTN.IsEnabled = false;
            CreateCheckBoxes(FeaturesDisplayG);

            MessageBox.Show("Requierd tables and Feature tables created and filled with the datas.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void ModifyDatabase_Click(object sender, RoutedEventArgs e)
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
            Navigation.OpenPage(Navigation.GetTypeByName("CreateEmployee"));
        }

        private void RegisterStaff_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("CreateStaffPage"));
        }

        private void ManageDatabase_Click(object sender, RoutedEventArgs e)
        {
            CloseBeforeOpen();

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

            RegisterEmployee.FontSize = e.NewSize.Height * 0.03;
            RegisterStaff.FontSize = e.NewSize.Height * 0.03;
            ManageDatabase.FontSize = e.NewSize.Height * 0.03;
            manageRoles.FontSize = e.NewSize.Height * 0.03;
            ToTheApp.FontSize = e.NewSize.Height * 0.03;
            CreateRequiredTablesBTN.FontSize = e.NewSize.Height * 0.03;
            ModifyDatabase.FontSize = e.NewSize.Height * 0.03;            
        }

        private void manageRoles_Click(object sender, RoutedEventArgs e)
        {
            ManageRolePage manageRolePage = new ManageRolePage();
            RolesContent.Content = manageRolePage;
        }

        private void ToTheApp_Click(object sender, RoutedEventArgs e)
        {
            if(Tables.databases.Tables.Count > 0)
            {
                SAdminName = string.Empty;
                SAdminEmail = string.Empty;
                SAdminPassword = string.Empty;

                MainWindow currentWindow = Window.GetWindow(this) as MainWindow;
                Home newHome = new Home();
                currentWindow.content.Navigate(newHome);
            }
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

        private void manageEmail_Click(object sender, RoutedEventArgs e)
        {
            EditEmail editEmail = new EditEmail();
            editEmail.ShowDialog();
        }
    }
}