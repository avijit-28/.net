using System;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class AllGridProp : System.Web.UI.Page
    {
        private const string SessionTableKey = "AllGridProp_EmployeeTable";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        #region Data Source & Binding

        private DataTable GetEmployeeTable()
        {
            if (Session[SessionTableKey] == null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("EmployeeId", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Gender", typeof(string));
                dt.Columns.Add("City", typeof(string));
                dt.Columns.Add("AnnualSalary", typeof(int));
                dt.Columns.Add("Country", typeof(string));
                dt.Columns.Add("CountryCulture", typeof(string));

                // Sample data from slides
                dt.Rows.Add(1, "John Johnson", "Male", "New York", 55000, "US", "en-US");
                dt.Rows.Add(2, "Pam Roberts", "Female", "Mumbai", 62000, "India", "en-IN");
                dt.Rows.Add(3, "David John", "Male", "London", 45000, "UK", "en-GB");
                dt.Rows.Add(4, "Rose Mary", "Female", "Cape Town", 78000, "South Africa", "en-ZA"); // Salary > 70000 -> Red highlight
                dt.Rows.Add(5, "Mark Mell", "Male", "Kuala Lumpur", 69000, "Malaysia", "en-MY");

                Session[SessionTableKey] = dt;
            }

            return (DataTable)Session[SessionTableKey];
        }

        private void BindGrid()
        {
            GridView1.DataSource = GetEmployeeTable();
            GridView1.DataBind();
        }

        #endregion

        #region RowDataBound: Multi-Culture Currency, Hidden Column & Red Highlighting (Pages 10 - 19)

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Column 7 is CountryCulture. Hide it on Header, DataRow, and Footer (Pages 14 - 16)
            if (e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.Footer)
            {
                if (e.Row.Cells.Count > 7)
                {
                    e.Row.Cells[7].Visible = false;
                }
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells.Count > 7)
                {
                    e.Row.Cells[7].Visible = false;
                }

                DataRowView drv = e.Row.DataItem as DataRowView;
                if (drv != null)
                {
                    int salary = Convert.ToInt32(drv["AnnualSalary"]);
                    string culture = drv["CountryCulture"].ToString();

                    // Format AnnualSalary with country specific currency (Pages 10 - 16)
                    Label lblSalary = (Label)e.Row.FindControl("LabelSalary");
                    if (lblSalary != null)
                    {
                        try
                        {
                            lblSalary.Text = string.Format(new CultureInfo(culture), "{0:c}", salary);
                        }
                        catch
                        {
                            lblSalary.Text = salary.ToString("c");
                        }
                    }

                    // Conditional Highlighting: If Salary > 70000, set Red background and White text (Pages 17 - 19)
                    if (salary > 70000)
                    {
                        e.Row.BackColor = System.Drawing.Color.Red;
                        e.Row.ForeColor = System.Drawing.Color.White;
                        e.Row.Font.Bold = true;
                    }
                }
            }
        }

        #endregion

        #region Inline Editing, Updating, and Deleting (Pages 20 - 30)

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            BindGrid();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int employeeId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);
            GridViewRow row = GridView1.Rows[e.RowIndex];

            TextBox tbName = (TextBox)row.FindControl("txtNameEdit");
            DropDownList ddlGender = (DropDownList)row.FindControl("DropDownList1");
            TextBox tbCity = (TextBox)row.FindControl("txtCityEdit");
            TextBox tbSalary = (TextBox)row.FindControl("txtSalaryEdit");
            TextBox tbCountry = (TextBox)row.FindControl("txtCountryEdit");

            if (tbName != null && ddlGender != null && tbCity != null && tbSalary != null)
            {
                string name = tbName.Text.Trim();
                string gender = ddlGender.SelectedValue;
                string city = tbCity.Text.Trim();
                int salary = 0;
                int.TryParse(tbSalary.Text.Trim(), out salary);
                string country = tbCountry != null ? tbCountry.Text.Trim() : "US";
                string culture = ResolveCulture(country);

                DataTable dt = GetEmployeeTable();
                foreach (DataRow dr in dt.Rows)
                {
                    if (Convert.ToInt32(dr["EmployeeId"]) == employeeId)
                    {
                        dr["Name"] = name;
                        dr["Gender"] = gender;
                        dr["City"] = city;
                        dr["AnnualSalary"] = salary;
                        dr["Country"] = country;
                        dr["CountryCulture"] = culture;
                        break;
                    }
                }

                Session[SessionTableKey] = dt;
                GridView1.EditIndex = -1;
                BindGrid();

                lblMessage.Text = "Employee updated successfully!";
                lblMessage.Visible = true;
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int employeeId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);
            DataTable dt = GetEmployeeTable();

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (Convert.ToInt32(dt.Rows[i]["EmployeeId"]) == employeeId)
                {
                    dt.Rows.RemoveAt(i);
                    break;
                }
            }

            Session[SessionTableKey] = dt;
            GridView1.EditIndex = -1;
            BindGrid();

            lblMessage.Text = "Employee deleted successfully!";
            lblMessage.Visible = true;
        }

        #endregion

        #region Inline Insert via FooterTemplate (Pages 27 - 30)

        protected void lbInsert_Click(object sender, EventArgs e)
        {
            if (GridView1.FooterRow == null) return;

            TextBox txtName = (TextBox)GridView1.FooterRow.FindControl("txtName");
            DropDownList ddlInsertGender = (DropDownList)GridView1.FooterRow.FindControl("ddlInsertGender");
            TextBox txtCity = (TextBox)GridView1.FooterRow.FindControl("txtCity");
            TextBox txtSalary = (TextBox)GridView1.FooterRow.FindControl("txtSalary");
            TextBox txtCountry = (TextBox)GridView1.FooterRow.FindControl("txtCountry");

            if (txtName != null && ddlInsertGender != null && txtCity != null && txtSalary != null)
            {
                string name = txtName.Text.Trim();
                string gender = ddlInsertGender.SelectedValue;
                string city = txtCity.Text.Trim();
                int salary = 0;
                int.TryParse(txtSalary.Text.Trim(), out salary);
                string country = (txtCountry != null && !string.IsNullOrEmpty(txtCountry.Text.Trim())) ? txtCountry.Text.Trim() : "US";
                string culture = ResolveCulture(country);

                DataTable dt = GetEmployeeTable();

                int maxId = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    int currentId = Convert.ToInt32(dr["EmployeeId"]);
                    if (currentId > maxId) maxId = currentId;
                }
                int newId = maxId + 1;

                dt.Rows.Add(newId, name, gender, city, salary, country, culture);
                Session[SessionTableKey] = dt;

                BindGrid();

                lblMessage.Text = "New employee inserted successfully!";
                lblMessage.Visible = true;
            }
        }

        private string ResolveCulture(string country)
        {
            if (string.Equals(country, "UK", StringComparison.OrdinalIgnoreCase)) return "en-GB";
            if (string.Equals(country, "India", StringComparison.OrdinalIgnoreCase)) return "en-IN";
            if (string.Equals(country, "South Africa", StringComparison.OrdinalIgnoreCase)) return "en-ZA";
            if (string.Equals(country, "Malaysia", StringComparison.OrdinalIgnoreCase)) return "en-MY";
            return "en-US";
        }

        #endregion
    }
}