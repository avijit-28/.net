using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace FirstMVCProject.Models
{
    public class EmployeeExceptionFilter : FilterAttribute, IExceptionFilter
    {
        private static readonly object _fileLock = new object();

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null || filterContext.Exception == null)
            {
                return;
            }

            string controllerName = filterContext.RouteData?.Values["controller"]?.ToString() ?? "Unknown";
            string actionName = filterContext.RouteData?.Values["action"]?.ToString() ?? "Unknown";
            string exceptionMessage = filterContext.Exception.Message;

            Debug.WriteLine("========== EXCEPTION FILTER ==========");
            Debug.WriteLine("Controller: " + controllerName);
            Debug.WriteLine("Action: " + actionName);
            Debug.WriteLine("Exception: " + exceptionMessage);
            Debug.WriteLine("======================================");

            Log(controllerName, actionName, exceptionMessage);

            // Mark exception as handled
            filterContext.ExceptionHandled = true;

            // Show Error view with error info model
            filterContext.Result = new ViewResult
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary<HandleErrorInfo>(
                    new HandleErrorInfo(filterContext.Exception, controllerName, actionName)
                )
            };
        }

        private void Log(string controller, string action, string exceptionMessage)
        {
            try
            {
                string logEntry =
                    "Exception Filter: Controller:" + controller +
                    " Action:" + action +
                    " Exception:" + exceptionMessage +
                    " Date:" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") +
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
                // Silently handle logging exceptions
            }
        }
    }
}
