using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniProject
{
    public partial class CookiesGet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie cookie = Request.Cookies["UserDetails"];
            if (cookie != null && (!string.IsNullOrEmpty(cookie["Name"]) || !string.IsNullOrEmpty(cookie["Email"])))
            {
                LblName.Text = !string.IsNullOrEmpty(cookie["Name"]) ? cookie["Name"] : "Not Specified";
                LblEmail.Text = !string.IsNullOrEmpty(cookie["Email"]) ? cookie["Email"] : "Not Specified";
            }
            else
            {
                LblName.Text = "No cookie found / Expired";
                LblEmail.Text = "No cookie found / Expired";
            }
        }
    }
}