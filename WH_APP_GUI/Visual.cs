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
    internal class Visual
    {
        public static Orientation orientation = Orientation.Horizontal;
        public static double sizeHorizontally = 0;
        public static double sizeVertically = 0;
        public static double numberOfSquaresVertically = 0;
        public static double numberOfSquaresHorizontally = 0;

        public static double squaresInUSe = 0;

        public static DataRow sector;



        static Visual()
        {

        }


        public static void calculaTeNumberOfSquares(double width)
        {
            double size = 1;
            double numberOfSquares = 0;
            int maxnumberOfSquares = 0;


            if (width <= 30)
            {
                maxnumberOfSquares = 10;
            }
            else
            {
                maxnumberOfSquares = 15;
            }

            while (width % size != 0 || width / size > maxnumberOfSquares)
            {
                size += 0.5;
                if (size == 5 && width % size != 0)
                {
                    width--;
                    size = 1;
                }
            }


            numberOfSquares = width / size;
            sizeHorizontally = size;
            numberOfSquaresHorizontally = numberOfSquares;

        }

        public static void calculateNumberAndSizeOfSquaresVertically(double height)
        {
            double size = 1;
            double numberOfSquares = 0;
            int maxnumberOfSquares = 0;


            if (height <= 30)
            {
                maxnumberOfSquares = 10;
            }
            else
            {
                maxnumberOfSquares = 15;
            }

            while (height % size != 0 || height / size > maxnumberOfSquares)
            {
                size += 0.5;
                if (size == 5 && height % size != 0)
                {
                    height--;
                    size = 1;
                }
            }


            numberOfSquares = height / size;

            sizeVertically = size;
            numberOfSquaresVertically = numberOfSquares;
        }

        public static double CalCulateSquaresInTotal()
        {
            return numberOfSquaresHorizontally * numberOfSquaresVertically;
        }

        public static void calculateSquaresInUse()
        {
            if(sector !=  null)
            {
                if(Tables.sector.getShelfs(sector).Length > 0)
                {
                    squaresInUSe = Tables.sector.getShelfs(sector).Sum(row => (double)row["length"]);
                }
                else
                {
                    squaresInUSe = 0;
                }
            }
            
        }

        public static bool checkIfWidthIsCorrectFormat(string width)
        {
            bool isCorrect = false;

            try
            {
                if (width.Length != 0)
                {
                    double shelfWidth;
                    if (double.TryParse(width, out shelfWidth) == true)
                    {
                        if (orientation == Orientation.Horizontal && shelfWidth > 0 && shelfWidth <= sizeHorizontally)
                        {
                            isCorrect = true;
                        }
                        else if (orientation == Orientation.Vertical && shelfWidth <= sizeVertically && shelfWidth > 0)
                        {
                            isCorrect = true;
                        }
                        else
                        {
                            if (orientation == Orientation.Horizontal)
                            {
                                MessageBox.Show($"A megadott értéknek nagyobbnak kell lennie nullánál is kissebnek {sizeVertically}-nál");
                            }
                            else if (orientation == Orientation.Vertical)
                            {
                                MessageBox.Show($"A megadott értéknek nagyobbnak kell lennie nullánál is kissebnek {sizeHorizontally}-nál");
                            }
                        }



                    }
                    else
                    {
                        MessageBox.Show("A megadott érték nem szám");
                    }
                }
                else
                {
                    MessageBox.Show("A szelesseg mezot nem lehet uresen hagyni");
                }
            }
            catch (Exception)
            {

                throw;
            }
            return isCorrect;
        }

        public static void displayHorizontalShelf(Grid boxGrid, DataRow row)
        {
            double numberOfSquaresToFind = (double)row["length"];
            int index = 0;
            while (index < boxGrid.Children.Count && numberOfSquaresToFind != 0)
            {
                Button square = (Button)boxGrid.Children[index];
                for (int i = (int)row["startXindex"]; i < (int)row["startXindex"] + (double)row["length"]; i++)
                {
                    if (Grid.GetRow(square) == (int)row["startYindex"] && Grid.GetColumn(square) == i)
                    {
                        square.Background = Brushes.DarkMagenta;
                        square.BorderThickness = new System.Windows.Thickness(0);
                        square.Width = Visual.sizeHorizontally;
                        square.Height = (double)row["width"];
                        square.Tag = row["name"];

                        numberOfSquaresToFind--;
                    }

                }
                index++;
            }
        }

        public static void DisplayVerticalShelf(Grid boxGrid, DataRow row)
        {
            double numberOfSquaresToFind = (double)row["length"];
            int index = 0;
            while (index < boxGrid.Children.Count || numberOfSquaresToFind != 0)
            {
                Button square = (Button)boxGrid.Children[index];
                for (int i = (int)row["startYindex"]; i < (int)row["startYindex"] + (double)row["length"]; i++)
                {
                    if (Grid.GetColumn(square) == (int)row["startXindex"] && Grid.GetRow(square) == i)
                    {
                        square.Background = Brushes.DarkMagenta;
                        square.BorderThickness = new System.Windows.Thickness(0);
                        square.Width = (double)row["width"];
                        square.Height = Visual.sizeVertically;

                        square.Tag = row["name"];
                        numberOfSquaresToFind--;
                    }

                }
                index++;
            }
        }

        public static void initalizeGrid(Grid boxGrid)
        {
            boxGrid.Children.Clear();
            boxGrid.RowDefinitions.Clear();
            boxGrid.ColumnDefinitions.Clear();
            boxGrid.Width = (double)Visual.sector["length"];
            boxGrid.Height = (double)Visual.sector["width"];
        }


        public static void addRowsAndColumns(Grid boxGrid)
        {
            for (int i = 0; i < Visual.numberOfSquaresHorizontally; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(Visual.sizeHorizontally);
                boxGrid.ColumnDefinitions.Add(columnDefinition);
            }

            for (int i = 0; i < Visual.numberOfSquaresVertically; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(Visual.sizeVertically); // Set the row height to half the width
                boxGrid.RowDefinitions.Add(rowDefinition);

            }
        }

        public static void hideSquaresThatDoNotBelongToAShelf(Grid boxGrid)
        {
            foreach (Button button in boxGrid.Children)
            {
                if (button.Tag == null)
                {
                    button.Visibility = Visibility.Collapsed;
                }
            }
        }

        public static void showSquares(Grid boxGrid)
        {
            foreach (Button button in boxGrid.Children)
            {
                if (button.Tag == null)
                {
                    button.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
