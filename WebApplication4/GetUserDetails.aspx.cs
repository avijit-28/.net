using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace WebApplication4
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
            try
            {
                localhostmcc.WebService1 objUserDetails = new localhostmcc.WebService1();
                if (Request.Url != null)
                {
                    objUserDetails.Url = new Uri(Request.Url, ResolveUrl("~/WebService1.asmx")).ToString();
                }

                DataSet dsresult = new DataSet();
                XmlElement exelement = objUserDetails.GetSdtDetails(userName ?? string.Empty);

                if (exelement != null && exelement.HasChildNodes)
                {
                    XmlNodeReader nodereader = new XmlNodeReader(exelement);
                    dsresult.ReadXml(nodereader, XmlReadMode.Auto);

                    if (dsresult.Tables.Count > 0 && dsresult.Tables[0].Rows.Count > 0)
                    {
                        gvUserDetails.DataSource = dsresult.Tables[0];
                        gvUserDetails.DataBind();
                        return;
                    }
                }

                gvUserDetails.DataSource = null;
                gvUserDetails.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error fetching user details via WebService: " + ex.Message);
                gvUserDetails.DataSource = null;
                gvUserDetails.DataBind();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            BindUserDetails(txtUserName.Text.Trim());
        }
    }
}
