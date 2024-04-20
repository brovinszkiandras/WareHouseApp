using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WH_APP_GUI
{
    class Tables
    {
        public static DataSet databases = new DataSet();
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
        public static feature features;
        public static Sector sector;
        public static shelf shelf;
        public static forklift forklifts;

        static public void Ini()
        {
            addRequriedTablesToTables();
            if ((bool)features.database.Select("name = 'City'")[0]["in_use"])
            {
                addCityTableToTables();
            }
            if ((bool)features.database.Select("name = 'Fleet'")[0]["in_use"])
            {
                addFleetTablesToTables();
            }
            if ((bool)features.database.Select("name = 'Dock'")[0]["in_use"])
            {
                addDockTableToTables();
            }
            if ((bool)features.database.Select("name = 'Forklift'")[0]["in_use"])
            {
                addForkliftTableToTables();
            }
        }

        public static void addRequriedTablesToTables()
        {
            staff = new staff();
            warehouses = new warehouses();
            employees = new employees();
            orders = new orders();
            products = new products();
            roles = new roles();
            permissions = new permission();
            features = new feature("feature");
            sector = new Sector("sector");
            shelf = new shelf("shelf");

            Relations.makeRelation("staffRole", roles.database, staff.database, "id", "role_id");
            Relations.makeRelation("employeeRole", roles.database, employees.database, "id", "role_id");
            Relations.makeRelation("employeeWarehouse", warehouses.database, employees.database, "id", "warehouse_id");
            Relations.makeRelation("orderProduct", products.database, orders.database, "id", "product_id");
            Relations.makeRelation("shelfSector", sector.database, shelf.database, "id", "sector_id");

        }

        public static void addFleetTablesToTables()
        {
            transports = new transports();
            cars = new cars();
            Relations.makeRelation("transportEmployee", employees.database, transports.database, "id", "employee_id");
            Relations.makeRelation("transportCar", cars.database, transports.database, "id", "car_id");
            Relations.makeRelation("orderTransport", transports.database, orders.database, "id", "transport_id");

        }

        public static void addCityTableToTables()
        {
            cities = new cities();

            Tables.orders.Refresh();
            Tables.warehouses.Refresh();
            Relations.makeRelation("warehouseCity", cities.database, warehouses.database, "id", "city_id");
        }

        public static void addDockTableToTables()
        {
            docks = new dock();
            if (bool.Parse(Tables.features.database.Select("name = 'Fleet'")[0]["in_use"].ToString()) && bool.Parse(Tables.features.database.Select("name = 'Dock'")[0]["in_use"].ToString()))
            {
                Relations.makeRelation("transportDock", docks.database, transports.database, "id", "dock_id");

            }
            Relations.makeRelation("dockWarehouse", warehouses.database, docks.database, "id", "warehouse_id");
        }

        public static void addForkliftTableToTables()
        {
            forklifts = new forklift();

            Relations.makeRelation("forkliftWarehouse", warehouses.database, forklifts.database, "id", "warehouse_id");
        }

        public static void DisableFleetFeature()
        {
            transports = null;
            cars = null;

            databases.Relations.Remove("transportEmployee");
            databases.Relations.Remove("transportCar");
            databases.Relations.Remove("orderTransport");

            orders.database.Constraints.Remove("orderTransport");
            orders.database.Columns.Remove("transport_id");

        }

        public static void disableDockFeature()
        {
            docks = null;
            
            if (Tables.features.isFeatureInUse("Fleet") && Tables.features.isFeatureInUse("Dock"))
            {
               
                databases.Relations.Remove("transportDock");
                transports.database.Constraints.Remove("transportDock");
                Tables.transports.database.Columns.Remove("dock_id");
            }
            databases.Relations.Remove("dockWarehouse");
        }

        public static void disableCityFeature()
        {
            cities = null;


            databases.Relations.Remove("warehouseCity");
            warehouses.database.Constraints.Remove("warehouseCity");
            Tables.warehouses.database.Columns.Remove("city_id");
            Tables.orders.database.Columns.Remove("city_id");
        }
        public static void disableForkliftFeauture()
        {
            forklifts = null;

            databases.Relations.Remove("forkliftWarehouse");
        }

    }
}