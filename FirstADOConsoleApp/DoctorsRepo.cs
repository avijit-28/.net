using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace FirstADOConsoleApp
{
    public class DoctorsRepo
    {
        public string conStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        //public List<Doctors> doctors = new List<Doctors>();

        public void addDoctor(Doctors doctor)
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                string query = "Insert into hpl.doctors(first_name,last_name," +
                    "specialization,department_id,email,phone_number) values " +
                    "(@first_name,@last_name,@specialization,@department_id,@email," +
                    "@phone_number)";

                using(SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@first_name", doctor.FirstName);
                    cmd.Parameters.AddWithValue("@last_name", doctor.Lastname);
                    cmd.Parameters.AddWithValue("@specialization", doctor.Specialization);
                    cmd.Parameters.AddWithValue("@department_id", doctor.DepartmentId);
                    cmd.Parameters.AddWithValue("@email", doctor.Email);
                    cmd.Parameters.AddWithValue("@phone_number", doctor.PhoneNumber);
                    cmd.ExecuteNonQuery();

                }
            }

        }

        
    }
}
