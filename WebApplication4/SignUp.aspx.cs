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
            lblOnlineUsers.Text = (Application["user"] != null ? Application["user"].ToString() : "0");
            outUserName.Text = Session["UserName"] != null ? Session["UserName"].ToString() : "Guest";
            outPassword.Text = Session["Password"] != null ? Session["Password"].ToString() : "••••••••";
            lblOnlineUsers.Text = (Application["user"] != null ? Application["user"].ToString() : "0");
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
    }
}
