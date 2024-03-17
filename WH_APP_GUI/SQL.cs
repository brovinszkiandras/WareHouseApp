using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySqlConnector;
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
            connectionstring = $"datasource={DataSource};port={Port};username={Username};password={password};database=;";
            con = new MySqlConnection(connectionstring);

            SQL.SqlCommandWithoutDatabase($"CREATE DATABASE IF NOT EXISTS {DatabaseName} DEFAULT CHARACTER SET utf8;");

            connectionstring = $"datasource={DataSource};port={Port};username={Username};password={password};database={DatabaseName};";
            con = new MySqlConnection(connectionstring);

            StreamWriter databaseWrite = new StreamWriter("database.txt");
            databaseWrite.WriteLine($"datasource {DataSource}");
            databaseWrite.WriteLine($"port {Port}");
            databaseWrite.WriteLine($"username {Username}");
            databaseWrite.WriteLine($"password  {password}");
            databaseWrite.WriteLine($"databasename {DatabaseName}");

            databaseWrite.Close();
        }

        public static void FillStaticDatabaseValues()
        {
            datasource = File.ReadAllLines("database.txt")[0].Split(' ')[1] != null ? File.ReadAllLines("database.txt")[0].Split(' ')[1] : string.Empty;
            port = int.TryParse(File.ReadAllLines("database.txt")[1].Split(' ')[1], out int tmp) ? tmp : 0;
            username = File.ReadAllLines("database.txt")[2].Split(' ')[1] != null ? File.ReadAllLines("database.txt")[2].Split(' ')[1] : string.Empty;
            password = File.ReadAllLines("database.txt")[3].Split(' ')[1] != null ? File.ReadAllLines("database.txt")[3].Split(' ')[1] : string.Empty;
            database = File.ReadAllLines("database.txt")[4].Split(' ')[1] != null ? File.ReadAllLines("database.txt")[4].Split(' ')[1] : string.Empty;


            connectionstring = $"datasource={datasource};port={port};username={username};password={password};database={database};";
            con = new MySqlConnection(connectionstring);
        }

    #region Set methoods
    private static void ModifyDatabaseTXT(string Data, object Value)
        {
            StreamReader readDatabaseData = new StreamReader("database.txt");
            List<string> DatabaseData = new List<string>();
            while (!readDatabaseData.EndOfStream)
            {
                string oneDataLine = readDatabaseData.ReadLine();
                DatabaseData.Add(oneDataLine);
            }
            readDatabaseData.Close();

            StreamWriter writeDatasToDBtxt = new StreamWriter("database.txt");
            for (int i = 0; i < DatabaseData.Count; i++)
            {
                if (DatabaseData[i].Split(' ')[0] == Data)
                {
                    writeDatasToDBtxt.WriteLine($"{Data} {Value}");
                }
                else
                {
                    writeDatasToDBtxt.WriteLine(DatabaseData[i]);    
                }
            }
            writeDatasToDBtxt.Close();
        }

        public static void SetDatasource(string NewDatasource)
        {
            datasource = NewDatasource;
            connectionstring = $"datasource={NewDatasource};port={port};username={username};password={password};database={database};";
            ModifyDatabaseTXT("datasource", NewDatasource);
            con = new MySqlConnection(connectionstring);
        }
        public static void SetPort(int NewPort)
        {
            port = NewPort;
            connectionstring = $"datasource={datasource};port={NewPort};username={username};password={password};database={database};";
            ModifyDatabaseTXT("port", NewPort);
            con = new MySqlConnection(connectionstring);
        }
        public static void SetUsername(string NewUsername)
        {
            username = NewUsername;
            connectionstring = $"datasource={datasource};port={port};username={NewUsername};password={password};database={database};";
            ModifyDatabaseTXT("username", NewUsername);
        }
        public static void SetPassword(string NewPassword)
        {
            password = NewPassword;
            connectionstring = $"datasource={datasource};port={port};username={username};password={NewPassword};database={database};";
            ModifyDatabaseTXT("password", NewPassword);
            con = new MySqlConnection(connectionstring);
        }
        public static void SetDatabaseName(string databaseName)
        {
            database = databaseName;
            connectionstring = $"datasource={datasource};port={port};username={username};password={password};database={databaseName};";
            ModifyDatabaseTXT("database", databaseName);
            con = new MySqlConnection(connectionstring);
        }

        #endregion
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
                Debug.WriteError(ex.ToString());
                throw;
            }
        }

        static public void SqlCommandWithoutDatabase(string command)
        {
            try
            {
                MessageBox.Show(connectionstring);
                MessageBox.Show(command);
                
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
                Debug.WriteError(ex.ToString());
                throw;
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
                        data = command.ExecuteScalar().ToString();
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex.ToString());
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
                        data = command.ExecuteScalar().ToString();
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex.ToString());
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
                Debug.WriteError(ex.ToString());
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
                Debug.WriteError(ex.ToString());
                throw;
            }
        }

        static public void DeleteTable(string tableName)
        {
            try
            {
                con.Open();
                string deleteTableQuery = $"USE {database}; DROP TABLE {tableName};";
                using (MySqlCommand command = new MySqlCommand(deleteTableQuery, con))
                {
                    command.ExecuteNonQuery();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "Error");
                Debug.WriteError(ex.ToString());
                throw;
            }
        }
        #endregion

        static public bool ContainsIllegalRegex(string input)
        {
            string[] illegalPatterns = { @"\s", @"[\W&&[^_]]", @"\b(select|insert|update|delete|table)\b", @"[!@#$%^&*()+=\[\]{};':"",.<>?/\\|~`]", "[áéűó]" };

            foreach (var pattern in illegalPatterns)
            {
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase) || char.IsDigit(input[0]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
