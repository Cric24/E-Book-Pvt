// AdminController.cs
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        // This action method handles customer authentication
        [Route("Admin/AdminAuth")]
        public IActionResult AdminAuth()
        {
            return View("~/Views/Home/AdminAuth.cshtml");
        }

        [HttpPost]
        public IActionResult AdminLogin(string username, string password)
        {
            // validate the admin username and password
        

            if (username == "admin" && password == "adminpassword")
            {
                // If authentication succeeds, redirect the admin to
                // the admin dashboard .
                return RedirectToAction("AdminDashboard");
            }
            else
            {
                // If authentication fails, set an error message in TempData
                TempData["ErrorMessage"] = "Invalid username or password.";

                // Redirect the admin back to the admin login page
                return RedirectToAction("AdminAuth");
            }
        }


        // Action method for Admin dashboard
        public IActionResult AdminDashboard()
        {
          
            return View("~/Views/Home/Admindash.cshtml");
        }
        



        // Action method to manage books details
        public IActionResult ManageBooks()
        {
            
            return View("~/Views/Home/ManageBooks.cshtml");
        }


        // Action method to manage customers
        public IActionResult ManageCustomers()
        {
            
            return View("~/Views/Home/ManageCustomer.cshtml");
        }

        // Action method to manage orders
        public IActionResult ManageOrders()
        {
            
            return View("~/Views/Home/ManageOrders.cshtml");
        }

        // Action method to generate reports
        public IActionResult GenerateReports()
        {
            
            return View("~/Views/Home/GenerateReports.cshtml");
        }


       






    }
}
