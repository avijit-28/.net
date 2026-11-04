using FoodArrival.Attributes;
using System.Data;
using System.Data.SqlClient;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FoodArrival.DAL;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    public class StaffController : Controller
    {
        private readonly StaffRepository _staffRepo = new StaffRepository();
        private readonly RestaurantRepository _restRepo = new RestaurantRepository();
        private readonly NotificationRepository _notifRepo = new NotificationRepository();
        private readonly OrderRepository _orderRepo = new OrderRepository();

        private bool IsStaffAuthenticated()
        {
            return Session["StaffId"] != null;
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (IsStaffAuthenticated())
            {
                return RedirectToAction("Dashboard");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateCustomAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var staff = _staffRepo.Login(model.Email.Trim().ToLower(), model.Password);
            if (staff != null)
            {
                if (!staff.IsVerified)
                {
                    ModelState.AddModelError("", "Your staff registration is currently under review by restaurant management or administrator. Once verified, you can log in to your PMS portal.");
                    return View(model);
                }

                Session["StaffId"] = staff.StaffId;
                Session["StaffCode"] = staff.StaffCode;
                Session["StaffName"] = staff.StaffName;
                Session["StaffEmail"] = staff.Email;
                Session["RestaurantId"] = staff.RestaurantId;
                Session["RestaurantName"] = staff.RestaurantName;
                Session["RoleTitle"] = staff.RoleTitle;
                Session["UserRole"] = "RestaurantStaff";

                TempData["Success"] = $"Welcome to your PMS Portal, {staff.StaffName}! (ID: {staff.StaffCode})";
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Invalid staff email/username or password.");
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            TempData["Success"] = "Kitchen Staff logged out.";
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            if (!IsStaffAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            int staffId = Convert.ToInt32(Session["StaffId"]);
            int restId = Convert.ToInt32(Session["RestaurantId"]);

            var staff = _staffRepo.GetById(staffId);
            ViewBag.Staff = staff;
            ViewBag.KitchenOrders = _restRepo.GetOrders(restId, excludePickedUp: true);
            var myDishes = _staffRepo.GetStaffDishes(staffId);

            return View(myDishes);
        }

        [HttpPost]
        [ValidateCustomAntiForgeryToken]
        public ActionResult ProposeDish(FoodItem item, HttpPostedFileBase dishImage)
        {
            if (!IsStaffAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            int staffId = Convert.ToInt32(Session["StaffId"]);
            int restId = Convert.ToInt32(Session["RestaurantId"]);
            string staffName = Session["StaffName"]?.ToString() ?? "Staff";

            if (dishImage != null && dishImage.ContentLength > 0)
            {
                string fileName = Path.GetFileName(dishImage.FileName);
                string uniqueName = Guid.NewGuid().ToString("N").Substring(0, 8) + "_" + fileName;
                string dirPath = Server.MapPath("~/Content/images");
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                string targetPath = Path.Combine(dirPath, uniqueName);
                dishImage.SaveAs(targetPath);
                item.ImageUrl = "/Content/images/" + uniqueName;
            }

            if (string.IsNullOrWhiteSpace(item.ImageUrl))
            {
                item.ImageUrl = "/Content/images/margherita.jpg";
            }

            string message;
            if (_staffRepo.ProposeDish(item, staffId, restId, out message))
            {
                // Notify Owner and Admin
                _notifRepo.AddNotification("Restaurant", null, "New Dish Proposed for Menu", $"{staffName} proposed new item '{item.ItemName}' for review.", "Menu", "/Restaurant/Dashboard");
                _notifRepo.AddNotification("Admin", null, "New Dish Proposed", $"{staffName} proposed item '{item.ItemName}'.", "Menu", "/Admin/Dashboard");

                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateCustomAntiForgeryToken]
        public ActionResult UpdateOrderStatus(int orderId, string status)
        {
            if (!IsStaffAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            int? restId = Session["RestaurantId"] != null ? (int?)Convert.ToInt32(Session["RestaurantId"]) : null;

            _restRepo.UpdateDeliveryStatus(orderId, status, restId);

            var order = _orderRepo.GetById(orderId);
            int? custId = order != null ? (int?)order.CustomerId : null;
            int? riderId = order != null ? order.RiderId : null;

            // If not directly on master order, check sub-orders for assigned rider via Stored Procedure
            if (!riderId.HasValue)
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("dbo.sp_Order_SyncAssignedRider", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    var outRider = new SqlParameter("@AssignedRiderId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outRider);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    if (outRider.Value != DBNull.Value && outRider.Value != null)
                    {
                        riderId = Convert.ToInt32(outRider.Value);
                    }
                }
            }

            // When staff marks Food Ready, the order is made available in the affiliated riders' Ready for Pickup queue!
            string orderNum = order != null && !string.IsNullOrEmpty(order.OrderNumber) ? order.OrderNumber : ($"#FD{1000 + orderId}");

            if (status == "ReadyForPickup")
            {
                if (riderId.HasValue)
                {
                    _notifRepo.AddNotification("Rider", riderId.Value, "Order Ready for Pickup!", $"Order {orderNum} is freshly prepared & ready for pickup from kitchen.", "Order", "/Rider/Dashboard");
                }
                if (custId.HasValue)
                {
                    _notifRepo.AddNotification("Customer", custId.Value, "Your Food is Prepared!", $"Order {orderNum} is freshly cooked & packed.", "Order", "/Order/History");
                }
                _notifRepo.AddNotification("Admin", null, "Order Ready for Pickup", $"Order {orderNum} is prepared and ready for rider pickup.", "Order", "/Admin/Orders");
            }

            TempData["Success"] = $"Order {orderNum} marked as {status}. Dispatched to delivery partner queue.";
            return RedirectToAction("Dashboard");
        }
    }
}
