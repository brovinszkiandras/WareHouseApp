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
using Xceed.Wpf.Toolkit.Primitives;

namespace WH_APP_GUI
{
    public partial class SectorWindow : Page
    {
        #region constructor
        public SectorWindow()
        {
            InitializeComponent();
            //Az oldal datacontexte a kiválasztott szector lesz
            this.DataContext = Visual.sector;


            #region calculate and initailize
            //Kiszámolom a sector mérete alapján a vízszintesen található kockák számát
            Visual.calculaTeNumberOfSquares((double)Visual.sector["length"]);
            ////Kiszámolom a sector mérete alapján a vízszintesen található kockák számát
            Visual.calculateNumberAndSizeOfSquaresVertically((double)Visual.sector["width"]);
            //Kiszámolom a már használatban lévő kockák számát hogy aztán meg tudjam jeleníteni
            Visual.calculateSquaresInUse();

            //Beállítom a megjelenített elemek értékeit
            initalizeUielements();

            //Hozzáadom a kiszámolt mennyiségű sorokat és oszlopokat a polcokat tartalmazó gridhez
            Visual.addRowsAndColumns(boxGrid);
            #endregion

            #region show standard squares
            //Végigmegyek a grid oszlopain
            for (int i = 0; i < boxGrid.ColumnDefinitions.Count; i++)
            {
                //Végigmegyek az oszlop sorain
                for (int j = 0; j < boxGrid.RowDefinitions.Count; j++)
                {
                    //Minen sorban létrehozok egy gombot
                    Button button = new Button();
                    //Megadok neki egy üres stylet hogy ne a metró kinézetét örökölje
                    button.Style = (Style)this.Resources["RectangleButtonStyle"];
                    //Megadom a click eventet
                    button.Click += box_Click;
                    //Megadom a gombok kinézetét
                    button.BorderBrush = Brushes.Black;
                    button.BorderThickness = new System.Windows.Thickness(0.03);
                    button.Background = new SolidColorBrush(Color.FromArgb(0, 0xCE, 0xA2, 0xFF));

                    //Beállítom a gombok méretét a max méretük felére a kinézet miatt
                    button.Height = (double)Visual.sizeVertically / 2;
                    button.Width = (double)Visual.sizeHorizontally / 2;
                    //Beállítom a gomb oszlopának a jelenlegi oszlopot
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    //Hozzáadom a gridhez a gombot
                    boxGrid.Children.Add(button);
                }
            }
            #endregion

            #region show shelfs
            //Végigmegyek a kiválasztott sector polcain
            foreach (DataRow row in Tables.sector.getShelfs(Visual.sector))
            {
                //Hogyha a polc horizontálisan helyezkedik el akkor kirajzolom horizontálisan
                if (row["orientation"].ToString() == "Horizontal")
                {

                    displayHorizontalShelf(boxGrid, row);

                }
                //Hogyha a polc vízszintesen helyezkedik el akkor kirajzolom horizontálisan
                else if (row["orientation"].ToString() == "Vertical")
                {

                    DisplayVerticalShelf(boxGrid, row);

                }

            }
            //Eltűntetek minden olyan kockát ami nem tartozik egy polchoz
            Visual.hideSquaresThatDoNotBelongToAShelf(boxGrid);
            #endregion
        }
        #endregion

