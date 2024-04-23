using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace WH_APP_GUI
{
    internal class shelfBuilder
    {
        public static DataRow newShelf;
        public static bool isAShelfBeingCreated = false;
        public static int lastXindex;
        public static int lastYindex;

        public static int currentXindex;
        public static int currentYindex;


        public static bool isDeleteBeingUsed = false;

        public static bool isDesignerModeActive = false;
        public static void setStartingPointOfShelf(Button button, string slotWidth)
        {
            newShelf["sector_id"] = Visual.sector["id"];
            newShelf["startXindex"] = Grid.GetColumn(button);
            newShelf["startYindex"] = Grid.GetRow(button);
            newShelf["width"] = slotWidth;
            newShelf["number_of_levels"] = 1;
            lastXindex = currentXindex;
            lastYindex = currentYindex;

            newShelf["length"] = 1;

            button.Tag = newShelf["name"];
            button.Background = Brushes.DarkMagenta;
            button.BorderThickness = new System.Windows.Thickness(0);

            addSquaresAreaToSectorsAreaInUse(button);
            Visual.squaresInUSe += 1;
        }

        public static void displaySquareAsHorizontalShelf(Button button, double width)
        {
            button.Width = Visual.sizeHorizontally;
            button.Height = width;
            newShelf["orientation"] = "Horizontal";
        }

        public static void displaySquareAsVerticalShelf(Button button, double width)
        {
            button.Height = Visual.sizeVertically;
            button.Width = width;
            newShelf["orientation"] = "Vertical";
        }

        public static void addSquareToshelf(Button button)
        {
            newShelf["length"] = (double)newShelf["length"] + 1;
            button.Tag = newShelf["name"];
            button.Background = Brushes.DarkMagenta;
            button.BorderThickness = new System.Windows.Thickness(0);

            addSquaresAreaToSectorsAreaInUse(button);
            addSquaresLengthToActualLength();
            Visual.squaresInUSe += 1;
        }

        public static bool checkIfTheresAshelfToTheRight()
        {
            bool isThereAshelf = false;

            if ((shelfBuilder.currentXindex + 1 == (int)shelfBuilder.newShelf["startXindex"])
                && currentYindex == (int)shelfBuilder.newShelf["startYindex"])
            {
                isThereAshelf = true;
            }

            return isThereAshelf;
        }

        public static bool checkifTheresAshelfToTheLeft()
        {
            bool isThereAshelf = false;
            if (currentXindex - 1 == shelfBuilder.lastXindex && currentYindex == (int)shelfBuilder.newShelf["startYindex"])
            {
                isThereAshelf = true;
            }

            return isThereAshelf;
        }

        public static bool checkIfTheresAshelfBelow()
        {
            bool isThereAshelf = false;
            if ((currentYindex + 1 == (int)shelfBuilder.newShelf["startYindex"] && currentXindex == (int)shelfBuilder.newShelf["startXindex"]))
            {
                isThereAshelf = true;
            }

            return isThereAshelf;
        }

        public static bool checkIfTheresAshelfAbove()
        {
            bool isThereAshelf = false;
            if (currentYindex - 1 == shelfBuilder.lastYindex && currentXindex == (int)shelfBuilder.newShelf["startXindex"])
            {
                isThereAshelf = true;
            }

            return isThereAshelf;
        }

        public static bool checkIfTHeSquarefHasAlreadyBeenTaken(Button button)
        {
            bool isTaken = false;
            if (button.Tag != null)
            {
                isTaken = true;
                MessageBox.Show("This square is already part of the shelf");
            }
            return isTaken;
        }


        public static bool checkIfSquareIsInTheMiddleOfTheShelf(Button button, DataRow shelf)
        {
            int column = Grid.GetColumn(button);
            int row = Grid.GetRow(button);

            // MessageBox.Show($"Column: {column}, startXindex: {shelf["startXindex"]} endindex: {(int)shelf["startXindex"] + (double)shelf["length"]}");
            if (shelf["orientation"].ToString() == "Horizontal" && column != (int)shelf["startXindex"] && column != (int)shelf["startXindex"] + (double)shelf["length"] - 1)
            {
                MessageBox.Show("You cant delete this button a");
                return true;
            }
            else if (shelf["orientation"].ToString() == "Vertical" && row != (int)shelf["startYindex"] && row != (int)shelf["startYindex"] + (double)shelf["length"] - 1)
            {
                MessageBox.Show("You cant delete this button b");
                return true;
            }
            else { return false; }
        }
        public static void removeSquareFromShelf(Button button)
        {

            DataRow shelf = Tables.shelf.database.Select($"name = '{button.Tag}' AND sector_id = {Visual.sector["id"]}")[0];
            //Check if square is at the start ot end
            if (checkIfSquareIsInTheMiddleOfTheShelf(button, shelf) == false)
            {

                button.Tag = null;
                shelf["length"] = (double)shelf["length"] - 1;
                button.Background = Brushes.Black;
                button.Width = Visual.sizeHorizontally / 2;
                button.Height = Visual.sizeVertically / 2;
                button.BorderThickness = new Thickness(0.03);
                button.BorderBrush = Brushes.YellowGreen;
                if (shelf["orientation"].ToString() == "Horizontal" && (int)shelf["startXindex"] == Grid.GetColumn(button))
                {
                    shelf["startXindex"] = (int)shelf["startXindex"] + 1;

                }
                if (shelf["orientation"].ToString() == "Vertical" && (int)shelf["startYindex"] == Grid.GetRow(button))
                {
                    shelf["startYindex"] = (int)shelf["startYindex"] + 1;

                }
                if ((double)shelf["length"] == 0)
                {
                    shelf.Delete();
                }
                removeSquaresAreaFromSectorsAreaInUse(button);
                removeSquaresLengthFromActualLength();
                Visual.squaresInUSe -= 1;

                Tables.shelf.updateChanges();
            }
        }

        public static void addSquaresAreaToSectorsAreaInUse(Button square)
        {
            Visual.sector["area_in_use"] = (double)Visual.sector["area_in_use"] + square.Width * square.Height;

            
            
        }

        public static void removeSquaresAreaFromSectorsAreaInUse(Button square)
        {
            Visual.sector["area_in_use"] = (double)Visual.sector["area_in_use"] - square.Width * square.Height;
        }

        private static void addSquaresLengthToActualLength()
        {
            if (newShelf["orientation"].ToString() == "Horizontal")
            {
                MessageBox.Show(newShelf["actual_length"].ToString());
                newShelf["actual_length"] = (double)newShelf["actual_length"] + Visual.sizeHorizontally;
            }
            else if (newShelf["orientation"].ToString() == "Vertical")
            {
                newShelf["actual_length"] = (double)newShelf["actual_length"] + Visual.sizeVertically;
            }
        }

        private static void removeSquaresLengthFromActualLength()
        {
            if (newShelf["orientation"].ToString() == "Horizontal")
            {
                newShelf["actual_length"] = (double)newShelf["actual_length"] - Visual.sizeHorizontally;
            }
            else if (newShelf["orientation"].ToString() == "Vertical")
            {
                newShelf["actual_length"] = (double)newShelf["actual_length"] - Visual.sizeVertically;
            }
        }
    }
}
