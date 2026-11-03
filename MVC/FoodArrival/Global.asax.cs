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
                var roleData = (ticket.UserData ?? "").Split('|');
                var roles = new System.Collections.Generic.List<string>();
                if (roleData.Length > 0 && !String.IsNullOrWhiteSpace(roleData[0])) roles.Add(roleData[0]);
                if (roleData.Length > 1 && roleData[0] == "Staff")
                {
                    if (roleData[1] == "Restaurant") roles.Add("RestaurantStaff");
                    if (roleData[1] == "DeliveryAgent") roles.Add("DeliveryAgent");
                }
                Context.User = new GenericPrincipal(new FormsIdentity(ticket), roles.ToArray());
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
