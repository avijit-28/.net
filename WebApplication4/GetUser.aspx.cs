using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniProject
{
    public partial class GetUser : System.Web.UI.Page
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
                PopulateUserDropdown();
                LoadUserProfile();
            }
        }

        private void PopulateUserDropdown()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    string sql = "SELECT UserId, UserName, Email FROM [Assignmet].[dbo].[UserDetails] ORDER BY UserId ASC";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            ddlUsers.Items.Clear();
                            ddlUsers.Items.Add(new ListItem("-- Choose a student to test QueryString --", ""));
                            while (rdr.Read())
                            {
                                string text = string.Format("#{0} - {1} ({2})", rdr["UserId"], rdr["UserName"], rdr["Email"]);
                                string val = rdr["UserId"].ToString();
                                ddlUsers.Items.Add(new ListItem(text, val));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceWarning("Could not load user dropdown: " + ex.Message);
            }
        }

        private void LoadUserProfile()
        {
            string queryUser = Request.QueryString["user"];
            string queryEmail = Request.QueryString["email"];
            string queryId = Request.QueryString["id"];

            bool hasQueryString = !string.IsNullOrWhiteSpace(queryUser) || !string.IsNullOrWhiteSpace(queryEmail) || !string.IsNullOrWhiteSpace(queryId);

            if (hasQueryString)
            {
                pnlQuerySuccess.Visible = true;
                pnlQueryNotice.Visible = false;

                litQueryUser.Text = Server.HtmlEncode(queryUser ?? "");
                litQueryEmail.Text = Server.HtmlEncode(queryEmail ?? "");

                // Try fetching full profile from DB by username or id
                if (!FetchProfileFromDatabase(queryUser, queryId))
                {
                    // Fallback to literal QueryString values
                    LblUserId.Text = !string.IsNullOrWhiteSpace(queryId) ? "#" + queryId : "N/A";
                    LblName.Text = !string.IsNullOrWhiteSpace(queryUser) ? Server.HtmlEncode(queryUser) : "No Username Provided";
                    LblEmail.Text = !string.IsNullOrWhiteSpace(queryEmail) ? Server.HtmlEncode(queryEmail) : "No Email Provided";
                    LblMobile.Text = "Not provided";
                    LblCity.Text = "Not provided";
                    litAvatarInitial.Text = !string.IsNullOrWhiteSpace(queryUser) ? queryUser.Substring(0, 1).ToUpper() : "&#128100;";
                }
            }
            else
            {
                // Direct access without QueryString: load the first available user from database
                pnlQuerySuccess.Visible = false;
                pnlQueryNotice.Visible = true;

                if (!FetchDefaultUserFromDatabase())
                {
                    LblUserId.Text = "#1";
                    LblName.Text = "Sample User";
                    LblEmail.Text = "user@example.com";
                    LblMobile.Text = "9876543210";
                    LblCity.Text = "General";
                    litAvatarInitial.Text = "S";
                }
            }
        }

        private bool FetchProfileFromDatabase(string userName, string userIdStr)
        {
            try
            {
                int.TryParse(userIdStr, out int userId);

                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    string sql = @"SELECT TOP 1 UserId, UserName, Email, MobileNo, City 
                                   FROM [Assignmet].[dbo].[UserDetails] 
                                   WHERE (@UserId > 0 AND UserId = @UserId) OR (@UserName <> '' AND UserName = @UserName)";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@UserName", userName ?? string.Empty);

                        con.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                string name = rdr["UserName"].ToString();
                                LblUserId.Text = "#" + rdr["UserId"].ToString();
                                LblName.Text = name;
                                LblEmail.Text = rdr["Email"].ToString();
                                LblMobile.Text = !string.IsNullOrWhiteSpace(rdr["MobileNo"]?.ToString()) ? rdr["MobileNo"].ToString() : "Not provided";
                                LblCity.Text = !string.IsNullOrWhiteSpace(rdr["City"]?.ToString()) ? rdr["City"].ToString() : "General";
                                litAvatarInitial.Text = !string.IsNullOrEmpty(name) ? name.Substring(0, 1).ToUpper() : "&#128100;";
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceWarning("Database lookup failed in GetUser: " + ex.Message);
            }
            return false;
        }

        private bool FetchDefaultUserFromDatabase()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    string sql = "SELECT TOP 1 UserId, UserName, Email, MobileNo, City FROM [Assignmet].[dbo].[UserDetails] ORDER BY UserId ASC";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                string name = rdr["UserName"].ToString();
                                LblUserId.Text = "#" + rdr["UserId"].ToString();
                                LblName.Text = name;
                                LblEmail.Text = rdr["Email"].ToString();
                                LblMobile.Text = !string.IsNullOrWhiteSpace(rdr["MobileNo"]?.ToString()) ? rdr["MobileNo"].ToString() : "Not provided";
                                LblCity.Text = !string.IsNullOrWhiteSpace(rdr["City"]?.ToString()) ? rdr["City"].ToString() : "General";
                                litAvatarInitial.Text = !string.IsNullOrEmpty(name) ? name.Substring(0, 1).ToUpper() : "&#128100;";
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceWarning("Default user lookup failed in GetUser: " + ex.Message);
            }
            return false;
        }

        protected void ddlUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlUsers.SelectedValue) && int.TryParse(ddlUsers.SelectedValue, out int selectedId))
            {
                // Fetch username & email for selected user to redirect via QueryString
                try
                {
                    using (SqlConnection con = new SqlConnection(GetConnectionString()))
                    {
                        string sql = "SELECT UserName, Email FROM [Assignmet].[dbo].[UserDetails] WHERE UserId = @Id";
                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@Id", selectedId);
                            con.Open();
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    string userName = rdr["UserName"].ToString();
                                    string email = rdr["Email"].ToString();

                                    string url = string.Format("~/GetUser?user={0}&email={1}&id={2}",
                                        HttpUtility.UrlEncode(userName),
                                        HttpUtility.UrlEncode(email),
                                        selectedId);

                                    Response.Redirect(url);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceWarning("Dropdown redirect failed: " + ex.Message);
                }
            }
        }

        protected void BtnBackToSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SearchUser");
        }

        protected void BtnGoToDashboard_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SignUp");
        }
    }
}