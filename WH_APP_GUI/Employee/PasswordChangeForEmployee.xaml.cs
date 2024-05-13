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

namespace WH_APP_GUI.Employee
{
    public partial class PasswordChangeForEmployee : Window
    {
        public PasswordChangeForEmployee()
        {
            InitializeComponent();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (Password.Text.Length >= 6 && Password.Text.Length <= 16 && !Password.Text.Contains(' '))
            {
                string hashedpswd = Hash.HashPassword(Password.Password);
                User.currentUser["password"] = hashedpswd;
                Tables.employees.updateChanges();
                MessageBox.Show("Password has been updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Password is in an incorrect form!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
