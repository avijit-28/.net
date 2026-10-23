using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AddSearch
{
    public partial class UserDetailsWithoutObjectSource : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                    int totalRows = 0;
                    GridView1.DataSource = UserDetailsAccessLayer.GetAllUsers(0, GridView1.PageSize, out totalRows);
                    GridView1.DataBind();
                    DatabindRepeater(0, GridView1.PageSize, totalRows);
                
                //BindGridViewData();
            }
        }

        private void DatabindRepeater(int pageIndex, int pageSize, int totalRows)
        {
            int totalPages = totalRows / pageSize;
            if ((totalRows % pageSize) != 0)
            {
                totalPages += 1;
            }
            List<ListItem> pages = new List<ListItem>();
            if (totalPages > 1)
            {
                for (int i = 1; i <= totalPages; i++)
                {
                    pages.Add(new ListItem(i.ToString(), i.ToString(), i != (pageIndex + 1)));
                }
            }
            repeaterPaging.DataSource = pages;
            repeaterPaging.DataBind();
        }

        protected void linkButton_Click(object sender, EventArgs e)
        {
            int totalRows = 0;
            int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
            pageIndex -= 1;
            GridView1.PageIndex = pageIndex;
            GridView1.DataSource =
                UserDetailsAccessLayer.GetAllUsers(
                    pageIndex,
                    GridView1.PageSize,
                    out totalRows);
            GridView1.DataBind();
            DatabindRepeater( pageIndex,GridView1.PageSize,totalRows);
        }

        private void BindGridViewData()
        {
            GridView1.DataSource = UserDetailsAccessLayer.GetAllUsers();
            //repeaterPaging.DataSource = pages;
            //repeaterPaging.DataBind();
            GridView1.DataBind();
        }
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRow")
            {
                int rowIndex = ((GridViewRow)((LinkButton)e.CommandSource).NamingContainer).RowIndex;
                GridView1.EditIndex = rowIndex;
                BindGridViewData();
            }
            else if (e.CommandName == "DeleteRow")
            {
                UserDetailsAccessLayer.DeleteUser(Convert.ToInt32(e.CommandArgument));
                BindGridViewData();
            }
            else if (e.CommandName == "CancelUpdate")
            {
                GridView1.EditIndex = -1;
                BindGridViewData();
            }
            else if (e.CommandName == "UpdateRow")
            {
                int rowIndex = ((GridViewRow)((LinkButton)e.CommandSource)
                                .NamingContainer).RowIndex;

                int userId = Convert.ToInt32(e.CommandArgument);

                string userName =
                    ((TextBox)GridView1.Rows[rowIndex]
                    .FindControl("txtUserName")).Text;

                string password =((TextBox)GridView1.Rows[rowIndex].FindControl("txtPassword")).Text;
                string email =((TextBox)GridView1.Rows[rowIndex].FindControl("txtEmail")).Text;
                string mobileNo =((TextBox)GridView1.Rows[rowIndex].FindControl("txtMobileNo")).Text;
                string city =((TextBox)GridView1.Rows[rowIndex].FindControl("txtCity")).Text;

                UserDetailsAccessLayer.UpdateUser(
                    userId,
                    userName,
                    password,
                    email,
                    mobileNo,
                    city);

                GridView1.EditIndex = -1;

                BindGridViewData();
            }
            else if (e.CommandName == "InsertRow")
            {
                int userId = Convert.ToInt32(((TextBox)GridView1.FooterRow.FindControl("txtUserId")).Text);
                string userName =((TextBox)GridView1.FooterRow.FindControl("txtUserNameFooter")).Text;
                string password =((TextBox)GridView1.FooterRow.FindControl("txtPasswordFooter")).Text;
                string email =((TextBox)GridView1.FooterRow.FindControl("txtEmailFooter")).Text;
                string mobileNo =((TextBox)GridView1.FooterRow.FindControl("txtMobileNoFooter")).Text;
                string city =((TextBox)GridView1.FooterRow.FindControl("txtCityFooter")).Text;
                UserDetailsAccessLayer.InsertUser(
                    userId,
                    userName,
                    password,
                    email,
                    mobileNo,
                    city);

                BindGridViewData();
            }


        }


    }
}