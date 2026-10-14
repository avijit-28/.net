namespace WebApplication4
{
    using System;

    /// <summary>
    /// Defines the <see cref="SignUp" />
    /// </summary>
    public partial class SignUp : System.Web.UI.Page
    {
        /// <summary>
        /// The Page_Load
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            if ((Session["UserName"] != null)|| (Session["Password"] != null))
            {
                lblOnlineUsers.Text = (Application["user"] != null ? Application["user"].ToString() : "0");
                outUserName.Text = Session["UserName"].ToString();
                outPassword.Text = Session["Password"].ToString();
                //lblOnlineUsers.Text = (Application["user"] != null ? Application["user"].ToString() : "0");
            }
            else
            {
                Response.Redirect("FirstPage.aspx");
            }
        }

        /// <summary>
        /// The BtnProductForm_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        protected void BtnProductForm_Click(object sender, EventArgs e)
        {
            Response.Redirect("ProductForm.aspx");
        }

        protected void BtnCounting_Click(object sender, EventArgs e)
        {
            Response.Redirect("Counting.aspx");
        }

        protected void BtnSearchUser_Click(object sender, EventArgs e)
        {
            Response.Redirect("SearchUser.aspx");
        }

        protected void BtnCookiesSend_Click(object sender, EventArgs e)
        {
            Response.Redirect("CookiesSend.aspx");
        }

        protected void BtnCheckAvailability_Click(object sender, EventArgs e)
        {
            Response.Redirect("CheckAvailability.aspx");
        }
    }
}
