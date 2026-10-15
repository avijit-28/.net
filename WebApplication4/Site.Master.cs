using System;
using System.Web.UI;

namespace WebApplication4
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Display active online user count from Application state
            lblOnlineUsers.Text = (Application["user"] != null) ? Application["user"].ToString() : "0";

            // Display active user from Session state
            if (Session["UserName"] != null && !string.IsNullOrWhiteSpace(Session["UserName"].ToString()))
            {
                lblCurrentUser.Text = Session["UserName"].ToString();
            }
            else
            {
                lblCurrentUser.Text = "Guest";
            }
        }
    }
}
