using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class ProductForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void BtnAddPrd_Click(object sender, EventArgs e)
        {
            String Dtls = "";
            Dtls += TxtPdrId.Text + " ";
            Dtls += TxtPrdName.Text + " ";
            Dtls += TxtPrdPrc.Text + " ";
            Dtls += TxtPrdStk.Text + " ";
            LblMsg.Text = Dtls;

        }
    }
}