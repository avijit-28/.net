using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class DetailPage : System.Web.UI.Page
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["DBConn"]?.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Page.RouteData.Values["id"] != null)
                {
                    string strId = Page.RouteData.Values["id"].ToString();
                    DisplayArticle(strId);
                } 
                else
                {
                    lblTitle.Text = "No Article Specified";
                    lblDescription.Text = "Please access this page through a routed URL (e.g., /Articles/{id}/{Title}).";
                    lblauthor.Text = "N/A";
                }
            }
        }

        private void DisplayArticle(string strId)
        {
            if (string.IsNullOrEmpty(_conStr))
            {
                lblTitle.Text = "Database Configuration Error";
                lblDescription.Text = "Connection string 'DBConn' is missing from Web.config.";
                lblauthor.Text = "System";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_conStr))
                {
                    using (SqlCommand cmd = new SqlCommand("select Id, Title, Description, author from Articletable where Id = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", strId);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                lblTitle.Text = dt.Rows[0]["Title"] != DBNull.Value ? dt.Rows[0]["Title"].ToString() : "Untitled";
                                lblDescription.Text = dt.Rows[0]["Description"] != DBNull.Value ? dt.Rows[0]["Description"].ToString() : "No description available.";
                                lblauthor.Text = dt.Rows[0]["author"] != DBNull.Value ? dt.Rows[0]["author"].ToString() : "Anonymous";
                            }
                            else
                            {
                                lblTitle.Text = "Article Not Found";
                                lblDescription.Text = "No article with ID " + HttpUtility.HtmlEncode(strId) + " was found in the database.";
                                lblauthor.Text = "N/A";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblTitle.Text = "Error Loading Article";
                lblDescription.Text = ex.Message;
                lblauthor.Text = "Error";
            }
        }
    }
}