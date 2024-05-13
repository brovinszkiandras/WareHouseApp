using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
using System.Windows.Shapes;

namespace WH_APP_GUI
{
    public partial class EditEmail : Window
    {
        public EditEmail()
        {
            InitializeComponent();

            if (TherIsExistingEmailTxt())
            {
                string[] datas = File.ReadAllLines("email.txt");
                Email.Text = datas[0];
                ApiKey.Text = datas[1];
            }
        }
        private bool TherIsExistingEmailTxt()
        {
            if (File.Exists("email.txt"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (Email.Text.Length != 0 && ApiKey.Text.Length != 0)
            {
                string pattern = @"^(?=.*@)(?=.*\.)[\S]+$";
                bool isMatch = Regex.IsMatch(Email.Text, pattern);
                if (isMatch == false)
                {
                    MessageBox.Show("Please give a valid email!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    string[] datas = { Email.Text, ApiKey.Text };
                    File.WriteAllLines("email.txt", datas);
                    MessageBox.Show("Email has been updated", "Success", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("You need to fill all the input fileds", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
