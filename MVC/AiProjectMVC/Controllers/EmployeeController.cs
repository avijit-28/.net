using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiProjectMVC.Models;

namespace AiProjectMVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepo _repo = new EmployeeRepo();

        // GET: Employee
        public ActionResult Index()
        {
            try
            {
                var employees = _repo.GetAllEmployee();
                return View(employees);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Unable to connect to the database. Error: " + ex.Message;
                return View(new List<Employee>());
            }
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var employee = _repo.GetEmployeeById(id);
                if (employee == null)
                {
                    TempData["ErrorMessage"] = $"Employee with ID #{id} was not found.";
                    return RedirectToAction("Index");
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error retrieving employee details: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repo.InsertEmployee(employee);
                    TempData["SuccessMessage"] = $"Employee '{employee.Name}' (ID: {employee.ID}) added successfully!";
                    return RedirectToAction("Index");
                }

                return View(employee);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to create employee: " + ex.Message);
                return View(employee);
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var employee = _repo.GetEmployeeById(id);
                if (employee == null)
                {
                    TempData["ErrorMessage"] = $"Employee with ID #{id} was not found.";
                    return RedirectToAction("Index");
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading employee for edit: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                        employee.Dept
                    );

                    TempData["SuccessMessage"] = $"Employee '{employee.Name}' (ID: {id}) updated successfully!";
                    return RedirectToAction("Index");
                }

                return View(employee);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to update employee: " + ex.Message);
                return View(employee);
            }
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var employee = _repo.GetEmployeeById(id);
                if (employee == null)
                {
                    TempData["ErrorMessage"] = $"Employee with ID #{id} was not found.";
                    return RedirectToAction("Index");
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading employee for deletion: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repo.DeleteEmployee(id);
                TempData["SuccessMessage"] = $"Employee with ID #{id} has been permanently deleted.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete employee: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
