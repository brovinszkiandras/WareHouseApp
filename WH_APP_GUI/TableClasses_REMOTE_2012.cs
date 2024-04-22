using MahApps.Metro.IconPacks;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Documents;

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
            setupAutoIncrement();
           
        }

        public table(string actualname)
        {
            this.actual_name = actualname;
            GetNames();
            fill();
            setupAutoIncrement();
           
        }

        public void setupAutoIncrement()
        {
            database.Columns["id"].AutoIncrement = true;
            database.Columns["id"].AutoIncrementStep = 1;

           

            if (database.Rows.Count > 0)
            {
                database.Columns["id"].AutoIncrementSeed = (int)database.Rows[database.Rows.Count - 1]["id"] + 1;

            }
            else
            {
                SQL.SqlCommand($"ALTER TABLE {actual_name} AUTO_INCREMENT = 1;");
                database.Columns["id"].AutoIncrementSeed = 1;
            }
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

            adapter.AcceptChangesDuringUpdate = true;

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
            using (MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(adapter))
            {

                //Console.WriteLine("Generated SQL commands:");
                //MessageBox.Show("Insert Command: " + commandBuilder.GetInsertCommand().CommandText);
                //MessageBox.Show("Update Command: " + commandBuilder.GetUpdateCommand().CommandText);
                //MessageBox.Show("Delete Command: " + commandBuilder.GetDeleteCommand().CommandText);

                //foreach (MySqlParameter parameter in commandBuilder.GetUpdateCommand().Parameters)
                //{
                //    parameter.SourceVersion = DataRowVersion.Current;
                //    parameter.Precision = 10;
                //    MessageBox.Show($"Parameter: {parameter.ParameterName}, MysqlDatatype: {parameter.MySqlDbType} Type: {parameter.DbType},Column: {parameter.SourceColumn}, Value: {parameter.Value}");
                //}

                adapter.Update(database);

                //if(database.GetChanges() != null)
                //{
                //    MessageBox.Show("there are still changes");
                //}

                // Display the SQL commands

            }

            //RefreshEverything();

           
        }

        public void RefreshEverything()
        {
            database = new DataTable();

            adapter.Fill(database);
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
            
            database.Columns["name"].Unique = true;
            database.Columns["name"].AllowDBNull = false;
            database.Columns["length"].AllowDBNull=false;
            database.Columns["width"].AllowDBNull=false;
            database.Columns["height"].AllowDBNull=false;
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
            database.Columns["name"].Unique = true;
            database.Columns["name"].AllowDBNull = false;
            database.Columns["warehouse_id"].AllowDBNull=false;
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
        public employees() : base() 
        {
            database.Columns["name"].AllowDBNull = false;
            database.Columns["email"].Unique = true;
            database.Columns["email"].AllowDBNull = false;
            database.Columns["password"].AllowDBNull = false;
            database.Columns["role_id"].AllowDBNull = false;
            database.Columns["warehouse_id"].AllowDBNull = true;        
        }
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
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;

            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            int second = DateTime.Now.Second;

            database.Columns["name"].AllowDBNull = false;
            database.Columns["buying_price"].AllowDBNull = false;
            database.Columns["selling_price"].AllowDBNull = false;
            database.Columns["description"].AllowDBNull = true;

            // Update the value in the DataTable with correctly formatted DateTime object

            MySqlDateTime dateTime = new MySqlDateTime(DateTime.Now);
          

            //database.Columns["created_at"].DefaultValue = new MySqlDateTime(DateTime.Now);


            //database.Columns["updated_at"].DefaultValue = new MySqlDateTime(DateTime.Now);
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
            database.Columns["role"].Unique = true;
            database.Columns["role"].AllowDBNull = false;
            database.Columns["description"].AllowDBNull = false;
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
            database.Columns["employee_id"].AllowDBNull = false;
            database.Columns["car_id"].AllowDBNull=false;
            database.Columns["status"].AllowDBNull = false;
            database.Columns["end_date"].AllowDBNull = true;       
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
            database.Columns["product_id"].AllowDBNull = false;
            database.Columns["qty"].AllowDBNull = false;
            database.Columns["shelf_id"].AllowDBNull=false;
            database.Columns["on_shelf_level"].AllowDBNull = false;

            if (Tables.features.isFeatureInUse("Storage"))
            {
                database.Columns["width"].AllowDBNull = false;
                database.Columns["height"].AllowDBNull = false;
                database.Columns["length"].AllowDBNull = false;
            }
        }

        public DataRow getProduct(DataRow item)
        {
            return Tables.products.database.Select($"id = {item["product_id"]}")[0];
        }

        public DataRow getShelf(DataRow item)
        {
            return Tables.shelf.database.Select($"id = {item["shelf_id"]}")[0];
        }
    }

    class Sector : table
    {
        public Sector(string actualname) : base(actualname)
        {
            database.Columns["name"].Unique = true;
            database.Columns["name"].AllowDBNull = false;
            database.Columns["length"].AllowDBNull=false;
            database.Columns["width"].AllowDBNull = false;
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
        public shelf(string actualname) : base(actualname) 
        {
            database.Columns["name"].Unique = true;
            database.Columns["name"].AllowDBNull = false;
            database.Columns["width"].AllowDBNull = false;
        }

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
            database.Columns["type"].AllowDBNull = false;
            database.Columns["status"].AllowDBNull = false;
        }

        public DataRow getWarehouse(DataRow forklift)
        {
            return Relations.parentRelation("forkliftWarehouse", forklift);
        }
    }
}