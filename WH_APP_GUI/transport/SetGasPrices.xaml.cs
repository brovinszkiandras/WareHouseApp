using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

namespace WH_APP_GUI.transport
{
    public partial class SetGasPrices : Window
    {
        class Gas
        {
            public string Type { get; set; }
            public double Min { get; set; }
            public double Avg  { get; set; }
            public double Max  { get; set; }
            public Gas(string line)
            {
                string[] dataArray = line.Split(';');

                Type = dataArray[0];
                Min = double.Parse(dataArray[1], CultureInfo.InvariantCulture);
                Avg = double.Parse(dataArray[2], CultureInfo.InvariantCulture);
                Max = double.Parse(dataArray[3], CultureInfo.InvariantCulture);
            }
        }
        public SetGasPrices()
        {
            InitializeComponent();
            Ini_GasPrices();
        }

        private void Ini_GasPrices()
        {
            GasPrices.Children.Clear();

            StreamReader readGas = new StreamReader("gas.txt");
            List<Gas> gasPrices = new List<Gas>();
            while (! readGas.EndOfStream)
            {
                string line = readGas.ReadLine();
                gasPrices.Add(new Gas(line));
            }

            for (int i = 0; i < gasPrices.Count; i++)
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = Brushes.Black;
                border.Margin = new Thickness(5);

                StackPanel stackPanel = new StackPanel();

                Label label = new Label();
                label.Content = gasPrices[i].Type;
                label.Margin = new Thickness(5);
                stackPanel.Children.Add(label);

                Button min = new Button();
                min.Content = gasPrices[i].Min;
                min.Margin = new Thickness(5);
                stackPanel.Children.Add(min);

                Button avg = new Button();
                avg.Content = gasPrices[i].Avg;
                avg.Margin = new Thickness(5);
                stackPanel.Children.Add(avg);

                Button max = new Button();
                max.Content = gasPrices[i].Max; 
                max.Margin = new Thickness(5);
                stackPanel.Children.Add(max);

                border.Child = stackPanel;
                GasPrices.Children.Add(border);
            }
        }
    }
}
