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
                if (e.Row.Cells[3].Text == "US")
                {
                    int salary = Convert.ToInt32(e.Row.Cells[2].Text);
                    string formattedString = string.Format(new System.Globalization.CultureInfo("en-US"), "{0:c}", salary);
                    e.Row.Cells[2].Text = formattedString;
                }
                else if (e.Row.Cells[3].Text == "UK")
                {
                    int salary = Convert.ToInt32(e.Row.Cells[2].Text);
                    string formattedString = string.Format(new System.Globalization.CultureInfo("en-GB"), "{0:c}", salary);
                    e.Row.Cells[2].Text = formattedString;
                }
                else if (e.Row.Cells[3].Text == "India")
                {
                    int salary = Convert.ToInt32(e.Row.Cells[2].Text);
                    string formattedString = string.Format(new System.Globalization.CultureInfo("en-IN"), "{0:c}", salary);
                    e.Row.Cells[2].Text = formattedString;
                }
                else if (e.Row.Cells[3].Text == "South Africa")
                {
                    int salary = Convert.ToInt32(e.Row.Cells[2].Text);
                    string formattedString = string.Format(new System.Globalization.CultureInfo("en-ZA"), "{0:c}", salary);
                    e.Row.Cells[2].Text = formattedString;
                }
                else if (e.Row.Cells[3].Text == "Malaysia")
                {
                    int salary = Convert.ToInt32(e.Row.Cells[2].Text);
                    string formattedString = string.Format(new System.Globalization.CultureInfo("en-MY"), "{0:c}", salary);
                    e.Row.Cells[2].Text = formattedString;
                }
                int AnnualSalary = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "AnnualSalary"));
                if(AnnualSalary > 60000)   
                {
                    e.Row.BackColor = System.Drawing.Color.Red;
                    e.Row.ForeColor = System.Drawing.Color.White;
                }
            }
        }
    }
}