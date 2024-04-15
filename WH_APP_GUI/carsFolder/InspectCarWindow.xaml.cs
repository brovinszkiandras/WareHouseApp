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

namespace WH_APP_GUI.carsFolder
{
    /// <summary>
    /// Interaction logic for InspectCarWindow.xaml
    /// </summary>
    public partial class InspectCarWindow : Window
    {
        public DataRow car;
        public InspectCarWindow(DataRow Car)
        {
            this.car = Car;
            this.DataContext = Car;
            InitializeComponent();

            if ((bool)Tables.features.isFeatureInUse("Storage") == true)
            {
                addStoreFeautereElements();
            }
            if ((bool)Tables.features.isFeatureInUse("Fuel") == true)
            {
                addFuelFeautereElements();
            }
        }

        public void addStoreFeautereElements()
        {

            Border border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.White;


            StackPanel stackPanel = new StackPanel();
            stackPanel.Background = Brushes.Green;
            stackPanel.Orientation = Orientation.Horizontal;

            Label label = new Label();
            label.FontSize = 17;
            label.Foreground = Brushes.White;
            label.Content = "Storage (cm2): ";

            stackPanel.Children.Add(label);


            Binding storageBinding = new Binding("[storage]");

            TextBlock storage = new TextBlock();
            storage.FontSize = 17;
            storage.Foreground = Brushes.White;
            storage.VerticalAlignment = VerticalAlignment.Center;
            storage.SetBinding(TextBlock.TextProperty, storageBinding);
            storage.Name = "storage";

            stackPanel.Children.Add(storage);

            border.Child = stackPanel;
            carsPanel.Children.Add(border);


            Border border2 = new Border();
            border2.BorderThickness = new Thickness(1);
            border2.BorderBrush = Brushes.White;

            StackPanel stackPanel2 = new StackPanel();
            stackPanel2.Orientation = Orientation.Horizontal;
            stackPanel2.Background = Brushes.Green;

            Label label2 = new Label();
            label2.FontSize = 17;
            label2.Foreground = Brushes.White;
            label2.Content = "Carrying capacity (kgm): ";
            stackPanel2.Children.Add(label2);


            Binding carrryingCapacityBinding = new Binding("[carrying_capacity]");

            TextBlock carrying_capacity = new TextBlock();
            carrying_capacity.FontSize = 17;
            carrying_capacity.Foreground = Brushes.White;
            carrying_capacity.VerticalAlignment = VerticalAlignment.Center;
            carrying_capacity.SetBinding(TextBlock.TextProperty, carrryingCapacityBinding);
            carrying_capacity.Name = "carrying_capacity";

            stackPanel2.Children.Add(carrying_capacity);

            border2.Child = stackPanel2;
            carsPanel.Children.Add(border2);

            Tables.cars.database.Columns["storage"].AllowDBNull = false;
            Tables.cars.database.Columns["carrying_capacity"].AllowDBNull = false;
        }

        public void addFuelFeautereElements()
        {

            Border border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.White;


            StackPanel stackPanel = new StackPanel();
            stackPanel.Background = Brushes.Green;
            stackPanel.Orientation = Orientation.Horizontal;

            Label label = new Label();
            label.FontSize = 17;
            label.Foreground = Brushes.White;
            label.Content = "consumption (liter/hour): ";

            stackPanel.Children.Add(label);


            Binding consumptionBinding = new Binding("[consumption]");

            TextBlock consumption = new TextBlock();
            consumption.FontSize = 17;
            consumption.Foreground = Brushes.White;
            consumption.VerticalAlignment = VerticalAlignment.Center;
            consumption.SetBinding(TextBlock.TextProperty, consumptionBinding);
            consumption.Name = "consumption";

            stackPanel.Children.Add(consumption);

            border.Child = stackPanel;
            carsPanel.Children.Add(border);


            Border border2 = new Border();
            border2.BorderThickness = new Thickness(1);
            border2.BorderBrush = Brushes.White;

            StackPanel stackPanel2 = new StackPanel();
            stackPanel2.Orientation = Orientation.Horizontal;
            stackPanel2.Background = Brushes.Green;

            Label label2 = new Label();
            label2.FontSize = 17;
            label2.Foreground = Brushes.White;
            label2.Content = "Gas tank (liter): ";
            stackPanel2.Children.Add(label2);


            Binding gasTankBinding = new Binding("[gas_tank_size]");

            TextBlock gas_tank_size = new TextBlock();
            gas_tank_size.FontSize = 17;
            gas_tank_size.Foreground = Brushes.White;
            gas_tank_size.VerticalAlignment = VerticalAlignment.Center;
            gas_tank_size.SetBinding(TextBlock.TextProperty, gasTankBinding);
            gas_tank_size.Name = "gas_tank_size";

            stackPanel2.Children.Add(gas_tank_size);

            border2.Child = stackPanel2;
            carsPanel.Children.Add(border2);

            Tables.cars.database.Columns["consumption"].AllowDBNull = false;
            Tables.cars.database.Columns["gas_tank_size"].AllowDBNull = false;
        }
    }
}
