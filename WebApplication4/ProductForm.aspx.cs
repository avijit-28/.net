using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class ProductForm : System.Web.UI.Page
    {
        public string conStr
        {
            get
            {
                var conn = ConfigurationManager.ConnectionStrings["DBConn"];
                return conn != null ? conn.ConnectionString : string.Empty;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LblMsg.Visible = false;
                LblMsg.Text = string.Empty;
            }
        }

        protected void BtnAddPrd_Click(object sender, EventArgs e)
        {
            string prdId = TxtPdrId.Text.Trim();
            string prdName = TxtPrdName.Text.Trim();
            string prdPrc = TxtPrdPrc.Text.Trim();
            string prdStk = TxtPrdStk.Text.Trim();

            // Validation: Ensure all fields are filled
            if (string.IsNullOrEmpty(prdId) || string.IsNullOrEmpty(prdName) ||
                string.IsNullOrEmpty(prdPrc) || string.IsNullOrEmpty(prdStk))
            {
                LblMsg.CssClass = "alert-box alert-warning";
                LblMsg.Text = "Please fill in all required product fields before submitting.";
                LblMsg.Visible = true;
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    string query = "insert into Products values(@PrdId,@PrdName,@PrdPrc,@PrdStk)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PrdId", prdId);
                        cmd.Parameters.AddWithValue("@PrdName", prdName);
                        cmd.Parameters.AddWithValue("@PrdPrc", prdPrc);
                        cmd.Parameters.AddWithValue("@PrdStk", prdStk);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            LblMsg.CssClass = "alert-box alert-success";
                            LblMsg.Text = "✓ Product added successfully!\n\n" +
                                          "• Product ID: " + prdId + "\n" +
                                          "• Name: " + prdName + "\n" +
                                          "• Price: " + prdPrc + "\n" +
                                          "• Stock: " + prdStk;
                            LblMsg.Visible = true;

                            // Reset form fields
                            TxtPdrId.Text = string.Empty;
                            TxtPrdName.Text = string.Empty;
                            TxtPrdPrc.Text = string.Empty;
                            TxtPrdStk.Text = string.Empty;
                        }
                        else
                        {
                            LblMsg.CssClass = "alert-box alert-danger";
                            LblMsg.Text = "Failed to add product. Please check your details and try again.";
                            LblMsg.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LblMsg.CssClass = "alert-box alert-danger";
                LblMsg.Text = "Database Error: " + ex.Message;
                LblMsg.Visible = true;
            }
        }
    }
}