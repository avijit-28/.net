using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EmployeeExpenseTracker
{
    public partial class Expenses : System.Web.UI.Page
    {
        private readonly string _conStr =
            ConfigurationManager.ConnectionStrings["FleetDbConn"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1. Employees table se dropdown load karein
                PopulateEmployeesDropdown();

                // 2. Expenses Grid load karein
                BindExpensesGrid();
            }
        }

        // Database se Employees fetch karke DropDown me bharna
        private void PopulateEmployeesDropdown()
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string query = "SELECT Id, Name FROM Employees ORDER BY Name ASC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        ddlEmployee.Items.Clear();
                        ddlEmployee.Items.Add(new ListItem("-- Select Employee --", ""));

                        while (reader.Read())
                        {
                            int empId = Convert.ToInt32(reader["Id"]);
                            string empName = reader["Name"].ToString();

                            // Display Text: E001 - Suresh Das | Value: 1 (Primary Key of Employees)
                            string text = $"E{empId:D3} - {empName}";
                            string value = empId.ToString();

                            ddlEmployee.Items.Add(new ListItem(text, value));
                        }
                    }
                }
            }
        }

        // GridView Bind Function
        private void BindExpensesGrid(string searchName = "", string category = "")
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string query = @"SELECT ExpenseId, EmployeeId, EmployeeName, Category, Amount, ExpenseDate, PaymentMethod, HasReceipt 
                                 FROM Expenses 
                                 WHERE (@SearchName = '' OR EmployeeName LIKE '%' + @SearchName + '%')
                                   AND (@Category = '' OR Category = @Category)
                                 ORDER BY ExpenseId DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SearchName", searchName.Trim());
                    cmd.Parameters.AddWithValue("@Category", category);

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        gvExpenses.DataSource = dt;
                        gvExpenses.DataBind();

                        // Summary cards calculation
                        if (dt.Rows.Count > 0)
                        {
                            decimal totalSum = Convert.ToDecimal(dt.Compute("SUM(Amount)", string.Empty));
                            decimal maxAmount = Convert.ToDecimal(dt.Compute("MAX(Amount)", string.Empty));

                            lblTotalAmount.Text = totalSum.ToString("C");
                            lblTotalCount.Text = dt.Rows.Count.ToString();
                            lblMaxExpense.Text = maxAmount.ToString("C");
                        }
                        else
                        {
                            lblTotalAmount.Text = 0.ToString("C");
                            lblTotalCount.Text = "0";
                            lblMaxExpense.Text = 0.ToString("C");
                        }
                    }
                }
            }
        }

        // Submit (Insert or Update) Event
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int expenseId = Convert.ToInt32(hfExpenseId.Value);
            int employeeId = Convert.ToInt32(ddlEmployee.SelectedValue);

            // Extract Name from dropdown text (e.g. "E001 - Suresh Das" -> "Suresh Das")
            string fullText = ddlEmployee.SelectedItem.Text;
            string employeeName = fullText.Contains("-")
                ? fullText.Substring(fullText.IndexOf('-') + 1).Trim()
                : fullText;

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string query;

                if (expenseId == 0)
                {
                    query = @"INSERT INTO Expenses (EmployeeId, EmployeeName, Category, Amount, ExpenseDate, PaymentMethod, HasReceipt)
                              VALUES (@EmployeeId, @EmployeeName, @Category, @Amount, @ExpenseDate, @PaymentMethod, @HasReceipt)";
                }
                else
                {
                    query = @"UPDATE Expenses 
                              SET EmployeeId = @EmployeeId,
                                  EmployeeName = @EmployeeName,
                                  Category = @Category,
                                  Amount = @Amount,
                                  ExpenseDate = @ExpenseDate,
                                  PaymentMethod = @PaymentMethod,
                                  HasReceipt = @HasReceipt
                              WHERE ExpenseId = @ExpenseId";
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@EmployeeName", employeeName);
                    cmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue);
                    cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(txtAmount.Text.Trim()));
                    cmd.Parameters.AddWithValue("@ExpenseDate", Convert.ToDateTime(txtDate.Text));
                    cmd.Parameters.AddWithValue("@PaymentMethod", rblPaymentMethod.SelectedValue);
                    cmd.Parameters.AddWithValue("@HasReceipt", chkHasReceipt.Checked);

                    if (expenseId > 0)
                    {
                        cmd.Parameters.AddWithValue("@ExpenseId", expenseId);
                    }

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            BindExpensesGrid();
            ClearForm();
        }

        // Edit Row Handler
        protected void gvExpenses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRow")
            {
                int expenseId = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    string query = "SELECT * FROM Expenses WHERE ExpenseId = @ExpenseId";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ExpenseId", expenseId);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hfExpenseId.Value = reader["ExpenseId"].ToString();

                                // Set Selected Employee
                                ddlEmployee.ClearSelection();
                                var empItem = ddlEmployee.Items.FindByValue(reader["EmployeeId"].ToString());
                                if (empItem != null) empItem.Selected = true;

                                ddlCategory.SelectedValue = reader["Category"].ToString();
                                txtAmount.Text = Convert.ToDecimal(reader["Amount"]).ToString("0.00");
                                txtDate.Text = Convert.ToDateTime(reader["ExpenseDate"]).ToString("yyyy-MM-dd");
                                rblPaymentMethod.SelectedValue = reader["PaymentMethod"].ToString();
                                chkHasReceipt.Checked = Convert.ToBoolean(reader["HasReceipt"]);

                                btnSubmit.Text = "Update Expense";
                                btnSubmit.CssClass = "btn btn-success flex-grow-1";
                                lblFormTitle.Text = "✏️ Edit Expense (ID #" + expenseId + ")";
                            }
                        }
                    }
                }
            }
        }

        // Delete Row Handler
        protected void gvExpenses_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int expenseId = Convert.ToInt32(gvExpenses.DataKeys[e.RowIndex].Value);

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string query = "DELETE FROM Expenses WHERE ExpenseId = @ExpenseId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ExpenseId", expenseId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            BindExpensesGrid();
        }

        // Filter / Search Buttons
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindExpensesGrid(txtSearchName.Text.Trim(), ddlFilterCategory.SelectedValue);
        }

        protected void btnResetFilter_Click(object sender, EventArgs e)
        {
            txtSearchName.Text = string.Empty;
            ddlFilterCategory.SelectedIndex = 0;
            BindExpensesGrid();
        }

        // Clear Form Button
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        // Reset Helper
        private void ClearForm()
        {
            hfExpenseId.Value = "0";
            ddlEmployee.ClearSelection();
            ddlEmployee.SelectedIndex = 0;
            ddlCategory.ClearSelection();
            ddlCategory.SelectedIndex = 0;
            txtAmount.Text = string.Empty;
            txtDate.Text = string.Empty;
            rblPaymentMethod.ClearSelection();
            rblPaymentMethod.SelectedIndex = 0;
            chkHasReceipt.Checked = false;

            btnSubmit.Text = "Submit Expense";
            btnSubmit.CssClass = "btn btn-primary flex-grow-1";
            lblFormTitle.Text = "➕ Add New Expense";
        }
    }
}