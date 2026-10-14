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
                string user = Request.QueryString["user"];
                string email = Request.QueryString["email"];

                LblName.Text = !string.IsNullOrWhiteSpace(user) ? Server.HtmlEncode(user) : "No Username Provided";
                LblEmail.Text = !string.IsNullOrWhiteSpace(email) ? Server.HtmlEncode(email) : "No Email Provided";
            }
        }

        protected void BtnBackToSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("SearchUser.aspx");
        }

        protected void BtnGoToDashboard_Click(object sender, EventArgs e)
        {
            Response.Redirect("SignUp.aspx");
        }
    }
}