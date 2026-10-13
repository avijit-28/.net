using System;
using System.Collections.Generic;
using System.Web.UI;
using MiniProject.Models;

namespace MiniProject
{
    public partial class Checkout : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var cart = CartSessionHelper.GetCart();
            if (cart.Items == null || cart.Items.Count == 0)
            {
                Response.Redirect("Cart.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindCheckoutSummary(cart);
            }
        }

        private void BindCheckoutSummary(Cart cart)
        {
            rptCheckoutItems.DataSource = cart.Items;
            rptCheckoutItems.DataBind();

            lblSubtotal.Text = cart.Subtotal.ToString("N2");
            lblShipping.Text = cart.Shipping == 0m ? "FREE" : $"${cart.Shipping:N2}";
            lblTax.Text = cart.Tax.ToString("N2");
            lblGrandTotal.Text = cart.GrandTotal.ToString("N2");
        }

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            var cart = CartSessionHelper.GetCart();
            if (cart.Items == null || cart.Items.Count == 0)
            {
                Response.Redirect("Cart.aspx");
                return;
            }

            // Create Order
            var order = new Order
            {
                OrderId = "TM-" + new Random().Next(100000, 999999),
                OrderDate = DateTime.Now,
                CustomerName = txtName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                City = txtCity.Text.Trim(),
                PostalCode = txtPostalCode.Text.Trim(),
                PaymentMethod = rblPayment.SelectedValue,
                Items = new List<CartItem>(cart.Items),
                Subtotal = cart.Subtotal,
                Shipping = cart.Shipping,
                Tax = cart.Tax,
                GrandTotal = cart.GrandTotal
            };

            // Store order in session for receipt
            CartSessionHelper.SetLastOrder(order);

            // Clear Cart
            CartSessionHelper.ClearCart();

            // Redirect to Success confirmation page
            Response.Redirect("OrderSuccess.aspx");
        }
    }
}
