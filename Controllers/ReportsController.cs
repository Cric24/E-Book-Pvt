using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using System;
using System.Linq;

namespace BookStore.Controllers
{
    public class ReportsController : Controller
    {
        private readonly AuthDbContext _context;

        public ReportsController(AuthDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetOrderData()
        {
            try
            {
                var orderData = _context.Orders
                    .Select(o => new { Date = o.OrderDate.ToShortDateString(), Amount = o.Quantity * o.UnitPrice })
                    .ToList();

                return Json(orderData);
            }
            catch (Exception ex)
            {
                return BadRequest("Error fetching order data: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAnotherChartData()
        {
            try
            {
                var anotherChartData = _context.Orders
                    .Select(o => new { Date = o.OrderDate.ToShortDateString(), Quantity = o.Quantity })
                    .ToList();

                return Json(anotherChartData);
            }
            catch (Exception ex)
            {
                return BadRequest("Error fetching data: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetSalesByBook()
        {
            try
            {
                var salesByBookData = _context.Orders
                    .GroupBy(o => o.BookTitle)
                    .Select(g => new { BookTitle = g.Key, TotalSales = g.Sum(o => o.Quantity * o.UnitPrice) })
                    .ToList();

                return Json(salesByBookData);
            }
            catch (Exception ex)
            {
                return BadRequest("Error fetching sales data: " + ex.Message);
            }
        }
    }
}
