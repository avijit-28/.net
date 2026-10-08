// DbHelper.cs
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace GenericSpDemo.Data
{
    public static class DbHelper
    {
        private static readonly string ConnStr = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

        /// <summary>
        /// Generic method: runs a stored procedure and returns a DataSet.
        /// T is any class whose properties map to the SP parameters.
        /// </summary>
        public static DataSet GetDataSet<T>(string procedureName, T parameters = default(T))
        {
            var ds = new DataSet();

            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(procedureName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    foreach (PropertyInfo prop in typeof(T).GetProperties())
                    {
                        object value = prop.GetValue(parameters, null) ?? DBNull.Value;
                        cmd.Parameters.AddWithValue("@" + prop.Name, value);
                    }
                }

                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(ds);   // opens/closes the connection itself
                }
            }
            return ds;
        }

        /// <summary>
        /// Generic method: maps a DataTable to List&lt;T&gt; by matching column names to property names.
        /// </summary>
        public static List<T> ToList<T>(DataTable table) where T : new()
        {
            var list = new List<T>();
            PropertyInfo[] props = typeof(T).GetProperties();

            foreach (DataRow row in table.Rows)
            {
                var item = new T();
                foreach (PropertyInfo prop in props)
                {
                    if (table.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                    {
                        Type targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        prop.SetValue(item, Convert.ChangeType(row[prop.Name], targetType), null);
                    }
                }
                list.Add(item);
            }
            return list;
        }
    }
}


    