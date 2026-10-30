using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace FirstMVCProject.Models
{
    public class EmployeeAuditFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(
            ActionExecutingContext filterContext)
        {
            Debug.WriteLine(
                "Employee action started: " +
                filterContext.ActionDescriptor.ActionName
            );
        }

        public override void OnActionExecuted(
            ActionExecutedContext filterContext)
        {
            Debug.WriteLine(
                "Employee action completed: " +
                filterContext.ActionDescriptor.ActionName
            );
        }
    }
}