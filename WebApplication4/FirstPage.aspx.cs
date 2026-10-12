using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class FirstPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["UserName"] = txtUserName.Text;
            //Session["Password"] = txtPassword.Text;
        }

        protected void BtnSign_Click(object sender, EventArgs e)
        {
            Response.Redirect("SignUp.aspx");

        }

        protected void txtUserName_TextChanged(object sender, EventArgs e)
        {

        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}