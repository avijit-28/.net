using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace AddSearch.Connected_Services
{
    public partial class ProductLinqCurd : System.Web.UI.Page
    {
        private readonly string conStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        ProductsDataContext db =new ProductsDataContext(ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);

        //ProductsDataContext db;
        protected void Page_Load(object sender, EventArgs e)
        {
            //db = new ProductsDataContext(conStr);
            if (!Page.IsPostBack)
            {
                BindGridview();
            }
        }
        protected void BindGridview()
        {
            var result = (
                            from p in db.Products
                            select new
                            {
                                ID = p.ID,
                                Name = p.Name,
                                Price = p.Price,
                                stock = p.stock
                            }
                         ).ToList();

            GridView1.DataSource = result;
            GridView1.DataBind();
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AddNew")
            {
                TextBox txtID = (TextBox)GridView1.FooterRow.FindControl("txtfID");
                TextBox txtName = (TextBox)GridView1.FooterRow.FindControl("txtfName");
                TextBox txtPrice = (TextBox)GridView1.FooterRow.FindControl("txtfPrice");
                TextBox txtStock = (TextBox)GridView1.FooterRow.FindControl("txtfStock");

                Product p = new Product();

                p.ID = Convert.ToInt32(txtID.Text);
                p.Name = txtName.Text;
                p.Price = Convert.ToInt32(txtPrice.Text);
                p.stock = Convert.ToInt32(txtStock.Text);

                db.Products.InsertOnSubmit(p);
                db.SubmitChanges();

                lblResult.ForeColor = Color.Green;
                lblResult.Text = "Record inserted successfully";

                BindGridview();
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
            int id = Convert.ToInt32(
                GridView1.DataKeys[e.RowIndex].Values["ID"]);

            TextBox txtName =
                (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtName");

            TextBox txtPrice =
                (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtPrice");

            TextBox txtStock =
                (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtStock");

            Product p = db.Products
                          .FirstOrDefault(x => x.ID == id);

            p.Name = txtName.Text;
            p.Price = Convert.ToInt32(txtPrice.Text);
            p.stock = Convert.ToInt32(txtStock.Text);

            db.SubmitChanges();

            GridView1.EditIndex = -1;
            lblResult.ForeColor = Color.Blue;
            lblResult.Text = "Record Updated successfully";

            BindGridview();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(
                GridView1.DataKeys[e.RowIndex].Values["ID"]);

            Product p = db.Products
                          .FirstOrDefault(x => x.ID == id);

            db.Products.DeleteOnSubmit(p);

            db.SubmitChanges();
            lblResult.ForeColor = Color.Red;
            lblResult.Text = "Product deleted successfully";


            BindGridview();
        }

        //protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        foreach (Control c in e.Row.Cells[e.Row.Cells.Count - 1].Controls)
        //        {
        //            if (c is LinkButton)
        //            {
        //                LinkButton btn = (LinkButton)c;

        //                if (btn.CommandName == "Delete")
        //                {
        //                    btn.OnClientClick =
        //                        "return confirm('Are you sure you want to delete this record?');";
        //                }
        //            }
        //        }
        //    }
        //}
    }
}