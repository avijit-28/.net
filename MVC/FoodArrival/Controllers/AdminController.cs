using System;
using System.Collections.Generic;
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
            return View(new AdminDashboardViewModel { ReadyOrders = repo.GetReadyOrders(), Agents = agents });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Assign(long orderId, int agentUserId)
        {
            try { new FoodRepository().AssignDelivery(Int32.Parse(User.Identity.Name), orderId, agentUserId); TempData["Notice"] = "Delivery assigned."; }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }
    }
}
