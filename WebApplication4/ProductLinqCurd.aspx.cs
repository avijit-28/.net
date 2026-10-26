using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class ProductLinqCurd : System.Web.UI.Page
    {
        private ProductsDataContext GetDataContext()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DBConn"]?.ConnectionString
                ?? ConfigurationManager.ConnectionStrings["AssignmetConnectionString"]?.ConnectionString;

            return !string.IsNullOrEmpty(connStr)
                ? new ProductsDataContext(connStr)
                : new ProductsDataContext();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlResult.Visible = false;
                BindGridview();
            }
        }

        protected void BindGridview()
        {
            try
            {
                using (var db = GetDataContext())
                {
                    var productList = (from p in db.Products
                                       orderby p.ID ascending
                                       select new
                                       {
                                           ID = p.ID,
                                           Name = p.Name,
                                           Price = p.Price,
                                           stock = p.stock
                                       }).ToList();

                    GridView1.DataSource = productList;
                    GridView1.DataBind();

                    ComputeMetrics(productList);
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Failed to load products via LINQ to SQL: " + ex.Message, "danger");
            }
        }

        private void ComputeMetrics<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                lblTotalProducts.Text = "0";
                lblInStock.Text = "0";
                lblLowStock.Text = "0";
                lblAvgPrice.Text = "$ 0.00";
                return;
            }

            int total = list.Count;
            int inStockCount = 0;
            int lowStockCount = 0;
            decimal priceSum = 0;

            foreach (dynamic item in list)
            {
                int stock = 0;
                if (item.stock != null)
                {
                    int.TryParse(item.stock.ToString(), out stock);
                }

                if (stock > 30)
                {
                    inStockCount++;
                }
                else
                {
                    lowStockCount++;
                }

                if (item.Price != null)
                {
                    decimal.TryParse(item.Price.ToString(), out decimal price);
                    priceSum += price;
                }
            }

            lblTotalProducts.Text = total.ToString("N0");
            lblInStock.Text = inStockCount.ToString("N0");
            lblLowStock.Text = lowStockCount.ToString("N0");
            lblAvgPrice.Text = string.Format("$ {0:N2}", total > 0 ? (priceSum / total) : 0);
        }

        protected string GetStockBadgeHtml(object stockObj)
        {
            if (stockObj == null || stockObj == DBNull.Value)
            {
                return "<span class=\"stock-pill stock-out\"><span class=\"stock-dot\"></span> Out of Stock (0)</span>";
            }

            int.TryParse(stockObj.ToString(), out int stock);

            if (stock > 30)
            {
                return string.Format("<span class=\"stock-pill stock-in\"><span class=\"stock-dot\"></span> In Stock ({0} units)</span>", stock);
            }
            else if (stock > 0)
            {
                return string.Format("<span class=\"stock-pill stock-low\"><span class=\"stock-dot\"></span> Low Stock ({0} units)</span>", stock);
            }
            else
            {
                return "<span class=\"stock-pill stock-out\"><span class=\"stock-dot\"></span> Out of Stock (0)</span>";
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            GridView1.EditIndex = -1;
            BindGridview();
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AddNew")
            {
                if (GridView1.FooterRow == null) return;

                TextBox txtID = (TextBox)GridView1.FooterRow.FindControl("txtfID");
                TextBox txtName = (TextBox)GridView1.FooterRow.FindControl("txtfName");
                TextBox txtPrice = (TextBox)GridView1.FooterRow.FindControl("txtfPrice");
                TextBox txtStock = (TextBox)GridView1.FooterRow.FindControl("txtfStock");

                string rawId = txtID?.Text.Trim();
                string rawName = txtName?.Text.Trim();
                string rawPrice = txtPrice?.Text.Trim();
                string rawStock = txtStock?.Text.Trim();

                // Validation
                if (string.IsNullOrEmpty(rawId) || string.IsNullOrEmpty(rawName) ||
                    string.IsNullOrEmpty(rawPrice) || string.IsNullOrEmpty(rawStock))
                {
                    ShowAlert("All fields (ID, Name, Price, Stock) are required to add a new product.", "warning");
                    return;
                }

                if (!int.TryParse(rawId, out int newId))
                {
                    ShowAlert("Product ID must be a valid integer number (e.g. 6).", "warning");
                    return;
                }

                if (!int.TryParse(rawPrice, out int newPrice) || newPrice < 0)
                {
                    ShowAlert("Price must be a valid non-negative integer amount.", "warning");
                    return;
                }

                if (!int.TryParse(rawStock, out int newStock) || newStock < 0)
                {
                    ShowAlert("Stock must be a valid non-negative integer quantity.", "warning");
                    return;
                }

                try
                {
                    using (var db = GetDataContext())
                    {
                        // Check if ID already exists
                        bool exists = db.Products.Any(x => x.ID == newId);
                        if (exists)
                        {
                            ShowAlert(string.Format("Product ID #{0} already exists in the catalog. Please choose a unique ID.", newId), "warning");
                            return;
                        }

                        Product p = new Product
                        {
                            ID = newId,
                            Name = rawName,
                            Price = newPrice,
                            stock = newStock
                        };

                        db.Products.InsertOnSubmit(p);
                        db.SubmitChanges();

                        ShowAlert(string.Format("Product #{0} ('{1}') inserted successfully via LINQ to SQL!", newId, rawName), "success");

                        // Maintain current PageIndex
                        BindGridview();
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error inserting product via LINQ: " + ex.Message, "danger");
                }
            }
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            BindGridview();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            BindGridview();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= GridView1.Rows.Count) return;

            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values["ID"]);

            TextBox txtName = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtName");
            TextBox txtPrice = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtPrice");
            TextBox txtStock = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtStock");

            string rawName = txtName?.Text.Trim();
            string rawPrice = txtPrice?.Text.Trim();
            string rawStock = txtStock?.Text.Trim();

            if (string.IsNullOrEmpty(rawName) || string.IsNullOrEmpty(rawPrice) || string.IsNullOrEmpty(rawStock))
            {
                ShowAlert("Product Name, Price, and Stock cannot be empty.", "warning");
                return;
            }

            if (!int.TryParse(rawPrice, out int priceVal) || priceVal < 0)
            {
                ShowAlert("Price must be a valid non-negative number.", "warning");
                return;
            }

            if (!int.TryParse(rawStock, out int stockVal) || stockVal < 0)
            {
                ShowAlert("Stock must be a valid non-negative integer.", "warning");
                return;
            }

            try
            {
                using (var db = GetDataContext())
                {
                    Product p = db.Products.FirstOrDefault(x => x.ID == id);
                    if (p != null)
                    {
                        p.Name = rawName;
                        p.Price = priceVal;
                        p.stock = stockVal;

                        db.SubmitChanges();

                        GridView1.EditIndex = -1;
                        ShowAlert(string.Format("Product #{0} ('{1}') updated successfully via LINQ to SQL!", id, rawName), "success");

                        // Maintain current PageIndex
                        BindGridview();
                    }
                    else
                    {
                        ShowAlert(string.Format("Product #{0} not found in database.", id), "danger");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error updating product via LINQ: " + ex.Message, "danger");
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= GridView1.Rows.Count) return;

            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values["ID"]);

            try
            {
                using (var db = GetDataContext())
                {
                    Product p = db.Products.FirstOrDefault(x => x.ID == id);
                    if (p != null)
                    {
                        string deletedName = p.Name;
                        db.Products.DeleteOnSubmit(p);
                        db.SubmitChanges();

                        ShowAlert(string.Format("Product #{0} ('{1}') deleted successfully via LINQ to SQL!", id, deletedName), "success");

                        // If the deleted item was the only item on the last page, step back one page
                        int totalRows = db.Products.Count();
                        int maxPageIndex = totalRows > 0 ? (int)Math.Ceiling((double)totalRows / GridView1.PageSize) - 1 : 0;
                        if (GridView1.PageIndex > maxPageIndex)
                        {
                            GridView1.PageIndex = Math.Max(0, maxPageIndex);
                        }

                        GridView1.EditIndex = -1;
                        BindGridview();
                    }
                    else
                    {
                        ShowAlert(string.Format("Product #{0} could not be found for deletion.", id), "warning");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error deleting product via LINQ: " + ex.Message, "danger");
            }
        }

        private void ShowAlert(string message, string alertType)
        {
            pnlResult.Visible = true;
            lblResult.Text = Server.HtmlEncode(message);

            switch (alertType.ToLower())
            {
                case "success":
                    divAlert.Attributes["class"] = "alert-banner alert-banner-success";
                    spAlertIcon.InnerHtml = "&#10004;";
                    break;
                case "danger":
                    divAlert.Attributes["class"] = "alert-banner alert-banner-danger";
                    spAlertIcon.InnerHtml = "&#10006;";
                    break;
                case "warning":
                    divAlert.Attributes["class"] = "alert-banner alert-banner-warning";
                    spAlertIcon.InnerHtml = "&#9888;";
                    break;
                default:
                    divAlert.Attributes["class"] = "alert-banner alert-banner-info";
                    spAlertIcon.InnerHtml = "&#9432;";
                    break;
            }
        }
    }
}
