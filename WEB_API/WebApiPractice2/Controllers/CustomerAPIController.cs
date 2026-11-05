using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiPractice2.Models.BEL;

namespace WebApiPractice2.Controllers
{
    public class CustomerAPIController : ApiController
    {
        private string _conStr;

        public CustomerAPIController()
        {
            var conSetting = ConfigurationManager.ConnectionStrings["DBConn"] 
                          ?? ConfigurationManager.ConnectionStrings["DBCon"];
            _conStr = conSetting != null ? conSetting.ToString() : string.Empty;
        }

        [HttpGet]
        [Route("api/CustomerAPI/GetCustomers")]
        public IHttpActionResult GetCustomers()
        {
            List<Customer> customerList = new List<Customer>();
            using (SqlConnection _myConn = new SqlConnection(_conStr))
            {
                SqlCommand command = new SqlCommand("spGetCustomers", _myConn);
                command.CommandType = CommandType.StoredProcedure;
                _myConn.Open();
                SqlDataReader result = command.ExecuteReader();
                while (result.Read())
                {
                    customerList.Add(new Customer
                    {
                        CustomerId = Convert.ToInt32(result["CustomerId"].ToString()),
                        Name = result["Name"].ToString(),
                        MobileNo = result["MobileNo"].ToString(),
                        EmailID = result["EmailID"].ToString(),
                        Country = result["Country"].ToString()
                    });
                }
                _myConn.Close();
            }
            return Ok(customerList);
        }

        [HttpGet]
        [Route("api/CustomerAPI/GetCustomerByID")]
        [Route("api/CustomerAPI/GetCustomerByID/{id}")]
        public HttpResponseMessage GetCustomerByID(int id)
        {
            Customer customer = null;
            using (SqlConnection _myConn = new SqlConnection(_conStr))
            {
                SqlCommand command = new SqlCommand("spGetCustomerByID", _myConn);
                command.Parameters.AddWithValue("@CustomerId", id);
                command.CommandType = CommandType.StoredProcedure;
                _myConn.Open();
                SqlDataReader result = command.ExecuteReader();
                while (result.Read())
                {
                    customer = new Customer
                    {
                        CustomerId = Convert.ToInt32(result["CustomerId"].ToString()),
                        Name = result["Name"].ToString(),
                        MobileNo = result["MobileNo"].ToString(),
                        EmailID = result["EmailID"].ToString(),
                        Country = result["Country"].ToString()
                    };
                }
                _myConn.Close();
            }
            if (customer != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, customer);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Customer with id " + id + " not found!!");
            }
        }

        [HttpPost]
        [Route("api/CustomerAPI/InsertCustomer")]
        public HttpResponseMessage InsertCustomer([FromBody] Customer customer)
        {
            try
            {
                if (customer == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Customer data is missing.");
                }

                using (SqlConnection _sqlCon = new SqlConnection(_conStr))
                {
                    SqlCommand command = new SqlCommand("spInsertCustomer", _sqlCon);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Name", customer.Name ?? string.Empty);
                    command.Parameters.AddWithValue("@MobileNo", customer.MobileNo ?? string.Empty);
                    command.Parameters.AddWithValue("@EmailID", customer.EmailID ?? string.Empty);
                    command.Parameters.AddWithValue("@Country", customer.Country ?? string.Empty);
                    
                    SqlParameter outParam = new SqlParameter("@CustomerID", SqlDbType.Int);
                    outParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(outParam);

                    _sqlCon.Open();
                    command.ExecuteNonQuery();

                    int custid = Convert.ToInt32(command.Parameters["@CustomerID"].Value);
                    var message = Request.CreateResponse(HttpStatusCode.Created, "Customer with ID " + custid + " saved successfully.");
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPut]
        [Route("api/CustomerAPI/UpdateCustomer/{id}")]
        [Route("api/CustomerAPI/UpdateCustomer")]
        public HttpResponseMessage UpdateCustomer(int id, [FromBody] Customer customer)
        {
            try
            {
                if (customer == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Customer data is required.");
                }

                using (SqlConnection _sqlCon = new SqlConnection(_conStr))
                {
                    SqlCommand _command = new SqlCommand("spUpdateCustomer", _sqlCon);
                    _command.CommandType = CommandType.StoredProcedure;
                    _command.Parameters.AddWithValue("@ID", id);
                    _command.Parameters.AddWithValue("@Name", customer.Name ?? string.Empty);
                    _command.Parameters.AddWithValue("@MobileNo", customer.MobileNo ?? string.Empty);
                    _command.Parameters.AddWithValue("@EmailID", customer.EmailID ?? string.Empty);
                    _command.Parameters.AddWithValue("@Country", customer.Country ?? string.Empty);

                    _sqlCon.Open();
                    int? result = _command.ExecuteNonQuery();
                    if (result == null || result == 0)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Customer with Id " + id + " not found to update");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Customer with " + id + " updated successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpDelete]
        [Route("api/CustomerAPI/DeleteCustomer/{id}")]
        [Route("api/CustomerAPI/DeleteCustomer")]
        public HttpResponseMessage DeleteCustomer(int id)
        {
            try
            {
                using (SqlConnection _sqlCon = new SqlConnection(_conStr))
                {
                    SqlCommand _command = new SqlCommand("spDeleteCustomer", _sqlCon);
                    _command.CommandType = CommandType.StoredProcedure;
                    _command.Parameters.AddWithValue("@ID", id);
                    _sqlCon.Open();
                    int? result = _command.ExecuteNonQuery();
                    if (result == null || result == 0)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Customer with Id = " + id + " not found to delete");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Customer with " + id + " deleted successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
