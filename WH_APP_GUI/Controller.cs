﻿using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
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
        #region Feture
    
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
            List<string> NameOfFeatures = new List<string>() { "Date Log", "Fleet", "Log", "Activity", "Revenue", "Storage", "Fuel", "Dock", "Forklift" };
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
            SQL.SqlCommand(File.ReadAllText(@"..\..\SQL\RolePermission.sql"));
        }

        #region Create Required Tables In Migartions
        private static void City()
        {
            try
            {
                CreateAndFillCityTable();        

                SQL.SqlCommand($"ALTER TABLE warehouses ADD city_id INT, ADD CONSTRAINT fk_city_id FOREIGN KEY (city_id) REFERENCES cities(id) ON DELETE CASCADE;");
                SQL.SqlCommand($"ALTER TABLE orders ADD city_id INT, ADD CONSTRAINT fk_city_id_orders FOREIGN KEY (city_id) REFERENCES cities(id) ON DELETE CASCADE;");
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
                throw;
            }
        }
        public static void CreateDefaultTables()
        {
            try
            {
                /*ROLES*/
                CreateAndFillRoles();

                /*STAFF*/
                SQL.SqlCommand($"CREATE TABLE staff (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255), email VARCHAR(255) UNIQUE, password VARCHAR(255), profile_picture VARCHAR(255) DEFAULT 'DefaultStaffProfilePicture.png', role_id INT, FOREIGN KEY (role_id) REFERENCES roles(id));");
                /*WAREHOUSES*/
                SQL.SqlCommand($"CREATE TABLE warehouses (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255) UNIQUE, length DOUBLE, width DOUBLE, height DOUBLE, volume DOUBLE);");
                /*EMPLOYEES*/
                SQL.SqlCommand($"CREATE TABLE employees (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255), email VARCHAR(255) UNIQUE, password VARCHAR(255), role_id INT, warehouse_id INT, profile_picture VARCHAR(255) DEFAULT 'DefaultEmployeeProfile.png', FOREIGN KEY (role_id) REFERENCES roles(id), FOREIGN KEY (warehouse_id) REFERENCES warehouses(id));");
                /*PRODUCTS*/
                SQL.SqlCommand($"CREATE TABLE products (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255), buying_price DOUBLE, selling_price DOUBLE, description TEXT, image VARCHAR(255) DEFAULT 'DefaultProductImage.png');");
                /*ORDERS*/
                SQL.SqlCommand($"CREATE TABLE orders (id INT PRIMARY KEY AUTO_INCREMENT, warehouse_id INT, product_id INT, qty INT, status VARCHAR(255) DEFAULT 'Registered', user_name VARCHAR(255), address VARCHAR(255), payment_method VARCHAR(255), order_date TIMESTAMP DEFAULT NOW(), FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE, FOREIGN KEY (warehouse_id) REFERENCES warehouses(id));");

                /*CITY*/
                City();

                /*PERMISSION*/
                CreateAndFillPermission();
                /*ROLE_PERMISSION*/
                CreateAndFillRolePermission();

                //TODO: Display missing for the boxes
                /*SECTOR*/
                SQL.SqlCommand($"CREATE TABLE sector (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255) UNIQUE, length DOUBLE, width DOUBLE, area DOUBLE, area_in_use DOUBLE DEFAULT 0, warehouse_id INT, FOREIGN KEY (warehouse_id) REFERENCES warehouses(id) ON DELETE CASCADE);");
                /*SHELF*/
                SQL.SqlCommand($"CREATE TABLE shelf (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255) UNIQUE, number_of_levels INT DEFAULT 1 NOT NULL, length DOUBLE, actual_length DOUBLE, width DOUBLE, sector_id INT, startXindex INT, startYindex INT, orientation VARCHAR(20), FOREIGN KEY (sector_id) REFERENCES sector(id) ON DELETE CASCADE);");

                Tables.addRequriedTablesToTables();
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
                throw;
            }
        }

        #endregion
        #region Features
        private static bool FeatureInUse(string FeatureName)
        {
            return bool.Parse(SQL.FindOneDataFromQuery($"SELECT in_use FROM feature WHERE name = '{FeatureName}'"));
        }
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
                    Tables_Refresh.Add(Tables.products.actual_name, Tables.products.Refresh);
                    Tables_Refresh.Add(Tables.roles.actual_name, Tables.roles.Refresh);

                    foreach (var Tables in Tables_Refresh)
                    {
                        SQL.SqlCommand($"ALTER TABLE {Tables.Key} ADD created_at TIMESTAMP DEFAULT NOW(), ADD updated_at TIMESTAMP DEFAULT NOW();");
                        Tables.Value();   
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

        public static void Fleet()
        {
            if (!FeatureInUse("Fleet"))
            {
                try
                {
                    SQL.SqlCommand(@"CREATE TABLE CARS (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        plate_number VARCHAR(255) UNIQUE NOT NULL,
                        type VARCHAR(255) NOT NULL,
                        ready BOOLEAN NOT NULL,
                        km DOUBLE,
                        last_service DATE,
                        last_exam DATE,
                        warehouse_id INT,
                        FOREIGN KEY (warehouse_id) REFERENCES warehouses(id) ON DELETE CASCADE
                    )");

                    SQL.SqlCommand($@"CREATE TABLE TRANSPORTS (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        employee_id INT,
                        car_id INT,
                        status VARCHAR(255),
                        start_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        end_date TIMESTAMP NULL,
                        warehouse_id INT,
                        FOREIGN KEY (employee_id) REFERENCES {Tables.employees.actual_name}(id) ON DELETE CASCADE,
                        FOREIGN KEY (car_id) REFERENCES CARS(id) ON DELETE CASCADE,
                        FOREIGN KEY (warehouse_id) REFERENCES warehouses(id) ON DELETE CASCADE
                    )");      

                    if (Tables.features.isFeatureInUse("Dock"))
                    {
                        SQL.SqlCommand($@"ALTER TABLE {Tables.transports.actual_name} 
                            ADD dock_id INT,
                            ADD CONSTRAINT fk_dock_id_to_transports FOREIGN KEY (dock_id) REFERENCES DOCK(id) ON DELETE CASCADE;
                        ");

                        SQL.SqlCommand($@"ALTER TABLE {Tables.orders.actual_name} DROP FOREIGN KEY fk_dock_id;
                            ALTER TABLE {Tables.orders.actual_name} DROP COLUMN dock_id;
                            ALTER TABLE {Tables.orders.actual_name} ADD transport_id INT;
                            ALTER TABLE {Tables.orders.actual_name} ADD CONSTRAINT fk_transport_id FOREIGN KEY (transport_id) REFERENCES {Tables.transports.actual_name}(id) ON DELETE CASCADE;
                        ");
                        Tables.orders.Refresh();
                        Tables.orders.database.Columns.Remove("dock_id");
                    }
                    else
                    {
                        SQL.SqlCommand($@"ALTER TABLE {Tables.orders.actual_name} 
                            ADD transport_id INT, 
                            ADD CONSTRAINT fk_transport_id FOREIGN KEY (transport_id) REFERENCES transports(id) ON DELETE CASCADE
                        ");
                        Tables.orders.Refresh();
                    }

                    Tables.addFleetTablesToTables();
                    Tables.features.getFeature("Fleet")["in_use"] = true;
                    Tables.features.updateChanges();
                    //Tables.addCityTableToTables();
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
                    SQL.SqlCommand($"CREATE TABLE LOG (id INT PRIMARY KEY AUTO_INCREMENT, email VARCHAR(255), log_message TEXT, updated_at TIMESTAMP DEFAULT NOW());");

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
                    SQL.SqlCommand($"ALTER TABLE {Tables.employees.actual_name} ADD activity BOOLEAN DEFAULT TRUE NOT NULL, ADD is_loggedin BOOLEAN DEFAULT FALSE NOT NULL;");

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
                    SQL.SqlCommand($"ALTER TABLE {Tables.warehouses.actual_name} ADD total_value DOUBLE DEFAULT 0 NOT NULL, ADD total_spending DOUBLE DEFAULT 0 NOT NULL, ADD total_income DOUBLE DEFAULT 0 NOT NULL;");

                    SQL.SqlCommand($"CREATE TABLE revenue_a_day (id INT PRIMARY KEY AUTO_INCREMENT, warehouse_id INT, date DATE, total_expenditure DOUBLE DEFAULT 0 NOT NULL, total_income DOUBLE DEFAULT 0 NOT NULL, FOREIGN KEY (warehouse_id) REFERENCES {Tables.warehouses.actual_name}(id) ON DELETE CASCADE);");
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
                    SQL.SqlCommand($"ALTER TABLE {Tables.products.actual_name} ADD weight DOUBLE DEFAULT 0 NOT NULL, ADD volume DOUBLE DEFAULT 0 NOT NULL, ADD width DOUBLE DEFAULT 0 NOT NULL, ADD heigth DOUBLE DEFAULT 0 NOT NULL, ADD length DOUBLE DEFAULT 0 NOT NULL;");
                    SQL.SqlCommand($"ALTER TABLE cars ADD storage DOUBLE DEFAULT 0, ADD carrying_capacity DOUBLE DEFAULT 0;");
                    SQL.SqlCommand($"ALTER TABLE {Tables.orders.actual_name} ADD sum_volume DOUBLE;");

                    foreach (DataRow warehosue in Tables.warehouses.database.Rows)
                    { 
                        SQL.SqlCommand($"ALTER TABLE {warehosue["name"]}" +
                            $" ADD width DOUBLE DEFAULT 0," +
                            $"ADD height DOUBLE DEFAULT 0," +
                            $"ADD length DOUBLE DEFAULT 0;");
                        foreach (DataTable warehosueTable in Tables.databases.Tables)
                        {
                            if (warehosueTable.TableName == warehosue["name"].ToString())
                            {
                                warehosueTable.Columns.Add("width");
                                warehosueTable.Columns.Add("height");
                                warehosueTable.Columns.Add("length");
                            }
                        }
                    }

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
            }
        }

        //Fuel: Addig nem hozható létre amíg a Fleet nincsen létrehozva. Létrehoz kettő új oszlopot a CARS táblába
        public static void Fuel()
        {
            if (!FeatureInUse("Fuel"))
            {
                try
                {
                    SQL.SqlCommand($"ALTER TABLE {Tables.cars.actual_name} ADD consumption DOUBLE DEFAULT 0 NOT NULL, ADD gas_tank_size DOUBLE DEFAULT 0 NOT NULL;");
                    Tables.features.getFeature("Fuel")["in_use"] = true;
                    Tables.features.updateChanges();

                    Tables.cars.Refresh();
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
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
                    SQL.SqlCommand($"CREATE TABLE DOCK (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255) UNIQUE, free BOOLEAN DEFAULT TRUE, warehouse_id INT, FOREIGN KEY (warehouse_id) REFERENCES {Tables.warehouses.actual_name}(id) ON DELETE CASCADE);");

                    if (Tables.features.isFeatureInUse("Fleet"))
                    {
                        SQL.SqlCommand($"ALTER TABLE {Tables.transports.actual_name} ADD dock_id INT, ADD CONSTRAINT fk_dock_id_to_transports FOREIGN KEY (dock_id) REFERENCES DOCK (id) ON DELETE CASCADE;");
                        Tables.transports.Refresh();
                    }
                    else
                    {
                        SQL.SqlCommand($"ALTER TABLE {Tables.orders.actual_name} ADD dock_id INT, ADD CONSTRAINT fk_dock_id FOREIGN KEY (dock_id) REFERENCES DOCK (id) ON DELETE CASCADE;");
                        Tables.orders.Refresh();
                    }

                    Tables.features.getFeature("Dock")["in_use"] = true;
                    Tables.features.updateChanges();

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
                    SQL.SqlCommand($"CREATE TABLE FORKLIFT(id INT PRIMARY KEY AUTO_INCREMENT, warehouse_id INT, type VARCHAR(255), status VARCHAR(255), operating_hours INT DEFAULT 0, FOREIGN KEY (warehouse_id) REFERENCES {Tables.warehouses.actual_name}(id) ON DELETE CASCADE);");
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
                    Tables_Refresh.Add(Tables.products.actual_name, Tables.products.Refresh);
                    Tables_Refresh.Add(Tables.roles.actual_name, Tables.roles.Refresh);

                    foreach (var Tables in Tables_Refresh)
                    {
                        
                       
                            SQL.SqlCommand($"ALTER TABLE {Tables.Key} DROP COLUMN created_at, DROP COLUMN updated_at");
                            Tables.Value();
                        
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
                    FuelOff();
                    StorageOff();

                    if (Tables.features.isFeatureInUse("Dock"))
                    {
                        //fk_dock_id_to_transports
                        SQL.SqlCommand($"ALTER TABLE `{Tables.orders.actual_name}` DROP CONSTRAINT `fk_transport_id`; ALTER TABLE `{Tables.orders.actual_name}` DROP COLUMN transport_id;");
                        SQL.SqlCommand($"ALTER TABLE `{Tables.orders.actual_name}` ADD dock_id INT, ADD CONSTRAINT fk_dock_id FOREIGN KEY (dock_id) REFERENCES DOCK (id) ON DELETE CASCADE;");
                        Tables.orders.Refresh();
                        
                        SQL.SqlCommand($"ALTER TABLE `{Tables.transports.actual_name}` DROP CONSTRAINT fk_dock_id_to_transports");

                        SQL.SqlCommand($"DROP TABLE `{Tables.transports.actual_name}`;");
                        SQL.SqlCommand($"DROP TABLE `{Tables.cars.actual_name}`;");
                    }
                    else
                    {
                        SQL.SqlCommand($"ALTER TABLE `{Tables.orders.actual_name}` DROP CONSTRAINT `fk_transport_id`; ALTER TABLE `{Tables.orders.actual_name}` DROP COLUMN transport_id;");

                        SQL.SqlCommand($"DROP TABLE `{Tables.transports.actual_name}`;");
                        SQL.SqlCommand($"DROP TABLE `{Tables.cars.actual_name}`;");
                    }

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

        public static void LogOff()
        {
            if (FeatureInUse("Log"))
            {
                try
                {
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
                    SQL.SqlCommand($"ALTER TABLE `{Tables.warehouses.actual_name}` DROP `total_value`, DROP `total_spending`,DROP `total_income`;");

                    SQL.SqlCommand($"ALTER TABLE `revenue_a_day` DROP CONSTRAINT `revenue_a_day_ibfk_1`;");
                    SQL.SqlCommand("DROP TABLE revenue_a_day");

                    Tables.features.getFeature("Revenue")["in_use"] = false;
                    Tables.features.updateChanges();

                    Tables.warehouses.database.Columns.Remove("total_value");
                    Tables.warehouses.database.Columns.Remove("total_spending");
                    Tables.warehouses.database.Columns.Remove("total_income");
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
                    SQL.SqlCommand($"ALTER TABLE {Tables.products.actual_name} DROP `weight`, DROP `volume`, DROP `width`, DROP `heigth`, DROP `length`;");
                    SQL.SqlCommand($"ALTER TABLE {Tables.cars.actual_name} DROP `storage`, DROP `carrying_capacity`;");
                    SQL.SqlCommand($"ALTER TABLE {Tables.orders.actual_name} DROP `sum_volume`;");

                    foreach (DataRow warehosue in Tables.warehouses.database.Rows)
                    {
                        SQL.SqlCommand($"ALTER TABLE {warehosue["name"]} DROP width, DROP height, DROP length");
                        foreach (DataTable warehosueTable in Tables.databases.Tables)
                        {
                            if(warehosueTable.TableName == warehosue["name"].ToString())
                            {
                                warehosueTable.Columns.Remove("width");
                                warehosueTable.Columns.Remove("height");
                                warehosueTable.Columns.Remove("length");
                            }
                        }
                    }


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
                    if (SQL.GetElementOfListArray(SQL.SqlQuery($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{Tables.orders.actual_name}';")).Contains("dock_id"))
                    {
                        SQL.SqlCommand($"ALTER TABLE `{Tables.orders.actual_name}` DROP CONSTRAINT `fk_dock_id`; ALTER TABLE `{Tables.orders.actual_name}` DROP `dock_id`;");
                    }

                    if (FeatureInUse("Fleet"))
                    {
                        SQL.SqlCommand($"ALTER TABLE `{Tables.transports.actual_name}` DROP CONSTRAINT `fk_dock_id_to_transports`; ALTER TABLE `{Tables.transports.actual_name}` DROP `dock_id`;");
                    }
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

        public static void CreateWarehouse(string WarehouseName)
        {
            try
            {
                if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Storage'"))
                {
                    SQL.SqlCommand($"CREATE TABLE {WarehouseName} (id INT PRIMARY KEY AUTO_INCREMENT NOT NULL, product_id INT, qty INT, shelf_id INT, width DOUBLE, height DOUBLE, length DOUBLE, on_shelf_level INT, is_in_box BOOLEAN, FOREIGN KEY (product_id) REFERENCES {Tables.products.actual_name}(id) ON DELETE CASCADE, FOREIGN KEY (shelf_id) REFERENCES shelf(id) ON DELETE CASCADE);");
                }
                else
                {
                    SQL.SqlCommand($"CREATE TABLE {WarehouseName} (id INT PRIMARY KEY AUTO_INCREMENT NOT NULL, product_id INT, qty INT, shelf_id INT, on_shelf_level INT, is_in_box BOOLEAN, FOREIGN KEY (product_id) REFERENCES {Tables.products.actual_name}(id) ON DELETE CASCADE, FOREIGN KEY (shelf_id) REFERENCES shelf(id) ON DELETE CASCADE);");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteError(ex);
                throw;
            }
        }

        public static void LogWrite(string email, string message)
        {
            if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Log'"))
            {
                SQL.SqlCommand($"INSERT INTO `log`(`email`, `log_message`) VALUES ('{email}', '{message}');");
            }
        }

        public static void AddToRevnue_A_Day_Expenditure(DataRow warehouse, double value)
        {
            if (Tables.features.isFeatureInUse("Revenue"))
            {
                List<string[]> datas = SQL.SqlQuery("SELECT id, warehouse_id, date, total_expenditure FROM revenue_a_day");
                bool Updated = false;
                for (int i = 0; i < datas.Count; i++)
                {
                    DateTime parsedDate = DateTime.Parse(datas[i][2]);
                    string date = parsedDate.ToString("yyyy-MM-dd");

                    if (date == DateTime.Now.ToString("yyyy-MM-dd") && warehouse["id"].ToString() == datas[i][1])
                    {
                        double NewValue = datas[i][3] != string.Empty ? double.Parse(datas[i][3]) + value : value;
                        SQL.SqlCommand($"UPDATE `revenue_a_day` SET `total_expenditure` = '{NewValue}' WHERE id = {datas[i][0]}");
                        Updated = true;
                        break;
                    }
                }

                if (! Updated)
                {
                    using (MySqlCommand command = new MySqlCommand("INSERT INTO `revenue_a_day`(`warehouse_id`, `date`, `total_expenditure`) VALUES (@warehouse_id, @date, @total_expenditure)", SQL.con))
                    {
                        command.Parameters.AddWithValue("@warehouse_id", (int)warehouse["id"]);
                        command.Parameters.AddWithValue("@date", DateTime.Now);
                        command.Parameters.AddWithValue("@total_expenditure", value);

                        SQL.con.Open();
                        command.ExecuteNonQuery();
                        SQL.con.Close();
                    }
                }
            }
        }        
        
        public static void AddToRevnue_A_Day_Income(DataRow warehouse, double value)
        {
            if (Tables.features.isFeatureInUse("Revenue"))
            {
                List<string[]> datas = SQL.SqlQuery("SELECT id, warehouse_id, date, total_income FROM revenue_a_day");
                bool Updated = false;
                for (int i = 0; i < datas.Count; i++)
                {
                    DateTime parsedDate = DateTime.Parse(datas[i][2]);
                    string date = parsedDate.ToString("yyyy-MM-dd");

                    if (date == DateTime.Now.ToString("yyyy-MM-dd") && warehouse["id"].ToString() == datas[i][1])
                    {
                        double NewValue = datas[i][3] != string.Empty ? double.Parse(datas[i][3]) + value : value;
                        SQL.SqlCommand($"UPDATE `revenue_a_day` SET `total_income` = '{NewValue}' WHERE id = {datas[i][0]}");
                        Updated = true;
                        break;
                    }
                }

                if (!Updated)
                {
                    using (MySqlCommand command = new MySqlCommand("INSERT INTO `revenue_a_day`(`warehouse_id`, `date`, `total_income`) VALUES (@warehouse_id, @date, @total_income)", SQL.con))
                    {
                        command.Parameters.AddWithValue("@warehouse_id", (int)warehouse["id"]);
                        command.Parameters.AddWithValue("@date", DateTime.Now);
                        command.Parameters.AddWithValue("@total_income", value);

                        SQL.con.Open();
                        command.ExecuteNonQuery();
                        SQL.con.Close();
                    }
                }
            }
        }
    }
}
