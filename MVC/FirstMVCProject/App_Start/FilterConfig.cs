using System.Web;
using System.Web.Mvc;
using FirstMVCProject.Models;

namespace FirstMVCProject
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // To register filters globally for all controllers across the whole application:
            // filters.Add(new EmployeeAuditFilter());
        }
    }
}
