using System;
using System.Web.UI;

namespace WebApplication4
{
    public partial class SiteMaster : MasterPage
    {
        // Custom event fired when user hits Search button on Master Page
        public event EventHandler SearchClicked;

        // Public property to read or write the search text across Master and Content pages
        public string SearchTerm
        {
            get
            {
                return txtMasterSearch != null ? txtMasterSearch.Text.Trim() : string.Empty;
            }
            set
            {
                if (txtMasterSearch != null)
                {
                    txtMasterSearch.Text = value;
                }
            }
        }

        // Direct control accessors
        public System.Web.UI.WebControls.TextBox MasterSearchTextBox => txtMasterSearch;
        public System.Web.UI.WebControls.Button MasterSearchButton => btnMasterSearch;

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

        protected void btnMasterSearch_Click(object sender, EventArgs e)
        {
            // If the active content page has registered an event handler (e.g., SearchUser.aspx)
            if (SearchClicked != null)
            {
                SearchClicked(this, EventArgs.Empty);
            }
            else
            {
                // If search was executed from any other page, redirect to SearchUser content page
                string term = SearchTerm;
                if (!string.IsNullOrEmpty(term))
                {
                    Response.Redirect("~/SearchUser?term=" + Server.UrlEncode(term));
                }
                else
                {
                    Response.Redirect("~/SearchUser");
                }
            }
        }
    }
}
