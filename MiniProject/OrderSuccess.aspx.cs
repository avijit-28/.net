using System;
using System.Web.UI;
using MiniProject.Models;

namespace MiniProject
{
    public partial class OrderSuccess : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var order = CartSessionHelper.GetLastOrder();
                if (order == null)
                {
                    pnlNoOrder.Visible = true;
                    pnlOrderDetails.Visible = false;
                    return;
                }

                pnlNoOrder.Visible = false;
                pnlOrderDetails.Visible = true;

                lblOrderId.Text = order.OrderId;
                lblOrderDate.Text = order.OrderDate.ToString("MMMM dd, yyyy h:mm tt");
                lblCustomerName.Text = order.CustomerName;
                lblShippingAddress.Text = order.Address;
                lblCityZip.Text = $"{order.City}, {order.PostalCode}";
                lblPhone.Text = order.Phone;
                lblEmail.Text = order.Email;
                lblPaymentMethod.Text = order.PaymentMethod;

                rptOrderItems.DataSource = order.Items;
                rptOrderItems.DataBind();

                lblSubtotal.Text = order.Subtotal.ToString("N2");
                lblShipping.Text = order.Shipping == 0m ? "FREE" : $"${order.Shipping:N2}";
                lblTax.Text = order.Tax.ToString("N2");
                lblGrandTotal.Text = order.GrandTotal.ToString("N2");
            }
        }
    }
}
