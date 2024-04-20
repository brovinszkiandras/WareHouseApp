﻿using System;
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
        private static List<string> ListOfDefaultTables = new List<string>() { "staff", "warehouses", "roles", "employees", "products", "orders", "permission" };
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
            string[] cities = File.ReadAllLines(@"..\..\SQL\WHhungarianCities.sql");
            string command = string.Empty;
            for (int i = 0; i < cities.Length; i++)
            {
                command += "\n" + cities[i];
            }
            //Console.WriteLine(command);
            SQL.SqlCommand(command);
        }

        private static void CreateAndFillPermission()
        {
            SQL.SqlCommand($"CREATE TABLE permission (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255), description TEXT);");
            string[] permission = File.ReadAllLines(@"..\..\SQL\Permission.sql");
            string command = string.Empty;
            for (int i = 0; i < permission.Length; i++)
            {
                command += "\n" + permission[i];
            }
            //Console.WriteLine(command);
            SQL.SqlCommand(command);
        }

        private static void CreateAndFillRoles()
        {
            SQL.SqlCommand($"CREATE TABLE roles (id INT PRIMARY KEY AUTO_INCREMENT, role VARCHAR(255) UNIQUE, in_warehouse BOOLEAN, description TEXT);");

            string[] roles = File.ReadAllLines(@"..\..\SQL\Roles.sql");
            string command = string.Empty;
            for (int i = 0; i < roles.Length; i++)
            {
                command += "\n" + roles[i];
            }
            //Console.WriteLine(command);
            SQL.SqlCommand(command);
        }

        private static void CreateAndFillRolePermission()
        {
            SQL.SqlCommand($"CREATE TABLE role_permission (role_id INT, permission_id INT, FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE CASCADE, FOREIGN KEY (permission_id) REFERENCES permission(id) ON DELETE CASCADE);");
            string[] role_permission = File.ReadAllLines(@"..\..\SQL\RolePermission.sql");
            string command = string.Empty;
            for (int i = 0; i < role_permission.Length; i++)
            {
                command += "\n" + role_permission[i];
            }
            //Console.WriteLine(command);
            SQL.SqlCommand(command);
        }

        #region Create Required Tables In Migartions
        public static void CreateDefaultTables(List<string> TableNames)
        {
            try
            {
                /*ROLES*/
                CreateAndFillRoles();

                /*STAFF*/
                SQL.SqlCommand($"CREATE TABLE {TableNames[0]} (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255), email VARCHAR(255) UNIQUE, password VARCHAR(255), profile_picture VARCHAR(255) DEFAULT 'DefaultStaffProfilePicture.png', role_id INT, FOREIGN KEY (role_id) REFERENCES roles(id));");
                /*WAREHOUSES*/
                SQL.SqlCommand($"CREATE TABLE {TableNames[1]} (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255) UNIQUE, length DOUBLE, width DOUBLE, height DOUBLE, volume DOUBLE);");
                /*EMPLOYEES*/
                SQL.SqlCommand($"CREATE TABLE {TableNames[2]} (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255), email VARCHAR(255) UNIQUE, password VARCHAR(255), role_id INT, warehouse_id INT, profile_picture VARCHAR(255) DEFAULT 'DefaultEmployeeProfile.png', FOREIGN KEY (role_id) REFERENCES roles(id), FOREIGN KEY (warehouse_id) REFERENCES {TableNames[1]}(id));");
                /*PRODUCTS*/
                SQL.SqlCommand($"CREATE TABLE {TableNames[3]} (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255), buying_price DOUBLE, selling_price DOUBLE, width DOUBLE, heigth DOUBLE, length DOUBLE, description TEXT, image VARCHAR(255) DEFAULT 'DefaultProductImage.png');");
                /*ORDERS*/
                SQL.SqlCommand($"CREATE TABLE {TableNames[4]} (id INT PRIMARY KEY AUTO_INCREMENT, qty INT, order_date DATETIME, address VARCHAR(255), status VARCHAR(255), product_id INT, user_name VARCHAR(255), payment_method VARCHAR(255), FOREIGN KEY (product_id) REFERENCES {TableNames[3]}(id) ON DELETE CASCADE);");

                /*PERMISSION*/
                CreateAndFillPermission();
                /*ROLE_PERMISSION*/
                CreateAndFillRolePermission();

                //TODO: Display missing for the boxes
                /*SECTOR*/
                SQL.SqlCommand($"CREATE TABLE sector (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255) UNIQUE, length DOUBLE, width DOUBLE, area DOUBLE, area_in_use DOUBLE DEFAULT 0, warehouse_id INT, FOREIGN KEY (warehouse_id) REFERENCES {TableNames[1]}(id) ON DELETE CASCADE);");
                /*SHELF*/
                SQL.SqlCommand($"CREATE TABLE shelf (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255), length DOUBLE, actual_length DOUBLE, width DOUBLE, sector_id INT, startXindex INT, startYindex INT, orientation VARCHAR(20), FOREIGN KEY (sector_id) REFERENCES sector(id) ON DELETE CASCADE);");
                /*LEVEL OF SHELF*/
                SQL.SqlCommand($"CREATE TABLE level_of_shelf (id INT PRIMARY KEY AUTO_INCREMENT, upper_space DOUBLE, weight_capacity DOUBLE, shelf_id INT, FOREIGN KEY (shelf_id) REFERENCES shelf(id) ON DELETE CASCADE);");

                Tables.addRequriedTablesToTables();
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
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

                    List<string> TablesNeedName = new List<string>() { "staff", "warehouses", "employees", "products", "orders" };
                    CreateDefaultTables(TablesNeedName);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error", "Can not create the requerd tables");
                    Debug.WriteError(ex);

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
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('roles', 'roles', 'Roles');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('employees', '{TableNamesByUser[2]}', 'Employees');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('products', '{TableNamesByUser[3]}', 'Products');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('orders', '{TableNamesByUser[4]}', 'Orders');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('permission', 'permission', 'Permission');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('role_permission', 'role_permission', 'Role Permission');");
                    CreateDefaultTables(TableNamesByUser);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error", "Can not create the requerd tables");
                    Debug.WriteError(ex);
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
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('roles', 'roles', 'Roles');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('employees', '{TableNamesByUser[3]}', '{TableNiceNamesByUser[3]}');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('products', '{TableNamesByUser[4]}', '{TableNiceNamesByUser[4]}');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('orders', '{TableNamesByUser[5]}', '{TableNiceNamesByUser[5]}');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('permission', 'permission', 'Permission');");
                    SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('role_permission', 'role_permission', 'Role Permission');");
                    CreateDefaultTables(TableNamesByUser);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error", "Can not create the requerd tables");
                    Debug.WriteError(ex);
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
                    Dictionary<string, Action> Tables_Refresh = new Dictionary<string, Action>();
                    Tables_Refresh.Add(Tables.staff.actual_name, Tables.staff.Refresh);
                    Tables_Refresh.Add(Tables.warehouses.actual_name, Tables.warehouses.Refresh);
                    Tables_Refresh.Add(Tables.employees.actual_name, Tables.employees.Refresh);
                    Tables_Refresh.Add(Tables.orders.actual_name, Tables.orders.Refresh);
                    Tables_Refresh.Add(Tables.products.actual_name, Tables.products.Refresh);
                    Tables_Refresh.Add(Tables.roles.actual_name, Tables.roles.Refresh);

                    foreach (var Tables in Tables_Refresh)
                    {
                        if (Tables.Key == "orders")
                        {
                            SQL.SqlCommand($"ALTER TABLE {Tables.Key} ADD created_at TIMESTAMP DEFAULT NOW();");
                            Tables.Value();
                        }
                        else
                        {
                            SQL.SqlCommand($"ALTER TABLE {Tables.Key} ADD created_at TIMESTAMP DEFAULT NOW(), ADD updated_at TIMESTAMP DEFAULT NOW();");
                            Tables.Value();
                        }
                    }
                    Tables.features.getFeature("Date log")["in_use"] = true;
                    Tables.features.updateChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
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

                    SQL.SqlCommand("CREATE TABLE CARS (id INT PRIMARY KEY AUTO_INCREMENT," +
                        " plate_number VARCHAR(255) UNIQUE NOT NULL," +
                        " type VARCHAR(255) NOT NULL," +
                        " ready BOOLEAN NOT NULL," +
                        " km DOUBLE," +
                        " last_service DATE," +
                        " last_exam DATE," +
                        " warehouse_id INT," +
                        " FOREIGN KEY (warehouse_id) REFERENCES warehouses(id) ON DELETE CASCADE);");

                    SQL.SqlCommand($"CREATE TABLE TRANSPORTS (id INT PRIMARY KEY AUTO_INCREMENT," +
                        $"employee_id INT," +
                        $"car_id INT," +
                        $"status VARCHAR(255)," +
                        $"start_date TIMESTAMP DEFAULT NOW()," +
                        $"end_date TIMESTAMP null," +
                        $"warehouse_id int," +
                        $"FOREIGN KEY (employee_id) REFERENCES {employeesActualName}(id) ON DELETE CASCADE," +
                        $"FOREIGN KEY (car_id) REFERENCES CARS(id) ON DELETE CASCADE," +
                        $"FOREIGN KEY (warehouse_id) REFERENCES warehouses(id) ON DELETE CASCADE);");

                    if (SQL.FindOneDataFromQuery("SELECT name FROM migrations WHERE name = 'cars'") == string.Empty && SQL.FindOneDataFromQuery("SELECT name FROM migrations WHERE name = 'transports'") == string.Empty)
                    {
                        SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('cars', 'cars', 'Cars');");
                        SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('transports', 'transports', 'Transports');");
                    }

                    SQL.SqlCommand($"ALTER TABLE {ordersActualName} ADD transport_id INT, ADD CONSTRAINT fk_transport_id FOREIGN KEY (transport_id) REFERENCES transports (id) ON DELETE CASCADE;");

                    Tables.features.getFeature("Fleet")["in_use"] = true;

                    Tables.orders.Refresh();
                    Tables.features.updateChanges();
                    Tables.addFleetTablesToTables();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
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
                    string ordersActualName = SQL.FindOneDataFromQuery("SELECT actual_name FROM migrations WHERE name = 'orders'");

                    CreateAndFillCityTable();
                    if (SQL.FindOneDataFromQuery("SELECT name FROM migrations WHERE name = 'cities'") == string.Empty)
                    {
                        SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('cities', 'cities', 'Cities');");
                    }

                    SQL.SqlCommand($"ALTER TABLE {warehousesActualName} ADD city_id INT, ADD CONSTRAINT fk_city_id FOREIGN KEY (city_id) REFERENCES cities(id) ON DELETE CASCADE;");
                    SQL.SqlCommand($"ALTER TABLE {ordersActualName} ADD city_id INT, ADD CONSTRAINT fk_city_id_orders FOREIGN KEY (city_id) REFERENCES cities(id) ON DELETE CASCADE;");

                    Tables.features.getFeature("City")["in_use"] = true;
                    Tables.warehouses.Refresh();
                    Tables.orders.Refresh();
                    Tables.features.updateChanges();
                    Tables.addCityTableToTables();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
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
                    SQL.SqlCommand($"CREATE TABLE LOG (id INT PRIMARY KEY AUTO_INCREMENT, employee_id INT, log_message TEXT, updated_at TIMESTAMP DEFAULT NOW(), FOREIGN KEY (employee_id) REFERENCES {employeesActualName}(id) ON DELETE CASCADE);");

                    if (SQL.FindOneDataFromQuery("SELECT name FROM migrations WHERE name = 'log'") == string.Empty)
                    {
                        SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('log', 'log', 'Log');");
                    }

                    Tables.features.getFeature("Log")["in_use"] = true;
                    Tables.features.updateChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
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
                    SQL.SqlCommand($"ALTER TABLE {Tables.employees.actual_name} ADD activity BOOLEAN DEFAULT FALSE NOT NULL, ADD is_loggedin BOOLEAN DEFAULT FALSE NOT NULL;");

                    Tables.features.getFeature("Activity")["in_use"] = true;
                    Tables.employees.Refresh();
                    Tables.features.updateChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
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
                    SQL.SqlCommand($"ALTER TABLE {Tables.employees.actual_name} ADD payment DOUBLE;");
                    SQL.SqlCommand($"ALTER TABLE {Tables.warehouses.actual_name} ADD total_value DOUBLE, ADD total_spending DOUBLE, ADD total_income DOUBLE;");

                    SQL.SqlCommand($"CREATE TABLE revenue_a_day (id INT PRIMARY KEY AUTO_INCREMENT, warehouse_id INT, date DATETIME, total_expenditure DOUBLE, total_income DOUBLE, FOREIGN KEY (warehouse_id) REFERENCES {Tables.warehouses.actual_name}(id));");

                    if (SQL.FindOneDataFromQuery("SELECT name FROM migrations WHERE name = 'revenue_a_day'") == string.Empty)
                    {
                        SQL.SqlCommand("INSERT INTO migrations (name, actual_name, nice_name) VALUE ('revenue_a_day', 'revenue_a_day', 'Revenue and expenditure');");
                    }

                    Tables.employees.Refresh();
                    Tables.warehouses.Refresh();

                    Tables.features.getFeature("Revenue")["in_use"] = true;
                    Tables.features.updateChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
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
                    SQL.SqlCommand($"ALTER TABLE {Tables.products.actual_name} ADD weight DOUBLE, ADD volume DOUBLE(10,2);");
                    SQL.SqlCommand($"ALTER TABLE cars ADD storage DOUBLE DEFAULT 0, ADD carrying_capacity DOUBLE DEFAULT 0;");
                    SQL.SqlCommand($"ALTER TABLE {Tables.orders.actual_name} ADD sum_volume DOUBLE;");

                    Tables.features.getFeature("Storage")["in_use"] = true;
                    Tables.features.updateChanges();

                    Tables.cars.Refresh();
                    Tables.products.Refresh();
                    Tables.orders.Refresh();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
                //Tables.cars.Refresh();
                //Tables.cars.database.Columns["storage"].AllowDBNull = false;
                //Tables.cars.database.Columns["carrying_capacity"].AllowDBNull = false;
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

                    SQL.SqlCommand($"ALTER TABLE {carsActualName} ADD consumption DOUBLE DEFAULT 0, ADD gas_tank_size DOUBLE DEFAULT 0;");
                    Tables.features.getFeature("Fuel")["in_use"] = true;
                    Tables.features.updateChanges();

                    Tables.cars.Refresh();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
                //Tables.cars.Refresh();
                //Tables.cars.database.Columns["consumption"].AllowDBNull = false;
                //Tables.cars.database.Columns["gas_tank_size"].AllowDBNull = false;
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

                    if (SQL.FindOneDataFromQuery("SELECT name FROM migrations WHERE name = 'dock'") == string.Empty)
                    {
                        SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('dock', 'dock', 'Dock');");
                    }

                    SQL.SqlCommand($"CREATE TABLE DOCK (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255), free BOOLEAN, warehouse_id INT, FOREIGN KEY (warehouse_id) REFERENCES {warehousesActualName}(id) ON DELETE CASCADE);");
                    SQL.SqlCommand($"ALTER TABLE {transportsActualName} ADD dock_id INT, ADD CONSTRAINT fk_dock_id FOREIGN KEY (dock_id) REFERENCES DOCK (id) ON DELETE CASCADE;");
                    Tables.features.getFeature("Dock")["in_use"] = true;
                    Tables.features.updateChanges();

                    Tables.transports.Refresh();
                    Tables.addDockTableToTables();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
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

                    if (SQL.FindOneDataFromQuery("SELECT name FROM migrations WHERE name = 'forklift'") == string.Empty)
                    {
                        SQL.SqlCommand($"INSERT INTO migrations (name, actual_name, nice_name) VALUE ('forklift', 'forklift', 'Forklift');");
                    }

                    SQL.SqlCommand($"CREATE TABLE FORKLIFT(id INT PRIMARY KEY AUTO_INCREMENT, warehouse_id INT, type VARCHAR(255), status VARCHAR(255), operating_hours INT DEFAULT 0, FOREIGN KEY (warehouse_id) REFERENCES {warehousesActualName}(id) ON DELETE CASCADE);");
                    Tables.features.getFeature("Forklift")["in_use"] = true;
                    Tables.features.updateChanges();

                    Tables.addForkliftTableToTables();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }
        #endregion
        #region Fetures off
        public static void DateLogOff()
        {
            if (FeatureInUse("Date Log"))
            {
                try
                {
                    Dictionary<string, Action> Tables_Refresh = new Dictionary<string, Action>();
                    Tables_Refresh.Add(Tables.staff.actual_name, Tables.staff.Refresh);
                    Tables_Refresh.Add(Tables.warehouses.actual_name, Tables.warehouses.Refresh);
                    Tables_Refresh.Add(Tables.employees.actual_name, Tables.employees.Refresh);
                    Tables_Refresh.Add(Tables.orders.actual_name, Tables.orders.Refresh);
                    Tables_Refresh.Add(Tables.products.actual_name, Tables.products.Refresh);
                    Tables_Refresh.Add(Tables.roles.actual_name, Tables.roles.Refresh);

                    foreach (var Tables in Tables_Refresh)
                    {
                        if (Tables.Key == "orders")
                        {
                            SQL.SqlCommand($"ALTER TABLE {Tables.Key} DROP COLUMN created_at");
                            Tables.Value();
                        }
                        else
                        {
                            SQL.SqlCommand($"ALTER TABLE {Tables.Key} DROP COLUMN created_at, DROP COLUMN updated_at");
                            Tables.Value();
                        }
                    }
                    Tables.features.getFeature("Date log")["in_use"] = false;
                    Tables.features.updateChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }
        public static void FleetOff()
        {
            if (FeatureInUse("Fleet"))
            {
                try
                {
                    DockOff();
                    FuelOff();
                    StorageOff();

                    //SQL.SqlCommand($"ALTER TABLE `{Tables.transports.actual_name}` DROP CONSTRAINT `employee_id`;");
                    //SQL.SqlCommand($"ALTER TABLE `{Tables.transports.actual_name}` DROP CONSTRAINT `car_id`;");
                    SQL.SqlCommand($"ALTER TABLE `{Tables.transports.actual_name}` DROP CONSTRAINT transports_ibfk_1");
                    SQL.SqlCommand($"ALTER TABLE `{Tables.transports.actual_name}` DROP CONSTRAINT transports_ibfk_2");
                    SQL.SqlCommand($"ALTER TABLE `{Tables.orders.actual_name}` DROP CONSTRAINT `fk_transport_id`; ALTER TABLE `{Tables.orders.actual_name}` DROP COLUMN transport_id;");

                    SQL.SqlCommand($"DROP TABLE `{Tables.transports.actual_name}`;");
                    SQL.SqlCommand($"DROP TABLE `{Tables.cars.actual_name}`;");

                    Tables.features.getFeature("Fleet")["in_use"] = false;
                    Tables.features.updateChanges();


                    Tables.DisableFleetFeature();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }
        public static void CityOff()
        {
            if (FeatureInUse("City"))
            {
                try
                {
                    SQL.SqlCommand($"ALTER TABLE `{Tables.warehouses.actual_name}` DROP CONSTRAINT `fk_city_id`; ALTER TABLE `{Tables.warehouses.actual_name}` DROP COLUMN city_id;");
                    SQL.SqlCommand($"ALTER TABLE `{Tables.orders.actual_name}` DROP CONSTRAINT `fk_city_id_orders`; ALTER TABLE `{Tables.orders.actual_name}` DROP COLUMN city_id;");
                    SQL.SqlCommand($"DROP TABLE `{Tables.cities.actual_name}`");

                    Tables.features.getFeature("City")["in_use"] = false;
                    Tables.features.updateChanges();

                    Tables.disableCityFeature();


                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }

        public static void LogOff()
        {
            if (FeatureInUse("Log"))
            {
                try
                {
                    SQL.SqlCommand($"ALTER TABLE `log` DROP CONSTRAINT `log_ibfk_1`;");
                    SQL.SqlCommand($"DROP TABLE `log`");

                    Tables.features.getFeature("Log")["in_use"] = false;
                    Tables.features.updateChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }

        public static void ActivityOff()
        {
            if (FeatureInUse("Activity"))
            {
                try
                {
                    SQL.SqlCommand($"ALTER TABLE {Tables.employees.actual_name} DROP COLUMN activity, DROP COLUMN is_loggedin;");

                    Tables.features.getFeature("Activity")["in_use"] = false;
                    Tables.features.updateChanges();

                    Tables.employees.database.Columns.Remove("activity");
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }

        public static void RevenueOff()
        {
            if (FeatureInUse("Revenue"))
            {
                try
                {
                    SQL.SqlCommand($"ALTER TABLE `{Tables.employees.actual_name}` DROP `payment`;");
                    SQL.SqlCommand($"ALTER TABLE `{Tables.warehouses.actual_name}` DROP `total_value`, DROP `total_spending`,DROP `total_income`;");

                    SQL.SqlCommand($"ALTER TABLE `revenue_a_day` DROP CONSTRAINT `revenue_a_day_ibfk_1`;");
                    SQL.SqlCommand("DROP TABLE revenue_a_day");

                    Tables.features.getFeature("Revenue")["in_use"] = false;
                    Tables.features.updateChanges();

                    Tables.warehouses.database.Columns.Remove("total_value");
                    Tables.warehouses.database.Columns.Remove("total_spending");
                    Tables.warehouses.database.Columns.Remove("total_income");
                    Tables.employees.database.Columns.Remove("payment");
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }

        public static void StorageOff()
        {
            if (FeatureInUse("Storage"))
            {
                try
                {
                    SQL.SqlCommand($"ALTER TABLE {Tables.products.actual_name} DROP `weight`, DROP `volume`;");
                    SQL.SqlCommand($"ALTER TABLE {Tables.cars.actual_name} DROP `storage`, DROP `carrying_capacity`;");
                    SQL.SqlCommand($"ALTER TABLE {Tables.orders.actual_name} DROP `sum_volume`;");

                    Tables.features.getFeature("Storage")["in_use"] = false;
                    Tables.features.updateChanges();

                    Tables.products.database.Columns.Remove("weight");
                    Tables.products.database.Columns.Remove("volume");
                    Tables.cars.database.Columns.Remove("storage");
                    Tables.cars.database.Columns.Remove("carrying_capacity");
                    Tables.orders.database.Columns.Remove("sum_volume");
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }

        public static void FuelOff()
        {
            if (FeatureInUse("Fuel"))
            {
                try
                {
                    SQL.SqlCommand($"ALTER TABLE {Tables.cars.actual_name} DROP `consumption`, DROP `gas_tank_size`;");

                    Tables.features.getFeature("Fuel")["in_use"] = false;
                    Tables.features.updateChanges();

                    Tables.cars.database.Columns.Remove("consumption");
                    Tables.cars.database.Columns.Remove("gas_tank_size");
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }
        public static void DockOff()
        {
            if (FeatureInUse("Dock"))
            {
                try
                {
                    SQL.SqlCommand($"ALTER TABLE {Tables.transports.actual_name} DROP CONSTRAINT `fk_dock_id`; ALTER TABLE `{Tables.transports.actual_name}` DROP `dock_id`;");
                    SQL.SqlCommand($"DROP TABLE `{Tables.docks.actual_name}`;");

                    

                    Tables.disableDockFeature();

                    Tables.features.getFeature("Dock")["in_use"] = false;
                    Tables.features.updateChanges();

                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }
        public static void ForkliftOff()
        {
            if (FeatureInUse("Forklift"))
            {
                try
                {
                    SQL.SqlCommand($"DROP TABLE `forklift`");
                    Tables.features.getFeature("Forklift")["in_use"] = false;
                    Tables.features.updateChanges();

                    Tables.disableForkliftFeauture();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
        }
        #endregion

        public static bool IsMigrationContainsAllDefaultTables()
        {
            if (SQL.Tables().Contains("migrations"))
            {
                List<string> TablesInMigrations = SQL.GetElementOfListArray(SQL.SqlQuery("SELECT name FROM migrations"));

                for (int i = 0; i < ListOfDefaultTables.Count; i++)
                {
                    if (!TablesInMigrations.Contains(ListOfDefaultTables[i]))
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

        public static void CreateWarehouse(string WarehouseName)
        {
            try
            {
                SQL.SqlCommand($"CREATE TABLE {WarehouseName} (id INT PRIMARY KEY AUTO_INCREMENT NOT NULL, product_id INT, qty INT, shelf_id INT, width DOUBLE, height DOUBLE, length DOUBLE, on_shelf_level INT, is_in_box BOOLEAN, FOREIGN KEY (product_id) REFERENCES {Tables.products.actual_name}(id) ON DELETE CASCADE, FOREIGN KEY (shelf_id) REFERENCES shelf(id) ON DELETE CASCADE);");
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
                throw;
            }
        }
    }
}
