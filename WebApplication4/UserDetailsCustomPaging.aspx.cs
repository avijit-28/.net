using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class UserDetailsCustomPaging : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Visible = false;
                BindPagedData(0);
            }
        }

        /// <summary>
        /// Binds a specific page of user data using stored procedure spGetUsers_by_PageIndex_and_PageSize.
        /// </summary>
        private void BindPagedData(int pageIndex)
        {
            try
            {
                int pageSize = GridView1.PageSize;
                int totalRows = 0;

                GridView1.PageIndex = pageIndex;
                List<UserDetails> users = UserDetailsAccessLayer.GetAllUsers(pageIndex, pageSize, out totalRows);
                GridView1.DataSource = users;
                GridView1.DataBind();

                DatabindRepeater(pageIndex, pageSize, totalRows);
                UpdateDiagnosticMetrics(pageIndex, pageSize, totalRows);
            }
            catch (Exception ex)
            {
                ShowStatus("Error loading page data from SQL Server: " + ex.Message, isError: true);
            }
        }

        /// <summary>
        /// Generates page navigation items for the custom Repeater pager control.
        /// </summary>
        private void DatabindRepeater(int pageIndex, int pageSize, int totalRows)
        {
            int totalPages = totalRows / pageSize;
            if ((totalRows % pageSize) != 0)
            {
                totalPages += 1;
            }

            if (totalPages == 0) totalPages = 1;

            List<ListItem> pages = new List<ListItem>();
            if (totalPages > 1)
            {
                for (int i = 1; i <= totalPages; i++)
                {
                    // Enabled is false for the active current page so it highlights as selected
                    pages.Add(new ListItem(i.ToString(), i.ToString(), i != (pageIndex + 1)));
                }
            }
            else
            {
                pages.Add(new ListItem("1", "1", false));
            }

            repeaterPaging.DataSource = pages;
            repeaterPaging.DataBind();
        }

        /// <summary>
        /// Updates the top metrics dashboard cards and bottom paging summary label.
        /// </summary>
        private void UpdateDiagnosticMetrics(int pageIndex, int pageSize, int totalRows)
        {
            int totalPages = totalRows / pageSize;
            if ((totalRows % pageSize) != 0)
            {
                totalPages += 1;
            }
            if (totalPages == 0) totalPages = 1;

            lblTotalUsers.Text = totalRows.ToString("N0");
            lblCurrentPage.Text = (pageIndex + 1).ToString();
            lblTotalPages.Text = totalPages.ToString();
            lblPageSize.Text = pageSize.ToString();

            int startRow = totalRows == 0 ? 0 : (pageIndex * pageSize) + 1;
            int endRow = Math.Min((pageIndex + 1) * pageSize, totalRows);
            lblPagingSummary.Text = string.Format("Showing records <strong>{0}</strong> to <strong>{1}</strong> of <strong>{2}</strong> total users (Page {3} of {4})",
                startRow, endRow, totalRows, pageIndex + 1, totalPages);
        }

        /// <summary>
        /// Handles click on numbered page buttons in the Repeater pager.
        /// </summary>
        protected void linkButton_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            if (btn != null && int.TryParse(btn.CommandArgument, out int targetPage))
            {
                int targetPageIndex = targetPage - 1;
                GridView1.EditIndex = -1; // Reset any active edit mode
                BindPagedData(targetPageIndex);
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                object userIdObj = DataBinder.Eval(e.Row.DataItem, "UserId");
                object userNameObj = DataBinder.Eval(e.Row.DataItem, "UserName");

                string userId = userIdObj != null ? userIdObj.ToString() : string.Empty;
                string userName = userNameObj != null ? userNameObj.ToString().Replace("'", "\\'") : "this user";

                // Attach confirmation modal prompts
                LinkButton lbDelete = e.Row.FindControl("lbDelete") as LinkButton;
                if (lbDelete != null)
                {
                    lbDelete.OnClientClick = string.Format("return confirm('Are you sure you want to DELETE user \"{0}\" (ID #{1})?\\n\\nThis action will remove the record from SQL Server database.');", userName, userId);
                }

                LinkButton lbUpdate = e.Row.FindControl("lbUpdate") as LinkButton;
                if (lbUpdate != null)
                {
                    lbUpdate.OnClientClick = string.Format("return confirm('Are you sure you want to SAVE updates to user \"{0}\" (ID #{1})?');", userName, userId);
                }
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int currentPageIndex = GridView1.PageIndex;

            if (e.CommandName == "EditRow")
            {
                int rowIndex = ((GridViewRow)((LinkButton)e.CommandSource).NamingContainer).RowIndex;
                GridView1.EditIndex = rowIndex;
                BindPagedData(currentPageIndex);
                ShowStatus("Editing row " + (rowIndex + 1) + ". Modify fields and click Update to save.", isError: false);
            }
            else if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int userId = Convert.ToInt32(e.CommandArgument);
                    UserDetailsAccessLayer.DeleteUser(userId);
                    BindPagedData(currentPageIndex);
                    ShowStatus(string.Format("User ID #{0} deleted successfully from SQL Server database!", userId), isError: false);
                }
                catch (Exception ex)
                {
                    ShowStatus("Delete failed: " + ex.Message, isError: true);
                }
            }
            else if (e.CommandName == "CancelUpdate")
            {
                GridView1.EditIndex = -1;
                BindPagedData(currentPageIndex);
                ShowStatus("Edit canceled.", isError: false);
            }
            else if (e.CommandName == "UpdateRow")
            {
                try
                {
                    int rowIndex = ((GridViewRow)((LinkButton)e.CommandSource).NamingContainer).RowIndex;
                    int userId = Convert.ToInt32(e.CommandArgument);

                    TextBox txtUserName = (TextBox)GridView1.Rows[rowIndex].FindControl("txtUserName");
                    TextBox txtPassword = (TextBox)GridView1.Rows[rowIndex].FindControl("txtPassword");
                    TextBox txtEmail = (TextBox)GridView1.Rows[rowIndex].FindControl("txtEmail");
                    TextBox txtMobileNo = (TextBox)GridView1.Rows[rowIndex].FindControl("txtMobileNo");
                    TextBox txtCity = (TextBox)GridView1.Rows[rowIndex].FindControl("txtCity");

                    string userName = txtUserName != null ? txtUserName.Text.Trim() : string.Empty;
                    string password = txtPassword != null ? txtPassword.Text.Trim() : string.Empty;
                    string email = txtEmail != null ? txtEmail.Text.Trim() : string.Empty;
                    string mobileNo = txtMobileNo != null ? txtMobileNo.Text.Trim() : string.Empty;
                    string city = txtCity != null ? txtCity.Text.Trim() : string.Empty;

                    if (string.IsNullOrWhiteSpace(userName))
                    {
                        ShowStatus("User Name cannot be empty.", isError: true);
                        return;
                    }

                    UserDetailsAccessLayer.UpdateUser(userId, userName, password, email, mobileNo, city);

                    GridView1.EditIndex = -1;
                    BindPagedData(currentPageIndex);
                    ShowStatus(string.Format("User '{0}' (ID #{1}) updated successfully in SQL Server database!", userName, userId), isError: false);
                }
                catch (Exception ex)
                {
                    ShowStatus("Update failed: " + ex.Message, isError: true);
                }
            }
            else if (e.CommandName == "InsertRow")
            {
                if (GridView1.FooterRow == null) return;

                TextBox txtUserId = (TextBox)GridView1.FooterRow.FindControl("txtUserId");
                TextBox txtUserNameFooter = (TextBox)GridView1.FooterRow.FindControl("txtUserNameFooter");
                TextBox txtPasswordFooter = (TextBox)GridView1.FooterRow.FindControl("txtPasswordFooter");
                TextBox txtEmailFooter = (TextBox)GridView1.FooterRow.FindControl("txtEmailFooter");
                TextBox txtMobileNoFooter = (TextBox)GridView1.FooterRow.FindControl("txtMobileNoFooter");
                TextBox txtCityFooter = (TextBox)GridView1.FooterRow.FindControl("txtCityFooter");

                string userName = txtUserNameFooter != null ? txtUserNameFooter.Text.Trim() : string.Empty;
                string password = txtPasswordFooter != null ? txtPasswordFooter.Text.Trim() : string.Empty;
                string email = txtEmailFooter != null ? txtEmailFooter.Text.Trim() : string.Empty;
                string mobileNo = txtMobileNoFooter != null ? txtMobileNoFooter.Text.Trim() : string.Empty;
                string city = txtCityFooter != null ? txtCityFooter.Text.Trim() : string.Empty;

                if (string.IsNullOrWhiteSpace(userName))
                {
                    ShowStatus("User Name is required to add a new user.", isError: true);
                    return;
                }

                int userId = 0;
                if (txtUserId != null && !string.IsNullOrWhiteSpace(txtUserId.Text))
                {
                    if (!int.TryParse(txtUserId.Text.Trim(), out userId) || userId <= 0)
                    {
                        ShowStatus("Please enter a valid positive numeric User ID, or leave blank to auto-generate.", isError: true);
                        return;
                    }
                }

                try
                {
                    UserDetailsAccessLayer.InsertUser(userId, userName, password, email, mobileNo, city);
                    BindPagedData(currentPageIndex);
                    ShowStatus(string.Format("New user '{0}' added successfully to SQL Server database!", userName), isError: false);
                }
                catch (Exception ex)
                {
                    ShowStatus("Insert failed: " + ex.Message, isError: true);
                }
            }
        }

        private void ShowStatus(string message, bool isError)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = isError ? "status-msg status-msg-error" : "status-msg";
            lblStatus.Visible = true;
        }
    }
}
