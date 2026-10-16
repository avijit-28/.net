using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class GridInsertUpdateDelete : System.Web.UI.Page
    {
        private const string TableKey = "GridInsertUpdateDelete_Data";

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

            TextBox tbName = (TextBox)row.FindControl("TextBox1");
            DropDownList ddlGender = (DropDownList)row.FindControl("DropDownList1");
            TextBox tbCity = (TextBox)row.FindControl("TextBox3");

            if (tbName != null && ddlGender != null && tbCity != null)
            {
                string name = tbName.Text.Trim();
                string gender = ddlGender.SelectedValue;
                string city = tbCity.Text.Trim();

                if (gender == "Select Gender" || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(city))
                {
                    e.Cancel = true;
                    return;
                }

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

                lblStatus.Text = string.Format("Employee ID {0} updated successfully!", employeeId);
                lblStatus.Visible = true;
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int employeeId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);
            DataTable dt = GetData();

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (Convert.ToInt32(dt.Rows[i]["EmployeeId"]) == employeeId)
                {
                    dt.Rows.RemoveAt(i);
                    break;
                }
            }

            Session[TableKey] = dt;
            GridView1.EditIndex = -1;
            BindGrid();

            lblStatus.Text = string.Format("Employee ID {0} deleted successfully!", employeeId);
            lblStatus.Visible = true;
        }

        protected void lbInsert_Click(object sender, EventArgs e)
        {
            if (GridView1.FooterRow == null) return;

            TextBox txtName = (TextBox)GridView1.FooterRow.FindControl("txtName");
            DropDownList ddlInsertGender = (DropDownList)GridView1.FooterRow.FindControl("ddlInsertGender");
            TextBox txtCity = (TextBox)GridView1.FooterRow.FindControl("txtCity");

            if (txtName != null && ddlInsertGender != null && txtCity != null)
            {
                string name = txtName.Text.Trim();
                string gender = ddlInsertGender.SelectedValue;
                string city = txtCity.Text.Trim();

                if (gender == "Select Gender" || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(city))
                {
                    return;
                }

                DataTable dt = GetData();

                int maxId = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    int currentId = Convert.ToInt32(dr["EmployeeId"]);
                    if (currentId > maxId) maxId = currentId;
                }
                int newId = maxId + 1;

                dt.Rows.Add(newId, name, gender, city);
                Session[TableKey] = dt;

                BindGrid();

                lblStatus.Text = string.Format("New Employee '{0}' inserted successfully with ID #{1}!", name, newId);
                lblStatus.Visible = true;
            }
        }
    }
}
