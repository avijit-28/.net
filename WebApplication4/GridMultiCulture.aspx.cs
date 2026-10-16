using System;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class GridMultiCulture : System.Web.UI.Page
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
            dt.Columns.Add("Salary", typeof(int));
            dt.Columns.Add("Country", typeof(string));

            dt.Rows.Add(1, "John", 55000, "US");
            dt.Rows.Add(2, "Pam", 62000, "India");
            dt.Rows.Add(3, "David", 45000, "UK");
            dt.Rows.Add(4, "Rose", 78000, "South Africa");
            dt.Rows.Add(5, "Mark", 69000, "Malaysia");

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check if the row that is being bound, is a datarow
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string country = e.Row.Cells[3].Text.Trim();
                int salary = Convert.ToInt32(e.Row.Cells[2].Text);
                string formattedString = string.Empty;

                if (country == "US")
                {
                    formattedString = string.Format(new CultureInfo("en-US"), "{0:c}", salary);
                }
                else if (country == "UK")
                {
                    formattedString = string.Format(new CultureInfo("en-GB"), "{0:c}", salary);
                }
                else if (country == "India")
                {
                    formattedString = string.Format(new CultureInfo("en-IN"), "{0:c}", salary);
                }
                else if (country == "South Africa")
                {
                    formattedString = string.Format(new CultureInfo("en-ZA"), "{0:c}", salary);
                }
                else if (country == "Malaysia")
                {
                    formattedString = string.Format(new CultureInfo("en-MY"), "{0:c}", salary);
                }
                else
                {
                    formattedString = string.Format(CultureInfo.InvariantCulture, "{0:c}", salary);
                }

                e.Row.Cells[2].Text = formattedString;
            }
        }
    }
}
