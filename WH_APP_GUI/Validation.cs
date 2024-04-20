using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.Primitives;

namespace WH_APP_GUI
{
    internal class Validation
    {
        public static bool ValidateTextbox(ValueRangeTextBox textBox, DataRow context)
        {
            
            bool HasError = false;
            DataTable contextTable = context.Table;
            
            if(textBox.Visibility == Visibility.Visible)
            {
                if (textBox.Text.Length == 0)
                {

                    if (contextTable.Columns[textBox.Name].AllowDBNull == false)
                    {
                        // Xceed.Wpf.Toolkit.MessageBox.Show(context[textBox.Name, DataRowVersion.Original].ToString());

                        textBox.Text = context[textBox.Name].ToString();
                        Xceed.Wpf.Toolkit.MessageBox.Show($"{textBox.Name} cannot be emtpy");
                        HasError = true;

                    }
                    //else
                    //{
                    //    context[textBox.Name] = null;
                    //}
                }
                else if (SQL.ContainsAllIllegalRegex(textBox.Text) == true)
                {
                    textBox.Text = context[textBox.Name].ToString();

                    Xceed.Wpf.Toolkit.MessageBox.Show($"{textBox.Name} cannot contain special characters");
                    HasError = true;

                }
                else if (textBox.HasParsingError == true)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show($"{textBox.Name} must be a {textBox.ValueDataType.Name}");


                    textBox.Text = context[textBox.Name].ToString();
                    HasError = true;

                }
                else if (contextTable.Columns[textBox.Name].Unique == true)
                {

                    DataRow[] matchingRows = contextTable.Select($"{textBox.Name} = '{textBox.Value}'");
                    if (matchingRows.Length != 0)
                    {
                      
                        if ((int)matchingRows[0]["id"] != (int)context["id"])
                        {
                            Xceed.Wpf.Toolkit.MessageBox.Show($"An element with this {textBox.Name} already exists");
                            HasError = true;
                        }
                    }
                }
                else if (textBox.IsValueOutOfRange == true)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show($"{textBox.Name} must be smaller or equal to {textBox.MaxValue}");
                    HasError = true;
                }
            }

            
            return HasError;
        }

        public static bool ValidateCombobox(ComboBox combobox, DataRow context)
        {
            if ( combobox.Visibility == Visibility.Visible)
            {
                bool HasError = false;
                DataTable contextTable = context.Table;
                if (combobox.SelectedIndex == -1)
                {
                    if (contextTable.Columns[combobox.Name].AllowDBNull == false) 
                    {
                        HasError = true;
                        MessageBox.Show($"{combobox.Name} must be selected");
                    }
                }
                return HasError;
            }
            else
            {
                return false;
            }
        }
    }
}
