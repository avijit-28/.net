using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace AddSearch
{
    public partial class service : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUserDetails("");
            }
        }
        protected void BindUserDetails(string userName)
        {
            localhost2.WebService1SoapClient objUserDetails = new localhost2.WebService1SoapClient();
            DataSet dsresult = new DataSet();
            XElement exelement =objUserDetails.GetUserDetails(userName);
            if (exelement != null)
            {
                XmlReader reader = exelement.CreateReader();
                dsresult.ReadXml(reader);
                gvUserDetails.DataSource = dsresult;
                gvUserDetails.DataBind();
            }
            else
            {
                gvUserDetails.DataSource = null;
                gvUserDetails.DataBind();
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            BindUserDetails(txtUserName.Text);
        }
    }
}