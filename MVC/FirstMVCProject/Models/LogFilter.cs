using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FirstMVCProject.Models
{
    public class LogFilter : ActionFilterAttribute
    {
        private static readonly object _fileLock = new object();

        public override void OnActionExecuting(
            ActionExecutingContext filterContext)
        {
            Log(
                "OnActionExecuting",
                filterContext.RouteData
            );
        }

        public override void OnActionExecuted(
            ActionExecutedContext filterContext)
        {
            Log(
                "OnActionExecuted",
                filterContext.RouteData
            );
        }

        public override void OnResultExecuting(
            ResultExecutingContext filterContext)
        {
            Log(
                "OnResultExecuting",
                filterContext.RouteData
            );
        }

        public override void OnResultExecuted(
            ResultExecutedContext filterContext)
        {
            Log(
                "OnResultExecuted",
                filterContext.RouteData
            );
        }

        private void Log(
            string methodName,
            RouteData routeData)
        {
            try
            {
                var controllerName = routeData?.Values["controller"];
                var actionName = routeData?.Values["action"];

                string message =
                    methodName +
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
                        File.AppendAllText(filePath, message);
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
