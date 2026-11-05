using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WebApiPractice2.Models.DAL
{
    public class DBFunction
    {
        private string conStr = string.Empty;
        private DataTable _dt;
        private DataSet _ds;

        public DBFunction()
        {
            var conSetting = ConfigurationManager.ConnectionStrings["DBCon"] 
                          ?? ConfigurationManager.ConnectionStrings["DBConn"];
            conStr = conSetting != null ? conSetting.ToString() : string.Empty;
            _dt = new DataTable();
        }

        public object executeScalarWithProc(string spName, params SqlParameter[] param)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand(spName, con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (param != null)
                    {
                        foreach (SqlParameter p in param)
                        {
                            command.Parameters.Add(p);
                        }
                    }
                    con.Open();
                    return command.ExecuteScalar();
                }
            }
        }

        public DataTable DTWithProc(string spName, params SqlParameter[] param)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(spName, con))
                {
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    if (param != null)
                    {
                        foreach (SqlParameter p in param)
                        {
                            da.SelectCommand.Parameters.Add(p);
                        }
                    }
                    _dt = new DataTable();
                    con.Open();
                    da.Fill(_dt);
                }
                return _dt;
            }
        }

        public DataSet DSWithProc(string spName, params SqlParameter[] param)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(spName, con))
                {
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    if (param != null)
                    {
                        foreach (SqlParameter p in param)
                        {
                            da.SelectCommand.Parameters.Add(p);
                        }
                    }
                    _ds = new DataSet();
                    con.Open();
                    da.Fill(_ds);
                }
                return _ds;
            }
        }
    }
}
