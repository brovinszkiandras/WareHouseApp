using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace WH_APP_GUI
{
    class SQL
    {
        public static string datasource = string.Empty;
        public static int port = 0;
        public static string username = string.Empty;
        public static string password = string.Empty;
        public static string database = string.Empty;

        public static string connectionstring = $"datasource={datasource};port={port};username={username};password={password};database={database};";
        public static MySqlConnection con = new MySqlConnection(connectionstring);
        public static List<string> Tables()
        {
            List<string> returnList = new List<string>();
            List<string[]> lis = SqlQuery($"SHOW TABLES FROM {database}");
            for (int i = 0; i < lis.Count; i++)
            {
                returnList.Add(lis[i][0]);
            }
            return returnList;   
        }
        public static bool IsDatabasetxtExist()
        {
            return File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.txt"));
        }
        public static void CreateDatabaseConnectionDatas(string DataSource, int Port, string Username, string password, string DatabaseName)
        {
            StreamWriter databaseWrite = new StreamWriter("database.txt");
            databaseWrite.WriteLine($"datasource {DataSource}");
            databaseWrite.WriteLine($"port {Port}");
            databaseWrite.WriteLine($"username {Username}");
            databaseWrite.WriteLine($"password  {password}");
            databaseWrite.WriteLine($"databasename {DatabaseName}");
            databaseWrite.Close();

            try
            {
                connectionstring = $"datasource={DataSource};port={Port};username={Username};password={password};database=;";
                con = new MySqlConnection(connectionstring);

                SQL.SqlCommandWithoutDatabase($"CREATE DATABASE IF NOT EXISTS {DatabaseName} DEFAULT CHARACTER SET utf8;");

                connectionstring = $"datasource={DataSource};port={Port};username={Username};password={password};database={DatabaseName};";
                con = new MySqlConnection(connectionstring);
            }
            catch (Exception)
            {
                MessageBox.Show("Can't connect to the specified database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        public static void FillStaticDatabaseValues()
        {
            try
            {
                datasource = File.ReadAllLines("database.txt")[0].Split(' ')[1] != null ? File.ReadAllLines("database.txt")[0].Split(' ')[1] : string.Empty;
                port = int.TryParse(File.ReadAllLines("database.txt")[1].Split(' ')[1], out int tmp) ? tmp : 0;
                username = File.ReadAllLines("database.txt")[2].Split(' ')[1] != null ? File.ReadAllLines("database.txt")[2].Split(' ')[1] : string.Empty;
                password = File.ReadAllLines("database.txt")[3].Split(' ')[1] != null ? File.ReadAllLines("database.txt")[3].Split(' ')[1] : string.Empty;
                database = File.ReadAllLines("database.txt")[4].Split(' ')[1] != null ? File.ReadAllLines("database.txt")[4].Split(' ')[1] : string.Empty;

                connectionstring = $"datasource={datasource};port={port};username={username};password={password};database={database};";
                con = new MySqlConnection(connectionstring);
            }
            catch (IndexOutOfRangeException)
            {
                File.Delete("database.txt");
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }
        #region Sql commands
        static public void SqlCommand(string command)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionstring))
                {
                    con.Open();
                    if (!command.Contains($"USE {SQL.database};"))
                    {
                        command = string.Concat($"USE {SQL.database}; ", command);
                    }
                    string insert = command;
                    using (MySqlCommand Command = new MySqlCommand(insert, con))
                    {
                        Command.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
                Debug.WriteError(command);
                throw;
            }
        }

        static public void SqlCommandWithoutDatabase(string command)
        {
            try
            {   
                using (MySqlConnection con = new MySqlConnection(connectionstring))
                {
                    con.Open();
                    string creatdb = command;
                    using (MySqlCommand Command = new MySqlCommand(creatdb, con))
                    {
                        Command.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
                throw;
            }
        }
        //TODO ez kell majd dokumnetációba
        public static bool BoolQuery(string query)
        {
            string result = FindOneDataFromQuery(query);
            if (bool.Parse(result))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string FindOneDataFromQuery(string query)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionstring))
                {
                    con.Open();
                    if (!query.Contains($"USE {SQL.database};"))
                    {
                        query = string.Concat($"USE {SQL.database}; ", query);
                    }
                    string data;
                    using (MySqlCommand command = new MySqlCommand(query, con))
                    {
                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            data = result.ToString();
                        }
                        else
                        {
                            data = string.Empty;
                        }
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
                throw;
            }
        }

        public static string FindOneDataFromQueryWithoutDatabase(string query)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionstring))
                {
                    con.Open();
                    string data;
                    using (MySqlCommand command = new MySqlCommand(query, con))
                    {
                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            data = result.ToString();
                        }
                        else
                        {
                            data = string.Empty;
                        }
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
                throw;
            }
        }

        static public List<string[]> SqlQuery(string query)
        {
            try
            {
                if (!query.Contains($"USE {SQL.database};"))
                {
                    query = string.Concat($"USE {SQL.database}; ", query);
                }

                List<string[]> results = new List<string[]>();
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] row = new string[reader.FieldCount];

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[i] = reader[i].ToString();
                        }

                        results.Add(row);
                    }
                }
                con.Close();
                return results;
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
                Debug.WriteError(query);
                throw;
            }
        }

        static public List<string[]> SqlQueryWithoutDatabase(string query)
        {
            try
            {
                List<string[]> results = new List<string[]>();
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] row = new string[reader.FieldCount];

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[i] = reader[i].ToString();
                        }

                        results.Add(row);
                    }
                }
                con.Close();
                return results;
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
                throw;
            }
        }
        #endregion


        public static List<string> GetElementOfListArray(List<string[]> ListWithArray)
        {
            List<string> returnList = new List<string>();
            for (int i = 0; i < ListWithArray.Count; i++)
            {
                returnList.Add(ListWithArray[i][0]);
            }

            return returnList;
        }

        public static string convertDateToCorrectFormat(DateTime date)
        {
            string datetimestring = date.ToString();

            // Adjust the format specifier to match the actual format of your datetime string
            if (DateTime.TryParseExact(datetimestring, "yyyy. MM. dd. H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeValue))
            {
                // Convert the datetime value to the desired format
                string formattedDateTimeString = dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss");

                // Update the value in the DataRow with the formatted datetime string
                

               return formattedDateTimeString;
            }
            else
            {
                return null;
            }
        }

        public static string convertShordDateTocorrectFormat(DateTime date)
        {
            string datetimestring = date.ToString();

            // Adjust the format specifier to match the actual format of your datetime string
            if (DateTime.TryParseExact(datetimestring, "yyyy. MM. dd.", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeValue))
            {
                // Convert the datetime value to the desired format
                string formattedDateTimeString = dateTimeValue.ToString("yyyy-MM-dd");

                // Update the value in the DataRow with the formatted datetime string
              

                return formattedDateTimeString;
            }
            else
            {
                //MessageBox.Show("Could not parse it");
                return null;
            }
        }
    }
}
