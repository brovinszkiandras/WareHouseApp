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
    public partial class CreateShelf : Window
    {

        public CreateShelf()
        {
            InitializeComponent();
            Visual.orientation = Orientation.Horizontal;
            if (Visual.orientation == Orientation.Horizontal)
            {
                maxWidth.Content = "max " + Visual.sizeHorizontally.ToString();
            }
            if (Visual.orientation == Orientation.Vertical)
            {
                maxWidth.Content += "max " + Visual.sizeVertically.ToString();
            }

        }

        private void change_orientation_Click(object sender, RoutedEventArgs e)
        {


            if (Visual.orientation == Orientation.Horizontal)
            {
                Visual.orientation = Orientation.Vertical;
                change_orientation.Content = "Orientation: Vertical";
                maxWidth.Content = "max " + Visual.sizeVertically.ToString();
            }
            else if (Visual.orientation == Orientation.Vertical)
            {
                Visual.orientation = Orientation.Horizontal;
                change_orientation.Content = "Orientation: Horizontal";
                maxWidth.Content = "max " + Visual.sizeHorizontally.ToString();

            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (Visual.checkIfWidthIsCorrectFormat(shelf_width.Text) == true)
            {
                shelfBuilder.newShelf = Tables.shelf.database.NewRow();
                shelfBuilder.newShelf["name"] = shelf_Name.Text;
                shelfBuilder.newShelf["width"] = double.Parse(shelf_width.Text);
                shelfBuilder.newShelf["orientation"] = Visual.orientation.ToString();
                shelfBuilder.isAShelfBeingCreated = true;


                this.Close();
            }

        }
    }
}
