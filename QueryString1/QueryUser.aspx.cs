using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace QueryString1
{
    public partial class QueryUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnSend_Click(object sender, EventArgs e)
        {
            Response.Redirect("StringGet.aspx?UserName=" + Server.UrlEncode(TxtUser.Text) + "UserEmail=" + Server.UrlEncode(TxtEmail.Text));
        }
    }
}