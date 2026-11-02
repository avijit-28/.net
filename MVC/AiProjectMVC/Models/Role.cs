using System;

namespace AiProjectMVC.Models
{
    /// <summary>
    /// Role enumeration used for custom role-based authorization filter (AuthorizeRoleAttribute).
    /// Matches the roles in the USERS table and system authorization architecture.
    /// </summary>
    public enum Role
    {
        SUPERADMIN = 1,
        ADMIN = 2,
        USER = 3,
        HR = 4,
        MANAGER = 5,
        EMPLOYEE = 6
    }
}
