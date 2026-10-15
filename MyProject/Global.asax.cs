using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace MyProject
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
        }
        public static void RegisterRoutes(RouteCollection routeCollection)
        {
            routeCollection.MapPageRoute("Customers", "Customers", "~/Customers.aspx");
            routeCollection.MapPageRoute("CustomerDetails", "Customers/{CustomerId}", "~/CustomerDetails.aspx");
        }
    }
}