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
    /// Interaction logic for SectorWindow.xaml
    /// </summary>
    public partial class SectorWindow : Page
    {
        public SectorWindow()
        {
            InitializeComponent();

            MessageBox.Show(Visual.sector["id"].ToString());


            this.DataContext = Visual.sector;

            Visual.initalizeGrid(boxGrid);

            Visual.calculaTeNumberOfSquares((double)Visual.sector["length"]);
            Visual.calculateNumberAndSizeOfSquaresVertically((double)Visual.sector["width"]);
            Visual.calculateSquaresInUse();

            //squareSize.Text = $"Maximuim size of a slot horizontally: {Visual.sizeHorizontally}, Vertically: {Visual.sizeVertically}.\nCurrent stize of a slot horizontally: {Visual.sizeHorizontally / 2}, Vertically: {Visual.sizeVertically / 2}.\nNumber of squares horizontally: {Visual.numberOfSquaresHorizontally}, number of squares vertically: {Visual.numberOfSquaresVertically}";

            Visual.addRowsAndColumns(boxGrid);



            for (int i = 0; i < boxGrid.ColumnDefinitions.Count; i++)
            {
                for (int j = 0; j < boxGrid.RowDefinitions.Count; j++)
                {
                    Button button = new Button();
                    button.Style = (Style)this.Resources["RectangleButtonStyle"];
                    button.Click += box_Click;
                    button.BorderBrush = Brushes.GreenYellow;
                    button.BorderThickness = new System.Windows.Thickness(0.03);

                    // Calculate the button height based on the height of the row

                    button.Background = Brushes.Black;

                    button.Height = (double)Visual.sizeVertically / 2;
                    button.Width = (double)Visual.sizeHorizontally / 2;
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    boxGrid.Children.Add(button);
                }
            }



            foreach (DataRow row in Tables.sector.getShelfs(Visual.sector))
            {

                if (row["orientation"].ToString() == "Horizontal")
                {

                    Visual.displayHorizontalShelf(boxGrid, row);

                }
                else if (row["orientation"].ToString() == "Vertical")
                {

                    Visual.DisplayVerticalShelf(boxGrid, row);

                }

            }
            Visual.hideSquaresThatDoNotBelongToAShelf(boxGrid);


            areaProgeressbar.Value = (double)Visual.sector["area_in_use"];

            areaUsagePercent.Content = Math.Round((double)Visual.sector["area_in_use"]
                / (double)Visual.sector["area"]
                * 100, 2) + "%";


            squares_in_use.Content = Visual.squaresInUSe.ToString();
            squaresProgressBar.Value = Visual.squaresInUSe;

            squaresProgressBar.Maximum = Visual.CalCulateSquaresInTotal();

            shelfBuilder.isAShelfBeingCreated = false;
            shelfBuilder.isDeleteBeingUsed = false;
            shelfBuilder.isDesignerModeActive = false;
        }

        public void box_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            shelfBuilder.currentXindex = Grid.GetColumn(button);
            shelfBuilder.currentYindex = Grid.GetRow(button);


            if (shelfBuilder.isAShelfBeingCreated == true && shelfBuilder.isDeleteBeingUsed == false)
            {
                if (shelfBuilder.checkIfTHeSquarefHasAlreadyBeenTaken(button) == false)
                {
                    if (shelfBuilder.newShelf["startXindex"] == DBNull.Value && shelfBuilder.newShelf["startYindex"] == DBNull.Value)
                    {
                        if (Visual.orientation == Orientation.Horizontal)
                        {
                            shelfBuilder.displaySquareAsHorizontalShelf(button, (double)shelfBuilder.newShelf["width"]);
                        }
                        if (Visual.orientation == Orientation.Vertical)
                        {
                            shelfBuilder.displaySquareAsVerticalShelf(button, (double)shelfBuilder.newShelf["width"]);
                        }

                        shelfBuilder.setStartingPointOfShelf(button, shelfBuilder.newShelf["width"].ToString());

                    }
                    else
                    {
                        if (Visual.orientation == Orientation.Horizontal)
                        {
                            if (shelfBuilder.checkIfTheresAshelfToTheRight() == true)
                            {
                                shelfBuilder.addSquareToshelf(button);
                                shelfBuilder.displaySquareAsHorizontalShelf(button, (double)shelfBuilder.newShelf["width"]);

                                shelfBuilder.newShelf["startXindex"] = shelfBuilder.currentXindex;
                            }
                            else if (shelfBuilder.checkifTheresAshelfToTheLeft() == true)
                            {
                                shelfBuilder.addSquareToshelf(button);
                                shelfBuilder.displaySquareAsHorizontalShelf(button, (double)shelfBuilder.newShelf["width"]);


                                shelfBuilder.lastXindex = shelfBuilder.currentXindex;
                            }
                            else
                            {
                                MessageBox.Show("Ezt a plocot csak vízszintesen egymás mellett lévő kockák alkothatják");
                            }
                        }
                        else if (Visual.orientation == Orientation.Vertical)
                        {
                            if (shelfBuilder.checkIfTheresAshelfBelow() == true)
                            {
                                shelfBuilder.addSquareToshelf(button);
                                shelfBuilder.displaySquareAsVerticalShelf(button, (double)shelfBuilder.newShelf["width"]);
                                shelfBuilder.newShelf["startYindex"] = shelfBuilder.currentYindex;


                            }
                            else if (shelfBuilder.checkIfTheresAshelfAbove())
                            {
                                shelfBuilder.addSquareToshelf(button);
                                shelfBuilder.displaySquareAsVerticalShelf(button, (double)shelfBuilder.newShelf["width"]);



                                shelfBuilder.lastYindex = Grid.GetRow(button);

                            }
                            else
                            {

                                MessageBox.Show("Ezt a plocot csak függőlegesen egymás mellett lévő kockák alkothatják");
                            }
                        }

                    }
                    areaProgeressbar.Value = (double)Visual.sector["area_in_use"];

                    areaUsagePercent.Content = Math.Round((double)Visual.sector["area_in_use"]
                        / (double)Visual.sector["area"]
                        * 100, 2) + "%";

                    squares_in_use.Content = Visual.squaresInUSe.ToString();
                    squaresProgressBar.Value = Visual.squaresInUSe;
                }
                //MessageBox.Show("startindex" + shelfBuilder.newShelf["startXindex"]);
                //MessageBox.Show("lastIndex" + shelfBuilder.lastXindex.ToString());
                //MessageBox.Show("currenTindex" + shelfBuilder.currentXindex.ToString());



            }
            else
            {
                if (shelfBuilder.isDeleteBeingUsed == true)
                {
                    if (button.Tag != null)
                    {
                        shelfBuilder.removeSquareFromShelf(button);

                        areaProgeressbar.Value = (double)Visual.sector["area_in_use"];

                        areaUsagePercent.Content = Math.Round((double)Visual.sector["area_in_use"]
                            / (double)Visual.sector["area"]
                            * 100, 2) + "%";

                        squares_in_use.Content = Visual.squaresInUSe.ToString();
                        squaresProgressBar.Value = Visual.squaresInUSe;
                    }
                    else
                    {
                        MessageBox.Show("This square doesnt belong to a shelf");
                    }
                }

            }




        }







        private void New_shelf_Click(object sender, RoutedEventArgs e)
        {
            CreateShelf createShelf = new CreateShelf();
            createShelf.ShowDialog();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            shelfBuilder.isAShelfBeingCreated = false;
            Tables.shelf.database.Rows.Add(shelfBuilder.newShelf);
            
            Tables.sector.updateChanges();

            Tables.shelf.updateChanges();

            changeClickEventToSelect();
            MessageBox.Show("You have created a new shelf");
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

            if (shelfBuilder.isDeleteBeingUsed == false)
            {
                shelfBuilder.isDeleteBeingUsed = true;
                Delete.Background = Brushes.Green;
                Delete.Content = "Done";
            }
            else
            {
                shelfBuilder.isDeleteBeingUsed = false;
                Delete.Background = Brushes.Red;
                Delete.Content = "Delete";
            }



        }

        private void zBox_Loaded(object sender, RoutedEventArgs e)
        {
            zBox.FillToBounds();
        }

        private void Designer_Click(object sender, RoutedEventArgs e)
        {
            if (shelfBuilder.isDesignerModeActive == false)
            {
                Visual.showSquares(boxGrid);
                desinger_viewButton.Content = "Designer view: ON";
                shelfBuilder.isDesignerModeActive = true;

                New_shelf.Visibility = Visibility.Visible;
                Done.Visibility = Visibility.Visible;
                Delete.Visibility = Visibility.Visible;
            }
            else if (shelfBuilder.isAShelfBeingCreated == false && shelfBuilder.isDeleteBeingUsed == false)
            {
                Visual.hideSquaresThatDoNotBelongToAShelf(boxGrid);
                desinger_viewButton.Content = "Designer view: OFF";
                shelfBuilder.isDesignerModeActive = false;

                New_shelf.Visibility = Visibility.Hidden;
                Done.Visibility = Visibility.Hidden;
                Delete.Visibility = Visibility.Hidden;
            }
            else
            {
                MessageBox.Show("Cannot turn off designer view while a shelf is being created or deleted");
            }
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            areaProgeressbar.Value = (double)Visual.sector["area_in_use"];

            areaUsagePercent.Content = Math.Round((double)Visual.sector["area_in_use"]
                / (double)Visual.sector["area"]
                * 100, 2) + "%";


            squares_in_use.Content = Visual.squaresInUSe.ToString();
            squaresProgressBar.Value = Visual.squaresInUSe;

        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if(shelfBuilder.isAShelfBeingCreated == true)
            {
                Tables.sector.database.RejectChanges();

                shelfBuilder.newShelf = null;

                

               Visual.calculateSquaresInUse();
                
                MessageBox.Show(Tables.sector.getShelfs(Visual.sector).Length.ToString());
                
                
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataRow shelf = Tables.shelf.database.Select($"id = '{button.Tag}'")[0];
        }

        private void changeClickEventToSelect()
        {
            foreach (Button children in boxGrid.Children)
            {
                children.Click -= box_Click;
                children.Click += Select_Click;
            }
        }
    }
}
