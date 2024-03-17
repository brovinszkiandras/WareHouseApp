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
using MySqlConnector;

namespace WH_APP_GUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (! SQL.IsDatabasetxtExist())
            {
                DatabaseSet.Visibility = Visibility.Visible;
            }
            else
            {
                SQL.FillStaticDatabaseValues();
                Tables.SetTablesWithClasses();
            }

            //TODO: úgy kéne megcsinálni az első belépést egy felhasználó(staff-nál) hogy az admin bejelntkezik(adatai eltárolása static válzoóként) majd beállitja a connection string adatait, ellennörzésként lekérdezi a megadott adatokat úgy hogy lekérdezi az adatbázisból a megadot connection-al az admin nevét WHERE email = this.email, ha egyezzik akkor jó ha nem akkor...megismétli a setup-ot
        }

        private void ConfirmDBdatas_Click(object sender, RoutedEventArgs e)
        {
            //TODO Sólyomank: Ez csak akkor futhat le ha az összes mezőben van adat, a port az szám stb...(a password lehet null mivel van olyan hogy nincs jelszó egy adatbázishoz)
            SQL.CreateDatabaseConnectionDatas(DataSourceFU.Text, int.Parse(portFU.Text), usernameFU.Text, passwrdFU.Text, DatabaseNameFU.Text);
            SQL.FillStaticDatabaseValues();
            DatabaseSet.Visibility = Visibility.Collapsed;
        }

        private void LogInBTN_Click(object sender, RoutedEventArgs e)
        {
            string AdminName = RegisterAdminName.Text;
            string AdminEmail = RegisterAdminEmail.Text;
            string AdminPassword = RegisterAdminPassword.Text;

            AdminHomePage adminHomePage = new AdminHomePage(AdminName, AdminEmail, AdminPassword);
            this.Hide();
            adminHomePage.Show();
        }
    }
}
