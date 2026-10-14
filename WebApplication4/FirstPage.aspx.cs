namespace WebApplication4
{
    using System;

    /// <summary>
    /// Defines the <see cref="FirstPage" />
    /// </summary>
    public partial class FirstPage : System.Web.UI.Page
    {
        /// <summary>
        /// The Page_Load
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            lblOnlineUsers.Text = (Application["user"] != null ? Application["user"].ToString() : "0");
        }

        /// <summary>
        /// The BtnSign_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        protected void BtnSign_Click(object sender, EventArgs e)
        {
            if (txtUserName.Text.ToString() == "avijit@gmail.com" && txtPassword.Text.ToString() == "1234")
            {
                Session["UserName"] = txtUserName.Text;
                Session["Password"] = txtPassword.Text;
                Response.Redirect("SignUp.aspx");

            }
            else
            {
                Response.Redirect("FirstPage.aspx");
            }
        }
    }
}
