using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class products : System.Web.UI.Page
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConn"]?.ConnectionString
                ?? ConfigurationManager.ConnectionStrings["AssignmetConnectionString"]?.ConnectionString;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindProductsGrid();
            }
        }

        private void BindProductsGrid()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    DataTable dt = new DataTable();

                    // 1. Try stored procedure sp_GetBasicProductDetails
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
                    catch (Exception)
                    {
                        // 2. Fallback to direct query if stored procedure is missing
                        using (SqlCommand cmdFallback = new SqlCommand("SELECT ID, Name, Price, Stock FROM Products ORDER BY ID ASC", con))
                        {
                            using (SqlDataAdapter da = new SqlDataAdapter(cmdFallback))
                            {
                                dt.Clear();
                                da.Fill(dt);
                            }
                        }
                    }

                    GridView1.DataSource = dt;
                    GridView1.DataBind();

                    ComputeMetrics(dt);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading products from SQL Server: " + ex.Message;
                lblStatus.CssClass = "status-msg status-msg-error";
                lblStatus.Visible = true;
            }
        }

        private void ComputeMetrics(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                lblTotalProducts.Text = "0";
                lblInStock.Text = "0";
                lblLowStock.Text = "0";
                lblAvgPrice.Text = "$ 0.00";
                return;
            }

            int total = dt.Rows.Count;
            int inStockCount = 0;
            int lowStockCount = 0;
            decimal priceSum = 0;

            foreach (DataRow row in dt.Rows)
            {
                int stock = 0;
                if (row["Stock"] != DBNull.Value)
                {
                    int.TryParse(row["Stock"].ToString(), out stock);
                }

                if (stock > 30)
                {
                    inStockCount++;
                }
                else
                {
                    lowStockCount++;
                }

                if (row["Price"] != DBNull.Value)
                {
                    decimal.TryParse(row["Price"].ToString(), out decimal price);
                    priceSum += price;
                }
            }

            lblTotalProducts.Text = total.ToString("N0");
            lblInStock.Text = inStockCount.ToString("N0");
            lblLowStock.Text = lowStockCount.ToString("N0");
            lblAvgPrice.Text = string.Format("$ {0:N2}", total > 0 ? (priceSum / total) : 0);
        }

        protected string GetStockBadgeHtml(object stockObj)
        {
            if (stockObj == null || stockObj == DBNull.Value)
            {
                return "<span class=\"stock-badge stock-out\"><span class=\"stock-dot\"></span> Out of Stock (0)</span>";
            }

            int.TryParse(stockObj.ToString(), out int stock);

            if (stock > 30)
            {
                return string.Format("<span class=\"stock-badge stock-in\"><span class=\"stock-dot\"></span> In Stock ({0} units)</span>", stock);
            }
            else if (stock > 0)
            {
                return string.Format("<span class=\"stock-badge stock-low\"><span class=\"stock-dot\"></span> Low Stock ({0} units)</span>", stock);
            }
            else
            {
                return "<span class=\"stock-badge stock-out\"><span class=\"stock-dot\"></span> Out of Stock (0)</span>";
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            if (btn != null)
            {
                string productId = btn.CommandArgument;
                if (string.IsNullOrWhiteSpace(productId))
                {
                    productId = btn.Text.Replace("#", "").Trim();
                }

                Response.Redirect("~/ProductDetails.aspx?ID=" + productId);
            }
        }
    }
}
