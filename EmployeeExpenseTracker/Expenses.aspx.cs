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

        private void BindExpensesGrid()
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string query = "SELECT Id, EmployeeName, Category, Amount, ExpenseDate, PaymentMethod, HasReceipt FROM Expenses ORDER BY Id DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
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

        // Binding & Summary Calculations using LINQ

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            string query = @"
        INSERT INTO Expenses
        (
            EmployeeName,
            Category,
            Amount,
            ExpenseDate,
            PaymentMethod,
            HasReceipt
        )
        VALUES
        (
            @EmployeeName,
            @Category,
            @Amount,
            @ExpenseDate,
            @PaymentMethod,
            @HasReceipt
        )";

            using (SqlConnection con = new SqlConnection(_conStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@EmployeeName", txtEmployeeName.Text.Trim());
                cmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue);
                cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(txtAmount.Text));
                cmd.Parameters.AddWithValue("@ExpenseDate", Convert.ToDateTime(txtDate.Text));
                cmd.Parameters.AddWithValue("@PaymentMethod", rblPaymentMethod.SelectedValue);
                cmd.Parameters.AddWithValue("@HasReceipt", chkHasReceipt.Checked);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindExpensesGrid();
            //ClearForm();
        }
        //protected void btnClear_Click(object sender, EventArgs e)
        //{
        //    ClearForm();
        //}
        // Form reset helper method
        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtEmployeeName.Text = string.Empty;
            ddlCategory.SelectedIndex = 0;
            txtAmount.Text = string.Empty;
            txtDate.Text = string.Empty;
            rblPaymentMethod.SelectedIndex = 0;
            chkHasReceipt.Checked = false;
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


        
    }
}