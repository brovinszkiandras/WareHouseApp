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
    internal class Relations
    {
        public static void makeRelation(string relationName,
            DataTable parentable,
            DataTable childtable,
            string parentcolumn,
            string childcolumn)
        {
            DataRelation relation = new DataRelation(relationName,
                parentable.Columns[parentcolumn],
                childtable.Columns[childcolumn]);
            Tables.databases.Relations.Add(relation);

        }

        public static DataRow parentRelation(string relationName, DataRow row)
        {
            return row.GetParentRow(relationName);
        }

        public static DataRow[] childRelation(string realionName, DataRow row)
        {
            return row.GetChildRows(realionName);
        }

        public static DataRow[] connectionTableRelation(DataRow element, string connectionTableName, string parenTableName, string parentForeigKey, string childForeignKey, DataTable childtable)
        {
            List<int> child_ids = new List<int>();

            SQL.con.Open();
            using (MySqlCommand command = new MySqlCommand($"SELECT {childForeignKey} FROM {connectionTableName} " +
                $"INNER JOIN {parenTableName} on {parenTableName}.id = {connectionTableName}.{parentForeigKey} " +
                $"WHERE {parenTableName}.id = {element["id"]}", SQL.con))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    child_ids.Add(reader.GetInt32(0));
                }
            }
            SQL.con.Close();
            DataRow[] element_childTAble_Relations = new DataRow[child_ids.Count];
            for (int i = 0; i < child_ids.Count; i++)
            {
                DataRow row = childtable.NewRow();
                row = childtable.Select($"id = {child_ids[i]}")[0];
                element_childTAble_Relations[i] = row;
            }

            return element_childTAble_Relations;
        }
    }
}