using System;
using System.Web;

namespace AiProjectMVC.Models
{
    /// <summary>
    /// Helper to enforce role-based authorization for administrative and record management actions.
    /// Rules: Only Admin, HR, and Manager can add employees or addresses.
    /// </summary>
    public static class RoleAuthHelper
    {
        /// <summary>
        /// Checks whether a given role string has privileges to create/add employees and addresses.
        /// Only Admin, HR, and Manager are allowed.
        /// </summary>
        public static bool CanManageRecords(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return false;
            }

            var cleanRole = role.Trim();
            return string.Equals(cleanRole, "Admin", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(cleanRole, "HR", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(cleanRole, "Manager", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks whether the currently logged-in user in Session can manage (add) employee and address records.
        /// </summary>
        public static bool CanManageRecords(HttpSessionStateBase session)
        {
            if (session == null)
            {
                return false;
            }

            var role = session["UserRole"] as string;
            if (string.IsNullOrWhiteSpace(role))
            {
                var user = session["CurrentUser"] as EmployeeUser;
                role = user?.Role;
            }

            return CanManageRecords(role);
        }

        /// <summary>
        /// Helper overload for classic ASP.NET HttpSessionState.
        /// </summary>
        public static bool CanManageRecords(System.Web.SessionState.HttpSessionState session)
        {
            if (session == null)
            {
                return false;
            }
            return CanManageRecords(new HttpSessionStateWrapper(session));
        }

        /// <summary>
        /// Checks whether a user is currently logged into the session.
        /// Even administrators must have an active session to be authenticated.
        /// </summary>
        public static bool IsLoggedIn(HttpSessionStateBase session)
        {
            return session != null && session["CurrentUser"] != null;
        }

        /// <summary>
        /// Retrieves the current user's role from Session, or "Guest".
        /// </summary>
        public static string GetCurrentRole(HttpSessionStateBase session)
        {
            if (session == null) return "Guest";
            var role = session["UserRole"] as string;
            if (string.IsNullOrWhiteSpace(role))
            {
                var user = session["CurrentUser"] as EmployeeUser;
                role = user?.Role;
            }
            return string.IsNullOrWhiteSpace(role) ? "Guest" : role.Trim();
        }

        /// <summary>
        /// Validates whether the employee's City matches the linked Address City or State.
        /// If employee city is Delhi and address is in New Delhi -> Valid.
        /// If employee city is Kolkata and address is in Delhi -> Invalid.
        /// </summary>
        public static bool IsCityMatching(string employeeCity, Address address)
        {
            if (string.IsNullOrWhiteSpace(employeeCity) || address == null)
                return false;

            string empCity = employeeCity.Trim().ToLower();
            string addrCity = (address.City ?? "").Trim().ToLower();
            string addrState = (address.State ?? "").Trim().ToLower();

            // Direct equality
            if (empCity == addrCity) return true;

            // Substring inclusion (e.g. "delhi" in "new delhi", or vice versa)
            if (addrCity.Contains(empCity) || empCity.Contains(addrCity)) return true;

            // State match (e.g. employee typed "delhi" which is the state for "new delhi")
            if (addrState.Contains(empCity) || empCity.Contains(addrState)) return true;

            // Common city aliases in India
            if ((empCity == "bengaluru" && addrCity.Contains("bangalore")) ||
                (empCity == "bangalore" && addrCity.Contains("bengaluru")) ||
                (empCity == "calcutta" && addrCity.Contains("kolkata")) ||
                (empCity == "kolkata" && addrCity.Contains("calcutta")) ||
                (empCity == "bombay" && addrCity.Contains("mumbai")) ||
                (empCity == "mumbai" && addrCity.Contains("bombay")) ||
                (empCity == "madras" && addrCity.Contains("chennai")) ||
                (empCity == "chennai" && addrCity.Contains("madras")))
            {
                return true;
            }

            return false;
        }
    }
}
