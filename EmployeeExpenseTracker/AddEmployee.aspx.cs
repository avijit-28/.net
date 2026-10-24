using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace EmployeeExpenseTracker
{
    public partial class AddEmployee : System.Web.UI.Page
    {
        private readonly string _conStr =
            ConfigurationManager.ConnectionStrings["FleetDbConn"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlSuccess.Visible = false;
            }
        }

        // Save Employee to Database
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string name = txtName.Text.Trim();
            string designation = ddlDesignation.SelectedValue;
            decimal salary = Convert.ToDecimal(txtSalary.Text.Trim());
            DateTime dob = Convert.ToDateTime(txtDOB.Text);
            DateTime doj = Convert.ToDateTime(txtDOJ.Text);
            string address = string.IsNullOrWhiteSpace(txtAddress.Text) ? "N/A" : txtAddress.Text.Trim();

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string query = @"INSERT INTO Employees (Name, Designation, DOB, DOJ, Salary, Address) 
                                 VALUES (@Name, @Designation, @DOB, @DOJ, @Salary, @Address)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Designation", designation);
                    cmd.Parameters.AddWithValue("@DOB", dob);
                    cmd.Parameters.AddWithValue("@DOJ", doj);
                    cmd.Parameters.AddWithValue("@Salary", salary);
                    cmd.Parameters.AddWithValue("@Address", address);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Successfully inserted -> Redirect back to Employee Directory
            Response.Redirect("Employees.aspx");
        }

        // Clear Form
        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtName.Text = string.Empty;
            ddlDesignation.SelectedIndex = 0;
            txtSalary.Text = string.Empty;
            txtDOB.Text = string.Empty;
            txtDOJ.Text = string.Empty;
            txtAddress.Text = string.Empty;
            pnlSuccess.Visible = false;
        }
    }
}