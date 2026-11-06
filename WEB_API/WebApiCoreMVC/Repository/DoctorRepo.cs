using System.Data;
using Microsoft.Data.SqlClient;
using WebApiCoreMVC.Models;

namespace WebApiCoreMVC.Repository
{
    public class DoctorRepo : IDoctorRepo
    {
        private readonly string _connectionString;

        public DoctorRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
        }

        public async Task<(IEnumerable<Doctor> Doctors, string Message)> GetAllDoctorsAsync()
        {
            var doctors = new List<Doctor>();
            string message = string.Empty;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("get_AllDoctorDetails", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                var msgParam = new SqlParameter("@msg", SqlDbType.VarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(msgParam);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        doctors.Add(MapDoctorFromReader(reader));
                    }
                }

                message = msgParam.Value?.ToString() ?? "Doctors fetched successfully.";
            }

            return (doctors, message);
        }

        public async Task<(Doctor? Doctor, string Message)> GetDoctorByIdAsync(int id)
        {
            Doctor? doctor = null;
            string message = string.Empty;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("get_DoctorDetails", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", id);

                var msgParam = new SqlParameter("@msg", SqlDbType.VarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(msgParam);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        doctor = MapDoctorFromReader(reader);
                    }
                }

                message = msgParam.Value?.ToString() ?? "Doctor details fetched.";
            }

            return (doctor, message);
        }

        public async Task<string> AddDoctorAsync(Doctor doctor)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("set_InsertDoctorDetails", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@first_name", doctor.FirstName);
                cmd.Parameters.AddWithValue("@last_name", doctor.LastName);
                cmd.Parameters.AddWithValue("@specialization", doctor.Specialization);
                cmd.Parameters.AddWithValue("@department_id", doctor.DepartmentId);
                cmd.Parameters.AddWithValue("@email", doctor.Email);
                cmd.Parameters.AddWithValue("@phone_number", doctor.PhoneNumber);

                var msgParam = new SqlParameter("@msg", SqlDbType.VarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(msgParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return msgParam.Value?.ToString() ?? "Doctor added successfully.";
            }
        }

        public async Task<string> UpdateDoctorAsync(Doctor doctor)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("UpdateDoctorDetails", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", doctor.DoctorId);
                cmd.Parameters.AddWithValue("@first_name", (object?)doctor.FirstName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@last_name", (object?)doctor.LastName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@specialization", (object?)doctor.Specialization ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@department_id", (object?)doctor.DepartmentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@email", (object?)doctor.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@phone_number", (object?)doctor.PhoneNumber ?? DBNull.Value);

                var msgParam = new SqlParameter("@msg", SqlDbType.VarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(msgParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return msgParam.Value?.ToString() ?? "Doctor updated successfully.";
            }
        }

        public async Task<string> DeleteDoctorAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("set_DeleteDoctorDetails", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", id);

                var msgParam = new SqlParameter("@msg", SqlDbType.VarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(msgParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return msgParam.Value?.ToString() ?? "Doctor deleted successfully.";
            }
        }

        private static Doctor MapDoctorFromReader(SqlDataReader reader)
        {
            return new Doctor
            {
                DoctorId = reader["doctor_id"] != DBNull.Value ? Convert.ToInt32(reader["doctor_id"]) : 0,
                FirstName = reader["first_name"] != DBNull.Value ? reader["first_name"].ToString()! : string.Empty,
                LastName = reader["last_name"] != DBNull.Value ? reader["last_name"].ToString()! : string.Empty,
                Specialization = reader["specialization"] != DBNull.Value ? reader["specialization"].ToString()! : string.Empty,
                DepartmentId = reader["department_id"] != DBNull.Value ? Convert.ToInt32(reader["department_id"]) : 0,
                Email = reader["email"] != DBNull.Value ? reader["email"].ToString()! : string.Empty,
                PhoneNumber = reader["phone_number"] != DBNull.Value ? reader["phone_number"].ToString()! : string.Empty
            };
        }
    }
}
