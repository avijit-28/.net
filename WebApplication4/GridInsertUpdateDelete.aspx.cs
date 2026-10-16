using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class GridInsertUpdateDelete : System.Web.UI.Page
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

                lblStatus.Text = string.Format("Employee ID #{0} updated successfully in SQL Server!", employeeId);
                lblStatus.Visible = true;
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int employeeId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                string sql = "DELETE FROM Employee WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@ID", employeeId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            GridView1.EditIndex = -1;
            BindGrid();

            lblStatus.Text = string.Format("Employee ID #{0} deleted successfully from SQL Server!", employeeId);
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

                int newId = 0;
                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    con.Open();

                    // Generate next available ID
                    using (SqlCommand cmdMax = new SqlCommand("SELECT ISNULL(MAX(ID), 0) + 1 FROM Employee", con))
                    {
                        newId = Convert.ToInt32(cmdMax.ExecuteScalar());
                    }

                    // Insert the new record into SQL Server Employee table
                    string sqlInsert = "INSERT INTO Employee (ID, Name, Salary, Gender, City, Dept) VALUES (@ID, @Name, 25000, @Gender, @City, 'General')";
                    using (SqlCommand cmdInsert = new SqlCommand(sqlInsert, con))
                    {
                        cmdInsert.Parameters.AddWithValue("@ID", newId);
                        cmdInsert.Parameters.AddWithValue("@Name", name);
                        cmdInsert.Parameters.AddWithValue("@Gender", gender);
                        cmdInsert.Parameters.AddWithValue("@City", city);

                        cmdInsert.ExecuteNonQuery();
                    }
                }

                BindGrid();

                lblStatus.Text = string.Format("New Employee '{0}' inserted successfully into SQL Server with ID #{1}!", name, newId);
                lblStatus.Visible = true;
            }
        }
    }
}
