using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

namespace MyProject
{
    public partial class Customers : System.Web.UI.Page
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["FleetDbConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                //DataTable dt = new DataTable();
                using (SqlConnection connection = new SqlConnection(_conStr))
                {
                    SqlCommand command = new SqlCommand();
                    command.CommandText = "SELECT * FROM customers";
                    command.Connection = connection;
                    connection.Open();
                    SqlDataReader resultSet = command.ExecuteReader();
                    GridView1.DataSource = resultSet;
                    GridView1.DataBind();
                }
            }
        }
    }
}