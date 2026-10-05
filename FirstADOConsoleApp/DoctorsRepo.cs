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
        public List<Doctors> doctors = new List<Doctors>();

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
            doctors.Add(doctor);

        }

        //public List<Doctors> doctors = new List<Doctors>();
        public void showDoctors()
        {

            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                //int rowCount;
                //string query1 = "select count(*) from hpl.doctors";
                //using (SqlCommand cmd = new SqlCommand(query1, conn))
                //{
                //    rowCount = (int)cmd.ExecuteScalar();
                //}

                string query2 = "select * from hpl.doctors";
                using (SqlCommand cmd = new SqlCommand(query2, conn))
                {
                    //Console.WriteLine(query);
                    
                    //int counting = 1;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()  )
                        {

                        int DcId = reader.GetInt32(0); 
                        string DcFirstName = reader.GetString(1);
                        string DcLastName = reader.GetString(2);
                        string DcSpecialization = reader.GetString(3);
                        int DcDepartmentId = reader.GetInt32(4);
                        string DcEmail = reader.GetString(5);
                        string DcPhoneNumber = reader.GetString(6);
                        var doc = doctors.Find(c => c.DoctorId == DcId);
                            if(doc == null )
                            {   
                                //Doctors docDetails = new Doctors(DcId, DcFirstName, DcLastName, DcSpecialization, DcDepartmentId, DcEmail, DcPhoneNumber);
                                Doctors docDetails = new Doctors();
                                docDetails.DoctorId = DcId;
                                docDetails.FirstName = DcFirstName;
                                docDetails.Lastname = DcLastName;
                                docDetails.Specialization = DcSpecialization;
                                docDetails.DepartmentId = DcDepartmentId;
                                docDetails.Email = DcEmail;
                                docDetails.PhoneNumber = DcPhoneNumber;
                                doctors.Add(docDetails);
                                Console.WriteLine("\n Added Doctors \n");
                                Console.WriteLine($"Doctor Id: {DcId} | Doctor Name : {DcFirstName} {DcLastName} | Specalization : {DcSpecialization} |  Deparmetnt ID : {DcDepartmentId} | Email : {DcEmail} | Phone : {DcPhoneNumber}");

                            }
                            else
                            {
                                Console.WriteLine($"\nDoctor Id: {DcId} | Doctor Name : {DcFirstName} {DcLastName} | Specalization : {DcSpecialization} |  Deparmetnt ID : {DcDepartmentId} | Email : {DcEmail} | Phone : {DcPhoneNumber}\n");
                            }

                        }
                    }
                }
            }

        }

        public void updateDoctorsDetails()
        {
            Console.Write("Doctor Id: ");
            int DcId = Convert.ToInt32(Console.ReadLine());
            var doc = doctors.Find(c => c.DoctorId == DcId);
            if (doc != null)
            {
                Console.Write("First Name (leave blank to skip): ");
                string DcFirstName = Console.ReadLine();

                Console.Write("Last Name (leave blank to skip): ");
                string DcLirstName = Console.ReadLine();

                Console.Write("Specialization (leave blank to skip): ");
                string DcSpecialization = Console.ReadLine();

                Console.Write("Department Id (leave blank to skip): ");
                string DcDepartmentId = Console.ReadLine();

                Console.Write("Email (leave blank to skip): ");
                string DcEmail = Console.ReadLine();

                Console.Write("Phone Number (leave blank to skip): ");
                string DcPhoneNumber = Console.ReadLine();

                List<string> updates = new List<string>();

                if (!string.IsNullOrWhiteSpace(DcFirstName))
                    updates.Add("first_name=@firstName");

                if (!string.IsNullOrWhiteSpace(DcLirstName))
                    updates.Add("last_name=@last_name");

                if (!string.IsNullOrWhiteSpace(DcSpecialization))
                    updates.Add("specialization = @specialization");

                if (!string.IsNullOrWhiteSpace(DcDepartmentId))
                    updates.Add("department_id = @department_id");

                if (!string.IsNullOrWhiteSpace(DcEmail))
                    updates.Add("email=@email");

                if (!string.IsNullOrWhiteSpace(DcPhoneNumber))
                    updates.Add("phone_number=@phone");

                string query = $"UPDATE hpl.doctors SET {string.Join(",", updates)} WHERE doctor_id=@id";

                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", DcId);

                        if (!string.IsNullOrWhiteSpace(DcFirstName))
                            cmd.Parameters.AddWithValue("@firstName", DcFirstName);

                        if (!string.IsNullOrWhiteSpace(DcLirstName))
                            cmd.Parameters.AddWithValue("@last_name", DcLirstName);

                        if (!string.IsNullOrWhiteSpace(DcSpecialization))
                            cmd.Parameters.AddWithValue("@specialization", DcSpecialization); ;

                        if (!string.IsNullOrWhiteSpace(DcDepartmentId))
                            cmd.Parameters.AddWithValue("@department_id", DcDepartmentId);

                        if (!string.IsNullOrWhiteSpace(DcEmail))
                            cmd.Parameters.AddWithValue("@email", DcEmail);

                        if (!string.IsNullOrWhiteSpace(DcPhoneNumber))
                            cmd.Parameters.AddWithValue("@phone", DcPhoneNumber);

                        cmd.ExecuteNonQuery();                    }

                }


            }
            else
            {
                Console.WriteLine("Nothing to update.");
                return;
            }

        }
    }
}
