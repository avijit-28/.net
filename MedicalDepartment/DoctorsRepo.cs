using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace MedicalDepartment
{
    public class DoctorsRepo
    {
        public string conStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        public static List<Doctors> doctors = new List<Doctors>();

        private List<Doctors> loadDoctorsFromDb()
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                string query = "SELECT doctor_id, first_name, last_name, specialization, department_id, email, phone_number FROM hpl.doctors";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        doctors.Add(new Doctors
                        {
                            DoctorId = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            Lastname = reader.GetString(2),
                            Specialization = reader.GetString(3),
                            DepartmentId = reader.GetInt32(4),
                            Email = reader.GetString(5),
                            PhoneNumber = reader.GetString(6)
                        });
                    }
                }
            }
            return doctors;
        }

        public void addDoctor(Doctors doctor)
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                string query = "Insert into hpl.doctors(first_name,last_name," +
                    "specialization,department_id,email,phone_number) values " +
                    "(@first_name,@last_name,@specialization,@department_id,@email," +
                    "@phone_number)" + "SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@first_name", doctor.FirstName);
                    cmd.Parameters.AddWithValue("@last_name", doctor.Lastname);
                    cmd.Parameters.AddWithValue("@specialization", doctor.Specialization);
                    cmd.Parameters.AddWithValue("@department_id", doctor.DepartmentId);
                    cmd.Parameters.AddWithValue("@email", doctor.Email);
                    cmd.Parameters.AddWithValue("@phone_number", doctor.PhoneNumber);
                    cmd.ExecuteNonQuery();
                    //doctor.DoctorId = Convert.ToInt32(cmd.ExecuteScalar());

                }

            }
            //doctors.Add(doctor);

        }

        //public List<Doctors> doctors = new List<Doctors>();
        public void showDoctors()
        {

            List<Doctors> doctors = loadDoctorsFromDb();
            foreach (var doc in doctors)
            {
                Console.WriteLine($"\nDoctor Id: {doc.DoctorId} | Doctor Name : {doc.FirstName} {doc.Lastname} | Specalization : {doc.Specialization} |  Deparmetnt ID : {doc.DepartmentId} | Email : {doc.Email} | Phone : {doc.PhoneNumber}");
            }
        }
        
        public void updateDoctorsDetails()
        {
            // Get Doctor ID
            //Console.Write("Doctor Id: ");
            //int doctorId = Convert.ToInt32(Console.ReadLine());

            Console.Write("\nEnter Doctor Id to update: ");
            if (!int.TryParse(Console.ReadLine(), out int doctorId))
            {
                Console.WriteLine("Invalid Doctor ID format.");
                return;
            }
            List<Doctors> doctors = loadDoctorsFromDb();

           
            // Find doctor from the main list
            Doctors existingDoctor = doctors.Find(d => d.DoctorId == doctorId);

            if (existingDoctor == null)
            {
                Console.WriteLine("Doctor not found.");
                return;
            }
            Console.WriteLine($"\nUpdating Doctor ID: {existingDoctor.DoctorId} (Press Enter to keep current value)");

            // Get new values from user

            Console.Write($"Enter First Name [{existingDoctor.FirstName}]: ");
            string firstNameInput = Console.ReadLine();
            string updatedFirstName = string.IsNullOrWhiteSpace(firstNameInput) ? existingDoctor.FirstName : firstNameInput;
            Console.Write($"Enter Last Name [{existingDoctor.Lastname}]: ");
            string lastNameInput = Console.ReadLine();
            string updatedLastName = string.IsNullOrWhiteSpace(lastNameInput) ? existingDoctor.Lastname : lastNameInput;
            Console.Write($"Enter Specialization [{existingDoctor.Specialization}]: ");
            string specInput = Console.ReadLine();
            string updatedSpecialization = string.IsNullOrWhiteSpace(specInput) ? existingDoctor.Specialization : specInput;
            Console.Write($"Enter Department ID [{existingDoctor.DepartmentId}]: ");
            string deptInput = Console.ReadLine();
            int updatedDeptId = existingDoctor.DepartmentId;
            if (!string.IsNullOrWhiteSpace(deptInput))
            {
                if (int.TryParse(deptInput, out int newDeptId))
                {
                    updatedDeptId = newDeptId;
                }
                else
                {
                    Console.WriteLine("Invalid Department ID format. Keeping previous value.");
                }
            }
            Console.Write($"Enter Email [{existingDoctor.Email}]: ");
            string emailInput = Console.ReadLine();
            string updatedEmail = string.IsNullOrWhiteSpace(emailInput) ? existingDoctor.Email : emailInput;
            Console.Write($"Enter Phone Number [{existingDoctor.PhoneNumber}]: ");
            string phoneInput = Console.ReadLine();
            string updatedPhone = string.IsNullOrWhiteSpace(phoneInput) ? existingDoctor.PhoneNumber : phoneInput;


            /// Update the database
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                string query = @"UPDATE hpl.doctors 
                         SET first_name = @first_name, 
                             last_name = @last_name, 
                             specialization = @specialization, 
                             department_id = @department_id, 
                             email = @email, 
                             phone_number = @phone_number 
                         WHERE doctor_id = @doctor_id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@first_name", updatedFirstName);
                    cmd.Parameters.AddWithValue("@last_name", updatedLastName);
                    cmd.Parameters.AddWithValue("@specialization", updatedSpecialization);
                    cmd.Parameters.AddWithValue("@department_id", updatedDeptId);
                    cmd.Parameters.AddWithValue("@email", updatedEmail);
                    cmd.Parameters.AddWithValue("@phone_number", updatedPhone);
                    cmd.Parameters.AddWithValue("@doctor_id", doctorId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Update the main doctors list only if database update succeeded
                    if (rowsAffected > 0)
                    {
                        existingDoctor.FirstName = updatedFirstName;
                        existingDoctor.Lastname = updatedLastName;
                        existingDoctor.Specialization = updatedSpecialization;
                        existingDoctor.DepartmentId = updatedDeptId;
                        existingDoctor.Email = updatedEmail;
                        existingDoctor.PhoneNumber = updatedPhone;
                        Console.WriteLine("\nDoctor updated successfully in database and in memory!");
                        Console.WriteLine($"Doctor Id: {existingDoctor.DoctorId} | Name: {existingDoctor.FirstName} {existingDoctor.Lastname} | Specialization: {existingDoctor.Specialization} | Dept ID: {existingDoctor.DepartmentId} | Email: {existingDoctor.Email} | Phone: {existingDoctor.PhoneNumber}\n");
                    }
                    else
                    {
                        Console.WriteLine("Doctor was not updated in database.");
                    }
                }
            }
        }

        public void DeleteDoctorsDetails()
        {
            //List<Doctors> doctors = loadDoctorsFromDb();
        }
    }
}
