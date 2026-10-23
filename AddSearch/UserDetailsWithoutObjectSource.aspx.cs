using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AddSearch
{
    public partial class UserDetailsWithoutObjectSource : System.Web.UI.Page
    {
        private const int PageSize = 5;

        private int CurrentPageIndex
        {
            get => ViewState["CurrentPageIndex"] != null ? (int)ViewState["CurrentPageIndex"] : 0;
            set => ViewState["CurrentPageIndex"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CurrentPageIndex = 0;
                BindGridViewData(0);
            }
        }

        private void BindGridViewData(int pageIndex)
        {
            int totalRows = 0;
            List<UserDetails> users = UserDetailsAccessLayer.GetAllUsers(pageIndex, PageSize, out totalRows);

            int totalPages = totalRows / PageSize;
            if ((totalRows % PageSize) != 0) totalPages += 1;
            if (totalPages == 0) totalPages = 1;

            // Clamping if deleting the last row of the current page
            if (pageIndex >= totalPages && totalPages > 0)
            {
                pageIndex = totalPages - 1;
                CurrentPageIndex = pageIndex;
                users = UserDetailsAccessLayer.GetAllUsers(pageIndex, PageSize, out totalRows);
            }

            GridView1.DataSource = users;
            GridView1.DataBind();

            DatabindRepeater(pageIndex, PageSize, totalRows);
        }

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

        protected void linkButton_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            if (btn != null && int.TryParse(btn.CommandArgument, out int targetPage))
            {
                int targetPageIndex = targetPage - 1;
                CurrentPageIndex = targetPageIndex;
                GridView1.EditIndex = -1;
                BindGridViewData(targetPageIndex);
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRow")
            {
                int rowIndex = ((GridViewRow)((LinkButton)e.CommandSource).NamingContainer).RowIndex;
                GridView1.EditIndex = rowIndex;
                BindGridViewData(CurrentPageIndex);
            }
            else if (e.CommandName == "DeleteRow")
            {
                int userId = Convert.ToInt32(e.CommandArgument);
                UserDetailsAccessLayer.DeleteUser(userId);
                BindGridViewData(CurrentPageIndex);
            }
            else if (e.CommandName == "CancelUpdate")
            {
                GridView1.EditIndex = -1;
                BindGridViewData(CurrentPageIndex);
            }
            else if (e.CommandName == "UpdateRow")
            {
                int rowIndex = ((GridViewRow)((LinkButton)e.CommandSource).NamingContainer).RowIndex;
                int userId = Convert.ToInt32(e.CommandArgument);

                string userName = ((TextBox)GridView1.Rows[rowIndex].FindControl("txtUserName")).Text.Trim();
                string password = ((TextBox)GridView1.Rows[rowIndex].FindControl("txtPassword")).Text.Trim();
                string email = ((TextBox)GridView1.Rows[rowIndex].FindControl("txtEmail")).Text.Trim();
                string mobileNo = ((TextBox)GridView1.Rows[rowIndex].FindControl("txtMobileNo")).Text.Trim();
                string city = ((TextBox)GridView1.Rows[rowIndex].FindControl("txtCity")).Text.Trim();

                UserDetailsAccessLayer.UpdateUser(userId, userName, password, email, mobileNo, city);
                GridView1.EditIndex = -1;
                BindGridViewData(CurrentPageIndex);
            }
            else if (e.CommandName == "InsertRow")
            {
                if (GridView1.FooterRow != null)
                {
                    TextBox txtUserId = (TextBox)GridView1.FooterRow.FindControl("txtUserId");
                    TextBox txtUserNameFooter = (TextBox)GridView1.FooterRow.FindControl("txtUserNameFooter");
                    TextBox txtPasswordFooter = (TextBox)GridView1.FooterRow.FindControl("txtPasswordFooter");
                    TextBox txtEmailFooter = (TextBox)GridView1.FooterRow.FindControl("txtEmailFooter");
                    TextBox txtMobileNoFooter = (TextBox)GridView1.FooterRow.FindControl("txtMobileNoFooter");
                    TextBox txtCityFooter = (TextBox)GridView1.FooterRow.FindControl("txtCityFooter");

                    int.TryParse(txtUserId?.Text?.Trim() ?? "0", out int userId);
                    string userName = txtUserNameFooter?.Text?.Trim() ?? string.Empty;
                    string password = txtPasswordFooter?.Text?.Trim() ?? string.Empty;
                    string email = txtEmailFooter?.Text?.Trim() ?? string.Empty;
                    string mobileNo = txtMobileNoFooter?.Text?.Trim() ?? string.Empty;
                    string city = txtCityFooter?.Text?.Trim() ?? string.Empty;

                    UserDetailsAccessLayer.InsertUser(userId, userName, password, email, mobileNo, city);
                    BindGridViewData(CurrentPageIndex);
                }
            }
        }
    }
}