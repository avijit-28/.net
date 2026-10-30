using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FirstMVCProject.Models
{
    public class LogFilter : ActionFilterAttribute
    {
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
            var controllerName =
                routeData.Values["controller"];

            var actionName =
                routeData.Values["action"];

            string message =
                methodName +
                " Controller:" +
                controllerName +
                " Action:" +
                actionName +
                " Date:" +
                DateTime.Now.ToString() +
                Environment.NewLine;

            File.AppendAllText(
                HttpContext.Current.Server.MapPath(
                    "~/Log/Log.txt"
                ),
                message
            );
        }
    }
}