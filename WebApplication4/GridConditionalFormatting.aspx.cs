using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class GridConditionalFormatting : System.Web.UI.Page
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
                string sql = "SELECT EmployeeId, FirstName, AnnualSalary, Country AS Department FROM AllEmployee ORDER BY EmployeeId ASC";
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
            // Loop thru each datarow in the gridview
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Retrieve salary using DataBinder.Eval
                decimal salary = 0;
                object rawSalary = DataBinder.Eval(e.Row.DataItem, "AnnualSalary");
                if (rawSalary != null)
                {
                    decimal.TryParse(rawSalary.ToString(), out salary);
                }

                // If Salary > 70000, set your styles
                if (salary > 70000)
                {
                    e.Row.BackColor = System.Drawing.Color.Red;
                    e.Row.ForeColor = System.Drawing.Color.White;
                    e.Row.Font.Bold = true;
                }
            }
        }
    }
}
