using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
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
                else if (textBox.Text.Contains("'") || textBox.Text.Contains('"'))
                {
                    textBox.Text = context[textBox.Name].ToString();

                    Xceed.Wpf.Toolkit.MessageBox.Show($"{textBox.Name} cannot contain ' and \" characters");
                    HasError = true;

                }
                else if (textBox.HasParsingError == true)
                {
                    MessageBox.Show($"{textBox.Name} must be a {textBox.ValueDataType.Name}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

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
                            MessageBox.Show($"An element with this {textBox.Name} already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            HasError = true;
                        }
                    }
                }
                else if (textBox.IsValueOutOfRange == true)
                {
                    string errorMessage = "";
                    if(textBox.MinValue != null)
                    {
                        errorMessage += $"{textBox.Name} must be bigger or equal to {textBox.MinValue}, ";
                    }
                    if(textBox.MaxValue != null)
                    {
                        errorMessage += $"{textBox.Name} must be smaller or equal to {textBox.MaxValue}";
                    }
                    MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    HasError = true;
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


        public static bool ValidateSQLNaming(string input, string input_name)
        {
            string pattern = @"^[a-zA-Z][a-zA-Z0-9_]*$";

            // Test the pattern against a string
           
            bool isMatch = Regex.IsMatch(input, pattern);
            if(isMatch == false)
            {
                MessageBox.Show($"{input_name} must start with a letter and can only contain letters, numbers and underscores");
            }

            return isMatch;
        }

        public static bool validateEmail(string email)
        {
            bool haserror = false;
            if(email.Length == 0)
            {
                MessageBox.Show("Email cannot be empty");
                haserror = true;
            }
            else
            {
                string pattern = @"^(?=.*@)(?=.*\.)[\S]+$";
                bool isMatch = Regex.IsMatch(email, pattern);
                if (isMatch == false)
                {
                    MessageBox.Show("Please give a valid email");
                    haserror = true;
                }
                else if (Tables.staff.database.Select($"email = '{email}'").Length > 0 || Tables.employees.database.Select($"email = '{email}'").Length > 0)
                {
                    MessageBox.Show("This email has already been registered");
                    haserror = true;
                }
            }
            return haserror;
        }
    }
}

