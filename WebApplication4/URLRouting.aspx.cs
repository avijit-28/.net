using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace WebApplication4
{
    public partial class URLRouting : System.Web.UI.Page
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            GridView1.DataSource = GetData();
            GridView1.DataBind();

        }
        private DataTable GetData()
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            {
                SqlDataAdapter da = new SqlDataAdapter("select Id,Title,Description,author from Articletable", conn);
                DataSet ds = new DataSet();
                da.Fill(ds, "Articletable");
                return ds.Tables["Articletable"];
            }
        }
        public string GetTitle(object title)
        {
            return title.ToString().Replace(" ", "-");
        }
    }
}