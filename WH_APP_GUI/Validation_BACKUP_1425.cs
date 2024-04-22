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
           
            if (textBox.Visibility == Visibility.Visible)
            {
                if (textBox.Text.Length == 0)
                {
                    if (contextTable.Columns[textBox.Name].AllowDBNull == false)
                    {
                        // Xceed.Wpf.Toolkit.MessageBox.Show(context[textBox.Name, DataRowVersion.Original].ToString());

                        textBox.Text = context[textBox.Name].ToString();
                        MessageBox.Show($"{textBox.Name} cannot be emtpy", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        HasError = true;

                    }
                    //else
                    //{
                    //    context[textBox.Name] = null;
                    //}
                }
<<<<<<< HEAD
                else if (textBox.Text.Contains("'") || textBox.Text.Contains('"'))
                {
                    textBox.Text = context[textBox.Name].ToString();

                    Xceed.Wpf.Toolkit.MessageBox.Show($"{textBox.Name} cannot contain ' and \" characters");
                    HasError = true;

                }
=======
>>>>>>> f4f7652eca21544b38e57d70343bd77069ff0bf7
                else if (textBox.HasParsingError == true)
                {
                    MessageBox.Show($"{textBox.Name} must be a {textBox.ValueDataType.Name}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    textBox.Text = context[textBox.Name].ToString();
                    HasError = true;
                }
<<<<<<< HEAD
               
                else if (contextTable.Columns[textBox.Name].Unique == true)
=======
                else if (contextTable.Columns[textBox.Name].Unique == true && textBox.Name != "email")
>>>>>>> f4f7652eca21544b38e57d70343bd77069ff0bf7
                {
                    DataRow[] matchingRows = contextTable.Select($"{textBox.Name} = '{textBox.Value}'");
                    if (matchingRows.Length != 0)
                    {
                        MessageBox.Show(matchingRows[0]["id"].ToString());
                        MessageBox.Show(context["id"].ToString());
                        if ((int)matchingRows[0]["id"] != (int)context["id"])
                        {
                            MessageBox.Show($"An element with this {textBox.Name} already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            HasError = true;
                        }
                    }
                }
                else if (textBox.IsValueOutOfRange == true)
                {
                    MessageBox.Show($"{textBox.Name} must be smaller or equal to {textBox.MaxValue}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    HasError = true;
                }
                else
                {
                    char[] Email_exceptions = { '@', '.' };
                    char[] Name_exceptions = { ' ', '-' };
                    if (textBox.Name == "email" && SQL.ContainsIllegalRegexWithExceptions(textBox.Text, Email_exceptions))
                    {
                        textBox.Text = context[textBox.Name].ToString();

                        MessageBox.Show($"{textBox.Name} cannot contain special characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        HasError = true;
                    }
                    else if (textBox.Name != "email" && SQL.ContainsIllegalRegexWithExceptions(textBox.Text, Name_exceptions) && (textBox.ValueDataType != typeof(int) && textBox.ValueDataType != (typeof(double))))
                    {
                        textBox.Text = context[textBox.Name].ToString();

                        MessageBox.Show($"{textBox.Name} cannot contain special characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        HasError = true;
                    }
                    else if(textBox.Name == "email")
                    {
                        DataRow[] matchingRows = contextTable.Select($"{textBox.Name} = '{textBox.Value}'");
                        if (matchingRows.Length != 0)
                        {
                            if ((int)matchingRows[0]["id"] != (int)context["id"])
                            {
                                MessageBox.Show($"An element with this {textBox.Name} already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                HasError = true;
                            }
                        }
                    }
                }
            }
            return HasError;
        }
        public static bool ValidateCombobox(ComboBox combobox, DataRow context)
        {
            if (combobox.Visibility == Visibility.Visible)
            {
                bool HasError = false;
                DataTable contextTable = context.Table;
                if (combobox.SelectedIndex == -1)
                {
                    if (contextTable.Columns[combobox.Name].AllowDBNull == false)
                    {
                        HasError = true;
                        MessageBox.Show($"{combobox.Name} must be selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

