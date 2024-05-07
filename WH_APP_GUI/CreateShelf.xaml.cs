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
       private static Orientation orientation = Orientation.Horizontal;
        public CreateShelf()
        {
            InitializeComponent();
           
            name.ValueDataType = typeof(string);
            width.ValueDataType = typeof(double);
            if (orientation == Orientation.Horizontal)
            {
                maxWidth.Content = "max " + Visual.sizeHorizontally.ToString();
            }
            if (orientation == Orientation.Vertical)
            {
                maxWidth.Content += "max " + Visual.sizeVertically.ToString();
            }

        }

        private void change_orientation_Click(object sender, RoutedEventArgs e)
        {


            if (orientation == Orientation.Horizontal)
            {
                orientation = Orientation.Vertical;
                change_orientation.Content = "Orientation: Vertical";
                maxWidth.Content = "max " + Visual.sizeVertically.ToString();
            }
            else if (orientation == Orientation.Vertical)
            {
                orientation = Orientation.Horizontal;
                change_orientation.Content = "Orientation: Horizontal";
                maxWidth.Content = "max " + Visual.sizeHorizontally.ToString();

            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            shelfBuilder.newShelf = Tables.shelf.database.NewRow();
            shelfBuilder.newShelf["orientation"] = orientation.ToString();
            if (Visual.checkIfWidthIsCorrectFormat(width.Text) == true && Validation.ValidateTextbox(name, shelfBuilder.newShelf) == false && Validation.ValidateTextbox(width, shelfBuilder.newShelf) == false)
            {
                
                shelfBuilder.newShelf["name"] = name.Text;
                shelfBuilder.newShelf["width"] = double.Parse(width.Text);
                shelfBuilder.isAShelfBeingCreated = true;


                this.Close();
            }

        }
    }
}
