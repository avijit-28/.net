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
                BindEmployees(string.Empty);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvEmployees.PageIndex = 0;
            BindEmployees(txtSearch.Text.Trim());
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            gvEmployees.PageIndex = 0;
            BindEmployees(string.Empty);
        }

        protected void gvEmployees_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEmployees.PageIndex = e.NewPageIndex;
            BindEmployees(txtSearch.Text.Trim());
        }

        private void BindEmployees(string name)
        {
            try
            {
                // Invoke Web Service WebService1.GetEmployeeDetails
                WebApplication4.WebService1 ws = new WebApplication4.WebService1();
                XmlElement xml = ws.GetEmployeeDetails(name ?? string.Empty);

                DataSet ds = new DataSet();
                if (xml != null && xml.HasChildNodes)
                {
                    using (StringReader sr = new StringReader(xml.OuterXml))
                    {
                        ds.ReadXml(sr);
                    }
                }

                DataTable dt = null;
                if (ds.Tables.Contains("EmployeeDetails") && ds.Tables["EmployeeDetails"].Rows.Count > 0)
                {
                    dt = ds.Tables["EmployeeDetails"];
                }
                else if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    dt = ds.Tables[0];
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    gvEmployees.DataSource = dt;
                    gvEmployees.DataBind();

                    int total = dt.Rows.Count;
                    int pageSize = gvEmployees.PageSize;
                    int currentPage = gvEmployees.PageIndex + 1;
                    int totalPages = (int)Math.Ceiling((double)total / pageSize);

                    lblRecordCount.Text = $"Page {currentPage} of {totalPages} &bull; Total {total} Record(s)";
                    pnlMessage.Visible = false;
                }
                else
                {
                    gvEmployees.DataSource = null;
                    gvEmployees.DataBind();
                    lblRecordCount.Text = "0 records found";
                    pnlMessage.Visible = true;
                    lblMessage.Text = string.IsNullOrWhiteSpace(name)
                        ? "No employee records found in the database."
                        : $"No employee records matched \"{HttpUtility.HtmlEncode(name)}\".";
                }
            }
            catch (Exception ex)
            {
                gvEmployees.DataSource = null;
                gvEmployees.DataBind();
                lblRecordCount.Text = "Error";
                pnlMessage.Visible = true;
                lblMessage.Text = "Error retrieving data from WebService: " + HttpUtility.HtmlEncode(ex.Message);
            }
        }
    }
}