using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FirstMVCProject.Models;
using FirstMVCProject.ViewModels;
namespace FirstMVCProject.Controllers
{
    //public class EmployeeController : Controller
    //{
    //    //private readonly EmployeeRepo _repo = new EmployeeRepo();
    //    //private readonly AddressRepo _addressRepo = new AddressRepo();
    //}

    //[RequireHttps]
    [RoutePrefix("AllEmployees")]
    [LogFilter]
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepo _repo = new EmployeeRepo();
        private readonly AddressRepo _addressRepo = new AddressRepo();


        // GET: Employee
        [Route("")]
        public ActionResult Index()
        {

            //var employees = _repo.GetAllEmployee();
            //return View(employees);
            List<Employee> employees = _repo.GetAllEmployee();

            List<Address> addresses = _addressRepo.GetAllAddress();

            EmployeeDetailsViewModel viewModel =
                new EmployeeDetailsViewModel
                {
                    Employees = employees,
                    Addresses = addresses,
                    PageTitle = "Employee and Address",
                    PageHeader = "Employee and Address Details"
                };

            return View(viewModel);


        }

        // GET: Employee/Details/105
        [Route("details/{id:int}")]
        public ActionResult Details(int id)
        {
            Employee employee = _repo.GetEmployeeById(id);

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

        // GET: Employee/Create
        [Route("create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public ActionResult Create(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repo.InsertEmployee(employee);

                    return RedirectToAction("Index");
                }

                return View(employee);
            }
            catch
            {
                return View(employee);
            }

        }

        // GET: Employee/Edit/5
        [Route("edit/{id:int}")]
        public ActionResult Edit(int id)
        {
            Employee employee = _repo.GetEmployeeById(id);

            if (employee == null)
            {
                return HttpNotFound();
            }

            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [Route("edit/{id:int}")]
        public ActionResult Edit(int id, Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repo.UpdateEmployee(
                        id,
                        employee.Name,
                        employee.Salary,
                        employee.Gender,
                        employee.City,
                        employee.Dept,
                         employee.AddressId
                    );

                    return RedirectToAction("Index");
                }

                return View(employee);
            }
            catch (Exception)
            {
                return View(employee);
            }

        }

        // GET: Employee/Delete/5
        [Route("delete/{id:int}")]
        public ActionResult Delete(int id)
        {
            Employee employee = _repo.GetEmployeeById(id);

            if (employee == null)
            {
                return HttpNotFound();
            }

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost]
        [Route("delete/{id:int}")]
        public ActionResult Delete(int id, Employee employee)
        {
            try
            {
                // TODO: Add delete logic here
                _repo.DeleteEmployee(id);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
