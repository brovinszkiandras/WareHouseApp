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
    public partial class CreateRoleWindow : Window
    {
        public CreateRoleWindow()
        {
            InitializeComponent();
        }

        private void AddNewRoleBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NewRoleName.Text != string.Empty && NewRoleDescription.Text != string.Empty)
                {
                    SQL.SqlCommand($"INSERT INTO `{Tables.roles.actual_name}`(`role`, `in_warehouse`, `description`) VALUES ('{NewRoleName.Text}', {Is_Belongst_To_Warehouse.IsChecked}, '{NewRoleDescription.Text}');");
                    Tables.roles.Refresh();
                    MessageBox.Show("Role hase been added, now you can set the permmsions to it!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
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
        }

        private void CancelRoleCreation_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
