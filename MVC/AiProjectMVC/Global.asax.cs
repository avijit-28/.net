using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace AiProjectMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            var authCookie = HttpContext.Current?.Request?.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null && !string.IsNullOrWhiteSpace(authCookie.Value))
            {
                try
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    if (authTicket != null && !authTicket.Expired)
                    {
                        var roles = (authTicket.UserData ?? string.Empty)
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(r => r.Trim())
                            .ToArray();

                        HttpContext.Current.User = new GenericPrincipal(new FormsIdentity(authTicket), roles);
                    }
                }
                catch
                {
                    // Ignore decryption failures from stale or invalid cookies
                }
            }
        }
    }
}
