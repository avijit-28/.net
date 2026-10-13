using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MiniProject.Models;

namespace MiniProject
{
    public partial class CartPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCart();
            }
        }

        private void BindCart()
        {
            var cart = CartSessionHelper.GetCart();

            if (cart.Items == null || cart.Items.Count == 0)
            {
                pnlEmptyCart.Visible = true;
                pnlCartContent.Visible = false;
            }
            else
            {
                pnlEmptyCart.Visible = false;
                pnlCartContent.Visible = true;

                rptCartItems.DataSource = cart.Items;
                rptCartItems.DataBind();

                lblItemCount.Text = cart.TotalItemCount.ToString();
                lblSubtotal.Text = cart.Subtotal.ToString("N2");

                if (cart.Shipping == 0m)
                {
                    lblShipping.Text = "<span class='badge bg-success text-white'>FREE</span>";
                }
                else
                {
                    lblShipping.Text = $"${cart.Shipping:N2}";
                }

                lblTax.Text = cart.Tax.ToString("N2");
                lblGrandTotal.Text = cart.GrandTotal.ToString("N2");
            }

            var master = Master as Site;
            if (master != null)
            {
                master.UpdateCartBadge();
            }
        }

        protected void rptCartItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int productId = Convert.ToInt32(e.CommandArgument);
            var cart = CartSessionHelper.GetCart();
            var item = cart.Items.Find(i => i.Product.Id == productId);

            if (item != null)
            {
                if (e.CommandName == "Increase")
                {
                    if (item.Quantity < item.Product.Stock)
                    {
                        cart.UpdateQuantity(productId, item.Quantity + 1);
                    }
                    else
                    {
                        lblAlertMessage.Text = $"Maximum available stock ({item.Product.Stock}) reached for {item.Product.Name}.";
                        pnlAlert.Visible = true;
                    }
                }
                else if (e.CommandName == "Decrease")
                {
                    if (item.Quantity > 1)
                    {
                        cart.UpdateQuantity(productId, item.Quantity - 1);
                    }
                    else
                    {
                        cart.RemoveItem(productId);
                    }
                }
                else if (e.CommandName == "Remove")
                {
                    cart.RemoveItem(productId);
                    lblAlertMessage.Text = $"{item.Product.Name} was removed from your cart.";
                    pnlAlert.Visible = true;
                }

                CartSessionHelper.SaveCart(cart);
                BindCart();
            }
        }

        protected void btnClearCart_Click(object sender, EventArgs e)
        {
            CartSessionHelper.ClearCart();
            BindCart();
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            var cart = CartSessionHelper.GetCart();
            if (cart.Items != null && cart.Items.Count > 0)
            {
                Response.Redirect("Checkout.aspx");
            }
        }
    }
}
