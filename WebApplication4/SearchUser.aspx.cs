using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication4;

namespace MiniProject
{
    public partial class SearchUser : Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            // Subscribe to Master Page search button event before postback processing
            if (Master != null)
            {
                Master.SearchClicked += Master_SearchClicked;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // If a search query parameter is passed via URL routing or query string
                string queryTerm = Request.QueryString["term"];
                if (!string.IsNullOrEmpty(queryTerm))
                {
                    if (Master != null)
                    {
                        Master.SearchTerm = queryTerm;
                    }
                    BindUserGrid(queryTerm);
                }
                else
                {
                    // Default: display all users / students
                    BindUserGrid(string.Empty);
                }
            }
        }

        /// <summary>
        /// Event handler triggered when the search button on the Master Page is clicked.
        /// Demonstrates passing data from Master Page (txtMasterSearch) to Content Page (GridViewUsers).
        /// </summary>
        private void Master_SearchClicked(object sender, EventArgs e)
        {
            string searchTerm = Master != null ? Master.SearchTerm : string.Empty;
            BindUserGrid(searchTerm);
        }

        /// <summary>
        /// Clears the active search filter and resets the Master Page search textbox.
        /// </summary>
        protected void btnShowAll_Click(object sender, EventArgs e)
        {
            if (Master != null)
            {
                Master.SearchTerm = string.Empty;
            }
            BindUserGrid(string.Empty);
        }

        /// <summary>
        /// Navigation back to User Dashboard (SignUp.aspx).
        /// </summary>
        protected void BtnGoToSignUp_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SignUp");
        }

        /// <summary>
        /// Queries the database table [Assignmet].[dbo].[UserDetails] for students whose name
        /// contains the search term and binds the result set to GridViewUsers.
        /// </summary>
        private void BindUserGrid(string searchTerm)
        {
            string connStr = ConfigurationManager.ConnectionStrings["DBConn"]?.ConnectionString
                          ?? ConfigurationManager.ConnectionStrings["AssignmetConnectionString"]?.ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string sql = @"SELECT UserId, UserName, Email, MobileNo, City 
                               FROM [Assignmet].[dbo].[UserDetails] 
                               WHERE (@term = '' OR UserName LIKE '%' + @term + '%') 
                               ORDER BY UserId ASC";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@term", searchTerm != null ? searchTerm.Trim() : string.Empty);

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        GridViewUsers.DataSource = dt;
                        GridViewUsers.DataBind();

                        // Update record count badge and active filter readout
                        int count = dt.Rows.Count;
                        lblRecordCount.Text = $"{count} student{(count == 1 ? "" : "s")} found";

                        if (!string.IsNullOrWhiteSpace(searchTerm))
                        {
                            pnlActiveFilter.Visible = true;
                            lblFilterTerm.Text = HttpUtility.HtmlEncode(searchTerm);
                            btnShowAll.Visible = true;
                        }
                        else
                        {
                            pnlActiveFilter.Visible = false;
                            btnShowAll.Visible = false;
                        }
                    }
                }
            }
        }
    }
}