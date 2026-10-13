using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MiniProject.Models;

namespace MiniProject
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateCategories();

                // Check query string parameters (e.g. ?cat=Audio or ?q=search)
                if (!string.IsNullOrEmpty(Request.QueryString["cat"]))
                {
                    string catParam = Request.QueryString["cat"];
                    var item = ddlCategory.Items.FindByValue(catParam);
                    if (item != null)
                    {
                        ddlCategory.SelectedValue = catParam;
                    }
                }

                if (!string.IsNullOrEmpty(Request.QueryString["q"]))
                {
                    txtSearch.Text = Request.QueryString["q"];
                }

                BindProducts();
            }
        }

        private void PopulateCategories()
        {
            ddlCategory.Items.Clear();
            ddlCategory.Items.Add(new ListItem("All Categories", "All"));

            var categories = ProductRepository.GetCategories();
            foreach (var cat in categories)
            {
                ddlCategory.Items.Add(new ListItem(cat, cat));
            }
        }

        private void BindProducts()
        {
            string query = txtSearch.Text;
            string category = ddlCategory.SelectedValue;
            string sort = ddlSort.SelectedValue;

            var products = ProductRepository.FilterProducts(query, category, sort);

            lblProductCount.Text = products.Count.ToString();
            pnlNoProducts.Visible = (products.Count == 0);
            rptProducts.Visible = (products.Count > 0);

            rptProducts.DataSource = products;
            rptProducts.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindProducts();
        }

        protected void ddlFilter_Changed(object sender, EventArgs e)
        {
            BindProducts();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            ddlCategory.SelectedValue = "All";
            ddlSort.SelectedValue = "featured";
            pnlAlert.Visible = false;
            BindProducts();
        }

        protected void btnQuickFilter_Click(object sender, EventArgs e)
        {
            var btn = (LinkButton)sender;
            string category = btn.CommandArgument;

            var item = ddlCategory.Items.FindByValue(category);
            if (item != null)
            {
                ddlCategory.SelectedValue = category;
            }
            else
            {
                ddlCategory.SelectedValue = "All";
            }

            txtSearch.Text = string.Empty;
            BindProducts();
        }

        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AddToCart")
            {
                int productId = Convert.ToInt32(e.CommandArgument);
                var product = ProductRepository.GetProductById(productId);

                if (product != null && product.InStock)
                {
                    var cart = CartSessionHelper.GetCart();
                    cart.AddItem(product, 1);
                    CartSessionHelper.SaveCart(cart);

                    // Show success alert
                    lblAlertMessage.Text = $"<strong>{Server.HtmlEncode(product.Name)}</strong> has been added to your cart! <a href='Cart.aspx' class='alert-link ms-2'>View Cart <i class='bi bi-arrow-right'></i></a>";
                    pnlAlert.Visible = true;

                    // Update Master Page badge
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
