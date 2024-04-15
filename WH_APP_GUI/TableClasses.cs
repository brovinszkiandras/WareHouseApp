using MahApps.Metro.IconPacks;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;

namespace WH_APP_GUI
{
    public class table
    {
        public string name;
        public string actual_name;
        public string nice_name;
        public DataTable database = new DataTable();
        public MySqlDataAdapter adapter;

        public table()
        {
            GetNames();
            fill();
        }

        public table(string actualname)
        {
            this.actual_name = actualname;
            GetNames();
            fill();
        }

        public void GetNames()
        {
            if (actual_name == null && name == null && nice_name == null)
            {
                name = this.GetType().Name;
                actual_name = SQL.FindOneDataFromQuery($"SELECT actual_name FROM migrations WHERE name = '{this.GetType().Name}'");
                nice_name = SQL.FindOneDataFromQuery($"SELECT nice_name FROM migrations WHERE name = '{this.GetType().Name}'");
            }
        }
        public void fill()
        {
            adapter = new MySqlDataAdapter($"SELECT * FROM {actual_name}", SQL.con);

            SQL.con.Open();

            adapter.Fill(database);
            database.AcceptChanges();
            Tables.databases.Tables.Add(database);

            SQL.con.Close();
        }
        public void Refresh()
        {
            if (database.Rows.Count > 0)
            {
                int lastID = (int)database.Rows[database.Rows.Count - 1]["id"];
                adapter = new MySqlDataAdapter($"SELECT * FROM {actual_name} WHERE id > {lastID}", SQL.con);
            }
            else
            {
                adapter = new MySqlDataAdapter($"SELECT * FROM {actual_name}", SQL.con);
            }

            SQL.con.Open();

            adapter.Fill(database);

            database.AcceptChanges();
            SQL.con.Close();
        }
        public void insert(List<string> data)
        {
            DataRow row = database.NewRow();
            for (int i = 0; i < data.Count; i++)
            {
                row[i + 1] = data[i];
                Console.WriteLine(i);
            }
            database.Rows.Add(row);
            updateChanges();
        }

        public DataRow findById(int id)
        {
            DataRow[] row = this.database.Select($"id = {id}"); //return this.database.Select($"id = {id")[0];
            return row[0];
        }

        public void updateChanges()
        {
            MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);

            adapter.Update(database);


