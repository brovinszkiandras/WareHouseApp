using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

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
        public void Update()
        {
            if (SQL.Tables().Contains("feature"))
            {
                if (bool.Parse(SQL.FindOneDataFromQuery($"SELECT in_use FROM feature WHERE name = '{this.FeatureNameInDataBase}'")))
                {
                    this.IsEnabled = false;
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
            }
        }
    }
    #endregion
    public partial class AdminHomePage : Window
    {
        public static string SAdminName;
        public static string SAdminEmail;
        public static string SAdminPassword;
        public static void CreateCheckBoxes(Panel Display)
        {
            #region Checkboxes
            DateLogFeature DateLogCBXexe = new DateLogFeature("Date Log");
            DateLogCBXexe.Foreground = Brushes.White;
            DateLogCBXexe.Content = "Include DateLog feature";
            Display.Children.Add(DateLogCBXexe);

            FleetFeature FleetCBXexe = new FleetFeature("Fleet");
            FleetCBXexe.Foreground = Brushes.White;
            FleetCBXexe.Content = "Include Fleet feature";
            Display.Children.Add(FleetCBXexe);

            CityFeature CityCBXexe = new CityFeature("City");
            CityCBXexe.Foreground = Brushes.White;
            CityCBXexe.Content = "Include City feature";
            Display.Children.Add(CityCBXexe);

            LogFeature LogCBXexe = new LogFeature("Log");
            LogCBXexe.Foreground = Brushes.White;
            LogCBXexe.Content = "Include Log feature";
            Display.Children.Add(LogCBXexe);

            ActivityFeature ActivityCBXexe = new ActivityFeature("Activity");
            ActivityCBXexe.Foreground = Brushes.White;
            ActivityCBXexe.Content = "Include Activity feature";
            Display.Children.Add(ActivityCBXexe);

            RevenueFeature RevenueCBXexe = new RevenueFeature("Revenue");
            RevenueCBXexe.Foreground = Brushes.White;
            RevenueCBXexe.Content = "Include Revenue feature";
            Display.Children.Add(RevenueCBXexe);

            StorageFeature StorageCBXexe = new StorageFeature("Storage");
            StorageCBXexe.IsEnabled = false;
            StorageCBXexe.Foreground = Brushes.White;
            StorageCBXexe.Content = "Include Storage feature";
            Display.Children.Add(StorageCBXexe);

            FuelFeature FuelCBXexe = new FuelFeature("Fuel");
            FuelCBXexe.IsEnabled = false;
            FuelCBXexe.Foreground = Brushes.White;
            FuelCBXexe.Content = "Include Fuel feature";
            Display.Children.Add(FuelCBXexe);

            DockFeature DockCBXexe = new DockFeature("Dock");
            DockCBXexe.IsEnabled = false;
            DockCBXexe.Foreground = Brushes.White;
            DockCBXexe.Content = "Include Dock feature";
            Display.Children.Add(DockCBXexe);

            ForkliftFeature ForkliftCBXexe = new ForkliftFeature("Forklift");
            ForkliftCBXexe.Foreground = Brushes.White;
            ForkliftCBXexe.Content = "Include Forklift feature";
            Display.Children.Add(ForkliftCBXexe);
            #endregion
        }
        public AdminHomePage(string AdminName, string AdminEmail, string AdminPassword)
        {
            InitializeComponent();

            SAdminName = AdminName;
            SAdminEmail = AdminEmail;
            SAdminPassword = AdminPassword;

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
                Debug.WriteError(ex.ToString());
                throw;
            }

            AdminNameDisplay.Content = AdminName;
            AdminEmailDisplay.Content = AdminEmail;
        }

        private void CreateRequiredTablesBTN_Click(object sender, RoutedEventArgs e)
        {
            Controller.CreateMigration();
            Controller.CreateFeature();
            Tables.SetTablesWithClasses();

            /*Required*/
            Controller.CreateDefaultTablesWithMigrationInsert();

            if (Controller.IsMigrationContainsAllDefaultTables())
            {
                CreateRequiredTablesBTN.IsEnabled = false;
                CreateCheckBoxes(FeaturesDisplayG);
            }

            MessageBox.Show("Requierd tables and Feature tables created and filled with the datas.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void IncludeFeture_Click(object sender, RoutedEventArgs e)
        {
            foreach (FeatureCheckbox cbx in FeaturesDisplayG.Children)
            {
                if (cbx.CanBeCreated)
                {
                    cbx.ActivateFeature();
                    cbx.Update();
                }
            }
            Tables.SetTablesWithClasses();
        }

        private void LoginAsAdmin_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"INSERT INTO `{Tables.staff.actual_name}`(`name`, `email`, `password`, `role_id`) VALUES ('{SAdminName}', '{SAdminEmail}', '{SAdminPassword}', 1)");           
            //try
            //{
            //    if (SQL.Tables().Contains("migrations"))
            //    {
            //        MessageBox.Show($"INSERT INTO `{Tables.staff.actual_name}`(`name`, `email`, `password`, `role_id`) VALUES ('{SAdminName}', '{SAdminEmail}', '{SAdminPassword}', 1)");           
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteError(ex.ToString());
            //    throw;
            //}
        }
    }
}
