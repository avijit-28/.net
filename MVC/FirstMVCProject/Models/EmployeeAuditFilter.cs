using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FirstMVCProject.Models
{
    public class EmployeeAuditFilter : ActionFilterAttribute
    {
        private static readonly object _fileLock = new object();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext?.ActionDescriptor?.ActionName ?? "Unknown";
            string message = "Employee action started: " + actionName;

            Debug.WriteLine(message);
            Log(message, filterContext?.RouteData);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string actionName = filterContext?.ActionDescriptor?.ActionName ?? "Unknown";
            string message = "Employee action completed: " + actionName;

            Debug.WriteLine(message);
            Log(message, filterContext?.RouteData);
        }
        public override void OnResultExecuting(
    ResultExecutingContext filterContext)
        {
            string actionName =
                filterContext?.RouteData?.Values["action"]?.ToString()
                ?? "Unknown";

            string message =
                "Employee result started: " + actionName;

            Debug.WriteLine(message);
            Log(message, filterContext?.RouteData);
        }

        public override void OnResultExecuted(
    ResultExecutedContext filterContext)
        {
            string actionName =
                filterContext?.RouteData?.Values["action"]?.ToString()
                ?? "Unknown";

            string message =
                "Employee result completed: " + actionName;

            Debug.WriteLine(message);
            Log(message, filterContext?.RouteData);
        }


        private void Log(string status, RouteData routeData)
        {
            try
            {
                var controllerName = routeData?.Values["controller"] ?? "Employee";
                var actionName = routeData?.Values["action"] ?? "Unknown";

                string logEntry =
                    status +
                    " Controller:" +
                    controllerName +
                    " Action:" +
                    actionName +
                    " Date:" +
                    DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") +
                    Environment.NewLine;

                var context = HttpContext.Current;
                if (context != null && context.Server != null)
                {
                    string filePath = context.Server.MapPath("~/Log/Log.txt");
                    string dir = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    lock (_fileLock)
                    {
                        File.AppendAllText(filePath, logEntry);
                    }
                }
            }
            catch
            {
                // Silently handle logging exceptions so web requests are not broken
            }
        }
    }
}
