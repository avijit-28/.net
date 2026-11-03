using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FoodArrival.DAL;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    public class AccountController : Controller
    {
        private readonly CustomerRepository _customerRepo = new CustomerRepository();
        private readonly RestaurantRepository _restRepo = new RestaurantRepository();
        private readonly RiderRepository _riderRepo = new RiderRepository();
        private readonly StaffRepository _staffRepo = new StaffRepository();
        private readonly AdminRepository _adminRepo = new AdminRepository();
        private readonly NotificationRepository _notifRepo = new NotificationRepository();

        private void PopulateRoleDropdowns()
        {
            ViewBag.Restaurants = _restRepo.GetAllVerified();
        }

        [HttpGet]
        public ActionResult Register(string role = "Customer")
        {
            if (Session["CustomerId"] != null)
            {
                return RedirectToAction("Index", "Food");
            }

            PopulateRoleDropdowns();
            var model = new RegisterViewModel
            {
                Role = string.IsNullOrWhiteSpace(role) ? "Customer" : role
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            PopulateRoleDropdowns();

            // Strict Role-specific model validation
            if (model.Role == "RestaurantOwner")
            {
                if (string.IsNullOrWhiteSpace(model.RestaurantName))
                    ModelState.AddModelError("RestaurantName", "Restaurant Name is required for restaurant owners.");
                if (string.IsNullOrWhiteSpace(model.RestaurantAddress))
                    ModelState.AddModelError("RestaurantAddress", "Kitchen/Restaurant Physical Address is required.");
            }
            else if (model.Role == "RestaurantStaff")
            {
                if (!model.SelectedRestaurantId.HasValue || model.SelectedRestaurantId.Value <= 0)
                    ModelState.AddModelError("SelectedRestaurantId", "Please select the restaurant where you work.");
            }
            else if (model.Role == "Rider")
            {
                if (!model.SelectedRestaurantId.HasValue || model.SelectedRestaurantId.Value <= 0)
                    ModelState.AddModelError("SelectedRestaurantId", "Please select the restaurant you want to deliver for.");

                if (model.VehicleType != "Bicycle" && model.VehicleType != "Electric Bicycle")
                {
                    if (string.IsNullOrWhiteSpace(model.LicenseNumber))
                        ModelState.AddModelError("LicenseNumber", "Driving License Number is mandatory for motorized vehicles.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string cleanUsername = model.Username.Trim().ToLower();
            string cleanEmail = model.Email.Trim().ToLower();
            string cleanPhone = model.PhoneNumber.Trim();

            // 1. REGISTER AS CUSTOMER
            if (model.Role == "Customer")
            {
                var customer = new Customer
                {
                    CustomerName = model.FullName,
                    Username = cleanUsername,
                    Email = cleanEmail,
                    PasswordHash = model.Password,
                    PhoneNumber = cleanPhone,
                    Address = model.Address
                };

                string message;
                int customerId;
                if (_customerRepo.Insert(customer, out message, out customerId))
                {
                    customer.CustomerId = customerId;
                    Session.Clear();
                    SetUserSession(customer);

                    _notifRepo.AddNotification("Customer", customerId, "Welcome to FoodReady!", $"Hello {customer.CustomerName}, your customer account is active. Order fresh food from top partner kitchens!", "Promo", "/Food/Index");

                    TempData["Success"] = $"Welcome to FoodReady, {customer.CustomerName}! Your customer account has been created.";
                    return RedirectToAction("Index", "Food");
                }

                ModelState.AddModelError("", message);
                return View(model);
            }

            // 2. REGISTER AS RESTAURANT OWNER
            if (model.Role == "RestaurantOwner")
            {
                var restaurant = new Restaurant
                {
                    RestaurantName = model.RestaurantName,
                    OwnerName = string.IsNullOrWhiteSpace(model.OwnerName) ? model.FullName : model.OwnerName,
                    Username = cleanUsername,
                    Email = cleanEmail,
                    PasswordHash = model.Password,
                    PhoneNumber = cleanPhone,
                    Address = model.RestaurantAddress,
                    Cuisine = string.IsNullOrWhiteSpace(model.Cuisine) ? "Multi-Cuisine" : model.Cuisine,
                    IsVerified = false
                };

                string message;
                int restaurantId;
                if (_restRepo.Register(restaurant, out message, out restaurantId))
                {
                    _notifRepo.AddNotification("Restaurant", restaurantId, "Registration Submitted",
                        $"Welcome '{restaurant.RestaurantName}'! Your kitchen is registered with ID #{restaurantId} and is pending administrator verification.", "Info", "/Restaurant/Dashboard");
                    _notifRepo.AddNotification("Admin", null, "New Restaurant Registered",
                        $"'{restaurant.RestaurantName}' registered by {restaurant.OwnerName} (ID: #{restaurantId}). Requires verification.", "Approval", "/Admin/Dashboard");

                    TempData["Success"] = message;
                    return RedirectToAction("Login", new { role = "RestaurantOwner" });
                }

                ModelState.AddModelError("", message);
                return View(model);
            }

            // 3. REGISTER AS RESTAURANT STAFF
            if (model.Role == "RestaurantStaff")
            {
                var staff = new RestaurantStaff
                {
                    StaffName = model.FullName,
                    Username = cleanUsername,
                    Email = cleanEmail,
                    PasswordHash = model.Password,
                    PhoneNumber = cleanPhone,
                    RestaurantId = model.SelectedRestaurantId.Value,
                    RoleTitle = string.IsNullOrWhiteSpace(model.RoleTitle) ? "Kitchen Staff" : model.RoleTitle
                };

                string message;
                int staffId;
                if (_staffRepo.Register(staff, out message, out staffId))
                {
                    _notifRepo.AddNotification("Restaurant", staff.RestaurantId, "New Staff Registration",
                        $"{staff.StaffName} registered as {staff.RoleTitle} (Staff ID: {staff.StaffCode}). Pending verification.", "Staff", "/Restaurant/Dashboard");
                    _notifRepo.AddNotification("Admin", null, "New Staff Registered",
                        $"{staff.StaffName} registered for Restaurant #{staff.RestaurantId}.", "Approval", "/Admin/Dashboard");

                    TempData["Success"] = message;
                    return RedirectToAction("Login", new { role = "RestaurantStaff" });
                }

                ModelState.AddModelError("", message);
                return View(model);
            }

            // 4. REGISTER AS RIDER
            if (model.Role == "Rider")
            {
                string licenseDocUrl = null;
                if (model.LicenseDocumentFile != null && model.LicenseDocumentFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(model.LicenseDocumentFile.FileName);
                    string uniqueName = "license_" + Guid.NewGuid().ToString("N").Substring(0, 8) + "_" + fileName;
                    string dirPath = Server.MapPath("~/Content/licenses");
                    if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                    string targetPath = Path.Combine(dirPath, uniqueName);
                    model.LicenseDocumentFile.SaveAs(targetPath);
                    licenseDocUrl = "/Content/licenses/" + uniqueName;
                }

                var rider = new Rider
                {
                    RiderName = model.FullName,
                    Username = cleanUsername,
                    Email = cleanEmail,
                    PasswordHash = model.Password,
                    PhoneNumber = cleanPhone,
                    VehicleType = model.VehicleType,
                    LicenseNumber = model.LicenseNumber,
                    LicenseDocumentUrl = licenseDocUrl
                };

                string message;
                int riderId;
                if (_riderRepo.Register(rider, model.SelectedRestaurantId.Value, out message, out riderId))
                {
                    _notifRepo.AddNotification("Restaurant", model.SelectedRestaurantId.Value, "New Rider Affiliation Request",
                        $"{rider.RiderName} requested to deliver for your restaurant ({rider.VehicleType}). Pending verification.", "Rider", "/Restaurant/Dashboard");
                    _notifRepo.AddNotification("Admin", null, "New Rider Registered",
                        $"{rider.RiderName} ({rider.VehicleType}) registered. Pending verification.", "Approval", "/Admin/Dashboard");

                    TempData["Success"] = message;
                    return RedirectToAction("Login", new { role = "Rider" });
                }

                ModelState.AddModelError("", message);
                return View(model);
            }

            ModelState.AddModelError("", "Invalid role selected.");
            return View(model);
        }

        [HttpGet]
        public ActionResult Login(string returnUrl, string role = "Auto")
        {
            // If already logged in, redirect appropriately
            if (Session["UserRole"] != null)
            {
                string currentRole = Session["UserRole"].ToString();
                if (currentRole == "Admin") return RedirectToAction("Dashboard", "Admin");
                if (currentRole == "Restaurant") return RedirectToAction("Dashboard", "Restaurant");
                if (currentRole == "RestaurantStaff") return RedirectToAction("Dashboard", "Staff");
                if (currentRole == "Rider") return RedirectToAction("Dashboard", "Rider");
                if (currentRole == "Customer") return RedirectToAction("Index", "Food");
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.SelectedRole = role;
            return View(new LoginViewModel { SelectedRole = role });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string cleanLoginId = model.Email.Trim().ToLower();
            string password = model.Password.Trim();

            // 1. CHECK ADMIN FIRST (Matches username: avi@123, password: 28072003)
            var admin = _adminRepo.Login(cleanLoginId, password);
            if (admin != null)
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                Session["AdminId"] = admin.AdminId;
                Session["AdminName"] = admin.AdminName;
                Session["AdminEmail"] = admin.Email;
                Session["AdminUsername"] = admin.Username;
                Session["UserRole"] = "Admin";

                TempData["Success"] = $"Welcome Master Administrator, {admin.AdminName}!";
                return RedirectToAction("Dashboard", "Admin");
            }

            // 2. CHECK RESTAURANT OWNER
            string restVerifyMsg;
            var restaurant = _restRepo.Login(cleanLoginId, password, out restVerifyMsg);
            if (!string.IsNullOrEmpty(restVerifyMsg))
            {
                ModelState.AddModelError("", restVerifyMsg);
                return View(model);
            }
            if (restaurant != null)
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                Session["RestaurantId"] = restaurant.RestaurantId;
                Session["RestaurantName"] = restaurant.RestaurantName;
                Session["OwnerName"] = restaurant.OwnerName;
                Session["RestaurantEmail"] = restaurant.Email;
                Session["UserRole"] = "Restaurant";

                TempData["Success"] = $"Welcome, {restaurant.RestaurantName} Kitchen Management!";
                return RedirectToAction("Dashboard", "Restaurant");
            }

            // 3. CHECK RESTAURANT STAFF
            var staff = _staffRepo.Login(cleanLoginId, password);
            if (staff != null)
            {
                if (!staff.IsVerified)
                {
                    ModelState.AddModelError("", "Your staff registration is awaiting approval by restaurant management or administrator.");
                    return View(model);
                }

                FormsAuthentication.SignOut();
                Session.Clear();
                Session["StaffId"] = staff.StaffId;
                Session["StaffCode"] = staff.StaffCode;
                Session["StaffName"] = staff.StaffName;
                Session["StaffEmail"] = staff.Email;
                Session["RestaurantId"] = staff.RestaurantId;
                Session["RestaurantName"] = staff.RestaurantName;
                Session["RoleTitle"] = staff.RoleTitle;
                Session["UserRole"] = "RestaurantStaff";

                TempData["Success"] = $"Welcome to your PMS Portal, {staff.StaffName}! (Staff ID: {staff.StaffCode})";
                return RedirectToAction("Dashboard", "Staff");
            }

            // 4. CHECK RIDER
            var rider = _riderRepo.Login(cleanLoginId, password);
            if (rider != null)
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                Session["RiderId"] = rider.RiderId;
                Session["RiderName"] = rider.RiderName;
                Session["RiderEmail"] = rider.Email;
                Session["VehicleType"] = rider.VehicleType;
                Session["UserRole"] = "Rider";

                TempData["Success"] = $"Welcome Delivery Partner, {rider.RiderName}!";
                return RedirectToAction("Dashboard", "Rider");
            }

            // 5. CHECK CUSTOMER
            var customer = _customerRepo.Login(cleanLoginId, password);
            if (customer != null)
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                SetUserSession(customer);

                TempData["Success"] = $"Welcome back, {customer.CustomerName}!";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Food");
            }

            ModelState.AddModelError("", "Invalid login credentials. Please check your username/email and password.");
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public new ActionResult Profile()
        {
            if (Session["CustomerId"] == null)
            {
                return RedirectToAction("Login", new { returnUrl = Url.Action("Profile") });
            }

            int customerId = Convert.ToInt32(Session["CustomerId"]);
            var customer = _customerRepo.GetById(customerId);
            if (customer == null)
            {
                return RedirectToAction("Logout");
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Profile(Customer model)
        {
            if (Session["CustomerId"] == null)
            {
                return RedirectToAction("Login");
            }

            int customerId = Convert.ToInt32(Session["CustomerId"]);
            model.CustomerId = customerId;

            string message;
            if (_customerRepo.Update(model, out message))
            {
                Session["CustomerName"] = model.CustomerName;
                Session["CustomerPhone"] = model.PhoneNumber;
                Session["CustomerAddress"] = model.Address;
                TempData["Success"] = "Profile updated successfully.";
                return RedirectToAction("Profile");
            }

            ModelState.AddModelError("", message);
            return View(model);
        }

        private void SetUserSession(Customer customer)
        {
            Session["CustomerId"] = customer.CustomerId;
            Session["CustomerName"] = customer.CustomerName;
            Session["CustomerEmail"] = customer.Email;
            Session["CustomerUsername"] = customer.Username;
            Session["CustomerPhone"] = customer.PhoneNumber;
            Session["CustomerAddress"] = customer.Address;
            Session["UserRole"] = "Customer";
        }
    }
}
