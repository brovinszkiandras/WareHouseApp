using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySqlConnector;
using System.IO;

namespace ClassesForWarehouseManagement
{
    class Debug
    {
        public static void CreateLog()
        {
            File.Create("log.txt").Close();
        }
        public static bool ThereIsExistingLog()
        {
            if (File.Exists("log.txt"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void WriteError(string error)
        {
            StreamWriter ir = new StreamWriter("log.txt");
            ir.Write($"[{DateTime.Now.ToString("yyyy/MM/dd:HH-mm")}] ERROR: {error}");
            ir.Close();
        }
    }
    class Tables
    {
        public string TableName { get; set; }
        public List<string> Columns = new List<string>();
        public Tables(string TableName)
        {
            this.TableName = TableName;
            List<string[]> lis = Controller.SqlQuery($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName}';");
            for (int i = 0; i < lis.Count; i++)
            {
                Columns.Add(lis[i][0]);
            }
        }
    }
    class Controller
    {
        public static string datasource = "127.0.0.1";
        public static int port = 3306;
        public static string username = "root";
        public static string password = "";
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
                //Do something with this bruh...
                //MessageBox.Show(ex.ToString(), "Error");
                Debug.WriteError(ex.ToString());
                throw;
            }
        }

        #region Set methoods
        public static void SetDatasource(string NewDatasource)
        {
            datasource = NewDatasource;
            connectionstring = $"datasource={NewDatasource};port={port};username={username};password={password};database={database};";
        }
        public static void SetPort(int NewPort)
        {
            port = NewPort;
            connectionstring = $"datasource={datasource};port={NewPort};username={username};password={password};database={database};";
        }
        public static void SetUsername(string NewUsername)
        {
            username = NewUsername;
            connectionstring = $"datasource={datasource};port={port};username={NewUsername};password={password};database={database};";
        }
        public static void SetPassword(string NewPassword)
        {
            password = NewPassword;
            connectionstring = $"datasource={datasource};port={port};username={username};password={NewPassword};database={database};";
        }
        public static void SetDatabaseName(string databaseName)
        {
            database = databaseName;
            connectionstring = $"datasource={datasource};port={port};username={username};password={password};database={databaseName};";
        }

        #endregion
        #region Sql commands
        //If we done just take out the throw so there wont be big big error for the user
        static public void SqlCommand(string command)
        {
            try
            {
                con.Open();
                if (!command.Contains($"USE {Controller.database};"))
                {
                    command = string.Concat($"USE {Controller.database}; ", command);
                }
                string insert = command;
                using (MySqlCommand Command = new MySqlCommand(insert, con))
                {
                    Command.ExecuteNonQuery();
                }
                con.Close();
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
                con.Open();
                if (!query.Contains($"USE {Controller.database};"))
                {
                    query = string.Concat($"USE {Controller.database}; ", query);
                }
                string data;
                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    data = command.ExecuteScalar().ToString();
                }
                con.Close();
                return data;
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
                if (!query.Contains($"USE {Controller.database};"))
                {
                    query = string.Concat($"USE {Controller.database}; ", query);
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

        public static void CreateMigration()
        {
            if (! Tables().Contains("migrations"))
            {
                SqlCommand("CREATE TABLE migrations(id int AUTO_INCREMENT PRIMARY KEY NOT NULL, name varchar(50), actual_name varchar(100), nice_name varchar(150) NULL);");
            }
        }

        private static bool CheckOutMigrations()
        {
            //Ez vissz add egy true-t ha az összes sor ami nekünk kell a migrartions-ban benne is van, ha nem...akkor meg false-t. Ez basicly arra kell hogy ne hozzuk létre többször ugyanazokat a sorkata a migrations-be
            List<string[]> ListOfDatasInMigartion = SqlQuery("SELECT name FROM migrations;");
            List<string> ListOfDefaultTables = new List<string>() { "Ceos", "Employees", "Warehouses" };
            for (int i = 0; i < ListOfDatasInMigartion.Count; i++)
            {
                if (ListOfDefaultTables.Contains(ListOfDatasInMigartion[i][0]))
                {
                    return false;
                }
            }
            return true;
        }

        public static void CreateDefaultRequiredTables()
        {
            string CeoTableName = FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'Ceos';");
            string WarehousesTableName = FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'Warehouses';");
            string EmployeesTableName = FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'Employees';");
            
            string[] ArrayOfRequierdTables = new string[] { CeoTableName, WarehousesTableName, EmployeesTableName };
            bool CanCreateTables = true;
            for (int i = 0; i < ArrayOfRequierdTables.Length; i++)
            {
                if (Tables().Contains(ArrayOfRequierdTables[i]))
                {
                    CanCreateTables = false;
                }
            }
            if (CanCreateTables)
            {
                try
                {
                    //This order is becuse of the foregin relations
                    /*CEOS*/SqlCommand($"CREATE TABLE {CeoTableName} (id INT PRIMARY KEY NOT NULL AUTO_INCREMENT, name VARCHAR(50) NOT NULL, email VARCHAR(50) UNIQUE NOT NULL, password VARCHAR(50) NOT NULL);");
                    /*WAREHOUSES*/SqlCommand($"CREATE TABLE {WarehousesTableName} (id INT PRIMARY KEY NOT NULL AUTO_INCREMENT, name VARCHAR(50) NOT NULL, ceo_id INT NOT NULL, FOREIGN KEY (ceo_id) REFERENCES {CeoTableName} (id));");
                    /*EMPLOYEES*/SqlCommand($"CREATE TABLE {EmployeesTableName} (id INT PRIMARY KEY NOT NULL AUTO_INCREMENT, name VARCHAR(50) NOT NULL, email VARCHAR(50) UNIQUE NOT NULL, password VARCHAR(50) NOT NULL, warehouse_id INT NOT NULL, FOREIGN KEY (warehouse_id) REFERENCES {WarehousesTableName} (id));");
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex.ToString());
                }
            }
        }

        public static void CreateRequiredTablesInMigartion(List<string> TableNamesByUser, List<string> TableNiceNamesByUser)
        {
            //Meg kéne beszélni hogy mik a köelező táblák és mik nem mert ezek csak randomok amket úgy éreztem hogy fix kellenek
            if (CheckOutMigrations())
            {
                try
                {
                    Controller.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('Ceos', '{TableNamesByUser[0]}', '{TableNiceNamesByUser[0]}');");
                    Controller.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('Employees', '{TableNamesByUser[2]}', '{TableNiceNamesByUser[2]}');");
                    Controller.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('Warehouses', '{TableNamesByUser[3]}', '{TableNiceNamesByUser[3]}');");
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error", "Can not create the requerd tables");
                    Debug.WriteError(ex.ToString());
                }
            }
        }
    }
    abstract class Table
    {
        //Ha létrehozol ebből egy példányt akkor annak a neve mindig legyen egyenlő azzal amit a 'name' -hez írsz a migrations-ben(az az általunk használt/ismert név)
        public string ActualName { get; set; }
        public string NiceName { get; set; }
        public List<string> Columns = new List<string>();
        public Table()
        {
            ActualName = Controller.FindOneDataFromQuery($"SELECT actual_name FROM migrations WHERE name = '{this.GetType().Name}';");

            List<string[]> ColumList = Controller.SqlQuery($"SHOW COLUMNS FROM {ActualName}");
            for (int i = 0; i < ColumList.Count; i++)
            {
                Columns.Add(ColumList[i][0]);
            }
            //Az id-t azt kiveszem a...mert nem kell az nekünk az adatbázis metódusokhoz(ha kell akkor meg kérdezd le ahogy tudod...)
            if (Columns.Contains("id"))
            {
                Columns.RemoveAt(Columns.IndexOf("id"));
            }
        }
        #region Table Methods
        private List<string> Values; //Insert

        private string Column; //Update
        private string Value; //Update
        private object LogicalExamination; //Update AND Delete

        #region Insert

        public Table InsertValuesToThisTable(List<string> Values)
        {
            this.Values = Values;
            return this;
        }
        public void ExecuteInsert()
        {
            string commandString = $"INSERT INTO `{ActualName}`(";
            for (int i = 0; i < Columns.Count; i++)
            {
                commandString += $"`{Columns[i]}`,";
            }
            commandString = commandString.Remove(commandString.Count() - 1);
            commandString += ") VALUES (";
            for (int i = 0; i < Values.Count; i++)
            {
                commandString += $"'{Values[i]}',";
            }
            commandString = commandString.Remove(commandString.Count() - 1);
            commandString += ");";
            Controller.SqlCommand(commandString);

            Values.Clear();
        }

        #endregion
        #region Update

        public Table UpdateColumn(string Column)
        {
            this.Column = Column;
            return this;
        }

        public Table ValueOf(string Value)
        {
            this.Value = Value;
            return this;
        }
        public Table Where(object LogicalExamination)
        {
            //Az object azért kell hogy kényelmesebben tud megadni azt hogy 1 itt ugye csak method(1) vagy WHERE name = 'Bozó' itt meg method("WHERE name = 'Bozó'")
            this.LogicalExamination = LogicalExamination;
            return this;
        }
        public void ExecuteUpdate()
        {
            Controller.SqlCommand($"UPDATE `{ActualName}` SET `{Column}`='{Value}' WHERE {LogicalExamination};");
            Column = null;
            Value = null;
            LogicalExamination = null;
        }

        #endregion
        #region Delete

        //EZ pontosan ugyan az mint az Update-ben a Where csak így jobban elkülönithetőbb
        public Table DeleteAt(object LogicalExamination)
        {
            this.LogicalExamination = LogicalExamination;
            return this;
        }

        public void ExecuteDelte()
        {
            Controller.SqlCommand($"DELETE FROM `{ActualName}` WHERE {LogicalExamination};");
            LogicalExamination = null;
        }

        #endregion

        #endregion
    }
    //TODO: Do all the classes for the tables...then i guess we are fine
    class Ceos : Table
    {
        
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            //Justr leav them here...trust me
            Controller.SetDatabaseName("test");
            Controller.CreateMigration();
            List<string> lis = new List<string>() { "Ceo", "warehouse manager", "employee", "warehouse" };
            Controller.CreateRequiredTablesInMigartion(lis, lis);
            Controller.CreateDefaultRequiredTables();
            if (!Debug.ThereIsExistingLog())
            {
                Debug.CreateLog();
            }

            //Debug for the Insert, Delete, Update methods in table
            //List<string> insert = new List<string>() { "Bozó", "bozo@gmail.com", "123456" };
            //Ceos ceo = new Ceos();
            //ceo.InsertValuesToThisTable(insert).ExecuteInsert();
            //ceo.UpdateColumn("name").ValueOf("Bozo").Where(1).ExecuteUpdate();
            //ceo.DeleteAt("id = 2").ExecuteDelte();


            Console.ReadKey();
        }
    }
}
