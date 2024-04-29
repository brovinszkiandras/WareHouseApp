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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WH_APP_GUI.sectors
{
    /// <summary>
    /// Interaction logic for UpdateSectorPage.xaml
    /// </summary>
    public partial class UpdateSectorPage : Page
    {
        public UpdateSectorPage()
        {
            InitializeComponent();
        }
        private void WtfJohny_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
        }
    }
}
