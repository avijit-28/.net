using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            try
            {
                using (var db = new LMS.Data.ApplicationDbContext())
                {
                    LMS.Data.DbSeeder.Seed(db);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Database seeding error: " + ex.Message);
            }
        }
    }
}
