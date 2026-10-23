using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AddSearch
{
    public partial class ProductDetails : System.Web.UI.Page
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
                PopulateSwitcher();
                LoadDetails();
            }
        }

        private void PopulateSwitcher()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    string sql = "SELECT ID, Name FROM Products ORDER BY ID ASC";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            ddlProductSwitch.Items.Clear();
                            while (rdr.Read())
                            {
                                string text = string.Format("#{0} - {1}", rdr["ID"], rdr["Name"]);
                                string val = rdr["ID"].ToString();
                                ddlProductSwitch.Items.Add(new ListItem(text, val));
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void LoadDetails()
        {
            string productIdStr = Request.QueryString["ID"];
            int productId = 1;

            if (!string.IsNullOrWhiteSpace(productIdStr) && int.TryParse(productIdStr, out int parsedId))
            {
                productId = parsedId;
                litIdDisplay.Text = productId.ToString();
            }
            else
            {
                litIdDisplay.Text = "None (Defaulted to #1)";
            }

            if (ddlProductSwitch.Items.FindByValue(productId.ToString()) != null)
            {
                ddlProductSwitch.SelectedValue = productId.ToString();
            }

            bool found = false;
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
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
                                SetDetails(rdr);
                                found = true;
                            }
                        }
                    }
                }
                catch
                {
                }

                if (!found)
                {
                    try
                    {
                        if (con.State != ConnectionState.Open) con.Open();
                        string sql = @"SELECT p.ID AS ProductID, p.Name, p.Price, 
                                              ISNULL(pd.Color, 'Standard') AS Color, 
                                              ISNULL(pd.Size, 'Regular') AS Size, 
                                              ISNULL(pd.Weight, 0.0) AS Weight, 
                                              p.Stock AS stock, 
                                              ISNULL(pd.Discontinued, 0) AS Discontinued, 
                                              ISNULL(pd.Supplier, 'General Supplier') AS Supplier
                                       FROM Products p
                                       LEFT JOIN ProductDetails pd ON p.ID = pd.ProductID
                                       WHERE p.ID = @ID";
                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@ID", productId);
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    SetDetails(rdr);
                                    found = true;
                                }
                            }
                        }
                    }
                    catch { }
                }
            }

            if (!found)
            {
                lblID.Text = productId.ToString();
                lblName.Text = "Product Not Found";
                lblPrice.Text = "0.00";
                lblColor.Text = "N/A";
                lblSize.Text = "N/A";
                lblWeight.Text = "0.00";
                lblQuantityInStock.Text = "0";
                lblDiscontinued.Text = "N/A";
                lblSupplier.Text = "N/A";
            }
        }

        private void SetDetails(SqlDataReader rdr)
        {
            lblID.Text = rdr["ProductID"]?.ToString() ?? "0";
            lblName.Text = rdr["Name"]?.ToString() ?? "Product";

            decimal price = 0;
            if (rdr["Price"] != DBNull.Value) decimal.TryParse(rdr["Price"].ToString(), out price);
            lblPrice.Text = price.ToString("N2");

            lblColor.Text = rdr["Color"]?.ToString() ?? "Standard";
            lblSize.Text = rdr["Size"]?.ToString() ?? "Regular";
            lblWeight.Text = rdr["Weight"]?.ToString() ?? "0.00";
            lblQuantityInStock.Text = rdr["stock"]?.ToString() ?? "0";

            string discontinuedRaw = rdr["Discontinued"]?.ToString() ?? "0";
            bool isDiscontinued = discontinuedRaw == "1" || discontinuedRaw.Equals("true", StringComparison.OrdinalIgnoreCase);

            if (isDiscontinued)
            {
                lblDiscontinued.Text = "<span class=\"status-pill status-discontinued\">&#10006; Discontinued</span>";
            }
            else
            {
                lblDiscontinued.Text = "<span class=\"status-pill status-active\">&#10004; Active Production</span>";
            }

            lblSupplier.Text = rdr["Supplier"]?.ToString() ?? "Authorized Supplier";
        }

        protected void ddlProductSwitch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlProductSwitch.SelectedValue))
            {
                Response.Redirect("ProductDetails.aspx?ID=" + ddlProductSwitch.SelectedValue);
            }
        }
    }
}