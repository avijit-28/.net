using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AiProjectMVC.Models
{
    public class AddressRepo
    {
        private readonly string _conStr;

        public AddressRepo()
        {
            var connSetting = ConfigurationManager.ConnectionStrings["DBConn"];
            _conStr = connSetting != null ? connSetting.ConnectionString : string.Empty;
        }

        // Insert Address
        public void InsertAddress(Address address)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                using (SqlCommand cmd = new SqlCommand("InsertAddress", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Country", (object)address.Country ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@State", (object)address.State ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", (object)address.City ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pin", (object)address.Pin ?? DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Get All Addresses
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
                                AddressId = reader["AddressId"] != DBNull.Value ? Convert.ToInt32(reader["AddressId"]) : 0,
                                Country = reader["Country"] != DBNull.Value ? reader["Country"].ToString() : string.Empty,
                                State = reader["State"] != DBNull.Value ? reader["State"].ToString() : string.Empty,
                                City = reader["City"] != DBNull.Value ? reader["City"].ToString() : string.Empty,
                                Pin = reader["Pin"] != DBNull.Value ? reader["Pin"].ToString() : string.Empty
                            };

                            addresses.Add(address);
                        }
                    }
                }
            }

            return addresses;
        }

        // Get Address by ID
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
                                AddressId = reader["AddressId"] != DBNull.Value ? Convert.ToInt32(reader["AddressId"]) : 0,
                                Country = reader["Country"] != DBNull.Value ? reader["Country"].ToString() : string.Empty,
                                State = reader["State"] != DBNull.Value ? reader["State"].ToString() : string.Empty,
                                City = reader["City"] != DBNull.Value ? reader["City"].ToString() : string.Empty,
                                Pin = reader["Pin"] != DBNull.Value ? reader["Pin"].ToString() : string.Empty
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
                    cmd.Parameters.AddWithValue("@Country", (object)address.Country ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@State", (object)address.State ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", (object)address.City ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pin", (object)address.Pin ?? DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Delete Address
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
