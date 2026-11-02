using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AiProjectMVC.Models
{
    /// <summary>
    /// Custom Role Authorization Filter attribute inheriting from AuthorizeAttribute.
    /// Accepts strongly typed Role enum values and restricts access based on authenticated user roles.
    /// Redirects unauthenticated users to ~/Account/Login and unauthorized users to ~/Account/AccessDenied.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        public AuthorizeRoleAttribute(params object[] roles)
        {
            if (roles != null && roles.Length > 0)
            {
                if (roles.Any(r => r.GetType().BaseType != typeof(Enum)))
                {
                    throw new ArgumentException("Roles must be enum values of Role type.", "roles");
                }

                this.Roles = string.Join(",", roles.Select(r => Enum.GetName(r.GetType(), r)));
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null) return false;

            // First check built-in principal roles (FormsAuthentication ticket)
            bool isAuthorized = base.AuthorizeCore(httpContext);
            if (isAuthorized) return true;

            // Fallback check against Session user role if principal is not yet hydrated
            if (httpContext.Session != null)
            {
                var sessionRole = httpContext.Session["UserRole"] as string;
                if (!string.IsNullOrWhiteSpace(sessionRole) && !string.IsNullOrWhiteSpace(this.Roles))
                {
                    var allowedRoles = this.Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(r => r.Trim());

                    // Check comma-separated roles
                    var userRoles = sessionRole.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(r => r.Trim());

                    if (userRoles.Any(ur => allowedRoles.Any(ar => string.Equals(ar, ur, StringComparison.OrdinalIgnoreCase))))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null) return;

            // Respect AllowAnonymous on controller or action
            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                                  || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);

            if (skipAuthorization)
            {
                return;
            }

            base.OnAuthorization(filterContext);

            // If not logged in, redirect to Login
            bool isAuthenticated = filterContext.HttpContext.User != null 
                                && filterContext.HttpContext.User.Identity != null 
                                && filterContext.HttpContext.User.Identity.IsAuthenticated;

            if (!isAuthenticated && filterContext.HttpContext.Session?["CurrentUser"] == null)
            {
                filterContext.Result = new RedirectResult("~/Account/Login?returnUrl=" + HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl));
                return;
            }

            // If authenticated but unauthorized for this role, redirect to AccessDenied
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectResult("~/Account/AccessDenied");
                return;
            }
        }
    }
}
