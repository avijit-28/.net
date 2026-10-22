using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class UserDetailsPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Visible = false;
            }
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

                // Attach confirmation popup to the Delete LinkButton
                LinkButton btnDelete = e.Row.FindControl("btnDelete") as LinkButton;
                if (btnDelete != null)
                {
                    btnDelete.OnClientClick = string.Format("return confirm('Are you sure you want to DELETE user \"{0}\" (ID #{1})?\\n\\nThis action will remove the record from SQL Server database.');", userName, userId);
                }

                // Attach confirmation popup to the Update LinkButton (visible in Edit mode)
                LinkButton btnUpdate = e.Row.FindControl("btnUpdate") as LinkButton;
                if (btnUpdate != null)
                {
                    btnUpdate.OnClientClick = string.Format("return confirm('Are you sure you want to SAVE updates to user \"{0}\" (ID #{1})?');", userName, userId);
                }
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AddNew")
            {
                TextBox txtUserId = GridView1.FooterRow?.FindControl("txtUserId") as TextBox;
                TextBox txtUserName = GridView1.FooterRow?.FindControl("txtUserName") as TextBox;
                TextBox txtPassword = GridView1.FooterRow?.FindControl("txtPassword") as TextBox;
                TextBox txtEmail = GridView1.FooterRow?.FindControl("txtEmail") as TextBox;
                TextBox txtMobileNo = GridView1.FooterRow?.FindControl("txtMobileNo") as TextBox;
                TextBox txtCity = GridView1.FooterRow?.FindControl("txtCity") as TextBox;

                string userName = txtUserName?.Text?.Trim() ?? string.Empty;
                string password = txtPassword?.Text?.Trim() ?? string.Empty;
                string email = txtEmail?.Text?.Trim() ?? string.Empty;
                string mobileNo = txtMobileNo?.Text?.Trim() ?? string.Empty;
                string city = txtCity?.Text?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(userName))
                {
                    ShowStatus("User Name is required to insert a new user.", isError: true);
                    return;
                }

                int userId = 0;
                if (txtUserId != null && !string.IsNullOrWhiteSpace(txtUserId.Text))
                {
                    if (!int.TryParse(txtUserId.Text.Trim(), out userId) || userId <= 0)
                    {
                        ShowStatus("Please enter a valid numeric User ID greater than 0, or leave blank to auto-generate.", isError: true);
                        return;
                    }
                }

                try
                {
                    ObjectDataSource1.InsertParameters["UserId"].DefaultValue = userId.ToString();
                    ObjectDataSource1.InsertParameters["UserName"].DefaultValue = userName;
                    ObjectDataSource1.InsertParameters["Password"].DefaultValue = password;
                    ObjectDataSource1.InsertParameters["Email"].DefaultValue = email;
                    ObjectDataSource1.InsertParameters["MobileNo"].DefaultValue = mobileNo;
                    ObjectDataSource1.InsertParameters["City"].DefaultValue = city;

                    ObjectDataSource1.Insert();

                    ShowStatus(string.Format("New user '{0}' inserted successfully into SQL Server database!", userName), isError: false);
                }
                catch (Exception ex)
                {
                    ShowStatus("Error inserting user: " + ex.Message, isError: true);
                }
            }
        }

        protected void ObjectDataSource1_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                ShowStatus("Insert failed: " + e.Exception.InnerException?.Message ?? e.Exception.Message, isError: true);
                e.ExceptionHandled = true;
            }
        }

        protected void ObjectDataSource1_Updated(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                ShowStatus("Update failed: " + e.Exception.InnerException?.Message ?? e.Exception.Message, isError: true);
                e.ExceptionHandled = true;
            }
            else
            {
                ShowStatus("User details updated successfully in SQL Server database!", isError: false);
            }
        }

        protected void ObjectDataSource1_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                ShowStatus("Delete failed: " + e.Exception.InnerException?.Message ?? e.Exception.Message, isError: true);
                e.ExceptionHandled = true;
            }
            else
            {
                ShowStatus("User deleted successfully from SQL Server database!", isError: false);
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
