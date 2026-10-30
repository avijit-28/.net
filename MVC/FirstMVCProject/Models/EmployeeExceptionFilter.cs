using System.Diagnostics;
using System.Web.Mvc;

namespace FirstMVCProject.Models
{
    public class EmployeeExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            Debug.WriteLine("========== EXCEPTION FILTER ==========");

            Debug.WriteLine(
                "Controller: " +
                filterContext.RouteData.Values["controller"]
            );

            Debug.WriteLine(
                "Action: " +
                filterContext.RouteData.Values["action"]
            );

            Debug.WriteLine(
                "Exception: " +
                filterContext.Exception.Message
            );

            Debug.WriteLine("======================================");
        }
    }
}