        #region box_click
        //Gomb hozzáadása vagy törlése a polcból
        public void box_Click(object sender, RoutedEventArgs e)
        {
            //Létrehozk egy gombot ami eggyenlő a megnyomott gombbal
            Button button = e.Source as Button;
            //Beállítom a jelenlegi pozicíót a gomb sorára és oszlopára
            shelfBuilder.currentXindex = Grid.GetColumn(button);
            shelfBuilder.currentYindex = Grid.GetRow(button);
            
            //Megnézem hoyg egy polc készítése folyamatban van e
            if (shelfBuilder.isAShelfBeingCreated == true && shelfBuilder.isDeleteBeingUsed == false)
            {
                //Leelenőrzöm hogyha a megnyomott gomb már egy polchoz tartozik hozzá akkor nem adom hozzá a jelenleg készülő polchoz
                if (shelfBuilder.checkIfTHeSquarefHasAlreadyBeenTaken(button) == false)
                {
                    //Ha a jelenleg készülő polcnak nincs kezdő pozicíója azaz most kezdődik a készítése akkor beállítom a kezdő értékeket
                    if (shelfBuilder.newShelf["startXindex"] == DBNull.Value && shelfBuilder.newShelf["startYindex"] == DBNull.Value)
                    {
                        //Ha horizontális a polc kirajzolom a megnyomott gombot vízsintesen
                        if (Visual.orientation == Orientation.Horizontal)
                        {
                            shelfBuilder.displaySquareAsHorizontalShelf(button, (double)shelfBuilder.newShelf["width"]);
                        }
                        //Ha vertikális a polc kirajzolom a megnyomott gombot vízsintesen
                        if (Visual.orientation == Orientation.Vertical)
                        {
                            shelfBuilder.displaySquareAsVerticalShelf(button, (double)shelfBuilder.newShelf["width"]);
                        }

                        //Beálíítom a jelenlegi pozicíót a polc kezdőpontjának
                        shelfBuilder.setStartingPointOfShelf(button);

                    }
                    //Ha a polcnak már van kezdő pozicíója
                    else
                    {
                        //Ha a polc vízszintes
                        if (Visual.orientation == Orientation.Horizontal)
                        {
                            //Megnézem hogy a megnyomott gomb a készülő polc bal oldalán van e ha igen
                            if (shelfBuilder.checkIfTheresAshelfToTheRight() == true)
                            {
                                //Hozzáadom a gombot a polchoz
                                shelfBuilder.addSquareToshelf(button);
                                //Öszzekötöm a gombot a polccal
                                shelfBuilder.displaySquareAsHorizontalShelf(button, (double)shelfBuilder.newShelf["width"]);

                                //Frissítem a polc kezdő pozicíóját a jelenlegi pozicíóra (balról jobbra)
                                shelfBuilder.newShelf["startXindex"] = shelfBuilder.currentXindex;
                            }
                            //Megnézem hogy a megnyomott gomb a készülő polc jobb oldalán van e ha igen
                            else if (shelfBuilder.checkifTheresAshelfToTheLeft() == true)
                            {
                                //Hozzáadom a gombot a polchoz
                                shelfBuilder.addSquareToshelf(button);
                                //Öszzekötöm a gombot a polccal
                                shelfBuilder.displaySquareAsHorizontalShelf(button, (double)shelfBuilder.newShelf["width"]);

                                //Frissítem a polc befejező pozicíóját a jelenlegi pozicíóra (balról jobbra)
                                shelfBuilder.lastXindex = shelfBuilder.currentXindex;
                            }
                            //Ha a megnyomott gomb se a polc baloldalán se a polc jobb oldalán nincsen
                            //(vízszintesen nincs mellette) hiba üzenetet írok ki
                            else
                            {
                                MessageBox.Show("Ezt a plocot csak vízszintesen egymás mellett lévő kockák alkothatják");
                            }
                        }
                        //Ha a polc vízszintes
                        else if (Visual.orientation == Orientation.Vertical)
                        {
                            //Megnézem hogy a megnyomott gomb a készülő polc fölött van e ha igen
                            if (shelfBuilder.checkIfTheresAshelfBelow() == true)
                            {
                                //Hozzáadom a gombot a polchoz
                                shelfBuilder.addSquareToshelf(button);
                                //Frissítem a polc befejező pozicíóját a jelenlegi pozicíóra (balról jobbra)
                                shelfBuilder.displaySquareAsVerticalShelf(button, (double)shelfBuilder.newShelf["width"]);
                                //Frissítem a polc kezdő pozicíóját a jelenlegi pozicíóra (felüről lefele)
                                shelfBuilder.newShelf["startYindex"] = shelfBuilder.currentYindex;


                            }
                            //Megnézem hogy a megnyomott gomb a készülő polc alatt van e ha igen
                            else if (shelfBuilder.checkIfTheresAshelfAbove())
                            {
                                //Hozzáadom a gombot a polchoz
                                shelfBuilder.addSquareToshelf(button);
                                //Frissítem a polc befejező pozicíóját a jelenlegi pozicíóra (balról jobbra)
                                shelfBuilder.displaySquareAsVerticalShelf(button, (double)shelfBuilder.newShelf["width"]);


                                //Frissítem a polc befejező pozicíóját a jelenlegi pozicíóra (felüről lefele)
                                shelfBuilder.lastYindex = Grid.GetRow(button);

                            }
                            //Ha a megnyomott gomb se a polc fölött se a polc alatt nincsen
                            //(függőlegesen nincs mellette) hiba üzenetet írok ki
                            else
                            {
                                MessageBox.Show("Ezt a plocot csak függőlegesen egymás mellett lévő kockák alkothatják");
                            }
                        }
                        //Frissítem a megjelenítő ui elementeket
                        UpdateUiElemets();
                    }
                }
            }
            //Ha egy polc nem készül
            #region delete functiom
            else
            {
                //Ha törlés funkció nincs bekapcsolva
                if (shelfBuilder.isDeleteBeingUsed == true)
                {
                    //Ha a megnyomott gom tartoziik egy polchoz
                    if (button.Tag != null)
                    {
                        //Eltávolítom a gombot a polcról
                        shelfBuilder.removeSquareFromShelf(button);

                        UpdateUiElemets();
                    }
                    //Ha nem akkor nem törlöm ki
                    else
                    {
                        MessageBox.Show("This square doesnt belong to a shelf");
                    }
                }
            }
            #endregion
        }
        #endregion

