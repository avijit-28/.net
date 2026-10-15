using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;

namespace WebApplication4
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Application["user"] = 0;
            RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterRoutes(RouteCollection routeCollection)
        {
            routeCollection.Ignore("{resource}.axd/{*pathInfo}");

            // Clean, friendly extensionless page routes
            routeCollection.MapPageRoute("HomeRoute", "Home", "~/Default.aspx");
            routeCollection.MapPageRoute("CheckAvailabilityRoute", "CheckAvailability", "~/CheckAvailability.aspx");
            routeCollection.MapPageRoute("CheckAvailabilitySlug", "check-availability", "~/CheckAvailability.aspx");
            routeCollection.MapPageRoute("SignUpRoute", "SignUp", "~/SignUp.aspx");
            routeCollection.MapPageRoute("DashboardSlug", "dashboard", "~/SignUp.aspx");
            routeCollection.MapPageRoute("ProductFormRoute", "ProductForm", "~/ProductForm.aspx");
            routeCollection.MapPageRoute("ProductsSlug", "products", "~/ProductForm.aspx");
            routeCollection.MapPageRoute("SearchUserRoute", "SearchUser", "~/SearchUser.aspx");
            routeCollection.MapPageRoute("SearchSlug", "search", "~/SearchUser.aspx");
            routeCollection.MapPageRoute("GetUserRoute", "GetUser", "~/GetUser.aspx");
            routeCollection.MapPageRoute("CookiesSendRoute", "CookiesSend", "~/CookiesSend.aspx");
            routeCollection.MapPageRoute("CookiesGetRoute", "CookiesGet", "~/CookiesGet.aspx");
            routeCollection.MapPageRoute("CountingRoute", "Counting", "~/Counting.aspx");
            routeCollection.MapPageRoute("CounterSlug", "counter", "~/Counting.aspx");
            routeCollection.MapPageRoute("FirstPageRoute", "FirstPage", "~/FirstPage.aspx");
            routeCollection.MapPageRoute("LoginSlug", "login", "~/FirstPage.aspx");
            routeCollection.MapPageRoute("URLRoutingRoute", "URLRouting", "~/URLRouting.aspx");

            // Dynamic Article Routes
            routeCollection.MapPageRoute("RouteForArticle", "Articles/{id}/{Title}", "~/DetailPage.aspx");
            routeCollection.MapPageRoute("RouteUrlTitle", "Articles/{id}/{Title}", "~/DetailPage.aspx");
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Application.Lock();
            Application["user"] = ((int?)Application["user"] ?? 0) + 1;
            Application.UnLock();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string path = Request.AppRelativeCurrentExecutionFilePath;

            // Automatically redirect direct GET requests ending with .aspx to clean extensionless routes
            if (!string.IsNullOrEmpty(path) &&
                path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase) &&
                !path.EndsWith(".axd", StringComparison.OrdinalIgnoreCase) &&
                Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                string cleanPath = path.Substring(0, path.Length - 5);
                if (cleanPath.Equals("~/Default", StringComparison.OrdinalIgnoreCase))
                {
                    cleanPath = "~/";
                }

                string targetUrl = VirtualPathUtility.ToAbsolute(cleanPath);
                if (!string.IsNullOrEmpty(Request.Url.Query))
                {
                    targetUrl += Request.Url.Query;
                }

                Response.RedirectPermanent(targetUrl, true);
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            int count = (int?)Application["user"] ?? 0;
            if (count > 0)
            {
                Application["user"] = count - 1;
            }
            Application.UnLock();
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}