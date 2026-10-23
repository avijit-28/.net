using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace AddSearch
{
    public partial class LINQDemo : System.Web.UI.Page

    {
        private readonly SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString());

        protected void Page_Load(object sender, EventArgs e)
        {

            
            SqlCommand cmd = new SqlCommand
            ("Select ID, FirstName, LastName, Gender from StudentsTbl where Gender='Male'", con);
            List<Students> listStudents = new List<Students>();
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                Students student = new Students();
                student.ID = Convert.ToInt32(rdr["ID"]);
                student.FirstName = rdr["FirstName"].ToString();
                student.LastName = rdr["LastName"].ToString();
                student.Gender = rdr["Gender"].ToString();
                listStudents.Add(student);
            }
            con.Close();
            GridView1.DataSource = listStudents;
            GridView1.DataBind();
        }
    }
}