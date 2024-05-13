using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WH_APP_GUI
{
    class Tables
    {
        #region defaultTables
        public static DataSet databases = new DataSet();
        public static feature features;
        public static dock docks;
        public static orders orders;
        public static employees employees;
        public static warehouses warehouses;
        public static staff staff;
        public static cities cities;
        public static products products;
        public static role_permission role_permission;
        public static permission permissions;
        public static roles roles;
        public static transports transports;
        public static cars cars;
        public static Sector sector;
        public static shelf shelf;
        public static forklift forklifts;
        public static List<warehouse> warehouseTables = new List<warehouse>();
        #endregion

        #region ini
        static public void Ini()
        {
            databases.Relations.Clear();
            foreach (DataTable table in databases.Tables)
            {
                table.Constraints.Clear();
            }
            databases.Tables.Clear();

            addRequriedTablesToTables();
            if ((bool)features.database.Select("name = 'Dock'")[0]["in_use"])
            {
                addDockTableToTables();
            }
            if ((bool)features.database.Select("name = 'Fleet'")[0]["in_use"])
            {
                addFleetTablesToTables();
            }
            if ((bool)features.database.Select("name = 'Forklift'")[0]["in_use"])
            {
                addForkliftTableToTables();
            }
        }

        public static void addRequriedTablesToTables()
        {
            features = new feature("feature");
            staff = new staff("staff");
            warehouses = new warehouses("warehouses");
            employees = new employees("employees");
            orders = new orders("orders");
            products = new products("products");
            roles = new roles("roles");
            permissions = new permission("permission");
            sector = new Sector("sector");
            shelf = new shelf("shelf");
            cities = new cities("cities");


            Relations.makeRelation("staffRole", roles.database, staff.database, "id", "role_id");
            Relations.makeRelation("employeeRole", roles.database, employees.database, "id", "role_id");
            Relations.makeRelation("employeeWarehouse", warehouses.database, employees.database, "id", "warehouse_id");
            Relations.makeRelation("orderProduct", products.database, orders.database, "id", "product_id");
            Relations.makeRelation("shelfSector", sector.database, shelf.database, "id", "sector_id");
            Relations.makeRelation("sectorWarehouse", warehouses.database, sector.database, "id", "warehouse_id");
            Relations.makeRelation("orderWarehouse", warehouses.database, orders.database, "id", "warehouse_id");
            Relations.makeRelation("orderCity", cities.database, orders.database, "id", "city_id");
            Relations.makeRelation("warehouseCity", cities.database, warehouses.database, "id", "city_id");

            loadInWarehouseTables();
        }
        #endregion

        #region fleet
        public static void addFleetTablesToTables()
        {
            transports = new transports("transports");
            cars = new cars("cars");
            Relations.makeRelation("transportEmployee", employees.database, transports.database, "id", "employee_id");
            Relations.makeRelation("transportCar", cars.database, transports.database, "id", "car_id");
            Relations.makeRelation("transportDock", docks.database, transports.database, "id", "dock_id");
            Relations.makeRelation("orderTransport", transports.database, orders.database, "id", "transport_id");
            Relations.makeRelation("transportWarehouse", warehouses.database, transports.database, "id", "warehouse_id");
            Relations.makeRelation("carWarehosue", warehouses.database, cars.database, "id", "warehouse_id");
            //Kikapcsolom az orders dock relationt és megcsinálom a transport dock relationt
            if (Tables.features.isFeatureInUse("Dock") == true)
            {
                if (docks != null)
                {
                    if (databases.Relations["orderDock"] != null)
                    {
                        databases.Relations.Remove("orderDock");
                    }
                    Relations.makeRelation("transportDock", docks.database, transports.database, "id", "dock_id");
                }
            }
        }

        public static void DisableFleetFeature()
        {
            DeleteRelationsWithDataTable(transports.database);
            DeleteRelationsWithDataTable(cars.database);
            transports = null;
            cars = null;
            
            //Hozzáadom az orderDock relationt és kikapcsolom a transport dock relationt 
            if (Tables.features.isFeatureInUse("Dock") == true)
            {
                
                Relations.makeRelation("orderDock", docks.database, orders.database, "id", "dock_id");
            }



            databases.Tables.Remove("transports");
            databases.Tables.Remove("cars");
        }
        #endregion

        #region dock
        public static void addDockTableToTables()
        {
            docks = new dock("dock");
            //transportDock relation létrehozása
            if (bool.Parse(Tables.features.database.Select("name = 'Fleet'")[0]["in_use"].ToString()))
            {
                if (transports != null && databases.Tables["transports"] != null)
                {
                    Relations.makeRelation("transportDock", docks.database, transports.database, "id", "dock_id");
                }
            }
            else
            {
                //Létrehozom az order dock relationt
                Relations.makeRelation("orderDock", docks.database, orders.database, "id", "dock_id");
            }
            Relations.makeRelation("dockWarehouse", warehouses.database, docks.database, "id", "warehouse_id");
        }
        public static void disableDockFeature()
        {
            DeleteRelationsWithDataTable(docks.database);
            docks = null;


            if (Tables.features.isFeatureInUse("Fleet"))
            {
                //Kikapcsolom a transport dock relationt
                
                Tables.transports.database.Columns.Remove("dock_id");
            }
            else
            {
                //Kikapcsolom az orderDock relationt
               
                Tables.orders.database.Columns.Remove("dock_id");
            }

           

            databases.Tables.Remove("dock");
        }
        #endregion

        #region forklift
        public static void addForkliftTableToTables()
        {
            forklifts = new forklift("forklift");

            Relations.makeRelation("forkliftWarehouse", warehouses.database, forklifts.database, "id", "warehouse_id");
        }



        public static void disableForkliftFeauture()
        {
            DeleteRelationsWithDataTable(forklifts.database);
            forklifts = null;
            databases.Tables.Remove("forklift");
        }
        #endregion

        public static warehouse getWarehosue(string name)
        {
          
            warehouse selectedWarehosue = null;
            foreach (warehouse warehouseTable in warehouseTables)
            {
               
                if (warehouseTable.database.TableName == name)
                {
                    selectedWarehosue = warehouseTable;
                }
            }
            return selectedWarehosue;
        }

        private static void loadInWarehouseTables()
        {
            foreach(DataRow warehouse in warehouses.database.Rows)
            {
                warehouseTables.Add(new warehouse(warehouse["name"].ToString()));
            }
        }

        private static void DeleteRelationsWithDataTable(DataTable targetDataTable)
        {
            if(targetDataTable.ChildRelations != null)
            {
                targetDataTable.ChildRelations.Clear();
            }
            if (targetDataTable.ParentRelations != null)
            {
                targetDataTable.ChildRelations.Clear();
            }
            if(targetDataTable.Constraints != null)
            {
                targetDataTable.Constraints.Clear();
            }
           
            foreach (DataTable table in databases.Tables)
            {
               
                    // Check if the table has any constraints
                    if (table.Constraints.Count > 0)
                    {
                    
                    // Iterate through each Constraint
                    for (int i = 0; i < table.Constraints.Count; i++)
                        {
                            Constraint constraint = table.Constraints[i];
                            // Check if the constraint is a ForeignKeyConstraint
                           
                            if (constraint is ForeignKeyConstraint foreignKeyConstraint)
                            {
                           
                                // Check if the ForeignKeyConstraint is related to the target DataTable
                                if (foreignKeyConstraint.RelatedTable == targetDataTable)
                                {
                                    table.Constraints.Remove(constraint); // Remove the constraint
                                   
                                }
                            }
                        }
                    }
            }

            if (databases.Relations.Count > 0)
            {
                // Iterate through each DataRelation
                for (int i = 0; i < databases.Relations.Count; i++)
                {
                    DataRelation relation = databases.Relations[i];
                    // Check if the relation is related to the target DataTable
                    if (relation.ParentTable == targetDataTable || relation.ChildTable == targetDataTable)
                    {
                        databases.Relations.Remove(relation); // Remove the relation
                        
                    }
                }
            }
        }
    }
}