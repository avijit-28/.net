using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace WebApplication4
{
    public partial class CheckAvailability : System.Web.UI.Page
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["DBConn"]?.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                checkun.Visible = false;
            }
        }

        protected void txtbtn_Click(object sender, EventArgs e)
        {
            PerformAvailabilityCheck();
        }

        protected void CheckUserNameAvailability(object sender, EventArgs e)
        {
            PerformAvailabilityCheck();
        }

        private void PerformAvailabilityCheck()
        {
            // Simulate brief latency for AJAX UpdateProgress spinner demonstration
            System.Threading.Thread.Sleep(1000);

            string username = txtun.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                checkun.Visible = true;
                checkun.Attributes["class"] = "status-box status-warning";
                shwimg.ImageUrl = "~/images/Cancel.png";
                lblmsg.Text = "Please enter a username before checking availability.";
                return;
            }

            if (string.IsNullOrEmpty(_conStr))
            {
                checkun.Visible = true;
                checkun.Attributes["class"] = "status-box status-danger";
                shwimg.ImageUrl = "~/images/Cancel.png";
                lblmsg.Text = "Database connection string 'DBConn' was not found in Web.config.";
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    string query = "SELECT COUNT(*) FROM UserDetails WHERE UserName = @un";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@un", username);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();

                        checkun.Visible = true;
                        if (count > 0)
                        {
                            checkun.Attributes["class"] = "status-box status-danger";
                            shwimg.ImageUrl = "~/images/Cancel.png";
                            lblmsg.Text = "UserName '" + username + "' Already Exists..!!";
                        }
                        else
                        {
                            checkun.Attributes["class"] = "status-box status-success";
                            shwimg.ImageUrl = "~/images/Accepted.png";
                            lblmsg.Text = "Congratulations! Username '" + username + "' is available.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                checkun.Visible = true;
                checkun.Attributes["class"] = "status-box status-danger";
                shwimg.ImageUrl = "~/images/Cancel.png";
                lblmsg.Text = "Database Error: " + ex.Message;
            }
        }
    }
}