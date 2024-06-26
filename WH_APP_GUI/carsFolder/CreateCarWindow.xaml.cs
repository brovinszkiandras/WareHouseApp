﻿using ControlzEx.Standard;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
using WH_APP_GUI.Warehouse;
using Xceed.Wpf.Toolkit.Primitives;

namespace WH_APP_GUI.carsFolder
{
    public partial class CreateCarWindow : Page
    {
        public DataRow car;
        public CreateCarWindow()
        {
            InitializeComponent();
            Ini_warehouse_id();

            car = Tables.cars.database.NewRow();
            plate_number.ValueDataType = typeof(string);
            type.ValueDataType = typeof(string);
            km.ValueDataType = typeof(double);
   
            car["ready"] = true;
            this.DataContext = car;

            DataRow storageFeature = Tables.features.database.Select("name = 'Storage'")[0];
            if (storageFeature != null)
            {
                if ((bool)storageFeature["in_use"] == true)
                {
                    addStoreFeautereElements();
                }
            }

            DataRow fuelFeautore = Tables.features.database.Select("name = 'Fuel'")[0];
            if (fuelFeautore != null)
            {
                if ((bool)fuelFeautore["in_use"] == true)
                {
                    addFuelFeautereElements();
                }
            }
        }
        private Dictionary<string, DataRow> warehouse_id_Dictionary = new Dictionary<string, DataRow>();
        private void Ini_warehouse_id()
        {
            warehouse_id.Visibility = Visibility.Visible;
            warehouse_id.Items.Clear();
            warehouse_id_Dictionary.Clear();

            foreach (DataRow warehouse in Tables.warehouses.database.Rows)
            {
                warehouse_id.Items.Add(warehouse["name"].ToString());
                warehouse_id_Dictionary.Add(warehouse["name"].ToString(), warehouse);
            }
        }
        public void addStoreFeautereElements()
        {
            RowDefinition definition = new RowDefinition();
            definition.Height = GridLength.Auto;
            carsGrid.RowDefinitions.Add(definition);
            StackPanel stackPanel = new StackPanel();
            Grid.SetRow(stackPanel, 7);
            Label label = new Label();
            label.Style = (Style)this.Resources["labelstyle"];
            label.Content = "Storage (cm2)";
            stackPanel.Children.Add(label);

            Binding storageBinding = new Binding("[storage]");

            ValueRangeTextBox storage = new ValueRangeTextBox();
            storage.FontFamily = new FontFamily("Baskerville Old Face");
            storage.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xCE, 0xA2));
            storage.Background = new SolidColorBrush(Color.FromRgb(0x39, 0x52, 0x50));
            storage.BorderBrush = Brushes.Black;
            storage.Margin = new Thickness(5);
            storage.ValueDataType = typeof(Decimal);
            storage.MaxLength = 13;
            storage.MaxLines = 13;
            storage.SetBinding(ValueRangeTextBox.TextProperty, storageBinding);
            storage.Name = "storage";

            stackPanel.Children.Add(storage);

            carsGrid.Children.Add(stackPanel);

            RowDefinition definition2 = new RowDefinition();
            definition2.Height = GridLength.Auto;
            carsGrid.RowDefinitions.Add(definition2);
            StackPanel stackPanel2 = new StackPanel();
            Grid.SetRow(stackPanel2, 8);
            Label label2 = new Label();
            label2.Style = (Style)this.Resources["labelstyle"];
            label2.Content = "Carrying capacity (kgm)";
            stackPanel2.Children.Add(label2);


            Binding carrryingCapacityBinding = new Binding("[carrying_capacity]");

