using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace WebApplication4
{
    public partial class GridViewAll : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindEmployees("");
            }

        }
        //protected void btnSearch_Click(object sender, EventArgs e)
        //{
        //string Name = txtSearch.Text.Trim();
        //localhost2.WebService1SoapClient obj = new localhost2.WebService1SoapClient();
        //XmlElement xmlData = obj.GetUserDetails(Name);
        //DataSet ds = new DataSet();
        //using (StringReader sr = new StringReader(xmlData.OuterXml))
        //{
        //    ds.ReadXml(sr);
        //}
        //gvEmployees.DataSource = ds.Tables[0];
        //gvEmployees.DataBind();
        //BindEmployees(txtSearch.Text);
        //}

        private void BindEmployees(string name)
        {
            ServiceReference1.WebService1SoapClient obj =
                new ServiceReference1.WebService1SoapClient("WebService1Soap");

            XmlElement xml = obj.GetEmployeeDetails(name);

            DataSet ds = new DataSet();

            using (StringReader sr = new StringReader(xml.OuterXml))
            {
                ds.ReadXml(sr);
            }

            gvEmployees.DataSource = ds.Tables["EmployeeDetails"];
            gvEmployees.DataBind();
        }
    }
}