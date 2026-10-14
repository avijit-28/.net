using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class DetailPage : System.Web.UI.Page
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.RouteData.Values["id"].ToString() != null)
            {
                string strId = Page.RouteData.Values["id"].ToString();
                DisplayArticle(strId);
            }


        }
        private void DisplayArticle(string strId)
        {
            SqlConnection conn = new SqlConnection(_conStr);
            SqlDataAdapter da = new SqlDataAdapter("select Id,Title,Description,author from Articletable where Id=" + strId, conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "Articletable");
            lblTitle.Text = ds.Tables["Articletable"].Rows[0][1].ToString();
            lblDescription.Text = ds.Tables["Articletable"].Rows[0][2].ToString();
            lblauthor.Text = ds.Tables["Articletable"].Rows[0][3].ToString();
        }
    }
}