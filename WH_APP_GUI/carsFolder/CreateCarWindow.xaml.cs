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
using Xceed.Wpf.Toolkit.Primitives;

namespace WH_APP_GUI.carsFolder
{
    /// <summary>
    /// Interaction logic for CreateCarWindow.xaml
    /// </summary>
    public partial class CreateCarWindow : Page
    {
        public DataRow car;
        public CreateCarWindow()
        {
            InitializeComponent();

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

        public void addStoreFeautereElements()
        {
            RowDefinition definition = new RowDefinition();
            definition.Height = GridLength.Auto;
            carsGrid.RowDefinitions.Add(definition);
            StackPanel stackPanel = new StackPanel();
            Grid.SetRow(stackPanel, 6);
            Label label = new Label();
            label.FontSize = 17;
            label.Content = "Storage (cm2)*";
            stackPanel.Children.Add(label);


            Binding storageBinding = new Binding("[storage]");

            ValueRangeTextBox storage = new ValueRangeTextBox();
            storage.FontSize = 17;
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
            Grid.SetRow(stackPanel2, 7);
            Label label2 = new Label();
            label2.FontSize = 17;
            label2.Content = "Carrying capacity (kgm)*";
            stackPanel2.Children.Add(label2);


            Binding carrryingCapacityBinding = new Binding("[carrying_capacity]");

            ValueRangeTextBox carrying_capacity = new ValueRangeTextBox();
            carrying_capacity.FontSize = 17;
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
            Grid.SetRow(stackPanel, 8);
            Label label = new Label();
            label.FontSize = 17;
            label.Content = "Consumption (liter/h)*";
            stackPanel.Children.Add(label);


            Binding consumptionBinding = new Binding("[consumption]");

            ValueRangeTextBox consumption = new ValueRangeTextBox();
            consumption.FontSize = 17;
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
            Grid.SetRow(stackPanel2, 9);
            Label label2 = new Label();
            label2.FontSize = 17;
            label2.Content = "Gas tank size (liter)*";
            stackPanel2.Children.Add(label2);


            Binding gasTankSizeBinding = new Binding("[gas_tank_size]");

            ValueRangeTextBox gasTankSize = new ValueRangeTextBox();
            gasTankSize.FontSize = 17;
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
            string formattedText = last_exam.Text;

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
                if (thereIsAnError == true)
                {
                    break;
                }
            }

            if (thereIsAnError == false)
            {

                // Xceed.Wpf.Toolkit.MessageBox.Show(car["last_exam"].ToString());
                //car["last_service"] = new MySqlDateTime((DateTime)last_service.Value);
                //car["last_exam"] = new MySqlDateTime((DateTime)last_exam.Value);
                Tables.cars.database.Rows.Add(car);
                Tables.cars.updateChanges();
                Xceed.Wpf.Toolkit.MessageBox.Show($"A new car has been created");
                CarsPage carsPage = new CarsPage();
                Navigation.content2.Navigate(carsPage);
            }
        }

        private void last_exam_InputValidationError(object sender, Xceed.Wpf.Toolkit.Core.Input.InputValidationErrorEventArgs e)
        {
            Xceed.Wpf.Toolkit.MessageBox.Show($"You can only input a date");
        }
    }
}
