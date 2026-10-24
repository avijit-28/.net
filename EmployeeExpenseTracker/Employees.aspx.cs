using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace EmployeeExpenseTracker
{
    public partial class Employees : System.Web.UI.Page
    {
        private readonly string _conStr =
            ConfigurationManager.ConnectionStrings["FleetDbConn"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindEmployeesGrid();
            }
        }

        // Main Binding Function with Search, Designation Filter & Dynamic Sorting
        private void BindEmployeesGrid()
        {
            string searchInput = txtSearch.Text.Trim();
            string designation = ddlDesignation.SelectedValue;
            string sortBy = ddlSortBy.SelectedValue;

            // Safe Whitelisting for ORDER BY clause
            string sortColumn;
            switch (sortBy)
            {
                case "Id":
                    sortColumn = "Id ASC";
                    break;
                case "Salary":
                    sortColumn = "Salary DESC";
                    break;
                case "DOJ":
                    sortColumn = "DOJ DESC";
                    break;
                default:
                    sortColumn = "Name ASC";
                    break;
            }

            // Check if user entered an ID search (e.g. "1" or "E001")
            int searchId = 0;
            string cleanSearch = searchInput;
            if (cleanSearch.StartsWith("E", StringComparison.OrdinalIgnoreCase))
            {
                cleanSearch = cleanSearch.Substring(1);
            }
            int.TryParse(cleanSearch, out searchId);

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                // Dynamic Filter Query
                string query = $@"
                    SELECT Id, Name, Designation, DOB, DOJ, Salary, Address 
                    FROM Employees 
                    WHERE 
                        (
                            @Search = '' 
                            OR Name LIKE '%' + @Search + '%' 
                            OR (@SearchId > 0 AND Id = @SearchId)
                        )
                        AND (@Designation = '' OR Designation = @Designation)
                    ORDER BY {sortColumn}";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Search", searchInput);
                    cmd.Parameters.AddWithValue("@SearchId", searchId);
                    cmd.Parameters.AddWithValue("@Designation", designation);

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        // Bind GridView
                        gvEmployees.DataSource = dt;
                        gvEmployees.DataBind();

                        // Calculate and Update KPI Dashboard Cards
                        if (dt.Rows.Count > 0)
                        {
                            decimal totalPayroll = Convert.ToDecimal(dt.Compute("SUM(Salary)", string.Empty));
                            decimal avgSalary = Convert.ToDecimal(dt.Compute("AVG(Salary)", string.Empty));

                            lblTotalEmployees.Text = dt.Rows.Count.ToString();
                            lblTotalPayroll.Text = "$" + totalPayroll.ToString("N2");
                            lblAvgSalary.Text = "$" + avgSalary.ToString("N2");
                        }
                        else
                        {
                            lblTotalEmployees.Text = "0";
                            lblTotalPayroll.Text = "$0.00";
                            lblAvgSalary.Text = "$0.00";
                        }
                    }
                }
            }
        }

        // Search Button Click
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindEmployeesGrid();
        }

        // Auto-Triggered on Designation or Sorted By Dropdown Change
        protected void FilterChanged(object sender, EventArgs e)
        {
            BindEmployeesGrid();
        }

        // Reset Button Click
        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            ddlDesignation.SelectedIndex = 0;
            ddlSortBy.SelectedValue = "Name";
            BindEmployeesGrid();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddEmployee.aspx");
        }

        // Helper: Generates initials for employee avatar (e.g. 'Ranjit Das' -> 'RD')
        public string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "EM";

            string[] parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();

            return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
        }

        // Helper: Distinct colorful badges for different designations
        public string GetRoleBadgeClass(string role)
        {
            switch (role?.Trim())
            {
                case "Manager":
                    return "bg-primary text-white";
                case "Project Manager":
                    return "bg-info text-dark";
                case "Team Leader":
                    return "bg-warning text-dark";
                default:
                    return "bg-secondary text-white";
            }
        }
    }
}