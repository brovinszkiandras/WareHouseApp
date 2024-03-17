using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void GetNames()
        {
            if (actual_name == null && name == null && nice_name == null)
            {
                name = this.GetType().Name;
                actual_name = SQL.FindOneDataFromQuery($"SELECT actual_name FROM migrations WHERE name = '{this.GetType().Name}'");
                nice_name = SQL.FindOneDataFromQuery($"SELECT nice_name FROM migrations WHERE name = '{this.GetType().Name}'");

                MessageBox.Show(name);
                MessageBox.Show(actual_name);
                MessageBox.Show(nice_name);

                //MessageBox.Show(this.GetType().Name);
                //MessageBox.Show($"SELECT actual_name FROM migrations WHERE name = '{this.GetType().Name}'");
                //MessageBox.Show($"SELECT nice_name FROM migrations WHERE name = '{this.GetType().Name}'");
            }
        }
        public void fill()
        {
            adapter = new MySqlDataAdapter($"SELECT * FROM {actual_name}", SQL.con);
            SQL.con.Open();

            adapter.Fill(database);
            Tables.databases.Tables.Add(database);

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
        }
    }

    class staff : table
    {
        public staff() : base() { }

        public DataRow getRole(DataRow person)
        {
            return Relations.parentRelation("staffRole", person);
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
            return Relations.parentRelation("orderTransport", order) ;
        }
    }
    class employees : table
    {
        public employees() : base() { }

        public DataRow getRole(DataRow employee)
        {
            return Relations.parentRelation("employeeRole", employee);
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
            return Relations.connectionTableRelation(role, "role_permission", "role", "role_id", "permission_id", Tables.permissions.database);
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
}
