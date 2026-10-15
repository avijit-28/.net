using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyProject
{
    public partial class CustomerDetails : System.Web.UI.Page
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["FleetDbConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                var customer_id = this.Page.RouteData.Values["CustomerId"];
                lblCustomerId.Text = customer_id.ToString();
                using (SqlConnection connection = new SqlConnection(_conStr))
                {
                    SqlCommand command = new SqlCommand();
                    command.CommandText = $"SELECT * FROM [customers] where customer_id={ customer_id} ";
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