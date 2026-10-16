using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class GridTemplateField : System.Web.UI.Page
    {
        private const string TableKey = "GridTemplateField_Data";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeData();
                BindGrid();
            }
        }

        private void InitializeData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("EmployeeId", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Gender", typeof(string));
            dt.Columns.Add("City", typeof(string));

            dt.Rows.Add(1, "Mark Hastings", "Male", "London");
            dt.Rows.Add(2, "Steve Pavez", "Male", "Sydney");
            dt.Rows.Add(3, "Mary Sava", "Female", "New York");
            dt.Rows.Add(4, "Valarie Vik", "Female", "Tokyo");
            dt.Rows.Add(5, "John Doe", "Male", "Chicago");

            Session[TableKey] = dt;
        }

        private DataTable GetData()
        {
            if (Session[TableKey] == null)
            {
                InitializeData();
            }
            return (DataTable)Session[TableKey];
        }

        private void BindGrid()
        {
            GridView1.DataSource = GetData();
            GridView1.DataBind();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            BindGrid();
            lblStatus.Text = "Editing row " + (e.NewEditIndex + 1);
            lblStatus.Visible = true;
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            BindGrid();
            lblStatus.Text = "Edit canceled.";
            lblStatus.Visible = true;
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int employeeId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);
            GridViewRow row = GridView1.Rows[e.RowIndex];

            // In BoundField columns, TextBox controls are generated at runtime
            string name = ((TextBox)row.Cells[2].Controls[0]).Text;
            DropDownList ddlGender = (DropDownList)row.FindControl("DropDownList1");
            string gender = ddlGender != null ? ddlGender.SelectedValue : "Select Gender";
            string city = ((TextBox)row.Cells[4].Controls[0]).Text;

            DataTable dt = GetData();
            foreach (DataRow dr in dt.Rows)
            {
                if (Convert.ToInt32(dr["EmployeeId"]) == employeeId)
                {
                    dr["Name"] = name;
                    dr["Gender"] = gender;
                    dr["City"] = city;
                    break;
                }
            }

            Session[TableKey] = dt;
            GridView1.EditIndex = -1;
            BindGrid();

            lblStatus.Text = "Employee updated successfully!";
            lblStatus.Visible = true;
        }
    }
}
