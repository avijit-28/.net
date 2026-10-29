namespace FirstMVCProject.Controllers
{
    using FirstMVCProject.Models;
    using FirstMVCProject.ViewModels;
    using System.Web.Mvc;

    
    public class EmpDetController : Controller
    {
        
        private readonly EmployeeRepo _employeeRepo = new EmployeeRepo();

        
        private readonly AddressRepo _addressRepo = new AddressRepo();

        //public ViewResult Index 

        // GET: EmpDet
     
        public ViewResult Details(int id)
        {
            Employee employee = _employeeRepo.GetEmployeeById(id);
            if (employee == null)
            {
                return null;
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
    }
}
