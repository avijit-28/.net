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

        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            string userName = TxtUser.Text.Trim();
            string email = TxtEmail.Text.Trim();

            // Navigate to GetUser.aspx with query string data
            Response.Redirect($"GetUser.aspx?user={Server.UrlEncode(userName)}&email={Server.UrlEncode(email)}");
        }

        protected void BtnGoToSignUp_Click(object sender, EventArgs e)
        {
            Response.Redirect("SignUp.aspx");
        }
    }
}