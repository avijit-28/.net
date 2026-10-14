using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniProject
{
    public partial class GetUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Read from QueryString parameters, with fallback to Session
                string userName = Request.QueryString["user"] ?? Request.QueryString["name"] ?? Session["SearchUser"]?.ToString();
                string email = Request.QueryString["email"] ?? Session["SearchEmail"]?.ToString();

                LblName.Text = !string.IsNullOrWhiteSpace(userName) ? userName : "Not specified";
                LblEmail.Text = !string.IsNullOrWhiteSpace(email) ? email : "Not specified";
            }
        }

        protected void BtnBackSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("SearchUser.aspx");
        }

        protected void BtnGoSignUp_Click(object sender, EventArgs e)
        {
            Response.Redirect("SignUp.aspx");
        }
    }
}