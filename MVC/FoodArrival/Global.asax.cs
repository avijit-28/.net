using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Security.Principal;
using System.Web.Security;
using System.Web.Helpers;

namespace FoodArrival
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // Clear any lingering Forms auth cookies
            if (Request.Cookies[".ASPXAUTH"] != null)
            {
                var deadCookie = new HttpCookie(".ASPXAUTH", string.Empty)
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Path = "/"
                };
                Response.Cookies.Add(deadCookie);
            }

            // In our session-based multi-role system, keep Context.User identity name empty
            // so AntiForgeryToken strictly validates tokens without throwing identity-mismatch exceptions across roles.
            Context.User = new GenericPrincipal(new GenericIdentity(string.Empty), new string[0]);
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            Context.User = new GenericPrincipal(new GenericIdentity(string.Empty), new string[0]);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            var baseEx = ex?.GetBaseException();

            if (ex is HttpAntiForgeryException || baseEx is HttpAntiForgeryException)
            {
                Server.ClearError();
                Response.Clear();
                FormsAuthentication.SignOut();

                string role = Session?["UserRole"]?.ToString();
                if (role == "Rider") Response.Redirect("~/Rider/Dashboard");
                else if (role == "Restaurant") Response.Redirect("~/Restaurant/Dashboard");
                else if (role == "RestaurantStaff") Response.Redirect("~/Staff/Dashboard");
                else if (role == "Admin") Response.Redirect("~/Admin/Dashboard");
                else if (role == "Customer") Response.Redirect("~/Food/Index");
                else Response.Redirect("~/Account/Login");
            }
        }

        protected void Application_Start()
        {
            // Suppress identity heuristic check to prevent HttpAntiForgeryException during account switches or multi-role sessions
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
