using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Security.Principal;
using System.Web.Security;

namespace FoodArrival
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_PostAuthenticateRequest()
        {
            // Do not bind Context.User to email to prevent cross-account AntiForgery token identity mismatches
            // when testing or switching between Customer, Rider, Staff, and Admin roles in the same browser session.
            if (Context.User == null || !Context.User.Identity.IsAuthenticated)
            {
                Context.User = new GenericPrincipal(new GenericIdentity(string.Empty), new string[0]);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex is HttpAntiForgeryException || (ex != null && ex.InnerException is HttpAntiForgeryException))
            {
                Server.ClearError();
                Response.Clear();
                FormsAuthentication.SignOut();
                Session.Clear();
                Response.Redirect("~/Account/Login");
            }
        }

        protected void Application_Start()
        {
            // Suppress identity heuristic check to prevent HttpAntiForgeryException during account switches or multi-role sessions
            System.Web.Helpers.AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
