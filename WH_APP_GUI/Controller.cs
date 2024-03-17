using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace WH_APP_GUI
{
    class Controller
    {
        private static List<string> ListOfDefaultTables = new List<string>() { "staff", "warehouses", "roles", "employees", "products", "orders", "permission", "role_permission", "revenue_a_day" };
        #region Migrations Feture
        public static void CreateMigration()
        {
            if (!SQL.Tables().Contains("migrations"))
            {
                SQL.SqlCommand("CREATE TABLE migrations(id int AUTO_INCREMENT PRIMARY KEY NOT NULL, name varchar(50) UNIQUE, actual_name varchar(100), nice_name varchar(150) NULL);");
            }
        }
        public static void CreateFeature()
        {
            if (!SQL.Tables().Contains("feature"))
            {
                SQL.SqlCommand("CREATE TABLE feature(id int AUTO_INCREMENT PRIMARY KEY NOT NULL, name varchar(50) UNIQUE, in_use BOOLEAN DEFAULT FALSE);");
                FillFeatureTable();
            }
        }
        private static void FillFeatureTable()
        {
            List<string> NameOfFeatures = new List<string>() { "Date Log", "Fleet", "City", "Log", "Activity", "Revenue", "Storage", "Fuel", "Dock", "Forklift" };
            for (int i = 0; i < NameOfFeatures.Count; i++)
            {
                SQL.SqlCommand($"INSERT INTO feature (name, in_use) VALUE ('{NameOfFeatures[i]}', FALSE);");
            }
        }
        #endregion
        private static void CreateAndFillCityTable()
        {
            string[] cities = File.ReadAllLines("WHhungarianCities.sql");
            string command = string.Empty;
            for (int i = 0; i < cities.Length; i++)
            {
                command += "\n" + cities[i];
            }
            //Console.WriteLine(command);
            SQL.SqlCommand(command);
        }

        #region Create Required Tables In Migartions
        private static void CreateRevenue_A_Day()
        {
            string warehousesActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'warehouses'");


            SQL.SqlCommand($"CREATE TABLE revenue_a_day(id INT PRIMARY KEY AUTO_INCREMENT, warehouse_id INT, date TIMESTAMP DEFAULT NOW(), total_expenditure DECIMAL(10,2), total_income DECIMAL(10,2), FOREIGN KEY (warehouse_id) REFERENCES {warehousesActualName}(id));");
            SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('revenue_a_day', 'revenue_a_day', 'Revenue and expenditure');");
        }
        public static void CreateDefaultTables(List<string> TableNames)
        {
            try
            {
                /*ROLES*/
                SQL.SqlCommand($"CREATE TABLE {TableNames[2]} (id INT PRIMARY KEY AUTO_INCREMENT, role_name VARCHAR(255) UNIQUE, description TEXT);");
                /*STAFF*/
                SQL.SqlCommand($"CREATE TABLE {TableNames[0]} (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255), email VARCHAR(255) UNIQUE, password VARCHAR(255), role_id INT, FOREIGN KEY (role_id) REFERENCES {TableNames[2]}(id));");
                /*WAREHOUSES*/
                SQL.SqlCommand($"CREATE TABLE {TableNames[1]} (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255) UNIQUE, staff_id INT, FOREIGN KEY (staff_id) REFERENCES {TableNames[0]}(id));");
                /*EMPLOYEES*/
                SQL.SqlCommand($"CREATE TABLE {TableNames[3]} (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255), email VARCHAR(255) UNIQUE, password VARCHAR(255), role_id INT, warehouse_id INT, profile_picture VARCHAR(255) DEFAULT 'profile_picture.png', FOREIGN KEY (role_id) REFERENCES {TableNames[2]}(id), FOREIGN KEY (warehouse_id) REFERENCES {TableNames[1]}(id));");
                /*PRODUCTS*/
                SQL.SqlCommand($"CREATE TABLE {TableNames[4]} (id INT PRIMARY KEY AUTO_INCREMENT, product_name VARCHAR(255), price DECIMAL(10, 2));");
                /*ORDERS*/
                SQL.SqlCommand($"CREATE TABLE {TableNames[5]} (id INT PRIMARY KEY AUTO_INCREMENT, qty INT, order_date DATETIME, warehouse_id INT, product_id INT, user_name VARCHAR(255), payment_method VARCHAR(255), FOREIGN KEY (warehouse_id) REFERENCES {TableNames[1]}(id), FOREIGN KEY (product_id) REFERENCES {TableNames[4]}(id));");
                /*PERMISSION*/
                SQL.SqlCommand($"CREATE TABLE {TableNames[6]} (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255), description TEXT);");
                /*ROLE_PERMISSION*/
                SQL.SqlCommand($"CREATE TABLE {TableNames[7]} (id INT PRIMARY KEY AUTO_INCREMENT, role_id INT, permission_id INT, FOREIGN KEY (role_id) REFERENCES {TableNames[2]}(id), FOREIGN KEY (permission_id) REFERENCES {TableNames[6]}(id));");

                CreateRevenue_A_Day();
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex.ToString());
                throw;
            }
        }
        private static List<string> NameRowsInMigrations()
        {
            List<string> returnList = new List<string>();
            List<string[]> lis = SQL.SqlQuery("SELECT name FROM migrations");
            for (int i = 0; i < lis.Count; i++)
            {
                returnList.Add(lis[i][0]);
            }
            return returnList;
        }

        public static void CreateDefaultTablesWithMigrationInsert()
        {
            if (!NameRowsInMigrations().OrderBy(e => e).SequenceEqual(ListOfDefaultTables.OrderBy(e => e)))
            {
                try
                {
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('staff', 'staff', 'Staff');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('warehouses', 'warehouses', 'Warehouses');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('roles', 'roles', 'Roles');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('employees', 'employees', 'Employees');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('products', 'products', 'Products');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('orders', 'orders', 'Orders');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('permission', 'permission', 'Permission');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('role_permission', 'role_permission', 'Role Permission');");
                    
                    CreateDefaultTables(ListOfDefaultTables);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error", "Can not create the requerd tables");
                    Debug.WriteError(ex.ToString());

                    throw;
                }
            }
        }
        public static void CreateDefaultTablesWithMigrationInsert(List<string> TableNamesByUser)
        {
            if (!NameRowsInMigrations().OrderBy(e => e).SequenceEqual(ListOfDefaultTables.OrderBy(e => e)))
            {
                try
                {
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('staff', '{TableNamesByUser[0]}', 'Staff');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('warehouses', '{TableNamesByUser[1]}', 'Warehouses');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('roles', '{TableNamesByUser[2]}', 'Roles');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('employees', '{TableNamesByUser[3]}', 'Employees');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('products', '{TableNamesByUser[4]}', 'Products');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('orders', '{TableNamesByUser[5]}', 'Orders');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('permission', '{TableNamesByUser[6]}', 'Permission');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('role_permission', '{TableNamesByUser[7]}', 'Role Permission');");
                    CreateDefaultTables(TableNamesByUser);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error", "Can not create the requerd tables");
                    Debug.WriteError(ex.ToString());
                }
            }
        }
        public static void CreateDefaultTablesWithMigrationInsert(List<string> TableNamesByUser, List<string> TableNiceNamesByUser)
        {
            if (!NameRowsInMigrations().OrderBy(e => e).SequenceEqual(ListOfDefaultTables.OrderBy(e => e)))
            {
                try
                {
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('staff', '{TableNamesByUser[0]}', '{TableNiceNamesByUser[0]}');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('warehouses', '{TableNamesByUser[1]}', '{TableNiceNamesByUser[1]}');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('roles', '{TableNamesByUser[2]}', '{TableNiceNamesByUser[2]}');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('employees', '{TableNamesByUser[3]}', '{TableNiceNamesByUser[3]}');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('products', '{TableNamesByUser[4]}', '{TableNiceNamesByUser[4]}');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('orders', '{TableNamesByUser[5]}', '{TableNiceNamesByUser[5]}');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('permission', '{TableNamesByUser[6]}', '{TableNiceNamesByUser[6]}');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('role_permission', '{TableNamesByUser[7]}', '{TableNiceNamesByUser[7]}');");
                    CreateDefaultTables(TableNamesByUser);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error", "Can not create the requerd tables");
                    Debug.WriteError(ex.ToString());
                }
            }
        }
        #endregion
        #region Features
        private static bool FeatureInUse(string FeatureName)
        {
            return bool.Parse(SQL.FindOneDataFromQuery($"SELECT in_use FROM feature WHERE name = '{FeatureName}'"));
        }
        //DateLog: Nincs másik feature-hez kötve
        public static void DateLog()
        {
            if (!FeatureInUse("Date Log"))
            {
                try
                {
                    List<string> Tables = new List<string>() { "staff", "warehouses", "employees", "orders", "products", "roles" };
                    for (int i = 0; i < Tables.Count; i++)
                    {
                        string actual_name = SQL.FindOneDataFromQuery($"SELECT actual_name FROM migrations WHERE name = '{Tables[i]}'");
                        if (actual_name == "orders")
                        {
                            SQL.SqlCommand($"ALTER TABLE {actual_name} ADD created_at TIMESTAMP DEFAULT NOW();");
                        }
                        else
                        {
                            SQL.SqlCommand($"ALTER TABLE {actual_name} ADD created_at TIMESTAMP DEFAULT NOW(), ADD udpdated_at TIMESTAMP DEFAULT NOW();");
                        }
                    }
                    SQL.SqlCommand($"UPDATE `feature` SET `in_use`= TRUE WHERE name = 'Date Log'");
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex.ToString());
                    throw;
                }
            }
        }

        //Fleet: Nincs másik feature-hez kötve, Létrehoz egy transport_id-t az ORDERS táblába
        public static void Fleet()
        {
            if (!FeatureInUse("Fleet"))
            {
                try
                {
                    string employeesActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'employees'");
                    string ordersActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'orders'");

                    SQL.SqlCommand("CREATE TABLE CARS (id INT PRIMARY KEY AUTO_INCREMENT, plate_number VARCHAR(255) UNIQUE, type VARCHAR(255), ready BOOLEAN, km DECIMAL(10, 2), last_service DATETIME, last_exam DATETIME);");
                    SQL.SqlCommand($"CREATE TABLE TRANSPORTS (id INT PRIMARY KEY AUTO_INCREMENT, employee_id INT, car_id INT, is_transported BOOLEAN, start_date TIMESTAMP DEFAULT NOW(), end_date TIMESTAMP DEFAULT NOW(), FOREIGN KEY (employee_id) REFERENCES {employeesActualName}(id), FOREIGN KEY (car_id) REFERENCES CARS(id));");

                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('cars', 'cars', 'Cars');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('transports', 'transports', 'Transports');");

                    SQL.SqlCommand($"ALTER TABLE {ordersActualName} ADD transport_id INT, ADD CONSTRAINT fk_transport_id FOREIGN KEY (transport_id) REFERENCES transports (id);");

                    SQL.SqlCommand($"UPDATE `feature` SET `in_use`= TRUE WHERE name = 'Fleet'");
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex.ToString());
                    throw;
                }
            }
        }

        //City: Nincs másik feature-hez kötve, Létrehoz egy city_id-t a WAREHOUSES táblába
        public static void City()
        {
            if (!FeatureInUse("City"))
            {
                try
                {
                    string warehousesActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'warehouses'");
                    //SQL.SqlCommand("CREATE TABLE CITIES (id INT PRIMARY KEY AUTO_INCREMENT, city_name VARCHAR(255), longtitude DECIMAL(10, 8), latitude DECIMAL(10, 8));");
                    CreateAndFillCityTable();
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('cities', 'cities', 'Cities');");
                    SQL.SqlCommand($"ALTER TABLE {warehousesActualName} ADD city_id INT, ADD CONSTRAINT fk_city_id FOREIGN KEY (city_id) REFERENCES cities(id);");

                    SQL.SqlCommand($"UPDATE `feature` SET `in_use`= TRUE WHERE name = 'City'");

                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex.ToString());
                    throw;
                }
            }
        }

        //Log: Nincs másik feature-hez kötve
        public static void Log()
        {
            if (!FeatureInUse("Log"))
            {
                try
                {
                    string employeesActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'employees'");
                    SQL.SqlCommand($"CREATE TABLE LOG (id INT PRIMARY KEY AUTO_INCREMENT, employee_id INT, log_message TEXT, updated_at TIMESTAMP DEFAULT NOW(), FOREIGN KEY (employee_id) REFERENCES {employeesActualName}(id));");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('log', 'log', 'Log');");

                    SQL.SqlCommand($"UPDATE `feature` SET `in_use`= TRUE WHERE name = 'Log'");
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex.ToString());
                    throw;
                }
            }
        }

        //Activity: Nincs másik feature-hez kötve, létrehoz kettő új oszlopot az EMPLOYEES táblába
        public static void Activity()
        {
            if (!FeatureInUse("Activity"))
            {
                try
                {
                    string employeesActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'employees'");
                    SQL.SqlCommand($"ALTER TABLE {employeesActualName} ADD activity BOOLEAN, ADD is_loggedin BOOLEAN;");

                    SQL.SqlCommand($"UPDATE `feature` SET `in_use`= TRUE WHERE name = 'Activity'");

                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex.ToString());
                    throw;
                }
            }
        }

        //Revenue: Nince másik feature-hez kötve, létrehoz 3 új oszlopt a WAREHOUSES táblába és egy új oszlopot az EMPLOYEES táblába
        public static void Revenue()
        {
            if (!FeatureInUse("Revenue"))
            {
                try
                {
                    string employeesActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'employees'");
                    string warehousesActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'warehouses'");

                    SQL.SqlCommand($"ALTER TABLE {employeesActualName} ADD payment DECIMAL(10, 2);");
                    SQL.SqlCommand($"ALTER TABLE {warehousesActualName} ADD total_value DECIMAL(10, 2), ADD total_spending DECIMAL(10, 2), ADD total_income DECIMAL(10, 2);");

                    SQL.SqlCommand($"UPDATE `feature` SET `in_use`= TRUE WHERE name = 'Revenue'");
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex.ToString());
                    throw;
                }
            }
        }

        //Storage: Addig nem hozható létre amíg a Fleet nincsen létrehozva. Létrehoz a CARS táblába egy új oszlopot, létrehoz kettő új oszlopot a PRODUCTS táblába
        public static void Storage()
        {
            if (!FeatureInUse("Storage"))
            {
                try
                {
                    string productsActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'products'");
                    string carsActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'cars'");
                    string ordersActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'orders'");

                    SQL.SqlCommand($"ALTER TABLE {productsActualName} ADD weight DECIMAL(10, 2), ADD volume DECIMAL(10,2);");
                    SQL.SqlCommand($"ALTER TABLE {carsActualName} ADD storage DECIMAL(10, 2), ADD carrying_capacity DECIMAL(10, 2);");
                    SQL.SqlCommand($"ALTER TABLE {ordersActualName} ADD sum_volume DECIMAL(10, 2);");


                    SQL.SqlCommand($"UPDATE `feature` SET `in_use`= TRUE WHERE name = 'Storage'");
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex.ToString());
                    throw;
                }
            }
        }

        //Fuel: Addig nem hozható létre amíg a Fleet nincsen létrehozva. Létrehoz kettő új oszlopot a CARS táblába
        public static void Fuel()
        {
            if (!FeatureInUse("Fuel"))
            {
                try
                {
                    string carsActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'cars'");

                    SQL.SqlCommand($"ALTER TABLE {carsActualName} ADD consumption DECIMAL(10, 2), ADD gas_tank_size DECIMAL(10, 2);");

                    SQL.SqlCommand($"UPDATE `feature` SET `in_use`= TRUE WHERE name = 'Fuel'");
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex.ToString());
                    throw;
                }
            }
        }

        //Dock: Addig nem hozható létre amíg a Fleet nincsen létrehozva. Létrehoz egy dock_id-t a TRANSPORTS táblába, létrehoz egy DOCK táblát
        public static void Dock()
        {
            if (!FeatureInUse("Dock"))
            {
                try
                {
                    string transportsActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'transports'");
                    string warehousesActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'warehouses'");

                    SQL.SqlCommand($"CREATE TABLE DOCK (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255), free BOOLEAN, warehouse_id INT, FOREIGN KEY (warehouse_id) REFERENCES {warehousesActualName}(id));");

                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('dock', 'dock', 'Dock');");

                    SQL.SqlCommand($"ALTER TABLE {transportsActualName} ADD dock_id INT, ADD CONSTRAINT fk_dock_id FOREIGN KEY (dock_id) REFERENCES DOCK (id);");

                    SQL.SqlCommand($"UPDATE `feature` SET `in_use`= TRUE WHERE name = 'Dock'");
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex.ToString());
                    throw;
                }
            }
        }
        //Nincs feature-hez kötve
        public static void Forklift()
        {
            if (!FeatureInUse("Forklift"))
            {
                try
                {
                    string warehousesActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'warehouses'");

                    SQL.SqlCommand($"CREATE TABLE FORKLIFT(id INT PRIMARY KEY AUTO_INCREMENT, warehouse_id INT, type VARCHAR(255), status VARCHAR(255), operating_hours INT, FOREIGN KEY (warehouse_id) REFERENCES {warehousesActualName}(id));");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('forklift', 'forklift', 'Forklift');");
                    SQL.SqlCommand($"UPDATE `feature` SET `in_use`= TRUE WHERE name = 'Forklift'");
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex.ToString());
                    throw;
                }
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

        public static List<string> GetElementOfListArray(List<string[]> ListWithArray, int index)
        {
            List<string> returnList = new List<string>();
            for (int i = 0; i < ListWithArray.Count; i++)
            {
                returnList.Add(ListWithArray[i][index]);
            }

            return returnList;
        }

        public static bool IsMigrationContainsAllDefaultTables()
        {
            if (SQL.Tables().Contains("migrations"))
            {
                List<string> TablesInMigrations = Controller.GetElementOfListArray(SQL.SqlQuery("SELECT name FROM migrations"));

                for (int i = 0; i < ListOfDefaultTables.Count; i++)
                {
                    if (! TablesInMigrations.Contains(ListOfDefaultTables[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
