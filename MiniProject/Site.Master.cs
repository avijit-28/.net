using System;
using System.Web.UI;
using MiniProject.Models;

namespace MiniProject
{
    public partial class Site : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            UpdateCartBadge();
        }

        public void UpdateCartBadge()
        {
            var cart = CartSessionHelper.GetCart();
            int count = cart != null ? cart.TotalItemCount : 0;
            lblCartCount.Text = count.ToString();
        }
    }
}
