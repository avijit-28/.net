using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AddSearch
{
    public partial class products : System.Web.UI.Page
    {
        private readonly static string _conStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetBasicProductDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    GridView1.DataSource = cmd.ExecuteReader();
                    GridView1.DataBind();
                }
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/ProductDetails.aspx?" + "ID=" + ((LinkButton)sender).Text);
        }
    }
}