using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniProject
{
    public partial class CookiesSend : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnCookies_Click(object sender, EventArgs e)
        {
            HttpCookie cookie = new HttpCookie("UserDetails");

            cookie["Name"] = !string.IsNullOrWhiteSpace(TxtName.Text) ? TxtName.Text.Trim() : "Guest";
            cookie["Email"] = !string.IsNullOrWhiteSpace(TxtEmail.Text) ? TxtEmail.Text.Trim() : "N/A";
            cookie.Expires = DateTime.Now.AddMinutes(2); // Set cookie expiration time to 2 minutes
            Response.Cookies.Add(cookie);
            Response.Redirect("CookiesGet.aspx");
        }
    }
}