        #region create shelf window
        //Új polc készítése gomb
        private void New_shelf_Click(object sender, RoutedEventArgs e)
        {
            //Megváltoztatom a gomb onclik funkcióját
            changeClickEventToBoxClick();

            //Megynitom a shelf creator windowt
            CreateShelf createShelf = new CreateShelf();
            //Beállítom a CreateShelf_Closed funkciót
            createShelf.Closed += CreateShelf_Closed; 
            createShelf.ShowDialog();
        }

        //Funkció amikor a createshelf window bezáródik
        private void CreateShelf_Closed(object sender, EventArgs e)
        {
            if(shelfBuilder.isAShelfBeingCreated == true)
            {
                showShelfInfo(shelfBuilder.newShelf);
                Delete.Visibility = Visibility.Visible;
            }
        }
        #endregion
        //Befejezem a polc készítését

        #region shelf display
        //Shelf készítés befejezése
        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if(shelfBuilder.isDeleteBeingUsed == false)
            {
                #region save data
                //Át állítom a változót ami egy polc készítését jelzi
                shelfBuilder.isAShelfBeingCreated = false;
                //Hogyha a polc még nem volt hozzáadva a polcok táblához akkor hozzáadom
                MessageBox.Show(shelfBuilder.newShelf.RowState.ToString());
                if (shelfBuilder.newShelf.RowState == DataRowState.Detached)
                {
                    Tables.shelf.database.Rows.Add(shelfBuilder.newShelf);
                    MessageBox.Show("You have created a new shelf");
                }
                //Ha már hozzá volt adva de meg az user megváltoztatta
                else if (shelfBuilder.newShelf.RowState == DataRowState.Modified)
                {
                    MessageBox.Show($"You have updated shelf {shelfBuilder.newShelf["name"]}");

                    foreach (Button children in boxGrid.Children)
                    {
                        if (children.Tag != null)
                        {
                            if (children.Tag.ToString() == shelfBuilder.newShelf["name"].ToString())
                            {
                                children.Background = Brushes.DarkMagenta;
                            }
                        }
                    }
                    toolPanel.Visibility = Visibility.Collapsed;
                    ProductsSRCW.Visibility = Visibility.Visible;
                }
                else if(shelfBuilder.newShelf.RowState == DataRowState.Unchanged)
                {
                    foreach (Button children in boxGrid.Children)
                    {
                        if (children.Tag != null)
                        {
                            if (children.Tag.ToString() == shelfBuilder.newShelf["name"].ToString())
                            {
                                children.Background = Brushes.DarkMagenta;
                            }
                        }
                    }
                    toolPanel.Visibility = Visibility.Collapsed;
                    ProductsSRCW.Visibility = Visibility.Visible;
                }

                //Frissítem a kiválasztott sectort az adatbázisban
                Tables.sector.updateChanges();

                //Feltöltöm a polcot ad adatbázisba
                Tables.shelf.updateChanges();
                #endregion
                //Megváltoztatom a polcok click eventjét select clickre
                #region ui_elements
                Delete.Visibility = Visibility.Collapsed;
                changeClickEventToSelect();
                shelfInfoSPNL.Visibility = Visibility.Collapsed;
                #endregion
            }
            else
            {
                MessageBox.Show("Please turn offf delete mode");
            }
            
        }

