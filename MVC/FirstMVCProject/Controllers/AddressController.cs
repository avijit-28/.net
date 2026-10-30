using FirstMVCProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FirstMVCProject.Controllers
{
    [RoutePrefix("addresses")]
    public class AddressController : Controller
    {
        private readonly AddressRepo _repo = new AddressRepo();

        // GET: Address
        [Route("")]
        public ActionResult Index()
        {
            var addresses = _repo.GetAllAddress();

            return View(addresses);
        }

        // GET: Address/Details/5
        [Route("details/{id:int}")]
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
        [Route("create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Address/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
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
        [Route("edit/{id:int}")]
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
        [ValidateAntiForgeryToken]
        [Route("edit/{id:int}")]
        public ActionResult Edit(int id, Address address)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    address.AddressId = id;
                    _repo.UpdateAddress(address);

                    return RedirectToAction("Index");
                }

                return View(address);
            }
            catch
            {
                return View(address);
            }
        }

        // GET: Address/Delete/5
        [Route("delete/{id:int}")]
        public ActionResult Delete(int id)
        {
            Address address = _repo.GetAddressById(id);

            if (address == null)
            {
                return HttpNotFound();
            }

            return View(address);
        }

        // POST: Address/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete/{id:int}")]
        public ActionResult Delete(int id, Address address)
        {
            try
            {
                _repo.DeleteAddress(id);

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
