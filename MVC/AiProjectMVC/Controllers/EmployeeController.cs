using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AiProjectMVC.Models;
using AiProjectMVC.ViewModels;

namespace AiProjectMVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepo _repo = new EmployeeRepo();
        private readonly AddressRepo _addressRepo = new AddressRepo();
        private readonly EmployeeUserRepo _userRepo = new EmployeeUserRepo();

        // Enforce login for all actions: Even admin must sign in first
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (Session["CurrentUser"] == null)
            {
                TempData["ErrorMessage"] = "Please sign in to continue. Even administrators must sign in.";
                var returnUrl = filterContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new { controller = "Account", action = "Login", returnUrl = returnUrl }
                    )
                );
            }
        }

        // GET: Employee/Dashboard
        public ActionResult Dashboard()
        {
            var currentUser = Session["CurrentUser"] as EmployeeUser;
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Please sign in to access your employee dashboard. Even administrators must sign in.";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Dashboard", "Employee") });
            }

            var dashboard = new EmployeeDashboardViewModel
            {
                CurrentUser = currentUser
            };

            try
            {
                var allEmployees = _repo.GetAllEmployee() ?? new List<Employee>();
                var allAddresses = _addressRepo.GetAllAddress() ?? new List<Address>();
                var allUsers = _userRepo.GetAllUsers() ?? new List<EmployeeUser>();

                dashboard.TotalEmployees = allEmployees.Count;
                dashboard.TotalAddresses = allAddresses.Count;
                dashboard.TotalUsers = allUsers.Count;
                dashboard.AverageSalary = allEmployees.Any() ? allEmployees.Average(e => e.Salary) : 0;
                dashboard.SystemUsers = allUsers.Take(8).ToList();

                // Find matching employee profile by ID or Name
                var empProfile = allEmployees.FirstOrDefault(e => e.ID == currentUser.ID)
                              ?? allEmployees.FirstOrDefault(e => string.Equals(e.Name, currentUser.Name, StringComparison.OrdinalIgnoreCase));

                dashboard.EmployeeProfile = empProfile;

                if (empProfile != null)
                {
                    // Find address
                    if (empProfile.AddressId.HasValue)
                    {
                        dashboard.AddressDetails = allAddresses.FirstOrDefault(a => a.AddressId == empProfile.AddressId.Value);
                    }

                    // Find department colleagues
                    if (!string.IsNullOrEmpty(empProfile.Dept))
                    {
                        dashboard.DepartmentColleagues = allEmployees
                            .Where(e => string.Equals(e.Dept, empProfile.Dept, StringComparison.OrdinalIgnoreCase) && e.ID != empProfile.ID)
                            .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Notice while compiling dashboard data: " + ex.Message;
            }

            return View(dashboard);
        }

        // GET: Employee
        public ActionResult Index()
        {
            var viewModel = new EmployeeDetailsViewModel
            {
                PageTitle = "Organization Directory",
                PageHeader = "Employee & Address Management"
            };

            try
            {
                viewModel.Employees = _repo.GetAllEmployee() ?? new List<Employee>();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Unable to load employees from database: " + ex.Message;
                viewModel.Employees = new List<Employee>();
            }

            try
            {
                viewModel.Addresses = _addressRepo.GetAllAddress() ?? new List<Address>();
            }
            catch (Exception ex)
            {
                if (ViewBag.ErrorMessage == null)
                {
                    ViewBag.ErrorMessage = "Unable to load addresses from database: " + ex.Message;
                }
                viewModel.Addresses = new List<Address>();
            }

            return View(viewModel);
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

                Address address = null;
                if (employee.AddressId.HasValue)
                {
                    try
                    {
                        address = _addressRepo.GetAddressById(employee.AddressId.Value);
                    }
                    catch
                    {
                        // Address lookup failed gracefully
                    }
                }

                var viewModel = new EmployeeDetailsViewModel
                {
                    Employee = employee,
                    Address = address,
                    PageTitle = $"Employee Profile - {employee.Name}",
                    PageHeader = "Employee & Address Overview"
                };

                return View(viewModel);
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
            if (!RoleAuthHelper.CanManageRecords(Session))
            {
                TempData["ErrorMessage"] = "Access Denied: Only Admin, HR, or Manager roles are authorized to add new employees.";
                return RedirectToAction("Index");
            }

            try
            {
                ViewBag.AvailableUsers = _userRepo.GetUnassignedUsers() ?? new List<EmployeeUser>();
                ViewBag.Addresses = _addressRepo.GetAllAddress() ?? new List<Address>();
            }
            catch
            {
                ViewBag.AvailableUsers = new List<EmployeeUser>();
                ViewBag.Addresses = new List<Address>();
            }
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            if (!RoleAuthHelper.CanManageRecords(Session))
            {
                TempData["ErrorMessage"] = "Access Denied: Only Admin, HR, or Manager roles are authorized to add new employees.";
                return RedirectToAction("Index");
            }

            // Validate that ID is selected from unassigned EmployeeUser records
            if (employee.ID <= 0)
            {
                ModelState.AddModelError("ID", "Please select a registered User ID from the dropdown.");
            }
            else
            {
                var existingEmp = _repo.GetEmployeeById(employee.ID);
                if (existingEmp != null)
                {
                    ModelState.AddModelError("ID", $"An employee record for ID #{employee.ID} already exists.");
                }

                var userRecord = _userRepo.GetUserById(employee.ID);
                if (userRecord == null)
                {
                    ModelState.AddModelError("ID", $"User ID #{employee.ID} is not registered in EmployeeUser table.");
                }
            }

            // Validate mandatory Linked Address & verify city correlation
            if (!employee.AddressId.HasValue || employee.AddressId.Value <= 0)
            {
                ModelState.AddModelError("AddressId", "Linked Address is required. Please select an address matching the employee's city.");
            }
            else
            {
                Address linkedAddr = null;
                try
                {
                    linkedAddr = _addressRepo.GetAddressById(employee.AddressId.Value);
                }
                catch { }

                if (linkedAddr == null)
                {
                    ModelState.AddModelError("AddressId", "The selected address does not exist. Please choose a valid address.");
                }
                else if (!RoleAuthHelper.IsCityMatching(employee.City, linkedAddr))
                {
                    ModelState.AddModelError("AddressId", $"The selected address in '{linkedAddr.City}, {linkedAddr.State}' does not match the employee city '{employee.City}'. Please choose an address corresponding to {employee.City}.");
                    ModelState.AddModelError("City", $"Employee city '{employee.City}' does not match the selected address in '{linkedAddr.City}, {linkedAddr.State}'.");
                }
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _repo.InsertEmployee(employee);
                    TempData["SuccessMessage"] = $"Employee '{employee.Name}' (ID: #{employee.ID}) added successfully!";
                    return RedirectToAction("Index");
                }

                ViewBag.AvailableUsers = _userRepo.GetUnassignedUsers() ?? new List<EmployeeUser>();
                ViewBag.Addresses = _addressRepo.GetAllAddress() ?? new List<Address>();
                return View(employee);
            }
            catch (Exception ex)
            {
                string friendlyMessage = "Unable to create employee record. Please ensure all required fields are correctly filled and the address matches the city.";
                if (ex.Message != null && ex.Message.IndexOf("AddressId", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    friendlyMessage = "Linked Address is required and must match the employee city.";
                }
                ModelState.AddModelError("", friendlyMessage);
                try
                {
                    ViewBag.AvailableUsers = _userRepo.GetUnassignedUsers() ?? new List<EmployeeUser>();
                    ViewBag.Addresses = _addressRepo.GetAllAddress() ?? new List<Address>();
                }
                catch { }
                return View(employee);
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            if (!RoleAuthHelper.CanManageRecords(Session))
            {
                TempData["ErrorMessage"] = "Access Denied: Only Admin, HR, or Manager roles are authorized to modify employees.";
                return RedirectToAction("Index");
            }

            try
            {
                var employee = _repo.GetEmployeeById(id);
                if (employee == null)
                {
                    TempData["ErrorMessage"] = $"Employee with ID #{id} was not found.";
                    return RedirectToAction("Index");
                }

                ViewBag.Addresses = _addressRepo.GetAllAddress() ?? new List<Address>();
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
            if (!RoleAuthHelper.CanManageRecords(Session))
            {
                TempData["ErrorMessage"] = "Access Denied: Only Admin, HR, or Manager roles are authorized to modify employees.";
                return RedirectToAction("Index");
            }

            int targetId = employee != null && employee.ID > 0 ? employee.ID : id;

            try
            {
                if (ModelState.IsValid)
                {
                    _repo.UpdateEmployee(
                        targetId,
                        employee.Name,
                        employee.Salary,
                        employee.Gender,
                        employee.City,
                        employee.Dept,
                        employee.AddressId
                    );

                    try
                    {
                        _userRepo.UpdateUserName(targetId, employee.Name);
                    }
                    catch
                    {
                        // Continue even if corresponding EmployeeUser row doesn't exist
                    }

                    TempData["SuccessMessage"] = $"Employee '{employee.Name}' (ID: #{targetId}) updated successfully!";
                    return RedirectToAction("Index");
                }

                ViewBag.Addresses = _addressRepo.GetAllAddress() ?? new List<Address>();
                return View(employee);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to update employee: " + ex.Message);
                try { ViewBag.Addresses = _addressRepo.GetAllAddress() ?? new List<Address>(); } catch { }
                return View(employee);
            }
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            if (!RoleAuthHelper.CanManageRecords(Session))
            {
                TempData["ErrorMessage"] = "Access Denied: Only Admin, HR, or Manager roles are authorized to delete employees.";
                return RedirectToAction("Index");
            }

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
            if (!RoleAuthHelper.CanManageRecords(Session))
            {
                TempData["ErrorMessage"] = "Access Denied: Only Admin, HR, or Manager roles are authorized to delete employees.";
                return RedirectToAction("Index");
            }

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
