using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiProjectMVC.Models;

namespace AiProjectMVC.Controllers
{
    public class AddressController : Controller
    {
        private readonly AddressRepo _repo = new AddressRepo();

        // GET: Address
        public ActionResult Index()
        {
            try
            {
                var addresses = _repo.GetAllAddress();
                return View(addresses);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Unable to load addresses from database: " + ex.Message;
                return View(new List<Address>());
            }
        }

        // GET: Address/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var address = _repo.GetAddressById(id);
                if (address == null)
                {
                    TempData["ErrorMessage"] = $"Address with ID #{id} was not found.";
                    return RedirectToAction("Index");
                }
                return View(address);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error retrieving address details: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Address/Create
        public ActionResult Create()
        {
            if (!RoleAuthHelper.CanManageRecords(Session))
            {
                TempData["ErrorMessage"] = "Access Denied: Only Admin, HR, or Manager roles are authorized to add new addresses.";
                return RedirectToAction("Index");
            }

            return View();
        }

        // POST: Address/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Address address)
        {
            if (!RoleAuthHelper.CanManageRecords(Session))
            {
                TempData["ErrorMessage"] = "Access Denied: Only Admin, HR, or Manager roles are authorized to add new addresses.";
                return RedirectToAction("Index");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _repo.InsertAddress(address);
                    TempData["SuccessMessage"] = $"Address for {address.City}, {address.State} added successfully!";
                    return RedirectToAction("Index");
                }

                return View(address);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to save address: " + ex.Message);
                return View(address);
            }
        }

        // GET: Address/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var address = _repo.GetAddressById(id);
                if (address == null)
                {
                    TempData["ErrorMessage"] = $"Address with ID #{id} was not found.";
                    return RedirectToAction("Index");
                }
                return View(address);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading address for editing: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Address/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Address address)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repo.UpdateAddress(address);
                    TempData["SuccessMessage"] = $"Address #{address.AddressId} updated successfully!";
                    return RedirectToAction("Index");
                }

                return View(address);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to update address: " + ex.Message);
                return View(address);
            }
        }

        // GET: Address/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var address = _repo.GetAddressById(id);
                if (address == null)
                {
                    TempData["ErrorMessage"] = $"Address with ID #{id} was not found.";
                    return RedirectToAction("Index");
                }
                return View(address);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading address for deletion: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Address/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repo.DeleteAddress(id);
                TempData["SuccessMessage"] = $"Address #{id} has been permanently deleted.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete address: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
