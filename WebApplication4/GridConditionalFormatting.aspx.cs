using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class GridConditionalFormatting : System.Web.UI.Page
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
            dt.Columns.Add("Department", typeof(string));

            dt.Rows.Add(1, "John", 55000, "IT");
            dt.Rows.Add(2, "Pam", 62000, "HR");
            dt.Rows.Add(3, "David", 45000, "IT");
            dt.Rows.Add(4, "Rose", 78000, "Payroll"); // Salary > 70000 -> Red background
            dt.Rows.Add(5, "Mark", 69000, "HR");

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Loop thru each datarow in the gridview
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Retrieve salary using DataBinder.Eval
                int salary = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "AnnualSalary"));

                // If Salary > 70000, set your styles
                if (salary > 70000)
                {
                    e.Row.BackColor = System.Drawing.Color.Red;
                    e.Row.ForeColor = System.Drawing.Color.White;
                }
            }
        }
    }
}
