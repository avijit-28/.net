using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4
{
    /// <summary>
    /// Model class representing a User entity in the UserDetails database table.
    /// </summary>
    public class UserDetails
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string City { get; set; }

        public string Mobile
        {
            get => MobileNo;
            set => MobileNo = value;
        }
    }
}
