using System;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class GridHiddenCulture : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("EmployeeId", typeof(int));
            dt.Columns.Add("FirstName", typeof(string));
            dt.Columns.Add("AnnualSalary", typeof(int));
            dt.Columns.Add("Country", typeof(string));
            dt.Columns.Add("CountryCulture", typeof(string));

            dt.Rows.Add(1, "John", 55000, "US", "en-US");
            dt.Rows.Add(2, "Pam", 62000, "India", "en-IN");
            dt.Rows.Add(3, "David", 45000, "UK", "en-GB");
            dt.Rows.Add(4, "Rose", 78000, "South Africa", "en-ZA");
            dt.Rows.Add(5, "Mark", 69000, "Malaysia", "en-MY");

            GridView1.DataSource = dt;
            GridView1.DataBind();
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
                int salary = Convert.ToInt32(e.Row.Cells[2].Text);

                // Retrieve the culture
                string countryCulture = e.Row.Cells[4].Text;

                // Turn off visibility of CountryCulture cell
                e.Row.Cells[4].Visible = false;

                // Format the currency using specific country culture
                string formattedString = string.Format(new CultureInfo(countryCulture), "{0:c}", salary);

                // Finally set formatted currency as text
                e.Row.Cells[2].Text = formattedString;
            }
        }
    }
}
