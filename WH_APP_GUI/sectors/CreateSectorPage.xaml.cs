﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.Primitives;

namespace WH_APP_GUI.sectors
{
    /// <summary>
    /// Interaction logic for CreateSectorPage.xaml
    /// </summary>
    public partial class CreateSectorPage : Page
    {
        DataRow sector;
        DataRow Warehouse;
        public CreateSectorPage(DataRow Warehouse)
        {
            InitializeComponent();
            sector = Tables.sector.database.NewRow();
            this.DataContext = sector;

            name.ValueDataType = typeof(string);
            length.ValueDataType = typeof(double);
            width.ValueDataType = typeof(double);

   
            length.MaxValue = (double)50;
            width.MaxValue = (double)50; 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           bool hasError = false;
            foreach (object element in sectorPanel.Children)
            {
               if(element.GetType() == typeof(ValueRangeTextBox))
                {
                    ValueRangeTextBox valueRangeTextBox = (ValueRangeTextBox)element;
                    hasError = Validation.ValidateTextbox(valueRangeTextBox, sector);
                }

               if(hasError == true)
                {
                    break;
                }
            }
            if(hasError == false) {
               
                sector["area"] = (double)sector["length"] * (double)sector["width"];
                sector["warehouse_id"] = Warehouse["id"];
                sector["area_in_use"] = 0.00;
                Tables.sector.database.Rows.Add(sector);

                Tables.sector.updateChanges();
                MessageBox.Show($"You have succesfully created a new sector", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                if (Navigation.PreviousPage != null)
                {
                    Navigation.OpenPage(Navigation.PreviousPage.GetType());
                }

                //sectorIndexWindow sectorIndexWindow = new sectorIndexWindow();
                //Navigation.content2.Navigate(sectorIndexWindow);
            }
        }
        private void CreateSectorPaage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
            headlabel.FontSize = e.NewSize.Height * 0.06;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.GetTypeByName("sectorIndexWindow"));
        }
    }
}
