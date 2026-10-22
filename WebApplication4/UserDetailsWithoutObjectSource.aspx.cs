using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class UserDetailsWithoutObjectSource : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Visible = false;
                BindGridViewData();
            }
        }

        private void BindGridViewData()
        {
            try
            {
                GridView1.DataSource = UserDetailsAccessLayer.GetAllUsers();
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                ShowStatus("Error loading user data: " + ex.Message, isError: true);
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            BindGridViewData();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Retrieve user details from data item for customized confirmation messages
                object userIdObj = DataBinder.Eval(e.Row.DataItem, "UserId");
                object userNameObj = DataBinder.Eval(e.Row.DataItem, "UserName");

                string userId = userIdObj != null ? userIdObj.ToString() : string.Empty;
                string userName = userNameObj != null ? userNameObj.ToString().Replace("'", "\\'") : "this user";

                // Attach confirmation popup prompt to the Delete LinkButton
                LinkButton lbDelete = e.Row.FindControl("lbDelete") as LinkButton;
                if (lbDelete != null)
                {
                    lbDelete.OnClientClick = string.Format("return confirm('Are you sure you want to DELETE user \"{0}\" (ID #{1})?\\n\\nThis action will remove the record from SQL Server database.');", userName, userId);
                }

                // Attach confirmation popup prompt to the Update LinkButton (visible in Edit mode)
                LinkButton lbUpdate = e.Row.FindControl("lbUpdate") as LinkButton;
                if (lbUpdate != null)
                {
                    lbUpdate.OnClientClick = string.Format("return confirm('Are you sure you want to SAVE updates to user \"{0}\" (ID #{1})?');", userName, userId);
                }
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRow")
            {
                int rowIndex = ((GridViewRow)((LinkButton)e.CommandSource).NamingContainer).RowIndex;
                GridView1.EditIndex = rowIndex;
                BindGridViewData();
                ShowStatus("Editing row " + (rowIndex + 1) + ". Modify fields and click Update to save.", isError: false);
            }
            else if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int userId = Convert.ToInt32(e.CommandArgument);
                    UserDetailsAccessLayer.DeleteUser(userId);
                    BindGridViewData();
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
                BindGridViewData();
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
                    BindGridViewData();
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
                    BindGridViewData();
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
