using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class AllGridProp : System.Web.UI.Page
    {
        private const string SessionCultureTableKey = "AllGridProp_CultureEmployees";
        private const string SessionCrudTableKey = "AllGridProp_CrudEmployees";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeDataSources();
                BindAllGrids();
            }
        }

        #region Data Source Initialization

        private void InitializeDataSources()
        {
            // 1. Data for Section 1 & Section 2 (Slides 10 - 19)
            DataTable dtCulture = new DataTable();
            dtCulture.Columns.Add("EmployeeId", typeof(int));
            dtCulture.Columns.Add("FirstName", typeof(string));
            dtCulture.Columns.Add("LastName", typeof(string));
            dtCulture.Columns.Add("Salary", typeof(int));
            dtCulture.Columns.Add("AnnualSalary", typeof(int));
            dtCulture.Columns.Add("Gender", typeof(string));
            dtCulture.Columns.Add("Department", typeof(string));
            dtCulture.Columns.Add("Country", typeof(string));
            dtCulture.Columns.Add("CountryCulture", typeof(string));

            // Populate exact slide records (US, India, UK, South Africa, Malaysia)
            dtCulture.Rows.Add(1, "John", "Johnson", 55000, 55000, "Male", "IT", "US", "en-US");
            dtCulture.Rows.Add(2, "Pam", "Roberts", 62000, 62000, "Female", "HR", "India", "en-IN");
            dtCulture.Rows.Add(3, "David", "John", 45000, 45000, "Male", "IT", "UK", "en-GB");
            dtCulture.Rows.Add(4, "Rose", "Mary", 78000, 78000, "Female", "Payroll", "South Africa", "en-ZA"); // High salary > 70000 (Highlights RED)
            dtCulture.Rows.Add(5, "Mark", "Mell", 69000, 69000, "Male", "HR", "Malaysia", "en-MY");

            Session[SessionCultureTableKey] = dtCulture;

            // 2. Data for Section 3 CRUD (Slides 20 - 30)
            DataTable dtCrud = new DataTable();
            dtCrud.Columns.Add("EmployeeId", typeof(int));
            dtCrud.Columns.Add("Name", typeof(string));
            dtCrud.Columns.Add("Gender", typeof(string));
            dtCrud.Columns.Add("City", typeof(string));

            dtCrud.Rows.Add(1, "Mark Hastings", "Male", "London");
            dtCrud.Rows.Add(2, "Steve Pavez", "Male", "Sydney");
            dtCrud.Rows.Add(3, "Mary Sava", "Female", "New York");
            dtCrud.Rows.Add(4, "Valarie Vik", "Female", "Tokyo");
            dtCrud.Rows.Add(5, "John Doe", "Male", "Chicago");

            Session[SessionCrudTableKey] = dtCrud;
        }

        private DataTable GetCultureDataTable()
        {
            if (Session[SessionCultureTableKey] == null)
            {
                InitializeDataSources();
            }
            return (DataTable)Session[SessionCultureTableKey];
        }

        private DataTable GetCrudDataTable()
        {
            if (Session[SessionCrudTableKey] == null)
            {
                InitializeDataSources();
            }
            return (DataTable)Session[SessionCrudTableKey];
        }

        #endregion

        #region Grid Binding Methods

        private void BindAllGrids()
        {
            BindGridView1();
            BindGridView2();
            BindGridView3();
        }

        private void BindGridView1()
        {
            DataTable dt = GetCultureDataTable();
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        private void BindGridView2()
        {
            DataTable dt = GetCultureDataTable();
            GridView2.DataSource = dt;
            GridView2.DataBind();
        }

        private void BindGridView3()
        {
            DataTable dt = GetCrudDataTable();
            GridView3.DataSource = dt;
            GridView3.DataBind();
        }

        #endregion

        #region SECTION 1: Multi-Culture Currency Formatting (Slides 10 - 13)

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check if the row that is being bound, is a datarow
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Cell index 2 = Salary, Cell index 3 = Country
                string country = e.Row.Cells[3].Text.Trim();
                int salary = 0;
                int.TryParse(e.Row.Cells[2].Text.Trim(), out salary);

                string formattedString = string.Empty;

                if (country == "US")
                {
                    formattedString = string.Format(new CultureInfo("en-US"), "{0:c}", salary);
                }
                else if (country == "UK")
                {
                    formattedString = string.Format(new CultureInfo("en-GB"), "{0:c}", salary);
                }
                else if (country == "India")
                {
                    formattedString = string.Format(new CultureInfo("en-IN"), "{0:c}", salary);
                }
                else if (country == "South Africa")
                {
                    formattedString = string.Format(new CultureInfo("en-ZA"), "{0:c}", salary);
                }
                else if (country == "Malaysia")
                {
                    formattedString = string.Format(new CultureInfo("en-MY"), "{0:c}", salary);
                }
                else
                {
                    formattedString = string.Format(CultureInfo.InvariantCulture, "{0:c}", salary);
                }

                e.Row.Cells[2].Text = formattedString;
            }
        }

        #endregion

        #region SECTION 2: Hidden Culture Column & Conditional Highlight (Slides 14 - 19)

        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // If the row being databound is a header row
            if (e.Row.RowType == DataControlRowType.Header)
            {
                // Hide CountryCulture header table cell (Cell index 4)
                e.Row.Cells[4].Visible = false;
            }
            // If the row being databound is a data row
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Retrieve unformatted salary from cell index 2
                int salary = 0;
                int.TryParse(e.Row.Cells[2].Text.Trim(), out salary);

                // Retrieve the culture code from cell index 4
                string countryCulture = e.Row.Cells[4].Text.Trim();

                // Turn off visibility of CountryCulture cell
                e.Row.Cells[4].Visible = false;

                // Format the currency using specific country culture
                try
                {
                    string formattedString = string.Format(new CultureInfo(countryCulture), "{0:c}", salary);
                    e.Row.Cells[2].Text = formattedString;
                }
                catch
                {
                    e.Row.Cells[2].Text = salary.ToString("c");
                }

                // Conditional row formatting: If Salary > 70000, set RED background and WHITE text
                if (salary > 70000)
                {
                    e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#e11d48");
                    e.Row.ForeColor = System.Drawing.Color.White;
                    e.Row.Font.Bold = true;
                }
            }
        }

        #endregion

        #region SECTION 3: Inline CRUD Operations & Event Handlers (Slides 20 - 30)

        protected void GridView3_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView3.EditIndex = e.NewEditIndex;
            BindGridView3();
            lblCrudStatus.Text = "&#9998; Editing record at row #" + (e.NewEditIndex + 1);
            lblCrudStatus.Visible = true;
        }

        protected void GridView3_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView3.EditIndex = -1;
            BindGridView3();
            lblCrudStatus.Text = "&#10006; Edit mode canceled.";
            lblCrudStatus.Visible = true;
        }

        protected void GridView3_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int employeeId = Convert.ToInt32(GridView3.DataKeys[e.RowIndex].Value);
            GridViewRow row = GridView3.Rows[e.RowIndex];

            TextBox tbName = (TextBox)row.FindControl("TextBox1");
            DropDownList ddlGender = (DropDownList)row.FindControl("DropDownList1");
            TextBox tbCity = (TextBox)row.FindControl("TextBox3");

            if (tbName != null && ddlGender != null && tbCity != null)
            {
                string name = tbName.Text.Trim();
                string gender = ddlGender.SelectedValue;
                string city = tbCity.Text.Trim();

                // Validate selection
                if (gender == "Select Gender" || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(city))
                {
                    e.Cancel = true;
                    return;
                }

                DataTable dt = GetCrudDataTable();
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

                Session[SessionCrudTableKey] = dt;
                GridView3.EditIndex = -1;
                BindGridView3();

                lblCrudStatus.Text = string.Format("&#10004; Employee ID {0} ('{1}') updated successfully!", employeeId, name);
                lblCrudStatus.Visible = true;
            }
        }

        protected void GridView3_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int employeeId = Convert.ToInt32(GridView3.DataKeys[e.RowIndex].Value);
            DataTable dt = GetCrudDataTable();

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (Convert.ToInt32(dt.Rows[i]["EmployeeId"]) == employeeId)
                {
                    dt.Rows.RemoveAt(i);
                    break;
                }
            }

            Session[SessionCrudTableKey] = dt;
            GridView3.EditIndex = -1;
            BindGridView3();

            lblCrudStatus.Text = string.Format("&#128465; Employee ID {0} deleted successfully!", employeeId);
            lblCrudStatus.Visible = true;
        }

        protected void lbInsert_Click(object sender, EventArgs e)
        {
            if (GridView3.FooterRow == null) return;

            TextBox txtName = (TextBox)GridView3.FooterRow.FindControl("txtName");
            DropDownList ddlInsertGender = (DropDownList)GridView3.FooterRow.FindControl("ddlInsertGender");
            TextBox txtCity = (TextBox)GridView3.FooterRow.FindControl("txtCity");

            if (txtName != null && ddlInsertGender != null && txtCity != null)
            {
                string name = txtName.Text.Trim();
                string gender = ddlInsertGender.SelectedValue;
                string city = txtCity.Text.Trim();

                if (gender == "Select Gender" || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(city))
                {
                    return;
                }

                DataTable dt = GetCrudDataTable();

                // Calculate next ID
                int maxId = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    int currentId = Convert.ToInt32(dr["EmployeeId"]);
                    if (currentId > maxId) maxId = currentId;
                }
                int newId = maxId + 1;

                dt.Rows.Add(newId, name, gender, city);
                Session[SessionCrudTableKey] = dt;

                BindGridView3();

                lblCrudStatus.Text = string.Format("&#10004; New Employee '{0}' ({1}, {2}) inserted successfully with ID #{3}!", name, gender, city, newId);
                lblCrudStatus.Visible = true;
            }
        }

        #endregion

        #region Reset Demo Records

        protected void btnResetData_Click(object sender, EventArgs e)
        {
            InitializeDataSources();
            GridView3.EditIndex = -1;
            BindAllGrids();

            lblTopMessage.Text = "&#10004; Demo data reset successfully to slide defaults!";
            lblTopMessage.Visible = true;
            lblCrudStatus.Visible = false;
        }

        #endregion
    }
}