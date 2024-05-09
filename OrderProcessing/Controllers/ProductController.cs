using BAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace OrderProcessing.Controllers
{
    public class ProductController : Controller
    {
        private readonly BusinessLayer businessLayer;

        public ProductController()
        {
            businessLayer = new BusinessLayer();
        }
        public ActionResult Index()
        {
           
            return View();
        }
        [HttpPost]
        public ActionResult ValidateEmail(string email)
        {

            if (!Regex.IsMatch(email, @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b", RegexOptions.IgnoreCase))
            {
                ModelState.AddModelError("email", "Please enter a valid email address.");
                return View("Index");
            }
            try
            {
                int? customerId = GetCustomerIdByEmail(email);

                if (customerId == null)
                {
                    ViewBag.ErrorMessage = "Email address not found.";
                    return View("Index");
                }
                Session["CustomerId"] = customerId;
                return RedirectToAction("ProductSelection");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Internal server error.";
               
                return View("Index");
            }

        }

        private int? GetCustomerIdByEmail(string email)
        {

            return businessLayer.GetCustomerIdByEmail(email);
        }

        public ActionResult ProductSelection()
        {
            try
            {
                int? customerId = (int?)Session["CustomerId"];
                if (customerId == null)
                {

                    return RedirectToAction("Index");
                }
                List<Product> products = businessLayer.GetProducts();
                ViewBag.Products = products;
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Internal server error.";

                return View("ProductSelection");
            }
        }
        public ActionResult PlaceOrder(List<int> productIds)
        {

            try
            {
                int? customerId = Session["CustomerId"] as int?;
                if (customerId == null)
                {
                    TempData["ErrorMessage"] = "Customer ID not found in session.";
                    return RedirectToAction("Index");
                }

               
                Dictionary<int, int> quantities = new Dictionary<int, int>();
                foreach (var productId in productIds)
                {
                    int quantity = int.Parse(Request.Form[$"quantities[{productId}]"]);
                    quantities.Add(productId, quantity);
                }

                businessLayer.InsertOrder(customerId.Value, productIds, quantities);

                TempData["SuccessMessage"] = "Order placed successfully!";
                return RedirectToAction("ProductSelection");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while placing the order.";
                return RedirectToAction("ProductSelection");
            }
        }

    }
}