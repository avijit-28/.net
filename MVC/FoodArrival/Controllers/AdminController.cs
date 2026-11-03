using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FoodArrival.Models;

namespace FoodArrival.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            var repo = new FoodRepository();
            var agents = new List<SelectListItem>();
            foreach (var agent in repo.GetActiveAgents()) agents.Add(new SelectListItem { Value = agent.Key.ToString(), Text = agent.Value });
            var users = repo.GetAdminUsers();
            var owners = new List<SelectListItem>();
            foreach (var user in users.Where(x => x.Role == "Staff" && x.StaffType == "Restaurant" && x.IsActive)) owners.Add(new SelectListItem { Value = user.UserId.ToString(), Text = user.FullName + " · " + user.Email });
            return View(new AdminDashboardViewModel
            {
                Orders = repo.GetAdminOrders(),
                ReadyOrders = repo.GetReadyOrders(), Agents = agents, Users = users,
                Restaurants = repo.GetAdminRestaurants(), RestaurantOwners = owners,
                NewRestaurant = new AdminRestaurantViewModel { IsActive = true }
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateOrder(long orderId, string status)
        {
            if (status != "Accepted" && status != "Rejected") return new HttpStatusCodeResult(400);
            try
            {
                new FoodRepository().SetAdminOrderStatus(Int32.Parse(User.Identity.Name), orderId, status);
                TempData["Notice"] = "Order " + status.ToLowerInvariant() + ".";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Assign(long orderId, int agentUserId)
        {
            try { new FoodRepository().AssignDelivery(Int32.Parse(User.Identity.Name), orderId, agentUserId); TempData["Notice"] = "Delivery assigned."; }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateUser(int userId, string role, string staffType, bool isActive)
        {
            if (!new[] { "Customer", "Staff", "Admin" }.Contains(role)) return new HttpStatusCodeResult(400);
            if (role == "Staff" && !new[] { "Restaurant", "DeliveryAgent" }.Contains(staffType))
            {
                TempData["Error"] = "Choose a staff area before granting the Staff role.";
                return RedirectToAction("Index");
            }
            try { new FoodRepository().UpdateUserRole(Int32.Parse(User.Identity.Name), userId, role, staffType, isActive); TempData["Notice"] = "Account updated."; }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveRestaurant(AdminRestaurantViewModel model)
        {
            if (!ModelState.IsValid) { TempData["Error"] = "Complete all required restaurant fields."; return RedirectToAction("Index"); }
            try { new FoodRepository().SaveRestaurant(Int32.Parse(User.Identity.Name), model); TempData["Notice"] = "Restaurant saved."; }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }
    }
}
