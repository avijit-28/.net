using System;
using System.Collections.Generic;
using System.Linq;
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
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null || String.IsNullOrEmpty(cookie.Value)) return;
            try
            {
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                if (ticket == null || ticket.Expired) return;
                Context.User = new GenericPrincipal(new FormsIdentity(ticket), new[] { ticket.UserData });
            }
            catch (Exception) { FormsAuthentication.SignOut(); }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
