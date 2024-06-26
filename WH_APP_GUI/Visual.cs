﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using ControlzEx.Standard;
using Microsoft.Maps.MapControl.WPF;
using static Xceed.Wpf.Toolkit.Calculator;
using System.Diagnostics.Metrics;

namespace WH_APP_GUI
{
    internal class Visual
    {
        
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
                        if (shelfBuilder.newShelf["orientation"].ToString() == "Horizontal" && shelfWidth > 0 && shelfWidth <= sizeHorizontally)
                        {
                            isCorrect = true;
                        }
                        else if (shelfBuilder.newShelf["orientation"].ToString() == "Vertical" && shelfWidth <= sizeVertically && shelfWidth > 0)
                        {
                            isCorrect = true;
                        }
                        else
                        {
                            if (shelfBuilder.newShelf["orientation"].ToString() == "Horizontal")
                            {
                                MessageBox.Show($"The value entered must be greater than zero and less than {sizeVertically}. Please adjust your input accordingly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else if (shelfBuilder.newShelf["orientation"].ToString() == "Vertical")
                            {
                                MessageBox.Show($"The value entered must be greater than zero and less than {sizeHorizontally}. Please adjust your input accordingly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }



                    }
                    else
                    {
                        MessageBox.Show("Invalid entry. The value provided must be a numeric figure. Please correct your input.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Width field cannot be left blank. Please enter a value to ensure continued operation.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return isCorrect;
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
