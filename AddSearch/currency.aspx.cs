using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AddSearch
{
    public partial class currency : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string country = DataBinder.Eval(e.Row.DataItem, "Country").ToString();

                DropDownList ddlCountry =
                    (DropDownList)e.Row.FindControl("DropDownList1");

                ddlCountry.SelectedValue = country;

                int salary = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "AnnualSalary"));

                if (country == "US")
                    e.Row.Cells[2].Text = string.Format(
                        new System.Globalization.CultureInfo("en-US"),
                        "{0:c}", salary);

                else if (country == "UK")
                    e.Row.Cells[2].Text = string.Format(
                        new System.Globalization.CultureInfo("en-GB"),
                        "{0:c}", salary);

                else if (country == "India")
                    e.Row.Cells[2].Text = string.Format(
                        new System.Globalization.CultureInfo("en-IN"),
                        "{0:c}", salary);

                else if (country == "South Africa")
                    e.Row.Cells[2].Text = string.Format(
                        new System.Globalization.CultureInfo("en-ZA"),
                        "{0:c}", salary);

                else if (country == "Malaysia")
                    e.Row.Cells[2].Text = string.Format(
                        new System.Globalization.CultureInfo("ms-MY"),
                        "{0:c}", salary);

                if (salary > 60000)
                {
                    e.Row.BackColor = System.Drawing.Color.Red;
                    e.Row.ForeColor = System.Drawing.Color.White;
                }
            }
        }
    }
}

//int AnnualSalary = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "AnnualSalary"));
//if (AnnualSalary > 60000)
//{
//    e.Row.BackColor = System.Drawing.Color.Red;
//    e.Row.ForeColor = System.Drawing.Color.White;
//}