using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml.Linq;
using MahApps.Metro.Controls;
using MySqlConnector;

namespace WH_APP_GUI
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            //Console.IniConsole();
            //Email.send("szsoly04@gmail.com", "Megkérdezzem?", "Mivan mivan mivan");
            if (! SQL.IsDatabasetxtExist())
            {
                DatabaseSet.Visibility = Visibility.Visible;
            }
            else
            {
                SQL.FillStaticDatabaseValues();
                if (SQL.Tables().Contains("feature"))
                {
                    Tables.Ini();
                    if (SQL.Tables().Contains(Tables.staff.actual_name))
                    {
                        List<string> RolesInStaff = SQL.GetElementOfListArray(SQL.SqlQuery($"SELECT role_id FROM {Tables.staff.actual_name}"));
                        if (RolesInStaff.Contains("1"))
                        {
                            LogIn.Visibility = Visibility.Visible;
                            Login_button.Visibility = Visibility.Visible;
                            RegisterAsAdmin.Visibility = Visibility.Collapsed;

                            //Belépéshez nem kell név csak email meg jelszó
                            Name.Visibility = Visibility.Collapsed;
                            NameBorder.Visibility = Visibility.Collapsed;
                            NameLBL.Visibility = Visibility.Collapsed;
                        }
                        else 
                        {
                            AdminLogInshow();
                        }
                    }
                    else
                    {
                        AdminLogInshow();
                    }
                }
                else
                {
                    AdminLogInshow();
                }
            }
        }
        private void AdminLogInshow()
        {
            LogIn.Visibility = Visibility.Visible;
            Login_button.Visibility = Visibility.Collapsed;
            RegisterAsAdmin.Visibility = Visibility.Visible;

            Name.Visibility = Visibility.Visible;
            NameLBL.Visibility = Visibility.Visible;
        }
        private void FoOldal_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
            Name.FontSize = e.NewSize.Height * 0.02;
            Emali.FontSize = e.NewSize.Height * 0.02;
            Password.FontSize = e.NewSize.Height * 0.02;

            passwrdFU.FontSize = e.NewSize.Height * 0.02;
            usernameFU.FontSize = e.NewSize.Height * 0.02;
            portFU.FontSize = e.NewSize.Height * 0.02;
            DataSourceFU.FontSize = e.NewSize.Height * 0.02;
            DatabaseNameFU.FontSize = e.NewSize.Height * 0.02;

            Name.FontSize = e.NewSize.Height * 0.02;
            Emali.FontSize = e.NewSize.Height * 0.02;
            RegisterAsAdmin.FontSize = e.NewSize.Height * 0.02;
        }

        private void ConfirmDBdatas_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.ValidateSQLNaming(DatabaseNameFU.Text, "Database name") == true &&  DataSourceFU.Text != string.Empty && portFU.Text != string.Empty && usernameFU.Text != string.Empty && DatabaseNameFU.Text != string.Empty)
            {
                try
                {
                    SQL.CreateDatabaseConnectionDatas(DataSourceFU.Text, int.Parse(portFU.Text), usernameFU.Text, passwrdFU.Text, DatabaseNameFU.Text);
                    SQL.FillStaticDatabaseValues();
                    DatabaseSet.Visibility = Visibility.Collapsed;
                    LogIn.Visibility = Visibility.Visible;
                }
                catch (Exception)
                {
                    MessageBox.Show("Couldnt connect to the specified database");
                }
            }
            else
            {
                MessageBox.Show("Empty input field", "Missing data", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (Emali.Text != string.Empty && Password.Password != string.Empty)
            {
                string hpsw = Hash.HashPassword(Password.Password);

                List<string> employees = SQL.GetElementOfListArray(SQL.SqlQuery($"SELECT email FROM {Tables.employees.actual_name}"));
                List<string> staffs = SQL.GetElementOfListArray(SQL.SqlQuery($"SELECT email FROM {Tables.staff.actual_name}"));

                if (employees.Contains(Emali.Text))
                {
                    List<string[]> datasOfUser = SQL.SqlQuery($"SELECT * FROM {Tables.employees.actual_name} WHERE email = '{Emali.Text}'");

                    if (datasOfUser[0][4] != "" || datasOfUser[0][5] != "")
                    {
                        User.SetCurrentUser(Emali.Text, hpsw);
                        User.MainWindow = this;
                        if (Tables.features.isFeatureInUse("Activity"))
                        {
                            if (!(bool)User.currentUser["activity"])
                            {
                                MessageBox.Show("Your activity is set to Inactive. You won't be able to log in until you're active again", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                Controller.LogWrite(User.currentUser["email"].ToString(), "User has been logged in to the application");
                                if (User.currentUser != null)
                                {
                                    User.currentUser["is_loggedin"] = true;
                                    Tables.employees.updateChanges();

                                    LogIn.Visibility = Visibility.Visible;
                                    content.Visibility = Visibility.Visible;
                                    content.Content = null;
                                    content.Navigate(new Uri("Home.xaml", UriKind.Relative));
                                }
                            }
                        }
                        else
                        {
                            Controller.LogWrite(User.currentUser["email"].ToString(),"User has been logged in to the application");
                            if (User.currentUser != null)
                            {
                                LogIn.Visibility = Visibility.Visible;
                                content.Visibility = Visibility.Visible;
                                content.Content = null;
                                content.Navigate(new Uri("Home.xaml", UriKind.Relative));
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Entry blocked. The user's data is either incomplete or non-existent.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                else if (staffs.Contains(Emali.Text))
                {
                    List<string[]> datasOfUser = SQL.SqlQuery($"SELECT * FROM {Tables.staff.actual_name} WHERE email = '{Emali.Text}'");

                    if (datasOfUser[0][4] != "")
                    {
                        User.SetCurrentUser(Emali.Text, hpsw);
                        User.MainWindow = this;
                        Controller.LogWrite(User.currentUser["email"].ToString(), "User has been logged in to the application");
                        if (User.currentUser != null)
                        {
                            LogIn.Visibility = Visibility.Visible;
                            content.Visibility = Visibility.Visible;
                            content.Content = null;
                            content.Navigate(new Uri("Home.xaml", UriKind.Relative));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Entry blocked. The user's data is either incomplete or non-existent.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("This person not existing!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Empty input field", "Missing data", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterAsAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (Name.Text != string.Empty && Emali.Text != string.Empty && Password.Password != string.Empty)
            {
                string AdminName = Name.Text;
                string AdminEmail = Emali.Text;
                string AdminPassword = Hash.HashPassword(Password.Password);

                LogIn.Visibility = Visibility.Collapsed;
                content.Visibility = Visibility.Visible;

                AdminHomePage adminHomePage = new AdminHomePage(AdminName, AdminEmail, AdminPassword);
                content.Content = null;
                content.Navigate(adminHomePage);
            }
            else
            {
                MessageBox.Show("Empty input field", "Missing data", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void content_Navigating(object sender, NavigatingCancelEventArgs e)
        {
          
        }
    }
}
