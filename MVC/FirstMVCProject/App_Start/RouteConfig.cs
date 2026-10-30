using System.Web.Mvc;
using System.Web.Routing;

namespace FirstMVCProject
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Ignore .axd files
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Enable Attribute Routing
            routes.MapMvcAttributeRoutes();

            // Employee List
            routes.MapRoute(
                name: "EmployeeList",
                url: "employees",
                defaults: new
                {
                    controller = "Employee",
                    action = "Index"
                }
            );

            // Employee Details
            routes.MapRoute(
                name: "EmployeeDetails",
                url: "employees/details/{id}",
                defaults: new
                {
                    controller = "Employee",
                    action = "Details"
                },
                constraints: new
                {
                    id = @"\d+"
                }
            );

            // Employee Create
            routes.MapRoute(
                name: "EmployeeCreate",
                url: "employees/create",
                defaults: new
                {
                    controller = "Employee",
                    action = "Create"
                }
            );

            // Employee Edit
            routes.MapRoute(
                name: "EmployeeEdit",
                url: "employees/edit/{id}",
                defaults: new
                {
                    controller = "Employee",
                    action = "Edit"
                },
                constraints: new
                {
                    id = @"\d+"
                }
            );

            // Employee Delete
            routes.MapRoute(
                name: "EmployeeDelete",
                url: "employees/delete/{id}",
                defaults: new
                {
                    controller = "Employee",
                    action = "Delete"
                },
                constraints: new
                {
                    id = @"\d+"
                }
            );

            // Address List
            routes.MapRoute(
                name: "AddressList",
                url: "addresses",
                defaults: new
                {
                    controller = "Address",
                    action = "Index"
                }
            );

            // Address Details
            routes.MapRoute(
                name: "AddressDetails",
                url: "addresses/details/{id}",
                defaults: new
                {
                    controller = "Address",
                    action = "Details"
                },
                constraints: new
                {
                    id = @"\d+"
                }
            );

            // Default MVC Route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Employee",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );
        }
    }
}