using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

public partial class CrystalReportPractice : System.Web.UI.Page
{
    SqlConnection con;
    SqlDataAdapter da = new SqlDataAdapter();
    DataSet ds = new DataSet();
    ReportDocument rd = new ReportDocument();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            BindCountry();
            LoadReport();
        }
    }
    
    private void BindCountry()
    {
        con = new SqlConnection("Data Source=.;Initial Catalog=Assignmet;User ID=sa;Password=mcc#1234");
        da = new SqlDataAdapter("SELECT distinct Country FROM [dbo].[AllEmployee] order by Country ", con);
        DataTable dt = new DataTable();
        da.Fill(dt);

        ddlCountry.DataSource = dt;
        ddlCountry.DataTextField = "Country";
        ddlCountry.DataValueField = "Country";
        ddlCountry.DataBind();

        ddlCountry.Items.Insert(0, new ListItem("--Select Country--", " "));

    }
    public void LoadReport()
    {
        con = new SqlConnection("Data Source=.;Initial Catalog=Assignmet;User ID=sa;Password=mcc#1234");
        da = new SqlDataAdapter("SELECT * FROM [dbo].[AllEmployee] where FirstName like'%'+@fname+'%' AND (@country='' OR Country=@country) ", con);
        da.SelectCommand.Parameters.AddWithValue("@fname", txtFirstName.Text);
        da.SelectCommand.Parameters.AddWithValue("@country", ddlCountry.SelectedValue);
        da.Fill(ds);

        rd.Load(Server.MapPath("~/CrystalReport.rpt"));
        rd.SetDataSource(ds.Tables[0]);
        CrystalReportViewer1.ReportSource = rd;
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        
        LoadReport();
    }

    protected void btnPdf_Download(object sender, EventArgs e)
    {
        LoadReport();
        convertToPDF();
    }
    public void convertToPDF()
    {
        ExportFormatType formatType = ExportFormatType.PortableDocFormat;
        rd.ExportToHttpResponse(formatType, Response, true, "demo");
        Response.End();
    }
}