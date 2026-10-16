using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class GridValidation : System.Web.UI.Page
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
                string sql = "SELECT ID AS EmployeeId, Name, Gender, City FROM Employee ORDER BY ID ASC";
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

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            BindGrid();
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

            TextBox tbName = (TextBox)row.FindControl("txtName");
            DropDownList ddlGender = (DropDownList)row.FindControl("DropDownList1");
            TextBox tbCity = (TextBox)row.FindControl("txtCity");

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

                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    string sql = "UPDATE Employee SET Name = @Name, Gender = @Gender, City = @City WHERE ID = @ID";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@City", city);
                        cmd.Parameters.AddWithValue("@ID", employeeId);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                GridView1.EditIndex = -1;
                BindGrid();

                lblStatus.Text = string.Format("Validated & Updated Employee #{0} successfully in SQL Server!", employeeId);
                lblStatus.Visible = true;
            }
        }
    }
}
