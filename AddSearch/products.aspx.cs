using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AddSearch
{
    public partial class products : System.Web.UI.Page
    {
        private static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DBConn"]?.ConnectionString
                    ?? ConfigurationManager.ConnectionStrings["AssignmetConnectionString"]?.ConnectionString;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                DataTable dt = new DataTable();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetBasicProductDetails", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                catch
                {
                    using (SqlCommand cmdFallback = new SqlCommand("SELECT ID, Name, Price, Stock FROM Products ORDER BY ID ASC", con))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmdFallback))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        protected string GetStockPill(object stockObj)
        {
            if (stockObj == null || stockObj == DBNull.Value)
            {
                return "<span class=\"stock-pill stock-out\">Out of Stock (0)</span>";
            }

            int.TryParse(stockObj.ToString(), out int stock);
            if (stock > 30)
            {
                return string.Format("<span class=\"stock-pill stock-in\">● In Stock ({0} units)</span>", stock);
            }
            else if (stock > 0)
            {
                return string.Format("<span class=\"stock-pill stock-low\">● Low Stock ({0} units)</span>", stock);
            }
            else
            {
                return "<span class=\"stock-pill stock-out\">● Out of Stock (0)</span>";
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            if (btn != null)
            {
                string id = btn.CommandArgument;
                if (string.IsNullOrWhiteSpace(id))
                {
                    id = btn.Text.Replace("#", "").Trim();
                }
                Response.Redirect("ProductDetails.aspx?ID=" + id);
            }
        }
    }
}