            database.AcceptChanges();


        }
    }

    class staff : table
    {
        public staff() : base() { }

        public DataRow getRole(DataRow person)
        {
            if (Tables.staff.database.Select($"email = '{person["email"]}'").Length != 0)
            {
                return Relations.parentRelation("staffRole", person);
            }
            else if (Tables.employees.database.Select($"email = '{person["email"]}'").Length != 0)
            {
                return Relations.parentRelation("employeeRole", person);
            }
            else
            {
                return null;
            }
        }
    }
    class cities : table
    {

        public cities() : base()
        {

        }

        public DataRow[] getWarehouses(DataRow city)
        {
            return Relations.childRelation("warehouseCity", city);
        }
    }
    class warehouses : table
    {

        public warehouses() : base()
        {

        }

        public DataRow[] getEmployees(DataRow warehouse)
        {
            return Relations.childRelation("employeeWarehouse", warehouse);
        }

        public DataRow[] getOrders(DataRow warehouse)
        {
            return Relations.childRelation("orderWarehouse", warehouse);
        }

        public DataRow getCity(DataRow warehouse)
        {
            return Relations.parentRelation("warehouseCity", warehouse);
        }

        public DataRow[] getSectors(DataRow warehouse)
        {
            return Relations.childRelation("sectorWarehouse", warehouse);
        }

        public DataRow[] getDocks(DataRow warehouse)
        {
            return Relations.childRelation("dockWarehouse", warehouse);
        }

        public DataRow[] getForklifts(DataRow warehouse)
        {
            return Relations.childRelation("forkliftWarehouse", warehouse);
        }
    }
    class dock : table
    {
        public dock() : base()
        {

        }

        public DataRow[] getTransports(DataRow dock)
        {
            return Relations.childRelation("transportDock", dock);
        }

        public DataRow getWarehouse(DataRow dock)
        {
            return Relations.parentRelation("dockWarehouse", dock);
        }
    }
    class orders : table
    {
        public orders() : base()
        {

        }

        public DataRow getWarehouse(DataRow order)
        {
            return Relations.parentRelation("orderWarehouse", order);
        }

        public DataRow getProduct(DataRow order)
        {
            return Relations.parentRelation("orderProduct", order);
        }

        public DataRow getTransport(DataRow order)
        {
            return Relations.parentRelation("orderTransport", order);
        }
    }
    class employees : staff
    {
        public employees() : base() { }



        public DataRow getWarehouse(DataRow employee)
        {
            return Relations.parentRelation("employeeWarehouse", employee);
        }

        public DataRow[] getTransports(DataRow employee)
        {
            return Relations.childRelation("transportEmployee", employee);
        }
    }
    class products : table
    {


        public products() : base()
        {

        }

        public DataRow[] getOrders(DataRow product)
        {
            return Relations.childRelation("orderProduct", product);
        }
    }
    class roles : table
    {

        public roles() : base()
        {

        }

        public DataRow[] getStaff(DataRow role)
        {
            return Relations.childRelation("staffRole", role);
        }

        public DataRow[] getEmployees(DataRow role)
        {
            return Relations.childRelation("employeeRole", role);
        }

        public DataRow[] getPermission(DataRow role)
        {
            return Relations.connectionTableRelation(role, "role_permission", "roles", "role_id", "permission_id", Tables.permissions.database);
        }
    }
    class permission : table
    {

        public permission() : base()
        {

        }

        public DataRow[] getRoles(DataRow permission)
        {
            return Relations.connectionTableRelation(permission, "role_permission", "permission", "permission_id", "role_id", Tables.roles.database);
        }
    }
    class role_permission : table
    {
        public role_permission() : base()
        {
            actual_name = "role_permission";
        }
    }
    class cars : table
    {

        public cars() : base()
        {
            database.Columns["id"].AutoIncrement = true;
            database.Columns["plate_number"].AllowDBNull = false;
            database.Columns["plate_number"].Unique = true;
            database.Columns["type"].AllowDBNull = false;
            database.Columns["km"].AllowDBNull = true;
            database.Columns["last_service"].AllowDBNull = true;
            database.Columns["last_exam"].AllowDBNull = true;
        }

        public DataRow[] getTransports(DataRow car)
        {
            return Relations.childRelation("transportCar", car);
        }
    }
    class transports : table
    {
        public transports() : base()
        {

        }

        public DataRow getEmployee(DataRow transport)
        {
            return Relations.parentRelation("transportEmployee", transport);
        }

        public DataRow getCar(DataRow transport)
        {
            return Relations.parentRelation("transportCar", transport);
        }

        public DataRow getDock(DataRow transport)
        {
            return Relations.parentRelation("transportDock", transport);
        }

        public DataRow[] getOrders(DataRow transport)
        {
            return Relations.childRelation("orderTransport", transport);
        }
    }
    class feature : table
    {
        public feature(string actualname) : base(actualname)
        {

        }

        public DataRow getFeature(string name)
        {
            return database.Select($"name = '{name}'")[0];
        }

        public bool isFeatureInUse(string name)
        {
            return (bool)getFeature(name)["in_use"];
        }
    }

    class warehouse : table
    {
        public warehouse(string actualname) : base(actualname)
        {

        }

        public DataRow getProduct(DataRow item)
        {
            Relations.makeRelation(actual_name + "Product", Tables.products.database, this.database, "id", "product_id");

            return Relations.parentRelation(actual_name + "Product", item);
        }

        public DataRow getShelf(DataRow item)
        {
            Relations.makeRelation(actual_name + "shelf", Tables.shelf.database, this.database, "id", "shelf_id");
            return Relations.parentRelation(actual_name + "shelf", item);
        }
    }

    class Sector : table
    {
        public Sector(string actualname) : base(actualname)
        {

        }

        public DataRow[] getShelfs(DataRow sector)
        {
            return Relations.childRelation("shelfSector", sector);
        }

        public DataRow getWarehouse(DataRow sector)
        {
            return Relations.parentRelation("sectorWarehouse", sector);
        }
    }

    class shelf : table
    {
        public shelf(string actualname) : base(actualname) { }

        public DataRow getSector(DataRow shelf)
        {
            return Relations.parentRelation("shelfSector", shelf);
        }

        public DataRow[] getWarehouseProducts(warehouse warehouse, DataRow shelf)
        {
            return Relations.childRelation(warehouse.actual_name + "shelf", shelf);
        }
    }

    class forklift : table
    {
        public forklift() : base()
        {

        }

        public DataRow getWarehouse(DataRow forklift)
        {
            return Relations.parentRelation("forkliftWarehouse", forklift);
        }
    }
}