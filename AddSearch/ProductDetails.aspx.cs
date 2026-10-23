using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AddSearch
{
    public partial class ProductDetails : System.Web.UI.Page
    {
        private readonly static string _conStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string productId = Request.QueryString["ID"];
                if (productId == null)
                {
                    Response.Redirect("Products.aspx");
                }
            }
        }
    }
}