using System;
using System.Web.UI.WebControls;

namespace AddSearch
{
    public partial class UserDetails1 : System.Web.UI.Page
    {
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AddNew")
            {
                ObjectDataSource1.InsertParameters["UserName"].DefaultValue =
                    ((TextBox)GridView1.FooterRow.FindControl("txtUserName")).Text;

                ObjectDataSource1.InsertParameters["Password"].DefaultValue =
                    ((TextBox)GridView1.FooterRow.FindControl("txtPassword")).Text;

                ObjectDataSource1.InsertParameters["Email"].DefaultValue =
                    ((TextBox)GridView1.FooterRow.FindControl("txtEmail")).Text;

                ObjectDataSource1.InsertParameters["MobileNo"].DefaultValue =
                    ((TextBox)GridView1.FooterRow.FindControl("txtMobileNo")).Text;

                ObjectDataSource1.InsertParameters["City"].DefaultValue =
                    ((TextBox)GridView1.FooterRow.FindControl("txtCity")).Text;

                ObjectDataSource1.Insert();
            }
        }
    }
}