        //Bekapcsolja és kikapcsolja a törlés módot
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            //Ha ki van kapcsolva a törlés mód bekapcsolja
            if (shelfBuilder.isDeleteBeingUsed == false)
            {
                shelfBuilder.isDeleteBeingUsed = true;
                Delete.Background = Brushes.Green;
                Delete.Content = "Done";
            }
            //Ha be van kapcsolva a törlés mód bekapcsolja
            else
            {
                shelfBuilder.isDeleteBeingUsed = false;
                Delete.Background = Brushes.Red;
                Delete.Content = "Delete";
                //Frissíti az adatbázist
                Tables.shelf.updateChanges();
            }



        }

        //Ki meg bekapcsolja a designert (azoknak a gomboknak is a megjeleítése
        //amik nem tartoznak polcokhoz) 
        private void Designer_Click(object sender, RoutedEventArgs e)
        {
            //Ha ki van kapcsolva a designer mód bekapcsolja és megjeleníti a designer mód gombokat
            if (shelfBuilder.isDesignerModeActive == false)
            {
                Visual.showSquares(boxGrid);
                desinger_viewButton.Content = "Designer view: ON";
                shelfBuilder.isDesignerModeActive = true;

                if(shelfBuilder.isAShelfBeingCreated == false)
                {
                    New_shelf.Visibility = Visibility.Visible;
                }
                Done.Visibility = Visibility.Visible;
                
            }
            //Ha be van kapcsolva a designer mód és egyik desinger funkció sincs használatban
            //(törlés, hozzáadás, editelés) kikapcsolja és elrejti a designer mód gombokat
            else if (shelfBuilder.isAShelfBeingCreated == false && shelfBuilder.isDeleteBeingUsed == false)
            {
                Visual.hideSquaresThatDoNotBelongToAShelf(boxGrid);
                desinger_viewButton.Content = "Designer view: OFF";
                shelfBuilder.isDesignerModeActive = false;

                if (shelfBuilder.isAShelfBeingCreated == false)
                {
                    New_shelf.Visibility = Visibility.Hidden;
                }
                Done.Visibility = Visibility.Hidden;
                Delete.Visibility = Visibility.Hidden;
            }
            //Ha megporóbálja kikapcsolni a desinger módot amíg használatban van,
            //nem kapcsolja ki és értesíti az usert
            else
            {
                MessageBox.Show("Cannot turn off designer view while a shelf is being created or deleted");
            }
        }
        
        //Polc megváltoztatása
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            DataRow shelf = productsPanel.DataContext as DataRow;
            foreach (Button children in boxGrid.Children)
            {
                //Beállítom a háttérszűket fehérre
                if(children.Tag != null)
                {
                    if (children.Tag.ToString() == shelf["name"].ToString())
                    {
                        children.Background = Brushes.White;
                    }
                }
            }

            //Megváltoztatom polchoz tartozó gombok click eventjét a box clickre
            changeClickEventToBoxClick();
            //Bekapcsolom a shelf hozzáadása módot
            shelfBuilder.isAShelfBeingCreated = true;
            //Beállítom a készülő polcnak a kiválasztott polcot
            shelfBuilder.newShelf = shelf;

            //Beállítom a polc utolsó pozicíóját a polc kezdő pozicíója + a polc hosszúságára
            if (shelf["orientation"].ToString() == "Horizontal")
            {
                shelfBuilder.lastXindex = (int)shelf["startXindex"] + (int)(double)shelf["length"] - 1;
                Visual.orientation = Orientation.Horizontal;
            }
            else if (shelf["orientation"].ToString() == "Vertical")
            {
                shelfBuilder.lastYindex = (int)shelf["startYindex"] + (int)(double)shelf["length"] - 1;
                Visual.orientation = Orientation.Vertical;
            }

            Delete.Visibility = Visibility.Visible;
            ProductsSRCW.Visibility = Visibility.Collapsed;
            toolPanel.Visibility = Visibility.Visible;
            showShelfInfo(shelfBuilder.newShelf);
        }
        
        //Vízszintes polcok megjelenítése adatbázísból
        private void displayHorizontalShelf(Grid boxGrid, DataRow row)
        {
            //A gombok mennyisége ami a polcban található
            double numberOfSquaresToFind = (double)row["length"];

            //Végigmegyek a polcokat tároló grid elemein
            int index = 0;
            while (index < boxGrid.Children.Count && numberOfSquaresToFind != 0)
            {
                //Gombbá alakítom őket
                Button square = (Button)boxGrid.Children[index];
                //Minden gombnál elindul egy for cilkus ami polc kezdő pozicíójátol
                //a polc hosszúságig megy (az összes X pozició polcban)
                for (int i = (int)row["startXindex"]; i < (int)row["startXindex"] + (double)row["length"]; i++)
                {
                    //Ha az éppen vizsgált gomb poziciója egyezik az egyik pozicíóval a polcban akkor
                    //összekapcsolja a polccal (a gomb sora egyezik a polc kezdő Y pozicíójával
                    //és az X pozició egyezik a jelenleg vizsált X poziciíóval)
                    if (Grid.GetRow(square) == (int)row["startYindex"] && Grid.GetColumn(square) == i)
                    {
                        square.Background = Brushes.DarkMagenta;
                        square.BorderThickness = new System.Windows.Thickness(0);
                        square.Width = Visual.sizeHorizontally;
                        square.Height = (double)row["width"];
                        square.Tag = row["name"];
                        square.Click -= box_Click;
                        square.Click += Select_Click;

                        //Megtalálandó gombok számát csökkenti
                        numberOfSquaresToFind--;
                    }
                }
                index++;
            }
        }

        //Függőleges polcok megjelenítése adatbázísból
        private void DisplayVerticalShelf(Grid boxGrid, DataRow row)
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
                        square.Click -= box_Click;
                        square.Click += Select_Click;

                        square.Tag = row["name"];
                        numberOfSquaresToFind--;
                    }

                }
                index++;
            }
        }
        #endregion

        #region ui
        //Ha a zoombox betöltött
        private void zBox_Loaded(object sender, RoutedEventArgs e)
        {
            zBox.FillToBounds();
        }

        //Akkor fut le ha ezt az oldalt elhagyják
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //Ha úgy hagyja el az oldalt hogy egy shelf készülőben van
            if(shelfBuilder.isAShelfBeingCreated == true)
            {
                //Kitörlöm az adatbázisból a változásokat mert nem mentett a felhasznákó
                Tables.sector.database.RejectChanges();

                shelfBuilder.newShelf.RejectChanges();
                //newshelfet üresre állítom
                shelfBuilder.newShelf = null;

                
               //újra ki számolom a polchoz tartozó kockák számát 
               Visual.calculateSquaresInUse();
                
                //MessageBox.Show(Tables.sector.getShelfs(Visual.sector).Length.ToString());
            }
        }

        private void changeClickEventToSelect()
        {
            //Eltávolítom a box click eventet(shelfek létrehozása) és hozzáadom
            //a select click eventet(egy shelf kiválasztása) a gombokhoz 
            foreach (Button children in boxGrid.Children)
            {
                if(children.Tag != null)
                {
                    children.Click -= box_Click;
                    children.Click += Select_Click;
                }
            }
        }

        private void changeClickEventToBoxClick()
        {
            foreach (Button children in boxGrid.Children)
            {
                if(children.Tag != null)
                {
                    children.Click -= Select_Click;
                    children.Click += box_Click;
                }
                
            }
        }

        //Beállítja a kezdetleges értékeit az ui elementeknek
        private void initalizeUielements()
        {
            areaProgeressbar.Value = (double) Visual.sector["area_in_use"];

        areaUsagePercent.Content = Math.Round((double) Visual.sector["area_in_use"]
                / (double) Visual.sector["area"]
                * 100, 2) + "%";

            Visual.calculateSquaresInUse();
            squares_in_use.Content = Visual.squaresInUSe.ToString();
            squaresProgressBar.Value = Visual.squaresInUSe;

            squaresProgressBar.Maximum = Visual.CalCulateSquaresInTotal();

            shelfBuilder.isAShelfBeingCreated = false;
            shelfBuilder.isDeleteBeingUsed = false;
            shelfBuilder.isDesignerModeActive = false;

            Visual.initalizeGrid(boxGrid);
        }

        //Frissítí az ui elementeket
        private void UpdateUiElemets()
        {
            areaProgeressbar.Value = (double)Visual.sector["area_in_use"];

            areaUsagePercent.Content = Math.Round((double)Visual.sector["area_in_use"]
                / (double)Visual.sector["area"]
                * 100, 2) + "%";

            squares_in_use.Content = Visual.squaresInUSe.ToString();
            squaresProgressBar.Value = Visual.squaresInUSe;

            if(shelfBuilder.newShelf != null)
            {
                Size.Content = $"{shelfBuilder.newShelf["width"]}x{shelfBuilder.newShelf["actual_length"]} m2";
            }
        }

        //Megmutatja egy polc adatait
        private void showShelfInfo(DataRow shelf)
        {
            //Megmutatom az információkat a készülő polcról
            shelfInfoSPNL.Visibility = Visibility.Visible;
            //Hogyha egy polc készül készülőben van (a createshelf oldalon sikeresen elkezdte
            //az user a polc készítését)
            
               
                //Megmutatom a polc adatait
                
                shelfName.Content = shelf["name"].ToString();
                Size.Content = $"{shelf["width"]}x{shelf["actual_length"]} m2";
                shelfOrientation.Content = shelf["orientation"].ToString();
            
        }
        #endregion

        #region products
        //Kiválasztok egy polcot
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Selected");
            Button button = (Button)sender;

            //Kiválasztom azt a shelfet aminek a neve a gomb azonosítója
            DataRow shelf = Tables.shelf.database.Select($"name = '{button.Tag}'")[0];

            //Végigmegyek azokon a gombokon amiknek az azonosítója a gomb neve
        
            showShelfInfo(shelf);
            toolPanel.Visibility = Visibility.Collapsed;
            ProductsSRCW.Visibility = Visibility.Visible;
            showProducts(shelf);
        }

        private void showProducts(DataRow shelf)
        {
            productsPanel.Children.Clear();
            productsPanel.DataContext = shelf;
            warehouse warehouseTable = Tables.getWarehosue(Tables.shelf.getWarehouse(shelf)["name"].ToString());
            for (int i = 1; i <= (int)shelf["number_of_levels"]; i++)
            {
                #region levelLabel
                StackPanel levelPanel = new StackPanel();
                levelPanel.Orientation = Orientation.Horizontal;
                productsPanel.Children.Add(levelPanel);

                Label level_of_shelf = new Label();
                level_of_shelf.FontSize = 17;
                level_of_shelf.Content = "Level " + i;
                levelPanel.Children.Add(level_of_shelf);
                

                DataRow[] productsOfLevel = Tables.shelf.getWarehouseProducts(shelf)
                    .Where(product => (int)product["on_shelf_level"] == i)
                    .ToArray();

                if(i == (int)shelf["number_of_levels"])
                {
                    Button deleteLevel = new Button();
                    deleteLevel.Background = Brushes.Crimson;
                    deleteLevel.FontSize = 17;
                    deleteLevel.Content = "Delete";
                    deleteLevel.Tag = shelf;
                    deleteLevel.Click += deleteLevel_Click;
                    levelPanel.Children.Add(deleteLevel);
                }
                #endregion

                if (productsOfLevel.Length > 0)
                {
                    foreach (DataRow warehouseProduct in productsOfLevel)
                    {
                        DataRow product = warehouseTable.getProduct(warehouseProduct);

                        #region showProductInfo
                        Expander expander = new Expander();
                        expander.Header = $"{product["name"]} x {warehouseProduct["qty"]}";
                        expander.Margin = new Thickness(0, 0, 2, 0);
                        productsPanel.Children.Add(expander);

                        StackPanel productinfo = new StackPanel();
                        expander.Content = productinfo;

                        if(Tables.features.isFeatureInUse("Storage") == true)
                        {
                            TextBlock widthLBL = new TextBlock();
                            widthLBL.Text = "Width";
                            widthLBL.Style = (Style)this.Resources["LabelTextblockStyle"];
                            productinfo.Children.Add(widthLBL);

                            TextBlock width = new TextBlock();
                            width.Text = warehouseProduct["width"].ToString();
                            width.Style = (Style)this.Resources["ValueTextblockStyle"];
                            productinfo.Children.Add(width);

                            TextBlock heightLBL = new TextBlock();
                            heightLBL.Text = "Height";
                            heightLBL.Style = (Style)this.Resources["LabelTextblockStyle"];
                            productinfo.Children.Add(heightLBL);

                            TextBlock height = new TextBlock();
                            height.Text = warehouseProduct["height"].ToString();
                            height.Style = (Style)this.Resources["ValueTextblockStyle"];
                            productinfo.Children.Add(height);

                            TextBlock lengthLBL = new TextBlock();
                            lengthLBL.Text = "Length";
                            lengthLBL.Style = (Style)this.Resources["LabelTextblockStyle"];
                            productinfo.Children.Add(lengthLBL);

                            TextBlock length = new TextBlock();
                            length.Text = warehouseProduct["length"].ToString();
                            length.Style = (Style)this.Resources["ValueTextblockStyle"];
                            productinfo.Children.Add(length);
                        }

                        TextBlock isInBoxLBL = new TextBlock();
                        isInBoxLBL.Text = "Is in a box";
                        isInBoxLBL.Style = (Style)this.Resources["LabelTextblockStyle"];
                        productinfo.Children.Add(isInBoxLBL);

                        TextBlock isInBox = new TextBlock();
                        isInBox.Text = warehouseProduct["is_in_box"].ToString();
                        isInBox.Style = (Style)this.Resources["ValueTextblockStyle"];
                        productinfo.Children.Add(isInBox);
                        #endregion
                    }
                }
            }
        }

        private void deleteLevel_Click(object sender, RoutedEventArgs e)
        {
            DataRow shelf = productsPanel.DataContext as DataRow;
            int lastLevelOfShelf = (int)shelf["number_of_levels"];

            DataRow[] productsOfLevel = Tables.shelf.getWarehouseProducts(shelf)
                    .Where(product => (int)product["on_shelf_level"] == lastLevelOfShelf)
                    .ToArray();

            if(productsOfLevel.Length == 0)
            {
                shelf["number_of_levels"] = (int)shelf["number_of_levels"] - 1;
                Tables.shelf.updateChanges();
                productsPanel.Children.Clear();
                showProducts(shelf);
            }
            else
            {
                MessageBox.Show("Cant delete level while there are still products on it");
            }
        }

        private void add_level_Click(object sender, RoutedEventArgs e)
        {
            DataRow shelf = productsPanel?.DataContext as DataRow;

            shelf["number_of_levels"] = (int)shelf["number_of_levels"] + 1;
            Tables.shelf.updateChanges();
            productsPanel.Children.Clear();
            showProducts(shelf);
        }


        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            ProductsSRCW.Visibility = Visibility.Collapsed;
            toolPanel.Visibility = Visibility.Visible;
        }
        #endregion

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Navigation.PreviousPage != null)
            {
                Navigation.OpenPage(Navigation.PreviousPage.GetType());
            }
            else
            {
                Navigation.OpenPage(Navigation.GetTypeByName("InspectWarehouse"));
            }
        }
    }
}
