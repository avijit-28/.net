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
            //filters.Add(new LogFilter());
        }
    }
}
