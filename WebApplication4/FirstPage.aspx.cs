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
            //lblOnlineUsers.Text = (Application["user"] != null ? Application["user"].ToString() : "0");
        }

        protected void BtnSign_Click(object sender, EventArgs e)
        {   
            if (txtUserName.Text == "avijit@gmail.com" && txtPassword.Text == "123456789"){
                Session["UserName"] = txtUserName.Text;
                Session["Password"] = txtPassword.Text;
                

            }
            Response.Redirect("SignUp.aspx");

        }

       

        
    }
}