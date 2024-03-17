using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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

        public static void addRequriedTablesToTables()
        {
           staff = new staff();
           warehouses = new warehouses();
           employees = new employees();
           orders = new orders();
           products = new products();
           roles = new roles();
           permissions = new permission();

            Relations.makeRelation("staffRole", roles.database, staff.database, "id", "role_id");
            Relations.makeRelation("employeeRole", roles.database, employees.database, "id", "role_id");
            Relations.makeRelation("employeeWarehouse", warehouses.database, employees.database, "id", "warehouse_id");
            Relations.makeRelation("orderWarehouse", warehouses.database, orders.database, "id", "warehouse_id");
            Relations.makeRelation("orderProduct", products.database, orders.database, "id", "product_id");
           
        }

        public static void addFleetTablesToTables()
        {
            transports = new transports();
            cars = new cars();

            Relations.makeRelation("transportEmployee", employees.database,transports.database ,"id", "employee_id");
            Relations.makeRelation("transportCar", cars.database, transports.database, "id", "car_id");
            Relations.makeRelation("transportDock", docks.database, transports.database, "id", "dock_id");
            Relations.makeRelation("orderTransport", transports.database, orders.database, "id", "transport_id");
        }

        public static void addCityTableToTables()
        {
            Relations.makeRelation("warehouseCity", cities.database, warehouses.database, "id", "city_id");
        }

        public static void addDockTableToTables()
        {
            docks = new dock();
        }
    }
}
