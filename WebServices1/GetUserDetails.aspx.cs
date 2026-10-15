using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml;

namespace WebServices1
{
    public partial class GetUserDetails : System.Web.UI.Page
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
            // Use the local WebService1 implementation instead of a missing 'localhost' proxy
            WebService1 objUserDetails = new WebService1();
            DataSet dsresult = new DataSet();
            // Call GetSdtDetails which returns XmlElement for student details
            XmlElement exelement = objUserDetails.GetSdtDetails(userName);
            if (exelement != null)
            {
                XmlNodeReader nodereader = new XmlNodeReader(exelement);
                dsresult.ReadXml(nodereader, XmlReadMode.Auto);
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