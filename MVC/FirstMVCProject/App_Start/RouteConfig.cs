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
            //routes.MapMvcAttributeRoutes();

            // Employee List (AllEmployees)
            routes.MapRoute(
                name: "AllEmployeeList",
                url: "AllEmployees",
                defaults: new
                {
                    controller = "Home",
                    action = "Index"
                }
            );

            // Employee Details (AllEmployees)
            routes.MapRoute(
                name: "AllEmployeeDetails",
                url: "AllEmployees/details/{id}",
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

            // Employee Create (AllEmployees)
            routes.MapRoute(
                name: "AllEmployeeCreate",
                url: "AllEmployees/create",
                defaults: new
                {
                    controller = "Employee",
                    action = "Create"
                }
            );

            // Employee Edit (AllEmployees)
            routes.MapRoute(
                name: "AllEmployeeEdit",
                url: "AllEmployees/edit/{id}",
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

            // Employee Delete (AllEmployees)
            routes.MapRoute(
                name: "AllEmployeeDelete",
                url: "AllEmployees/delete/{id}",
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

            // Employee List (employees)
            routes.MapRoute(
                name: "EmployeeList",
                url: "employees",
                defaults: new
                {
                    controller = "Employee",
                    action = "Index"
                }
            );

            // Employee Details (employees)
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

            // Employee Create (employees)
            routes.MapRoute(
                name: "EmployeeCreate",
                url: "employees/create",
                defaults: new
                {
                    controller = "Employee",
                    action = "Create"
                }
            );

            // Employee Edit (employees)
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

            // Employee Delete (employees)
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

            // Address Create
            routes.MapRoute(
                name: "AddressCreate",
                url: "addresses/create",
                defaults: new
                {
                    controller = "Address",
                    action = "Create"
                }
            );

            // Address Edit
            routes.MapRoute(
                name: "AddressEdit",
                url: "addresses/edit/{id}",
                defaults: new
                {
                    controller = "Address",
                    action = "Edit"
                },
                constraints: new
                {
                    id = @"\d+"
                }
            );

            // Address Delete
            routes.MapRoute(
                name: "AddressDelete",
                url: "addresses/delete/{id}",
                defaults: new
                {
                    controller = "Address",
                    action = "Delete"
                },
                constraints: new
                {
                    id = @"\d+"
                }
            );

            // AllEmployees Generic Action Route
            routes.MapRoute(
                name: "AllEmployeesActions",
                url: "AllEmployees/{action}/{id}",
                defaults: new
                {
                    controller = "Employee",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );

            // employees Generic Action Route
            routes.MapRoute(
                name: "EmployeesActions",
                url: "employees/{action}/{id}",
                defaults: new
                {
                    controller = "Employee",
                    action = "Index",
                    id = UrlParameter.Optional
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
