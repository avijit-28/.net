using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace FirstMVCProject.Models
{

    public class AddressRepo
    {
        private readonly string _conStr =
            ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;


        //Insert Address
        public void InsertAddress(Address address)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand("InsertAddress", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Country", address.Country);
                    cmd.Parameters.AddWithValue("@State", address.State);
                    cmd.Parameters.AddWithValue("@City", address.City);
                    cmd.Parameters.AddWithValue("@Pin", address.Pin);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Get all Address
        public List<Address> GetAllAddress()
        {
            List<Address> addresses = new List<Address>();

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand("GetAllAddress", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Address address = new Address
                            {
                                AddressId = Convert.ToInt32(reader["AddressId"]),
                                Country = reader["Country"].ToString(),
                                State = reader["State"].ToString(),
                                City = reader["City"].ToString(),
                                Pin = reader["Pin"].ToString()
                            };

                            addresses.Add(address);
                        }
                    }
                }
            }

            return addresses;
        }

        // Get by Address ID
        public Address GetAddressById(int addressId)
        {
            Address address = null;

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand("GetAddressById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@AddressId", addressId);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            address = new Address
                            {
                                AddressId = Convert.ToInt32(reader["AddressId"]),
                                Country = reader["Country"].ToString(),
                                State = reader["State"].ToString(),
                                City = reader["City"].ToString(),
                                Pin = reader["Pin"].ToString()
                            };
                        }
                    }
                }
            }

            return address;
        }

        // Update Address
        public void UpdateAddress(Address address)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateAddress", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@AddressId", address.AddressId);
                    cmd.Parameters.AddWithValue("@Country",
                        (object)address.Country ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@State",
                        (object)address.State ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City",
                        (object)address.City ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pin",
                        (object)address.Pin ?? DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Delete Address
        public void DeleteAddress(int addressId)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteAddress", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@AddressId", addressId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }




    }

}