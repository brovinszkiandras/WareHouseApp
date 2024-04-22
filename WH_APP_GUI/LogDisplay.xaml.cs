using Microsoft.Maps.MapControl.WPF.Overlays;
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
using System.Windows.Shapes;

namespace WH_APP_GUI
{
    public partial class LogDisplay : Page
    {
        public LogDisplay()
        {
            InitializeComponent();
            IniDatasInLog();
            IniEmails();
        }
        private void IniEmails()
        {
            LogsByEmail.Items.Clear();

            foreach (DataRow employees in Tables.employees.database.Rows)
            {
                LogsByEmail.Items.Add(employees["email"]);
            }

            foreach (DataRow staff in Tables.staff.database.Rows)
            {
                LogsByEmail.Items.Add(staff["email"]);
            }
        }
        private void IniDatasInLog()
        {
            Logs.Visibility = Visibility.Visible;
            Logs.Items.Clear();
            List<string[]> Datas = SQL.SqlQuery("SELECT * FROM log");
            for (int i = 0; i < Datas.Count; i++)
            {
                Logs.Items.Add($"[{Datas[i][3]}]: {Datas[i][2]}");
            }
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            Logs.Visibility = Visibility.Visible;
            NotFoundLogs.Visibility = Visibility.Collapsed;
            SQL.SqlCommand("DELETE FROM log");
            MessageBox.Show("All recorded data has been deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            IniDatasInLog();
        }

        private void LogsByEmail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NotFoundLogs.Visibility = Visibility.Collapsed;
            List<string[]> Datas = SQL.SqlQuery($"SELECT * FROM log WHERE email = '{LogsByEmail.SelectedItem}'");
            if (0 == Datas.Count)
            {
                Logs.Visibility = Visibility.Collapsed;
                NotFoundLogs.Visibility = Visibility.Visible;
            }
            else
            {
                Logs.Items.Clear();
                Logs.Visibility = Visibility.Visible;
                for (int i = 0; i < Datas.Count; i++)
                {
                    Logs.Items.Add($"[{Datas[i][3]}]: {Datas[i][2]}");
                }
            }
        }
    }
}
