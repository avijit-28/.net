using System;
using System.Web;
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

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnforceAccessGate();
        }

        /// <summary>
        /// Central Access Control Gate:
        /// 1. If user's session expired (was previously logged in), redirect immediately to FirstPage (sign-in) with timeout notification.
        /// 2. If user is unauthenticated, allow only Home (Default.aspx) and prevent navigating to any other internal page.
        /// </summary>
        private void EnforceAccessGate()
        {
            bool isAuthenticated = Session["UserName"] != null && !string.IsNullOrWhiteSpace(Session["UserName"].ToString());
            bool wasLoggedInCookie = Request.Cookies["UserWasLoggedIn"] != null && Request.Cookies["UserWasLoggedIn"].Value == "1";

            // Case A: User's session expired/ended
            if (!isAuthenticated && wasLoggedInCookie)
            {
                // Expire and clear the tracking cookie
                HttpCookie expireCookie = new HttpCookie("UserWasLoggedIn", "")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Path = "/"
                };
                Response.Cookies.Add(expireCookie);

                // Automatically redirect to the sign in page
                Response.Redirect("~/FirstPage?timeout=1", true);
                return;
            }

            // Case B: User has not signed in
            if (!isAuthenticated)
            {
                string appRelative = Request.AppRelativeCurrentExecutionFilePath ?? string.Empty;
                bool isHomePage = string.Equals(appRelative, "~/Default.aspx", StringComparison.OrdinalIgnoreCase)
                               || string.Equals(appRelative, "~/", StringComparison.OrdinalIgnoreCase);

                string rawPath = Request.Path ?? string.Empty;
                if (rawPath.TrimEnd('/').EndsWith("/Home", StringComparison.OrdinalIgnoreCase) ||
                    rawPath.TrimEnd('/').EndsWith("/Default", StringComparison.OrdinalIgnoreCase))
                {
                    isHomePage = true;
                }

                // If not signed in and trying to go to any page other than Home, keep user on Home page
                if (!isHomePage)
                {
                    Response.Redirect("~/Home?restricted=1", true);
                    return;
                }
            }
        }

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
                lblCurrentUser.Text = "Guest (Unsigned)";
            }
        }

        protected void btnMasterSearch_Click(object sender, EventArgs e)
        {
            // If user is not signed in, restrict navigation and stay on Home page
            if (Session["UserName"] == null || string.IsNullOrWhiteSpace(Session["UserName"].ToString()))
            {
                Response.Redirect("~/Home?restricted=1");
                return;
            }

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
