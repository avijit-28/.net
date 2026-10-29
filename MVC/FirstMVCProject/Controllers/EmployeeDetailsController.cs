using FirstMVCProject.Models;
using FirstMVCProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FirstMVCProject.Controllers
{
    public class EmployeeDetailsController : Controller
    {
        private readonly EmployeeRepo _employeeRepo = new EmployeeRepo();
        private readonly AddressRepo _addressRepo = new AddressRepo();
        // GET: EmployeeDetails
        public ActionResult Index()
        {
            return View();
        }

        // GET: EmployeeDetails/Details/5
        public ActionResult Details(int id)
        {
            Employee employee = _employeeRepo.GetEmployeeById(id);

            if (employee == null)
            {
                return HttpNotFound();
            }

            Address address = null;

            if (employee.AddressId.HasValue)
            {
                address = _addressRepo.GetAddressById(employee.AddressId.Value);
            }

            EmployeeDetailsViewModel employeeDetailsViewModel =
                new EmployeeDetailsViewModel
                {
                    Employee = employee,
                    Address = address,
                    PageTitle = "Employee Details Page",
                    PageHeader = "Employee Details"
                };

            return View(employeeDetailsViewModel);
        }

        // GET: EmployeeDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeeDetails/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EmployeeDetails/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EmployeeDetails/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EmployeeDetails/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EmployeeDetails/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
