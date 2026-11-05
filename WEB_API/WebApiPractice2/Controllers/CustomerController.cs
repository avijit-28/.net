using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using WebApiPractice2.Models.BEL;

namespace WebApiPractice2.Controllers
{
    public class CustomerController : Controller
    {
        private Uri GetApiBaseUri()
        {
            if (Request != null && Request.Url != null)
            {
                return new Uri(string.Format("{0}://{1}/api/", Request.Url.Scheme, Request.Url.Authority));
            }
            return new Uri("http://localhost:58572/api/");
        }

        // GET: Customer
        public ActionResult Index()
        {
            IEnumerable<Customer> cust = null;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = GetApiBaseUri();
                try
                {
                    var consumeData = client.GetAsync("CustomerAPI/GetCustomers");
                    consumeData.Wait();
                    var dataRead = consumeData.Result;
                    if (dataRead.IsSuccessStatusCode)
                    {
                        var results = dataRead.Content.ReadAsAsync<IList<Customer>>();
                        results.Wait();
                        cust = results.Result;
                    }
                    else
                    {
                        cust = Enumerable.Empty<Customer>();
                        ModelState.AddModelError(string.Empty, "Server error. Could not retrieve customers.");
                    }
                }
                catch (Exception ex)
                {
                    cust = Enumerable.Empty<Customer>();
                    ModelState.AddModelError(string.Empty, "Connection error: " + ex.Message);
                }
            }
            return View(cust);
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer cust)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = GetApiBaseUri();
                try
                {
                    var postTask = client.PostAsJsonAsync<Customer>("CustomerAPI/InsertCustomer", cust);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                    return View(cust);
                }
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            return View(cust);
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(int id)
        {
            Customer customer = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = GetApiBaseUri();
                try
                {
                    var responseTask = client.GetAsync("CustomerAPI/GetCustomerByID?id=" + id);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<Customer>();
                        readTask.Wait();
                        customer = readTask.Result;
                    }
                    else
                    {
                        return HttpNotFound("Customer not found.");
                    }
                }
                catch
                {
                    return HttpNotFound();
                }
            }
            return View(customer);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Customer cust)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = GetApiBaseUri();
                try
                {
                    var putTask = client.PutAsJsonAsync<Customer>("CustomerAPI/UpdateCustomer/" + id, cust);
                    putTask.Wait();
                    var result = putTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                    return View(cust);
                }
            }
            ModelState.AddModelError(string.Empty, "Server error while updating.");
            return View(cust);
        }

        // GET or POST: Customer/Delete/5
        public ActionResult Delete(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = GetApiBaseUri();
                try
                {
                    var deleteTask = client.DeleteAsync("CustomerAPI/DeleteCustomer/" + id);
                    deleteTask.Wait();
                }
                catch
                {
                    // Ignore or log
                }
            }
            return RedirectToAction("Index");
        }
    }
}
