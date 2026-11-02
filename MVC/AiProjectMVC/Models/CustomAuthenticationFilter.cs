using System;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace AiProjectMVC.Models
{
    /// <summary>
    /// Custom Authentication Filter implementing IAuthenticationFilter and ActionFilterAttribute.
    /// Separates authentication concerns from authorization filters.
    /// Runs OnAuthentication first to inspect credentials/session, and OnAuthenticationChallenge
    /// to redirect unauthenticated requests to the Login page.
    /// </summary>
    public class CustomAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (filterContext == null) return;

            // Check if user is authenticated via Session or Forms Authentication
            bool hasSessionUser = !string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session?["UserName"]));
            bool hasPrincipalUser = filterContext.HttpContext.User != null && filterContext.HttpContext.User.Identity.IsAuthenticated;

            if (!hasSessionUser && !hasPrincipalUser)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext == null) return;

            // If authentication failed or unauthorized result, redirect to Account/Login
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Account" },
                        { "action", "Login" },
                        { "returnUrl", filterContext.HttpContext.Request.RawUrl }
                    });
            }
        }
    }
}
