using System;
using System.Web.UI;
using MiniProject.Models;

namespace MiniProject
{
    public partial class ProductDetails : Page
    {
        private int _productId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (int.TryParse(Request.QueryString["id"], out _productId))
            {
                if (!IsPostBack)
                {
                    LoadProductDetails(_productId);
                }
            }
            else
            {
                ShowNotFound();
            }
        }

        private void LoadProductDetails(int id)
        {
            var product = ProductRepository.GetProductById(id);
            if (product == null)
            {
                ShowNotFound();
                return;
            }

            pnlNotFound.Visible = false;
            pnlDetails.Visible = true;

            // Page & Breadcrumbs
            Title = product.Name;
            lblBreadcrumbTitle.Text = product.Name;
            lnkCategoryBreadcrumb.Text = product.Category;
            lnkCategoryBreadcrumb.NavigateUrl = $"Default.aspx?cat={Server.UrlEncode(product.Category)}";

            // Product Details
            lblProductName.Text = product.Name;
            lblCategory.Text = product.Category;
            lblDescription.Text = product.Description;
            lblPrice.Text = product.Price.ToString("N2");
            lblRating.Text = product.Rating.ToString("F1");
            lblReviewCount.Text = product.ReviewCount.ToString();
            imgProduct.ImageUrl = product.ImageUrl;
            imgProduct.AlternateText = product.Name;

            if (product.OriginalPrice.HasValue && product.OriginalPrice > product.Price)
            {
                lblOriginalPrice.Text = $"${product.OriginalPrice.Value:N2}";
                lblOriginalPrice.Visible = true;
                lblDiscountBadge.Text = $"-{product.DiscountPercent}% OFF";
                lblDiscountBadge.Visible = true;
            }
            else
            {
                lblOriginalPrice.Visible = false;
                lblDiscountBadge.Visible = false;
            }

            if (product.InStock)
            {
                lblStockStatus.Text = $"In Stock ({product.Stock} available)";
                lblStockStatus.CssClass = "badge bg-success-subtle text-success border border-success-subtle px-3 py-2 fs-6 rounded-pill";
                btnAddToCart.Enabled = true;
            }
            else
            {
                lblStockStatus.Text = "Out of Stock";
                lblStockStatus.CssClass = "badge bg-danger-subtle text-danger border border-danger-subtle px-3 py-2 fs-6 rounded-pill";
                btnAddToCart.Enabled = false;
            }

            // Related Products
            var related = ProductRepository.GetRelatedProducts(product.Id, product.Category, 3);
            rptRelated.DataSource = related;
            rptRelated.DataBind();
        }

        private void ShowNotFound()
        {
            pnlNotFound.Visible = true;
            pnlDetails.Visible = false;
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (int.TryParse(Request.QueryString["id"], out int id))
            {
                var product = ProductRepository.GetProductById(id);
                if (product != null && product.InStock)
                {
                    int qty = 1;
                    int.TryParse(txtQuantity.Text, out qty);
                    if (qty <= 0) qty = 1;
                    if (qty > product.Stock) qty = product.Stock;

                    var cart = CartSessionHelper.GetCart();
                    cart.AddItem(product, qty);
                    CartSessionHelper.SaveCart(cart);

                    lblAlertMessage.Text = $"Successfully added <strong>{qty}x {Server.HtmlEncode(product.Name)}</strong> to your shopping cart! <a href='Cart.aspx' class='alert-link ms-2'>View Cart & Checkout <i class='bi bi-arrow-right'></i></a>";
                    pnlAlert.Visible = true;

                    var master = Master as Site;
                    if (master != null)
                    {
                        master.UpdateCartBadge();
                    }
                }
            }
        }
    }
}
