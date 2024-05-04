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
            name.ValueDataType = typeof(string);
            width.ValueDataType = typeof(double);
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
            shelfBuilder.newShelf = Tables.shelf.database.NewRow();
            if (Visual.checkIfWidthIsCorrectFormat(width.Text) == true && Validation.ValidateTextbox(name, shelfBuilder.newShelf) == false && Validation.ValidateTextbox(width, shelfBuilder.newShelf) == false)
            {
                
                shelfBuilder.newShelf["name"] = name.Text;
                shelfBuilder.newShelf["width"] = double.Parse(width.Text);
                shelfBuilder.newShelf["orientation"] = Visual.orientation.ToString();
                shelfBuilder.isAShelfBeingCreated = true;


                this.Close();
            }

        }
    }
}
