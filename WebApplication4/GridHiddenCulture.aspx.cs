using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class GridHiddenCulture : System.Web.UI.Page
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConn"]?.ConnectionString
                ?? ConfigurationManager.ConnectionStrings["AssignmetConnectionString"]?.ConnectionString;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                string sql = @"SELECT EmployeeId, FirstName, AnnualSalary, Country,
                                      CASE Country
                                          WHEN 'US' THEN 'en-US'
                                          WHEN 'India' THEN 'en-IN'
                                          WHEN 'UK' THEN 'en-GB'
                                          WHEN 'South Africa' THEN 'en-ZA'
                                          WHEN 'Malaysia' THEN 'en-MY'
                                          ELSE 'en-US'
                                      END AS CountryCulture
                               FROM AllEmployee
                               ORDER BY EmployeeId ASC";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                    }
                }
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // If the row being databound is a header row
            if (e.Row.RowType == DataControlRowType.Header)
            {
                // Hide CountryCulture header table cell
                e.Row.Cells[4].Visible = false;
            }
            // If the row being databound is a data row
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Retrieve the unformatted salary
                decimal salary = 0;
                decimal.TryParse(e.Row.Cells[2].Text, out salary);

                // Retrieve the culture
                string countryCulture = e.Row.Cells[4].Text;

                // Turn off visibility of CountryCulture cell
                e.Row.Cells[4].Visible = false;

                // Format the currency using specific country culture
                string formattedString = string.Empty;
                try
                {
                    formattedString = string.Format(new CultureInfo(countryCulture), "{0:c}", salary);
                }
                catch
                {
                    formattedString = string.Format(CultureInfo.InvariantCulture, "{0:c}", salary);
                }

                // Finally set formatted currency as text
                e.Row.Cells[2].Text = formattedString;
            }
        }
    }
}
