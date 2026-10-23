using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AddSearch
{
    public partial class LINQDemo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            SampleDataContext dataContext = new SampleDataContext();
            GridView1.DataSource = from student in dataContext.CS1061: 'SampleDataContext' does not contain a definition for 'Students' and no accessible extension method 'Students' accepting a first argument of type 'SampleDataContext' could be found (are you missing a using directive or an assembly reference?)
SampleDataContext dataContext = new SampleDataContext();
GridView1.DataSource = from student in dataContext.StudentsTbls
                       where student.Gender == "Male"
                       select student;
GridView1.DataBind();
                                   where student.Gender == "Male"
                                   select student;
            GridView1.DataBind();
        }
    }
}