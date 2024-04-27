using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WH_APP_GUI
{
    internal class User
    {
        public static DataRow currentUser;
        public static DataRow tempWarehouse;
        private static warehouse WHTable;
        public static void SetCurrentUser(string email, string password) //hased psw
        {
            //if (Tables.staff.database.Select($"email = '{email}' AND password = '{password}'").Length != 0)
            //{
            //    //TODO: index out of range exeption
            //    //MessageBox.Show(password);
            //    //for (int i = 0; i < Tables.staff.database.Rows.Count; i++)
            //    //{
            //    //    MessageBox.Show(Tables.staff.database.Rows[i]["name"].ToString());
            //    //    if (Tables.staff.database.Rows[i]["password"].ToString() == password)
            //    //    {
            //    //        MessageBox.Show("Bingó");
            //    //    }
            //    //}
            //    currentUser = Tables.staff.database.Select($"'email' = '{email}' AND password = '{password}'")[0];
            //}

            Tables.staff.Refresh();
            Tables.employees.Refresh();

            DataRow[] matchingRowsInStaff = Tables.staff.database.Select($"email = '{email}' AND password = '{password}'");
            DataRow[] matchingRowsInEmployees = Tables.employees.database.Select($"email = '{email}' AND password = '{password}'");

            if (matchingRowsInStaff.Length != 0)
            {
                try
                {
                    currentUser = matchingRowsInStaff[0];
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
            else if (matchingRowsInEmployees.Length != 0)
            {
                try
                {
                    //Employee login crahsing...and not becuse of the password, note: Technicaly this else if does not run down for some reason, always just the else
                    currentUser = matchingRowsInEmployees[0];
                }
                catch (Exception ex)
                {
                    Debug.WriteError(ex);
                    throw;
                }
            }
            else
            {
                MessageBox.Show("A person with this email does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static DataRow[] GetPersmissions()
        {
            return Tables.roles.getPermission(Tables.staff.getRole(currentUser));
        }

        public static bool DoesHavePermission(string permission)
        {
            for (int i = 0; i < GetPersmissions().Length; i++)
            {
                if (GetPersmissions()[i]["name"].ToString() == permission)
                {
                    return true;
                }
            }
            return false;
        }

        public static DataRow Warehouse()
        {
            if(currentUser.Table == Tables.employees.database)
            {
                return Tables.employees.getWarehouse(currentUser);
            }
            else
            {
                return tempWarehouse;
            } 
        }

        public static warehouse warehouseTable()
        {
            if(WHTable == null)
            {
                WHTable = new warehouse(Warehouse()["name"].ToString());
            }

            if (WHTable != null)
            {
                return WHTable;
            }
            else
            {
                return null;
            }
        }
    }
}
