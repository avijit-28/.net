using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class ProductDetails : System.Web.UI.Page
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
                PopulateProductSwitcher();
                LoadProductDetails();
            }
        }

        private void PopulateProductSwitcher()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    string sql = "SELECT ID, Name FROM Products ORDER BY ID ASC";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            ddlSwitchProduct.Items.Clear();
                            while (rdr.Read())
                            {
                                string text = string.Format("#{0} - {1}", rdr["ID"], rdr["Name"]);
                                string val = rdr["ID"].ToString();
                                ddlSwitchProduct.Items.Add(new ListItem(text, val));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceWarning("Failed to populate product switcher: " + ex.Message);
            }
        }

        private void LoadProductDetails()
        {
            string productIdStr = Request.QueryString["ID"];
            bool isExplicitQuery = !string.IsNullOrWhiteSpace(productIdStr) && int.TryParse(productIdStr, out _);

            int productId = 1;
            if (isExplicitQuery)
            {
                int.TryParse(productIdStr, out productId);
                pnlDefaultNotice.Visible = false;
            }
            else
            {
                // If ID is missing or invalid, default to the first product in DB instead of redirect loop
                pnlDefaultNotice.Visible = true;
                productId = GetFirstAvailableProductId();
            }

            litQueryStringID.Text = isExplicitQuery ? productId.ToString() : (productIdStr ?? "None (Defaulted to " + productId + ")");
            litBreadcrumbID.Text = productId.ToString();

            // Set dropdown selection
            if (ddlSwitchProduct.Items.FindByValue(productId.ToString()) != null)
            {
                ddlSwitchProduct.SelectedValue = productId.ToString();
            }

            FetchAndBindProduct(productId);
        }

        private int GetFirstAvailableProductId()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    string sql = "SELECT TOP 1 ID FROM Products ORDER BY ID ASC";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        object res = cmd.ExecuteScalar();
                        if (res != null && int.TryParse(res.ToString(), out int id))
                        {
                            return id;
                        }
                    }
                }
            }
            catch { }
            return 1;
        }

        private void FetchAndBindProduct(int productId)
        {
            bool found = false;

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                // 1. Try stored procedure sp_GetProductDetailsByID
                try
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetProductDetailsByID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ID", productId));
                        con.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                PopulateUiFields(rdr);
                                found = true;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Fall through to fallback query
                }

                // 2. Fallback if stored procedure was not found or failed
                if (!found)
                {
                    try
                    {
                        if (con.State != ConnectionState.Open) con.Open();

                        string sqlFallback = @"SELECT p.ID AS ProductID, p.Name, p.Price, 
                                                      ISNULL(pd.Color, 'Standard') AS Color, 
                                                      ISNULL(pd.Size, 'Regular') AS Size, 
                                                      ISNULL(pd.Weight, 0.0) AS Weight, 
                                                      p.Stock AS stock, 
                                                      ISNULL(pd.Discontinued, 0) AS Discontinued, 
                                                      ISNULL(pd.Supplier, 'Authorized Distributor') AS Supplier
                                               FROM Products p
                                               LEFT JOIN ProductDetails pd ON p.ID = pd.ProductID
                                               WHERE p.ID = @ID";

                        using (SqlCommand cmdFallback = new SqlCommand(sqlFallback, con))
                        {
                            cmdFallback.Parameters.AddWithValue("@ID", productId);
                            using (SqlDataReader rdrFallback = cmdFallback.ExecuteReader())
                            {
                                if (rdrFallback.Read())
                                {
                                    PopulateUiFields(rdrFallback);
                                    found = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.TraceError("Product details query failed: " + ex.Message);
                    }
                }
            }

            if (!found)
            {
                litHeaderProductName.Text = "Product Not Found";
                litHeaderProductID.Text = productId.ToString();
                lblID.Text = productId.ToString();
                lblName.Text = "Not Available";
                lblPrice.Text = "0.00";
                lblPriceHighlight.Text = "0.00";
                lblColor.Text = "N/A";
                lblSize.Text = "N/A";
                lblSizeHighlight.Text = "N/A";
                lblWeight.Text = "0.00";
                lblWeightHighlight.Text = "0.00";
                lblQuantityInStock.Text = "0";
                lblStockCount.Text = "0";
                lblDiscontinued.Text = "N/A";
                lblSupplier.Text = "N/A";
            }
        }

        private void PopulateUiFields(SqlDataReader rdr)
        {
            string id = rdr["ProductID"]?.ToString() ?? "0";
            string name = rdr["Name"]?.ToString() ?? "Product";
            decimal price = 0;
            if (rdr["Price"] != DBNull.Value) decimal.TryParse(rdr["Price"].ToString(), out price);

            string color = rdr["Color"]?.ToString() ?? "Standard";
            string size = rdr["Size"]?.ToString() ?? "Regular";
            string weight = rdr["Weight"]?.ToString() ?? "0.00";
            string stock = rdr["stock"]?.ToString() ?? "0";
            string discontinuedRaw = rdr["Discontinued"]?.ToString() ?? "0";
            string supplier = rdr["Supplier"]?.ToString() ?? "Authorized Distributor";

            bool isDiscontinued = discontinuedRaw == "1" || discontinuedRaw.Equals("true", StringComparison.OrdinalIgnoreCase);

            litHeaderProductName.Text = Server.HtmlEncode(name);
            litHeaderProductID.Text = id;
            lblID.Text = id;
            lblName.Text = Server.HtmlEncode(name);
            lblPrice.Text = price.ToString("N2");
            lblPriceHighlight.Text = price.ToString("N2");

            lblColor.Text = Server.HtmlEncode(color);
            SetColorIndicator(color);

            lblSize.Text = Server.HtmlEncode(size);
            lblSizeHighlight.Text = Server.HtmlEncode(size);

            lblWeight.Text = weight;
            lblWeightHighlight.Text = weight;

            lblQuantityInStock.Text = stock;
            lblStockCount.Text = stock;

            lblSupplier.Text = Server.HtmlEncode(supplier);

            if (isDiscontinued)
            {
                lblDiscontinued.Text = "<span class=\"status-pill status-discontinued\">&#10006; Discontinued</span>";
            }
            else
            {
                lblDiscontinued.Text = "<span class=\"status-pill status-active\">&#10004; Active Production</span>";
            }
        }

        private void SetColorIndicator(string color)
        {
            if (colorDotIndicator == null) return;

            string cLower = color.ToLower().Trim();
            string bgStyle = "#cbd5e1";

            if (cLower.Contains("white")) bgStyle = "#ffffff";
            else if (cLower.Contains("black")) bgStyle = "#0f172a";
            else if (cLower.Contains("silver") || cLower.Contains("grey") || cLower.Contains("gray")) bgStyle = "#94a3b8";
            else if (cLower.Contains("red")) bgStyle = "#ef4444";
            else if (cLower.Contains("blue")) bgStyle = "#3b82f6";
            else if (cLower.Contains("green")) bgStyle = "#10b981";
            else if (cLower.Contains("yellow") || cLower.Contains("gold")) bgStyle = "#f59e0b";

            colorDotIndicator.Attributes["style"] = "background-color: " + bgStyle + ";";
        }

        protected void ddlSwitchProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlSwitchProduct.SelectedValue))
            {
                Response.Redirect("~/ProductDetails.aspx?ID=" + ddlSwitchProduct.SelectedValue);
            }
        }
    }
}
