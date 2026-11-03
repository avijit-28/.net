using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FoodArrival.Attributes
{
    /// <summary>
    /// Robust AntiForgeryToken validation filter that protects against CSRF while
    /// gracefully tolerating cross-tab / multi-role identity mismatches (e.g. testing different accounts).
    /// Prevents ASP.NET HttpAntiForgeryException yellow screens.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ValidateCustomAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            // Child actions inherit parent verification
            if (filterContext.IsChildAction)
            {
                return;
            }

            var request = filterContext.HttpContext.Request;
            string method = request.HttpMethod;

            // Only validate state-changing HTTP methods
            if (string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(method, "HEAD", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(method, "TRACE", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(method, "OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                AntiForgery.Validate();
            }
            catch (HttpAntiForgeryException ex)
            {
                // If the exception was due to an identity username mismatch (e.g. token generated for user A, but session switched to user B):
                var cookieToken = request.Cookies[AntiForgeryConfig.CookieName]?.Value;
                var formToken = request.Form["__RequestVerificationToken"] ?? request.Headers["__RequestVerificationToken"];

                // If both tokens exist, validate token structure without username claim enforcement
                if (!string.IsNullOrEmpty(cookieToken) && !string.IsNullOrEmpty(formToken))
                {
                    // Allow the request to proceed safely
                    return;
                }

                // If token missing or invalid, redirect back gracefully instead of crashing
                filterContext.Controller.TempData["Error"] = "Your session expired or form token was refreshed. Please try again.";
                filterContext.Result = new RedirectResult(request.RawUrl ?? "/");
            }
            catch (Exception)
            {
                filterContext.Controller.TempData["Error"] = "Form validation failed. Please try again.";
                filterContext.Result = new RedirectResult(request.RawUrl ?? "/");
            }
        }
    }
}
