using System;
using System.Configuration;
using System.Data.SqlClient;

namespace WebApplication4
{
    public partial class AllGridProp : System.Web.UI.Page
    {
        public string DbServerStatus = "Connecting...";
        public int AllEmployeeCount = 0;
        public int EmployeeCount = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["DBConn"]?.ConnectionString
                              ?? ConfigurationManager.ConnectionStrings["AssignmetConnectionString"]?.ConnectionString;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    using (SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM AllEmployee", con))
                    {
                        AllEmployeeCount = Convert.ToInt32(cmd1.ExecuteScalar());
                    }
                    using (SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM Employee", con))
                    {
                        EmployeeCount = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                    DbServerStatus = "Connected to SQL Server: [Assignmet]";
                }
            }
            catch (Exception ex)
            {
                DbServerStatus = "SQL Server Offline: " + ex.Message;
            }
        }
    }
}