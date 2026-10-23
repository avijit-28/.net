using AddSearch;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace AddSearch
{
    public partial class LINQDemo : System.Web.UI.Page
    {
        
        private readonly string conStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

            using (SampleDataContext dataContext = new SampleDataContext(conStr))
            {
                GridView1.DataSource = from student in dataContext.StudentsTbl
                                       where student.Gender == "Male"
                                       select student;
                GridView1.DataBind();
            }
                
            
            //if (!IsPostBack)
            //{
            //    string conStr = ConfigurationManager
            //                    .ConnectionStrings["DBConn"]
            //                    .ConnectionString;

            //    List<Students> listStudents = new List<Students>();

            //    using (SqlConnection con = new SqlConnection(conStr))
            //    {
            //        SqlCommand cmd = new SqlCommand(
            //            "SELECT ID, FirstName, LastName, Gender " +
            //            "FROM StudentsTbl WHERE Gender = 'Male'", con);

            //        con.Open();

            //        SqlDataReader rdr = cmd.ExecuteReader();

            //        while (rdr.Read())
            //        {
            //            Students student = new Students();

            //            student.ID = Convert.ToInt32(rdr["ID"]);
            //            student.FirstName = rdr["FirstName"].ToString();
            //            student.LastName = rdr["LastName"].ToString();
            //            student.Gender = rdr["Gender"].ToString();

            //            listStudents.Add(student);
            //        }
            //    }

            //    GridView1.DataSource = listStudents;
            //    GridView1.DataBind();
        
        }
    }

}