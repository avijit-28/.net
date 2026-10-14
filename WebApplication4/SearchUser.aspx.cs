using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniProject
{
    public partial class SearchUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LblMsg.Visible = false;
                LblMsg.Text = string.Empty;
            }
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            string userName = TxtUser.Text.Trim();
            string email = TxtEmail.Text.Trim();

            if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(email))
            {
                LblMsg.CssClass = "alert-box alert-warning";
                LblMsg.Text = "Please enter a User Name or Email Address to search.";
                LblMsg.Visible = true;
                return;
            }

            // Persist in Session as a backup
            Session["SearchUser"] = userName;
            Session["SearchEmail"] = email;

            // Redirect to GetUser.aspx with QueryString parameters
            Response.Redirect("GetUser.aspx?user=" + Server.UrlEncode(userName) + "&email=" + Server.UrlEncode(email));
        }

        protected void BtnGoSignUp_Click(object sender, EventArgs e)
        {
            Response.Redirect("SignUp.aspx");
        }
    }
}