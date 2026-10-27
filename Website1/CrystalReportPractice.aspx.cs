using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;

public partial class CrystalReportPractice : System.Web.UI.Page
{
    SqlConnection con;
    SqlDataAdapter da = new SqlDataAdapter();
    DataSet ds = new DataSet();

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        con = new SqlConnection("Data Source=.;Initial Catalog=Assignmet;User ID=sa;Password=mcc#1234");
        da = new SqlDataAdapter("SELECT * FROM [dbo].[AllEmployee] where FirstName like  '%'+@fname+'%' ", con);
        da.SelectCommand.Parameters.AddWithValue("@fname", txtFirstName);
        da.Fill(ds);
        ReportDocument rd = new ReportDocument();
        rd.Load(Server.MapPath("CrystalReport.rpt"));
        rd.SetDataSource(ds.Tables[0]);
        CrystalReportViewer1.ReportSource = rd;
    }
}