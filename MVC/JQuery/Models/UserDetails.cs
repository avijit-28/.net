using System.Collections.Generic;

namespace JQuery.Models
{
    public class UserDetails
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Education { get; set; }
        public string Location { get; set; }
        public List<UserDetails> usersinfo { get; set; }
    }
}
