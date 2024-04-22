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
    /// <summary>
    /// Interaction logic for sectorIndexWindow.xaml
    /// </summary>
    public partial class sectorIndexWindow : Window
    {
        public sectorIndexWindow()
        {
            InitializeComponent();

            sectorsGrid.ItemsSource = Tables.sector.database.DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            DataRow selectedSector = Tables.sector.database.Select($"id = {button.Tag}")[0];

            Visual.sector = selectedSector;
            SectorWindow sectorWindow = new SectorWindow();
            sectorWindow.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {


            //FilterWindow filter = new FilterWindow(sectorFilter);
            //filter.ShowDialog();
            DataTable filteredTable = new DataTable();
            filteredTable.ImportRow(Tables.sector.database.Select("id = 1")[0]);

            foreach (DataColumn column in Tables.sector.database.Columns)
            {
                filteredTable.Columns.Add(column.ColumnName);
            }
            filteredTable.Rows[0]["name"] = "sector1";

            Tables.sector.updateChanges();
        }
    }
}
