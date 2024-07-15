using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BookStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly AuthDbContext _context;

        public OrderController(AuthDbContext context)
        {
            _context = context;
        }

        // GET: /Order/Add
        [HttpGet]
        public IActionResult Add()
        {
            return View("~/Views/Home/ManageOrders.cshtml");
        }

        // POST: /Order/Add
        [HttpPost]
        public IActionResult AddOrder(string bookTitle, int customerId, DateTime orderDate, int quantity, decimal unitPrice, string status)
        {
            try
            {
                if (customerId <= 0 || quantity <= 0 || unitPrice <= 0)
                {
                    ViewBag.ErrorMessage = "Customer ID, quantity, and unit price must be greater than 0.";
                    return View("~/Views/Home/ManageOrders.cshtml");
                }

                // Fetch book details by title
                var book = _context.BookDetails.FirstOrDefault(b => b.Title == bookTitle);
                if (book == null)
                {
                    ViewBag.ErrorMessage = "Book not found.";
                    return View("~/Views/Home/ManageOrders.cshtml");
                }

                // Check if requested quantity is available
                if (book.Quantity < quantity)
                {
                    ViewBag.ErrorMessage = "Requested quantity not available.";
                    return View("~/Views/Home/ManageOrders.cshtml");
                }

                var newOrder = new Order
                {
                    BookTitle = bookTitle,
                    CustomerId = customerId,
                    OrderDate = orderDate,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    TotalAmount = quantity * unitPrice,
                    Status = status // Set the status received from the form
                };

                // Deduct quantity from book details
                book.Quantity -= quantity;

                _context.Orders.Add(newOrder);
                _context.SaveChanges();

                ViewBag.SuccessMessage = "Order added successfully!";
                return View("~/Views/Home/ManageOrders.cshtml");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while adding the order: " + ex.Message +
                    " Inner Exception: " + ex.InnerException?.Message;
                return View("~/Views/Home/ManageOrders.cshtml");
            }
        }




        // POST: /Order/DeleteOrder
        [HttpPost]
        public IActionResult DeleteOrder(int orderId)
        {
            try
            {
                var orderToDelete = _context.Orders.Find(orderId);
                if (orderToDelete == null)
                {
                    ViewBag.ErrorMessage = "Order not found.";
                    return View("~/Views/Home/ManageOrders.cshtml");
                }

                _context.Orders.Remove(orderToDelete);
                _context.SaveChanges();

                ViewBag.SuccessMessage = "Order deleted successfully!";
                return View("~/Views/Home/ManageOrders.cshtml");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while deleting the order: " + ex.Message +
                    " Inner Exception: " + ex.InnerException?.Message;
                return View("~/Views/Home/ManageOrders.cshtml");
            }
        }




        //update

        [HttpPost]
        public IActionResult UpdateOrder(int updateOrderId, string updateBookTitle, int? updateCustomerId, DateTime? updateOrderDate, int? updateQuantity, decimal? updateUnitPrice, string updateStatus)
        {
            try
            {
                // Find the order by ID
                var orderToUpdate = _context.Orders.Find(updateOrderId);

                // Check if the order exists
                if (orderToUpdate == null)
                {
                    ViewBag.UpdateErrorMessage = "Order not found.";
                    return View("~/Views/Home/ManageOrders.cshtml");
                }

                // Update only the provided properties
                if (!string.IsNullOrEmpty(updateBookTitle))
                    orderToUpdate.BookTitle = updateBookTitle;
                if (updateCustomerId.HasValue)
                    orderToUpdate.CustomerId = updateCustomerId.Value;
                if (updateOrderDate.HasValue)
                    orderToUpdate.OrderDate = updateOrderDate.Value;
                if (updateQuantity.HasValue)
                    orderToUpdate.Quantity = updateQuantity.Value;
                if (updateUnitPrice.HasValue)
                {
                    orderToUpdate.UnitPrice = updateUnitPrice.Value;
                    // Recalculate TotalAmount if UnitPrice is updated
                    orderToUpdate.TotalAmount = updateQuantity.Value * updateUnitPrice.Value;
                }
                if (!string.IsNullOrEmpty(updateStatus))
                    orderToUpdate.Status = updateStatus;

                // Save changes to the database
                _context.SaveChanges();

                ViewBag.UpdateSuccessMessage = "Order updated successfully!";
                return View("~/Views/Home/ManageOrders.cshtml");
            }
            catch (Exception ex)
            {
                ViewBag.UpdateErrorMessage = "An error occurred while updating the order: " + ex.Message +
                    " Inner Exception: " + ex.InnerException?.Message;
                return View("~/Views/Home/ManageOrders.cshtml");
            }
        }



        // GET: /Order/GetOrders
        [HttpGet]
        public IActionResult GetOrders()
        {
            var orders = _context.Orders
                .Select(o => new
                {
                    Id = o.Id,
                    BookTitle = o.BookTitle,
                    CustomerId = o.CustomerId,
                    OrderDate = o.OrderDate.ToString("yyyy-MM-dd"),
                    Quantity = o.Quantity,
                    UnitPrice = o.UnitPrice,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status ?? "Unknown"
                })
                .ToList();

            return Json(orders);
        }













    }
}
