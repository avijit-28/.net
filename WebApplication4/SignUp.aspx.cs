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
            outUserName.Text = Session["UserName"]?.ToString();
            outPassword.Text = Session["Password"]?.ToString();
            Response.Write("The num of users online = " + Application["user"].ToString());

        }
       
    }
}