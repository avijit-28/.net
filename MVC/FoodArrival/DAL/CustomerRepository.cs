using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FoodArrival.Models;

namespace FoodArrival.DAL
{
    public class CustomerRepository
    {
        public bool Insert(Customer customer, out string message, out int customerId)
        {
            message = string.Empty;
            customerId = 0;

            var parameters = new[]
            {
                new SqlParameter("@CustomerName", customer.CustomerName ?? string.Empty),
                new SqlParameter("@Email", customer.Email ?? string.Empty),
                new SqlParameter("@PasswordHash", customer.PasswordHash ?? string.Empty),
                new SqlParameter("@PhoneNumber", (object)customer.PhoneNumber ?? DBNull.Value),
                new SqlParameter("@Address", (object)customer.Address ?? DBNull.Value)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_Customer_Insert", parameters);
            if (dt.Rows.Count > 0)
            {
                int result = Convert.ToInt32(dt.Rows[0]["Result"]);
                message = dt.Rows[0]["Message"].ToString();
                if (result == 1)
                {
                    if (dt.Rows[0]["CustomerId"] != DBNull.Value)
                    {
                        customerId = Convert.ToInt32(dt.Rows[0]["CustomerId"]);
                    }
                    return true;
                }
            }
            return false;
        }

        public Customer Login(string email, string password)
        {
            var parameters = new[]
            {
                new SqlParameter("@Email", email ?? string.Empty),
                new SqlParameter("@PasswordHash", password ?? string.Empty)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_Customer_Login", parameters);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new Customer
                {
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    CustomerName = row["CustomerName"].ToString(),
                    Email = row["Email"].ToString(),
                    PhoneNumber = row["PhoneNumber"] != DBNull.Value ? row["PhoneNumber"].ToString() : null,
                    Address = row["Address"] != DBNull.Value ? row["Address"].ToString() : null,
                    IsActive = true
                };
            }
            return null;
        }

        public List<Customer> GetAll()
        {
            var list = new List<Customer>();
            var dt = DbHelper.ExecuteDataTable("dbo.sp_Customer_GetAll");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Customer
                {
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    CustomerName = row["CustomerName"].ToString(),
                    Email = row["Email"].ToString(),
                    PhoneNumber = row["PhoneNumber"] != DBNull.Value ? row["PhoneNumber"].ToString() : null,
                    Address = row["Address"] != DBNull.Value ? row["Address"].ToString() : null,
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                });
            }
            return list;
        }

        public Customer GetById(int customerId)
        {
            var parameters = new[]
            {
                new SqlParameter("@CustomerId", customerId)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_Customer_GetById", parameters);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new Customer
                {
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    CustomerName = row["CustomerName"].ToString(),
                    Email = row["Email"].ToString(),
                    PhoneNumber = row["PhoneNumber"] != DBNull.Value ? row["PhoneNumber"].ToString() : null,
                    Address = row["Address"] != DBNull.Value ? row["Address"].ToString() : null,
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                };
            }
            return null;
        }

        public bool Update(Customer customer, out string message)
        {
            message = string.Empty;
            var parameters = new[]
            {
                new SqlParameter("@CustomerId", customer.CustomerId),
                new SqlParameter("@CustomerName", customer.CustomerName ?? string.Empty),
                new SqlParameter("@PhoneNumber", (object)customer.PhoneNumber ?? DBNull.Value),
                new SqlParameter("@Address", (object)customer.Address ?? DBNull.Value)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_Customer_Update", parameters);
            if (dt.Rows.Count > 0)
            {
                int result = Convert.ToInt32(dt.Rows[0]["Result"]);
                message = dt.Rows[0]["Message"].ToString();
                return result == 1;
            }
            return false;
        }

        public bool Delete(int customerId, out string message)
        {
            message = string.Empty;
            var parameters = new[]
            {
                new SqlParameter("@CustomerId", customerId)
            };

            var dt = DbHelper.ExecuteDataTable("dbo.sp_Customer_Delete", parameters);
            if (dt.Rows.Count > 0)
            {
                int result = Convert.ToInt32(dt.Rows[0]["Result"]);
                message = dt.Rows[0]["Message"].ToString();
                return result == 1;
            }
            return false;
        }
    }
}
