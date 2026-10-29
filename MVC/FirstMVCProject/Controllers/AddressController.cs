using FirstMVCProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FirstMVCProject.Controllers
{
    public class AddressController : Controller
    {
        private readonly AddressRepo _repo = new AddressRepo();

        // GET: Address
        public ActionResult Index()
        {
            var addresses = _repo.GetAllAddress();

            return View(addresses);
        }

        // GET: Address/Details/5
        public ActionResult Details(int id)
        {
            Address address = _repo.GetAddressById(id);

            if (address == null)
            {
                return HttpNotFound();
            }

            return View(address);
        }

        // GET: Address/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Address/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Address address)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repo.InsertAddress(address);

                    return RedirectToAction("Index");
                }

                return View(address);
            }
            catch
            {
                return View(address);
            }
        }

        // GET: Address/Edit/5
        public ActionResult Edit(int id)
        {
            Address address = _repo.GetAddressById(id);

            if (address == null)
            {
                return HttpNotFound();
            }

            return View(address);
        }

        // POST: Address/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Address address)
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

        // GET: Address/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Address/Delete/5
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
