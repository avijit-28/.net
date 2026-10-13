using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class SignUp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblOnlineUsers.Text = (Application["user"] != null ? Application["user"].ToString() : "0");
            outUserName.Text = Session["UserName"] != null ? Session["UserName"].ToString() : "Guest";
            outPassword.Text = Session["Password"] != null ? Session["Password"].ToString() : "••••••••";
            lblOnlineUsers.Text = (Application["user"] != null ? Application["user"].ToString() : "0");
        }
       
    }
}