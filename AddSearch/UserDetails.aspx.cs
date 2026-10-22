using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AddSearch
{
    public partial class UserDetails1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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

                if (txtUserId == null || string.IsNullOrWhiteSpace(txtUserId.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('User ID is required.');", true);
                    return;
                }

                if (!int.TryParse(txtUserId.Text.Trim(), out int userId))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('User ID must be a valid number.');", true);
                    return;
                }

                EnsureInsertParameters();

                ObjectDataSource1.InsertParameters["UserId"].DefaultValue = userId.ToString();
                ObjectDataSource1.InsertParameters["UserName"].DefaultValue = txtUserName?.Text?.Trim() ?? string.Empty;
                ObjectDataSource1.InsertParameters["Password"].DefaultValue = txtPassword?.Text?.Trim() ?? string.Empty;
                ObjectDataSource1.InsertParameters["Email"].DefaultValue = txtEmail?.Text?.Trim() ?? string.Empty;
                ObjectDataSource1.InsertParameters["MobileNo"].DefaultValue = txtMobileNo?.Text?.Trim() ?? string.Empty;
                ObjectDataSource1.InsertParameters["City"].DefaultValue = txtCity?.Text?.Trim() ?? string.Empty;

                ObjectDataSource1.Insert();
            }
        }

        private void EnsureInsertParameters()
        {
            if (ObjectDataSource1.InsertParameters["UserId"] == null)
                ObjectDataSource1.InsertParameters.Add(new Parameter("UserId", TypeCode.Int32));
            if (ObjectDataSource1.InsertParameters["UserName"] == null)
                ObjectDataSource1.InsertParameters.Add(new Parameter("UserName", TypeCode.String));
            if (ObjectDataSource1.InsertParameters["Password"] == null)
                ObjectDataSource1.InsertParameters.Add(new Parameter("Password", TypeCode.String));
            if (ObjectDataSource1.InsertParameters["Email"] == null)
                ObjectDataSource1.InsertParameters.Add(new Parameter("Email", TypeCode.String));
            if (ObjectDataSource1.InsertParameters["MobileNo"] == null)
                ObjectDataSource1.InsertParameters.Add(new Parameter("MobileNo", TypeCode.String));
            if (ObjectDataSource1.InsertParameters["City"] == null)
                ObjectDataSource1.InsertParameters.Add(new Parameter("City", TypeCode.String));
        }
    }
}