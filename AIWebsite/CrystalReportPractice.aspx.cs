using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

public partial class CrystalReportPractice : System.Web.UI.Page
{
    private ReportDocument rd;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindCountry();
        }

        // Always ensure report data is bound so viewer pagination and export work seamlessly
        LoadReport();
    }

    private string GetConnectionString()
    {
        if (ConfigurationManager.ConnectionStrings["EmployeeDbConnection"] != null &&
            !string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings["EmployeeDbConnection"].ConnectionString))
        {
            return ConfigurationManager.ConnectionStrings["EmployeeDbConnection"].ConnectionString;
        }

        // Fallback default
        return "Data Source=.;Initial Catalog=Assignmet;User ID=sa;Password=mcc#1234;Connect Timeout=30;";
    }

    private void BindCountry()
    {
        try
        {
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                string query = "SELECT DISTINCT Country FROM [dbo].[AllEmployee] WHERE Country IS NOT NULL AND LTRIM(RTRIM(Country)) <> '' ORDER BY Country ASC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        ddlCountry.DataSource = dt;
                        ddlCountry.DataTextField = "Country";
                        ddlCountry.DataValueField = "Country";
                        ddlCountry.DataBind();

                        ddlCountry.Items.Insert(0, new ListItem("-- All Countries --", ""));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowAlert("Note on Countries: " + ex.Message, "warning");
        }
    }

    public void LoadReport()
    {
        try
        {
            string fname = (txtFirstName.Text ?? string.Empty).Trim();
            string country = (ddlCountry.SelectedValue ?? string.Empty).Trim();

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM [dbo].[AllEmployee] 
                                WHERE (@fname = '' OR FirstName LIKE '%' + @fname + '%') 
                                  AND (@country = '' OR Country = @country)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@fname", SqlDbType.NVarChar, 100).Value = fname;
                    cmd.Parameters.Add("@country", SqlDbType.NVarChar, 100).Value = country;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds, "AllEmployee");

                        int rowCount = ds.Tables.Count > 0 ? ds.Tables[0].Rows.Count : 0;
                        lblRecordCount.Text = string.Format("<i class='fa-solid fa-users me-1'></i> {0} Record{1} Found", rowCount, rowCount == 1 ? "" : "s");

                        string reportPath = Server.MapPath("~/CrystalReport2.rpt");
                        if (!File.Exists(reportPath))
                        {
                            reportPath = Server.MapPath("~/CrystalReport.rpt");
                        }

                        if (!File.Exists(reportPath))
                        {
                            ShowAlert("Report file not found. Please verify CrystalReport2.rpt exists in the project root.", "danger");
                            return;
                        }

                        // Close previous report if any
                        if (rd != null)
                        {
                            rd.Close();
                            rd.Dispose();
                        }

                        rd = new ReportDocument();
                        rd.Load(reportPath);
                        rd.SetDataSource(ds.Tables[0]);

                        CrystalReportViewer1.ReportSource = rd;
                        CrystalReportViewer1.DataBind();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowAlert("Error loading report: " + ex.Message, "danger");
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadReport();
        string fname = txtFirstName.Text.Trim();
        string country = ddlCountry.SelectedItem != null ? ddlCountry.SelectedItem.Text : "";

        string msg = "Search executed";
        if (!string.IsNullOrEmpty(fname) || (ddlCountry.SelectedIndex > 0))
        {
            msg += string.Format(" for Name: '{0}', Country: '{1}'", 
                string.IsNullOrEmpty(fname) ? "Any" : fname,
                ddlCountry.SelectedIndex > 0 ? country : "All");
        }
        ShowAlert(msg, "success");
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        txtFirstName.Text = string.Empty;
        if (ddlCountry.Items.Count > 0)
        {
            ddlCountry.SelectedIndex = 0;
        }
        LoadReport();
        ShowAlert("Filters have been reset. Showing all employees.", "info");
    }

    protected void btnPdf_Download(object sender, EventArgs e)
    {
        ExportReport(ExportFormatType.PortableDocFormat, "EmployeeReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf");
    }

    protected void btnExcel_Download(object sender, EventArgs e)
    {
        ExportReport(ExportFormatType.Excel, "EmployeeReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls");
    }

    private void ExportReport(ExportFormatType formatType, string fileName)
    {
        try
        {
            LoadReport();

            if (rd != null && rd.IsLoaded)
            {
                rd.ExportToHttpResponse(formatType, Response, true, fileName);
                Response.End();
            }
            else
            {
                ShowAlert("Unable to export: Report document is not loaded.", "warning");
            }
        }
        catch (System.Threading.ThreadAbortException)
        {
            // Expected exception caused by Response.End()
        }
        catch (Exception ex)
        {
            ShowAlert("Export failed: " + ex.Message, "danger");
        }
    }

    private void ShowAlert(string message, string alertType = "info")
    {
        pnlAlert.Visible = true;
        pnlAlert.CssClass = string.Format("alert alert-{0} alert-dismissible fade show", alertType);
        litAlertMessage.Text = string.Format("<strong>Notice:</strong> {0}", message);
    }

    protected void Page_Unload(object sender, EventArgs e)
    {
        // Clean up Crystal Report Document COM and temp file handles
        if (rd != null)
        {
            try
            {
                rd.Close();
                rd.Dispose();
            }
            catch
            {
                // Suppress disposal exceptions on page unload
            }
            finally
            {
                rd = null;
            }
        }
    }
}
