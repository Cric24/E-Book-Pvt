using Microsoft.AspNetCore.Mvc;
using BookStore.Models;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly AuthDbContext _context;

        public BookController(AuthDbContext context)
        {
            _context = context;
        }


        // POST: /Book/Add
        [HttpPost]
        public IActionResult AddBook(string title, string author, string isbn, int year, decimal price, int quantity, string description, string categories)
        {
            // Check if all required parameters are provided
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(isbn))
            {
                ViewBag.ErrorMessage = "Title and ISBN are required.";
                return View("~/Views/Home/Managebooks.cshtml");
            }

            try
            {
                // Check if the book already exists
                var existingBook = _context.BookDetails.FirstOrDefault(b => b.Title == title && b.ISBN == isbn);
                if (existingBook != null)
                {
                    ViewBag.ErrorMessage = "Book with the same title and ISBN already exists.";
                    return View("~/Views/Home/Managebooks.cshtml");
                }

                // Create a new book instance
                var newBook = new BookDetails
                {
                    Title = title,
                    Author = author,
                    ISBN = isbn,
                    Year = year,
                    Price = price,
                    Quantity = quantity,
                    Description = description,
                    Categories = categories
                };

                // Add the new book to the context
                _context.BookDetails.Add(newBook);
                // Save changes to the database
                _context.SaveChanges();

                ViewBag.AddSuccessMessage = "Book added successfully!";
                return View("~/Views/Home/Managebooks.cshtml");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while adding the book: " + ex.Message +
                    " Inner Exception: " + ex.InnerException?.Message;
                return View("~/Views/Home/Managebooks.cshtml"); // Return the Managebooks view with error message
            }
        }




       
        // POST: /Book/DeleteBook
        [HttpPost]
        public IActionResult DeleteBook(int id)
        {
            try
            {
                // Find the book by ID
                var bookToDelete = _context.BookDetails.Find(id);

                // Check if the book exists
                if (bookToDelete == null)
                {
                    ViewBag.ErrorMessage = "Book not found.";
                    return View("~/Views/Home/Managebooks.cshtml");
                }

                // Remove the book from the context
                _context.BookDetails.Remove(bookToDelete);
                // Save changes to the database
                _context.SaveChanges();

                ViewBag.DeleteSuccessMessage = "Book deleted successfully!";
                return View("~/Views/Home/Managebooks.cshtml");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while deleting the book: " + ex.Message +
                    " Inner Exception: " + ex.InnerException?.Message;
                return View("~/Views/Home/Managebooks.cshtml"); // Return the Managebooks view with error message
            }
        }



        // POST: /Book/UpdateBook
        [HttpPost]
        public IActionResult UpdateBook(int id, string title, string author, string isbn, int year, decimal price, int quantity, string description, string categories)
        {
            try
            {
                // Find the book by ID
                var bookToUpdate = _context.BookDetails.Find(id);

                // Check if the book exists
                if (bookToUpdate == null)
                {
                    ViewBag.ErrorMessage = "Book not found.";
                    return View("~/Views/Home/Managebooks.cshtml");
                }

                // Update only the provided properties
                if (!string.IsNullOrEmpty(title))
                    bookToUpdate.Title = title;
                if (!string.IsNullOrEmpty(author))
                    bookToUpdate.Author = author;
                if (!string.IsNullOrEmpty(isbn))
                    bookToUpdate.ISBN = isbn;
                if (year != 0)
                    bookToUpdate.Year = year;
                if (price != 0)
                    bookToUpdate.Price = price;
                if (quantity != 0)
                    bookToUpdate.Quantity = quantity;
                if (!string.IsNullOrEmpty(description))
                    bookToUpdate.Description = description;
                if (!string.IsNullOrEmpty(categories))
                    bookToUpdate.Categories = categories;

                // Save changes to the database
                _context.SaveChanges();

                ViewBag.UpdateSuccessMessage = "Book updated successfully!";
                return View("~/Views/Home/Managebooks.cshtml");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while updating the book: " + ex.Message +
                    " Inner Exception: " + ex.InnerException?.Message;
                return View("~/Views/Home/Managebooks.cshtml"); // Return the Managebooks view with error message
            }
        }


        // GET: /Book/GetBooks
        [HttpGet]
        public IActionResult GetBooks()
        {
            var books = _context.BookDetails
                .Select(b => new BookDetails
                {
                    Id = b.Id,
                    Title = b.Title ?? "Unknown",
                    Author = b.Author ?? "Unknown",
                    ISBN = b.ISBN ?? "Unknown",
                    Year = b.Year,
                    Price = b.Price,
                    Quantity = b.Quantity,
                    Description = b.Description ?? "Unknown",
                    Categories = b.Categories ?? "Unknown"
                })
                .ToList();

            return Json(books);
        }






    }
}