            ValueRangeTextBox carrying_capacity = new ValueRangeTextBox();
            carrying_capacity.FontFamily = new FontFamily("Baskerville Old Face");
            carrying_capacity.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xCE, 0xA2));
            carrying_capacity.Background = new SolidColorBrush(Color.FromRgb(0x39, 0x52, 0x50));
            carrying_capacity.BorderBrush = Brushes.Black;
            carrying_capacity.Margin = new Thickness(5);
            carrying_capacity.ValueDataType = typeof(Decimal);
            carrying_capacity.MaxLength = 13;
            carrying_capacity.MaxLines = 13;
            carrying_capacity.SetBinding(ValueRangeTextBox.TextProperty, carrryingCapacityBinding);
            carrying_capacity.Name = "carrying_capacity";

            stackPanel2.Children.Add(carrying_capacity);

            carsGrid.Children.Add(stackPanel2);

            Tables.cars.database.Columns["storage"].AllowDBNull = false;
            Tables.cars.database.Columns["carrying_capacity"].AllowDBNull = false;
        }

        public void addFuelFeautereElements()
        {
            RowDefinition definition = new RowDefinition();
            definition.Height = GridLength.Auto;
            carsGrid.RowDefinitions.Add(definition);
            StackPanel stackPanel = new StackPanel();
            Grid.SetRow(stackPanel, 9);
            Label label = new Label();
            label.Style = (Style)this.Resources["labelstyle"];
            label.Content = "Consumption (liter/h)";
            stackPanel.Children.Add(label);

            Binding consumptionBinding = new Binding("[consumption]");

            ValueRangeTextBox consumption = new ValueRangeTextBox();
            consumption.FontFamily = new FontFamily("Baskerville Old Face");
            consumption.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xCE, 0xA2));
            consumption.Background = new SolidColorBrush(Color.FromRgb(0x39, 0x52, 0x50));
            consumption.BorderBrush = Brushes.Black;
            consumption.Margin = new Thickness(5);
            consumption.ValueDataType = typeof(Decimal);
            consumption.MaxLength = 13;
            consumption.MaxLines = 13;
            consumption.SetBinding(ValueRangeTextBox.TextProperty, consumptionBinding);
            consumption.Name = "consumption";

            stackPanel.Children.Add(consumption);

            carsGrid.Children.Add(stackPanel);


            RowDefinition definition2 = new RowDefinition();
            definition2.Height = GridLength.Auto;
            carsGrid.RowDefinitions.Add(definition2);
            StackPanel stackPanel2 = new StackPanel();
            Grid.SetRow(stackPanel2, 10);
            Label label2 = new Label();
            label2.Style = (Style)this.Resources["labelstyle"];
            label2.Content = "Gas tank size (liter)";
            stackPanel2.Children.Add(label2);


            Binding gasTankSizeBinding = new Binding("[gas_tank_size]");

            ValueRangeTextBox gasTankSize = new ValueRangeTextBox();
            gasTankSize.FontFamily = new FontFamily("Baskerville Old Face");
            gasTankSize.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xCE, 0xA2));
            gasTankSize.Background = new SolidColorBrush(Color.FromRgb(0x39, 0x52, 0x50));
            gasTankSize.BorderBrush = Brushes.Black;
            gasTankSize.Margin = new Thickness(5);
            gasTankSize.ValueDataType = typeof(Decimal);
            gasTankSize.MaxLength = 13;
            gasTankSize.MaxLines = 13;
            gasTankSize.SetBinding(ValueRangeTextBox.TextProperty, gasTankSizeBinding);
            gasTankSize.Name = "gas_tank_size";

            stackPanel2.Children.Add(gasTankSize);

            carsGrid.Children.Add(stackPanel2);

            Tables.cars.database.Columns["consumption"].AllowDBNull = false;
            Tables.cars.database.Columns["gas_tank_size"].AllowDBNull = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool thereIsAnError = false;

            foreach (var gridElement in carsGrid.Children)
            {
                if (gridElement.GetType() == typeof(StackPanel))
                {
                    StackPanel stackPanel = (StackPanel)gridElement;
                    foreach (var element in stackPanel.Children)
                    {
                        if (element.GetType() == typeof(ValueRangeTextBox))
                        {

                            ValueRangeTextBox VTextBox = (ValueRangeTextBox)element;

                            thereIsAnError = Validation.ValidateTextbox(VTextBox, car);                          
                        }
                    }
                }

                if (warehouse_id.SelectedIndex == -1)
                {
                    thereIsAnError = true;
                }

                if (thereIsAnError == true)
                {
                    break;
                }
            }

            if (thereIsAnError == false)
            {

                //car["last_service"] = SQL.con((DateTime)car["last_service"]);
                //car["last_exam"] = SQL.convertDateToCorrectFormat((DateTime)car["last_exam"]);
                car["warehouse_id"] = warehouse_id_Dictionary[warehouse_id.SelectedItem.ToString()]["id"];
                Tables.cars.database.Rows.Add(car);
                Tables.cars.updateChanges();
                MessageBox.Show($"A new car has been created", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                CarsPage carsPage = new CarsPage();
                Navigation.content2.Navigate(carsPage);
            }
        }

        private void last_exam_InputValidationError(object sender, Xceed.Wpf.Toolkit.Core.Input.InputValidationErrorEventArgs e)
        {
            MessageBox.Show($"You can only input a date", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Navigation.PreviousPage != null)
            {
                Navigation.OpenPage(Navigation.PreviousPage.GetType());
            }
        }

        private void CreateCarWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
        }
    }
}
