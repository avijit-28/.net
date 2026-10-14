using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QueryString1
{
    public partial class StringGet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LblName.Text = Request.QueryString["UserName"];
            LblEmail.Text = Request.QueryString["UserEmail"];
        }
    }
}