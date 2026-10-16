namespace WebApplication4
{
    using System;
    using System.Web;

    /// <summary>
    /// Defines the <see cref="FirstPage" />
    /// </summary>
    public partial class FirstPage : System.Web.UI.Page
    {
        /// <summary>
        /// The Page_Load
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. Handle Logout Action
            if (string.Equals(Request.QueryString["action"], "logout", StringComparison.OrdinalIgnoreCase))
            {
                Session.Clear();
                Session.Abandon();

                if (Request.Cookies["UserWasLoggedIn"] != null)
                {
                    HttpCookie c = new HttpCookie("UserWasLoggedIn", "")
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        Path = "/"
                    };
                    Response.Cookies.Add(c);
                }

                Response.Redirect("~/FirstPage?logout=1", true);
                return;
            }

            // 2. Clear tracking cookie when landing from timeout or logout
            if (Request.QueryString["timeout"] == "1" || Request.QueryString["logout"] == "1")
            {
                if (Request.Cookies["UserWasLoggedIn"] != null)
                {
                    HttpCookie c = new HttpCookie("UserWasLoggedIn", "")
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        Path = "/"
                    };
                    Response.Cookies.Add(c);
                }
            }

            if (!IsPostBack)
            {
                lblOnlineUsers.Text = (Application["user"] != null ? Application["user"].ToString() : "0");
            }
        }

        /// <summary>
        /// The BtnSign_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        protected void BtnSign_Click(object sender, EventArgs e)
        {
            string email = (txtUserName.Text ?? string.Empty).Trim();
            string password = (txtPassword.Text ?? string.Empty).Trim();

            if (string.Equals(email, "avijit@gmail.com", StringComparison.OrdinalIgnoreCase) && password == "1234")
            {
                Session["UserName"] = email;
                Session["Password"] = password;

                // Set tracking cookie so the system detects when the session expires
                HttpCookie authCookie = new HttpCookie("UserWasLoggedIn", "1")
                {
                    HttpOnly = true,
                    Path = "/"
                };
                Response.Cookies.Add(authCookie);

                Response.Redirect("~/Home", true);
            }
            else
            {
                Response.Redirect("~/FirstPage?error=1", true);
            }
        }
    }
}
