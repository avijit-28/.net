using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class GridMultiCulture : System.Web.UI.Page
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
                string sql = "SELECT EmployeeId, FirstName, AnnualSalary AS Salary, Country FROM AllEmployee ORDER BY EmployeeId ASC";
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
            // Check if the row that is being bound, is a datarow
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string country = e.Row.Cells[3].Text;
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

                e.Row.Cells[2].Text = formattedString;
            }
        }
    }
}
