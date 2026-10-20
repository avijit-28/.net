using EmployeeExpenseTracker.Models; // Model class namespace
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EmployeeExpenseTracker
{
    public partial class Expenses : System.Web.UI.Page
    {
        private readonly string _conStr=ConfigurationManager.ConnectionStrings["FleetDbConn"].ConnectionString;
        // The LifeCycle Check
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindExpensesGrid();
            }
        }

        private void BindExpensesGrid(string searchName = "", string category = "")
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string query = "SELECT Id, EmployeeName, Category, Amount, ExpenseDate, PaymentMethod, HasReceipt " +
                    "FROM Expenses  WHERE (@SearchName = '' OR EmployeeName LIKE '%' + @SearchName + '%')\r\n                           AND (@Category = '' OR Category = @Category)\r\n                         ORDER BY Id DESC";


                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SearchName", searchName.Trim());
                    cmd.Parameters.AddWithValue("@Category", category);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        // 1. GridView ko bind karein
                        gvExpenses.DataSource = dt;
                        gvExpenses.DataBind();

                        // 2. Summary cards calculate karein
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
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindExpensesGrid(txtSearchName.Text.Trim(), ddlFilterCategory.SelectedValue);
        }

        // Binding & Summary Calculations using LINQ

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }
            int expenseId = Convert.ToInt32(hfExpenseId.Value);

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string query;
                if (expenseId == 0)
                {
                     query = @"INSERT INTO Expenses (EmployeeName, Category, Amount, ExpenseDate, PaymentMethod, HasReceipt)
                      VALUES (@EmployeeName, @Category, @Amount, @ExpenseDate, @PaymentMethod, @HasReceipt)";


                }
                else
                {
                     query = @"UPDATE Expenses 
                      SET EmployeeName = @EmployeeName,
                          Category = @Category,
                          Amount = @Amount,
                          ExpenseDate = @ExpenseDate,
                          PaymentMethod = @PaymentMethod,
                          HasReceipt = @HasReceipt
                      WHERE Id = @Id";
                }
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeName", txtEmployeeName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue);
                    cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(txtAmount.Text));
                    cmd.Parameters.AddWithValue("@ExpenseDate", Convert.ToDateTime(txtDate.Text));
                    cmd.Parameters.AddWithValue("@PaymentMethod", rblPaymentMethod.SelectedValue);
                    cmd.Parameters.AddWithValue("@HasReceipt", chkHasReceipt.Checked);

                    if (expenseId > 0)
                    {
                        cmd.Parameters.AddWithValue("@Id", expenseId);
                    }
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                
            }

            BindExpensesGrid();
            //btnClear_Click();
            ClearForm();
        }
        private void ClearForm()
        {
            // 1. TextBoxes empty karein
            txtEmployeeName.Text = string.Empty;
            txtAmount.Text = string.Empty;
            txtDate.Text = string.Empty;

            // 2. Dropdown reset karein
            ddlCategory.ClearSelection();
            ddlCategory.SelectedIndex = 0; // "-- Select Category --" par set karega

            // 3. Payment method reset karein
            rblPaymentMethod.ClearSelection();
            rblPaymentMethod.SelectedIndex = 0; // "Corporate Card" default select karega

            // 4. Receipt checkbox uncheck karein
            chkHasReceipt.Checked = false;

            // 5. Hidden field ko wapas 0 karein (taki next click par naya add ho, update na ho)
            hfExpenseId.Value = "0";

            // 6. Button aur Title ko default "Add" mode me wapas le aayein
            btnSubmit.Text = "Submit Expense";
            btnSubmit.CssClass = "btn btn-primary flex-grow-1";
            lblFormTitle.Text = "➕ Add New Expense";
        }
        //protected void btnClear_Click(object sender, EventArgs e)
        //{
        //    ClearForm();
        //}
        // Form reset helper method
        protected void btnResetFilter_Click(object sender, EventArgs e)
        {
            txtSearchName.Text = string.Empty;
            ddlFilterCategory.SelectedIndex = 0;
            BindExpensesGrid(); // Poora data wapas laye
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            hfExpenseId.Value = "0"; // Reset to add mode
            txtEmployeeName.Text = string.Empty;
            ddlCategory.SelectedIndex = 0;
            txtAmount.Text = string.Empty;
            txtDate.Text = string.Empty;
            rblPaymentMethod.SelectedIndex = 0;
            chkHasReceipt.Checked = false;
            // Reset button & title to default
            btnSubmit.Text = "Submit Expense";
            btnSubmit.CssClass = "btn btn-primary flex-grow-1";
            lblFormTitle.Text = "➕ Add New Expense";
        }
        protected void gvExpenses_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvExpenses.DataKeys[e.RowIndex].Value);

            string query = "DELETE FROM Expenses WHERE Id = @Id";

            using (SqlConnection con = new SqlConnection(_conStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindExpensesGrid();
        }

        protected void gvExpenses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRow")
            {
                int expenseId = Convert.ToInt32(e.CommandArgument);
                // Database se selected expense ka data le aayein
                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    string query = "SELECT * FROM Expenses WHERE Id = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Id", expenseId);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Form fields me data bhar dein
                                hfExpenseId.Value = reader["Id"].ToString();
                                txtEmployeeName.Text = reader["EmployeeName"].ToString();
                                ddlCategory.SelectedValue = reader["Category"].ToString();
                                txtAmount.Text = Convert.ToDecimal(reader["Amount"]).ToString("0.00");
                                txtDate.Text = Convert.ToDateTime(reader["ExpenseDate"]).ToString("yyyy-MM-dd");
                                rblPaymentMethod.SelectedValue = reader["PaymentMethod"].ToString();
                                chkHasReceipt.Checked = Convert.ToBoolean(reader["HasReceipt"]);
                                // Button aur title ko Edit mode me change karein
                                btnSubmit.Text = "Update Expense";
                                btnSubmit.CssClass = "btn btn-success flex-grow-1";
                                lblFormTitle.Text = "✏️ Edit Expense (ID #" + expenseId + ")";
                            }
                        }
                    }
                }
            }
            
        }

    }
}