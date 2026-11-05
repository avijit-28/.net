using System;
using System.Web.Http;
using WebApiPractice2.Models.BEL;
using WebApiPractice2.Models.BLL;

namespace WebApiPractice2.Controllers
{
    public class UserController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Loging(int UserID, string UserPassword)
        {
            string message = string.Empty;
            Users user = new Users();
            try
            {
                user = new UserService().getLoginDetails(UserID, UserPassword);
                if (user != null && user.UserID > 0)
                {
                    message = "Login successful!";
                }
                else
                {
                    message = "Invalid UserID or Password.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Ok(new { message, user });
        }

        [HttpPost]
        public IHttpActionResult InsertUser([FromBody] Users user)
        {
            string message = string.Empty;
            try
            {
                if (user == null)
                {
                    return BadRequest("User details are required.");
                }
                message = new UserService().InsertUser(user);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Ok(new { message });
        }

        [HttpPut]
        public IHttpActionResult UpdateUser(int UserID, [FromBody] Users user)
        {
            string message = string.Empty;
            try
            {
                if (user == null)
                {
                    return BadRequest("User details are required.");
                }
                message = new UserService().UpdateUser(UserID, user);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Ok(new { message });
        }

        [HttpDelete]
        public IHttpActionResult DeleteUser(int UserID)
        {
            string message = string.Empty;
            try
            {
                message = new UserService().DeleteUser(UserID);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Ok(new { message });
        }

        [HttpGet]
        [Route("api/User/GetAllUsers")]
        public IHttpActionResult GetAllUsers()
        {
            try
            {
                var list = new UserService().GetAllUsers();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
