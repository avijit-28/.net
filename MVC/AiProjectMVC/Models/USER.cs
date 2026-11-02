using System;

namespace AiProjectMVC.Models
{
    /// <summary>
    /// Model mapping to the [Assignmet].[dbo].[USERS] table.
    /// Stores login credentials and comma-separated Roles.
    /// </summary>
    public class USER
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string Roles { get; set; }
        public string Password { get; set; }
    }
}
