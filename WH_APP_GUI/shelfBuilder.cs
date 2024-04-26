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
        //a készülésben kévő polc
        public static DataRow newShelf;
        //változó ami azt jelzi hogy egy polc készülőben van -e
        public static bool isAShelfBeingCreated = false;
        //a polc utolsó X pozicíója 
        public static int lastXindex;
        //a polc utolsó Y pozicíója 
        public static int lastYindex;

        //egy gomb poziciói ha Vízszintes:
        //kezdő: legbaloldalibb kocka
        //utolsó: a legjobboldalibb kocka
        //egy gomb poziciói ha Függőleges:
        //kezdő: a legfelső kocka
        //utolsó: a legalsó kocka


        //A jelenlegi X pozició
        public static int currentXindex;
        //A jelenlegi Y pozició
        public static int currentYindex;

        //Törlés mód be van e kapcsolva
        public static bool isDeleteBeingUsed = false;

        //Designer mód be van e kapcsolva
        public static bool isDesignerModeActive = false;

        //Beállítja a polc kezdő pozicíóját
        public static void setStartingPointOfShelf(Button button)
        {
            //polc sector_id = kiválasztott sector id
            newShelf["sector_id"] = Visual.sector["id"];
            newShelf["startXindex"] = Grid.GetColumn(button);
            newShelf["startYindex"] = Grid.GetRow(button);
            
            //Beállítom az utolsó pozicíót a jelenlegi pozicíóra
            lastXindex = currentXindex;
            lastYindex = currentYindex;

            
            newShelf["length"] = 1;

            //a gomb azonosítója a polc neve lesz
            button.Tag = newShelf["name"];
            if(newShelf.RowState == DataRowState.Detached)
            {
                button.Background = Brushes.DarkMagenta;
            }
            else if(newShelf.RowState == DataRowState.Modified)
            {
                button.Background= Brushes.White;
            }
            button.BorderThickness = new System.Windows.Thickness(0);

            //Hozzáadom a gomb hosszúságát a polc hosszúságához
            addSquaresLengthToActualLength();
            //Hozzáadom a gomb területét a sector területéhez
            addSquaresAreaToSectorsAreaInUse(button);

            //Megnöveli a használatban lévő gombok számát
            Visual.squaresInUSe += 1;
        }

        //összeköt egy kockát egy vízszintes polccal
        public static void displaySquareAsHorizontalShelf(Button button, double width)
        {
            button.Width = Visual.sizeHorizontally;
            button.Height = width;
           
        }

        //összeköt egy kockát egy függőleges polccal
        public static void displaySquareAsVerticalShelf(Button button, double width)
        {
            button.Height = Visual.sizeVertically;
            button.Width = width;
            
        }

        //hozzáadja a gombot a polchoz
        public static void addSquareToshelf(Button button)
        {
            newShelf["length"] = (double)newShelf["length"] + 1;
            button.Tag = newShelf["name"];
            if (newShelf.RowState == DataRowState.Detached)
            {
                button.Background = Brushes.DarkMagenta;
            }
            else if (newShelf.RowState == DataRowState.Modified)
            {
                button.Background = Brushes.White;
            }
            button.BorderThickness = new System.Windows.Thickness(0);

            //Hozzáadom a gomb hosszúságát a polc hosszúságához
            addSquaresAreaToSectorsAreaInUse(button);
            //Hozzáadom a gomb területét a sector területéhez
            addSquaresLengthToActualLength();
            Visual.squaresInUSe += 1;
        }

        //Megnézi hogy a megnyomott gomb jobb oldalán van e polc
        public static bool checkIfTheresAshelfToTheRight()
        {
            bool isThereAshelf = false;

            //ha a jelenlegi X pozició + 1 egyenlő a polc kezdő poziciójával és a jelenlegi Y pozició
            //egyenlő a gomb kezdő Y poziciójával
            if ((shelfBuilder.currentXindex + 1 == (int)shelfBuilder.newShelf["startXindex"])
                && currentYindex == (int)shelfBuilder.newShelf["startYindex"])
            {
                isThereAshelf = true;
            }
            return isThereAshelf;
        }

        //Megnézi hogy a megnyomott gomb bal oldalán van e polc
        public static bool checkifTheresAshelfToTheLeft()
        {
            //ha a jelenlegi X pozició - 1 egyenlő a polc befejező poziciójával és a jelenlegi Y pozició
            //egyenlő a gomb kezdő Y poziciójával
            bool isThereAshelf = false;
            if (currentXindex - 1 == shelfBuilder.lastXindex && currentYindex == (int)shelfBuilder.newShelf["startYindex"])
            {
                isThereAshelf = true;
            }

            return isThereAshelf;
        }

        //Megnézi hogy a megnyomott gomb alatt van e polc
        public static bool checkIfTheresAshelfBelow()
        {
            bool isThereAshelf = false;
            //ha a jelenlegi Y pozició + 1 egyenlő a polc kezdő poziciójával és a jelenlegi X pozició
            //egyenlő a gomb kezdő X poziciójával
            if ((currentYindex + 1 == (int)shelfBuilder.newShelf["startYindex"] && currentXindex == (int)shelfBuilder.newShelf["startXindex"]))
            {
                isThereAshelf = true;
            }

            return isThereAshelf;
        }


        //Megnézi hogy a megnyomott gomb fölött van e polc
        public static bool checkIfTheresAshelfAbove()
        {
            //ha a jelenlegi Y pozició - 1 egyenlő a polc befejező poziciójával és a jelenlegi X pozició
            //egyenlő a gomb kezdő X poziciójával
            bool isThereAshelf = false;
            if (currentYindex - 1 == shelfBuilder.lastYindex && currentXindex == (int)shelfBuilder.newShelf["startXindex"])
            {
                isThereAshelf = true;
            }

            return isThereAshelf;
        }
        
        //Megnézi hogy a bekért gomb már tartozik e egy mások polchoz
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

        //Megnézi hogy a bekért polcnak egy olyan poziciójéban van e ami kitörlhető
        //Csak a kezdő és a befejező pozició törölhető ki
        private static bool checkIfSquareIsInTheMiddleOfTheShelf(Button button, DataRow shelf)
        {
            int column = Grid.GetColumn(button);
            int row = Grid.GetRow(button);

            //Ha a polc vízszintes és a gomb X poziciója nem egyezik a gomb kezdő poziciójával
            //(startXindex) és nem egyezik a gomb befejező poziciójával (startXindex + polc hosszúsága)
            if (shelf["orientation"].ToString() == "Horizontal" && column != (int)shelf["startXindex"] && column != (int)shelf["startXindex"] + (double)shelf["length"] - 1)
            {
                MessageBox.Show("You cant delete this button a");
                return true;
            }
            //Ha a polc függőleges és a gomb Y poziciója nem egyezik a gomb kezdő poziciójával
            //(startYindex) és nem egyezik a gomb befejező poziciójával (startYindex + polc hosszúsága)
            else if (shelf["orientation"].ToString() == "Vertical" && row != (int)shelf["startYindex"] && row != (int)shelf["startYindex"] + (double)shelf["length"] - 1)
            {
                MessageBox.Show("You cant delete this button b");
                return true;
            }
            else { return false; }
        }
        public static void removeSquareFromShelf(Button button)
        {
            //Polc amiból törölni fog
            DataRow shelf;
            //Ha egy polc nem készül akkor a bekért gomb azonosítója alapján lekéri a
            //a megadott polcot
            if (isAShelfBeingCreated == false)
            {
              shelf  = Tables.shelf.database.Select($"name = '{button.Tag}' AND sector_id = {Visual.sector["id"]}")[0];
            }
            //Ha egy polc készül akkor az lesz a polc amiből törölni fog
            else
            {
                shelf = newShelf;
            }
            //Le ellenőrzi hogy a kocka a polc elején vagy végén van e (törölhető)
            if (checkIfSquareIsInTheMiddleOfTheShelf(button, shelf) == false)
            {
                //Törli az azonosítóját
                button.Tag = null;
                //Lekapcsolja a polcról
                shelf["length"] = (double)shelf["length"] - 1;
                button.Background = Brushes.Black;
                button.Width = Visual.sizeHorizontally / 2;
                button.Height = Visual.sizeVertically / 2;
                button.BorderThickness = new Thickness(0.03);
                button.BorderBrush = Brushes.YellowGreen;

                //Ha a gomb a polc kezdő pozicióján van és a polc vízszintes
                if (shelf["orientation"].ToString() == "Horizontal" && (int)shelf["startXindex"] == Grid.GetColumn(button))
                {
                    //a tőle jobbra lévő gomb lesz az új kezdő pozició
                    shelf["startXindex"] = (int)shelf["startXindex"] + 1;

                }
                //Ha a gomb a polc befejező pozicióján van és a polc vízszintes
                if (shelf["orientation"].ToString() == "Vertical" && (int)shelf["startYindex"] == Grid.GetRow(button))
                {
                    //az alatta lévő gomb lesz az új kezdő pozició
                    shelf["startYindex"] = (int)shelf["startYindex"] + 1;

                }
                //Ha a gomb hosszúsága eléri a nullát kitörli a polcot
                if ((double)shelf["length"] == 0)
                {
                    if (isAShelfBeingCreated == false)
                    {
                        shelf.Delete();
                    }
                }
                //Kivonja a polc területét a sector használt területéből
                removeSquaresAreaFromSectorsAreaInUse(button);
                //Kivonja a gomb hosszúságát a polc hosszúságából
                removeSquaresLengthFromActualLength();
                Visual.squaresInUSe -= 1;

                
            }
        }

        private static void addSquaresAreaToSectorsAreaInUse(Button square)
        {
            //Hozzáadja a bekért gomb területét a sector használt területéhez
            Visual.sector["area_in_use"] = (double)Visual.sector["area_in_use"] + square.Width * square.Height;
        }

        private static void removeSquaresAreaFromSectorsAreaInUse(Button square)
        {
            //Kivonja a bekért gomb területét a sector használt területéből
            Visual.sector["area_in_use"] = (double)Visual.sector["area_in_use"] - square.Width * square.Height;
        }

        private static void addSquaresLengthToActualLength()
        {
            //Hozzáadja a gomb hosszúságát a polc hosszúságához
            if (newShelf["orientation"].ToString() == "Horizontal")
            {
                
                newShelf["actual_length"] = (double)newShelf["actual_length"] + Visual.sizeHorizontally;
            }
            else if (newShelf["orientation"].ToString() == "Vertical")
            {
                newShelf["actual_length"] = (double)newShelf["actual_length"] + Visual.sizeVertically;
            }
        }

        private static void removeSquaresLengthFromActualLength()
        {
            //Kivonja a gomb hosszúságát a polc hosszúságából
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
