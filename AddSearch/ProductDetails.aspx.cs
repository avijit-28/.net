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
                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetProductDetailsByID", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter parameter = new SqlParameter("@ID", productId);
                    cmd.Parameters.Add(parameter);
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lblID.Text = rdr["ProductID"].ToString();
                            lblName.Text = rdr["Name"].ToString();
                            lblPrice.Text = rdr["Price"].ToString();
                            lblColor.Text = rdr["Color"].ToString();
                            lblSize.Text = rdr["Size"].ToString();
                            lblWeight.Text = rdr["Weight"].ToString();
                            lblQuantityInStock.Text = rdr["stock"].ToString();
                            lblDiscontinued.Text = rdr["Discontinued"].ToString();
                            lblSupplier.Text = rdr["Supplier"].ToString();
                        }
                    }

                }
            }
        }
    }
}