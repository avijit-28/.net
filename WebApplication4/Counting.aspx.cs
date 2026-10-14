using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniProject
{
    public partial class Counting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //ViewState["Clicks"] = 0;
                Response.Write(IsPostBack);
                TextBox1.Text = "0";
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {


            //int count = (ViewState["Clicks"] != null) ? (int)ViewState["Clicks"] + 1 : 1;
            //ViewState["Clicks"] = count;
            //TextBox1.Text = count.ToString();
            //Response.Write(IsPostBack);
            int counts = Convert.ToInt32(TextBox1.Text) + 1;
            TextBox1.Text = counts.ToString();
        }

        protected void BtnReset_Click(object sender, EventArgs e)
        {
            //ViewState["Clicks"] = 0;
            //Response.Write(IsPostBack);
            TextBox1.Text = "0";
        }
    }
}