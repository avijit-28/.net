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
        protected void BtnSend_Click(object sender, EventArgs e)
        {
            Response.Redirect("GetUser.aspx?UserName=" + Server.UrlEncode(TxtUser.Text) + "&UserEmail=" + Server.UrlEncode(TxtEmail.Text));
        }
    }
}