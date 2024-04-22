using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            //Email.send("szsoly04@gmail.com", "Megkérdezzem?", "Mivan mivan mivan");

            if (! SQL.IsDatabasetxtExist())
            {
                DatabaseSet.Visibility = Visibility.Visible;
            }
            else
            {
                SQL.FillStaticDatabaseValues();
                if (SQL.Tables().Contains("migrations") && SQL.Tables().Contains("feature"))
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
            //TODO: úgy kéne megcsinálni az első belépést egy felhasználó(staff-nál) hogy az admin bejelntkezik(adatai eltárolása static válzoóként) majd beállitja a connection string adatait, ellennörzésként lekérdezi a megadott adatokat úgy hogy lekérdezi az adatbázisból a megadot connection-al az admin nevét WHERE email = this.email, ha egyezzik akkor jó ha nem akkor...megismétli a setup-ot
        }
        private void AdminLogInshow()
        {
            LogIn.Visibility = Visibility.Visible;
            Login_button.Visibility = Visibility.Collapsed;
            RegisterAsAdmin.Visibility = Visibility.Visible;

            //Belépéshez nem kell név csak email meg jelszó
            Name.Visibility = Visibility.Visible;
            NameLBL.Visibility = Visibility.Visible;
        }
        private void FoOldal_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
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
            if (Emali.Text != string.Empty && Password.Text != string.Empty)
            {
                string hpsw = Hash.HashPassword(Password.Text);

                List<string> employees = SQL.GetElementOfListArray(SQL.SqlQuery($"SELECT email FROM {Tables.employees.actual_name}"));
                List<string> staffs = SQL.GetElementOfListArray(SQL.SqlQuery($"SELECT email FROM {Tables.staff.actual_name}"));

                if (employees.Contains(Emali.Text))
                {
                    List<string[]> datasOfUser = SQL.SqlQuery($"SELECT * FROM {Tables.employees.actual_name} WHERE email = '{Emali.Text}'");

                    if (datasOfUser[0][4] != "" || datasOfUser[0][5] != "")
                    {
                        User.SetCurrentUser(Emali.Text, hpsw);
                        Controller.LogWrite(User.currentUser["email"].ToString(),"The user has been logged in to the application");
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
                else if (staffs.Contains(Emali.Text))
                {
                    List<string[]> datasOfUser = SQL.SqlQuery($"SELECT * FROM {Tables.staff.actual_name} WHERE email = '{Emali.Text}'");

                    if (datasOfUser[0][4] != "")
                    {
                        User.SetCurrentUser(Emali.Text, hpsw);
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
            if (Name.Text != string.Empty && Emali.Text != string.Empty && Password.Text != string.Empty)
            {
                string AdminName = Name.Text;
                string AdminEmail = Emali.Text;
                string AdminPassword = Hash.HashPassword(Password.Text);

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
           
            //if(Navigation.content2.Parent != null)
            //{
            //    Navigation.RemoveParent();
                
            //}
            
        }
    }
}
