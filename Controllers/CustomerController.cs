using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace BookStore.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AuthDbContext _context;

        public CustomerController(AuthDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Customer/CustomerAuth")]
        public IActionResult CustomerAuth()
        {
            return View("~/Views/Home/CustomerAuth.cshtml");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Redirect to the CustomerDashboard action
                return RedirectToAction("CustomerDashboard", new { username });

            }
            else
            {
                TempData["ErrorMessage"] = "Invalid username or password.";
                return RedirectToAction("CustomerAuth", "Customer");
            }
        }

        public IActionResult Logout()
        {
            
            // Redirect the user to the login page
            return RedirectToAction("CustomerAuth", "Customer");
        }



        public IActionResult CustomerDashboard(string username)
        {
            // Query the database to retrieve the customer details based on the username
            var customer = _context.Customers.FirstOrDefault(c => c.Name == username);

            if (customer == null)
            {
                // Log a message if the customer record is not found
                Console.WriteLine($"No customer record found for username: {username}");
            }

            // Pass the customer name and ID to the view using ViewData
            ViewData["CustomerName"] = customer?.Name;
            ViewData["CustomerId"] = customer?.Id.ToString();

            // Return the view
            return View("~/Views/Home/CustomerDashboard.cshtml", customer);
        }



        // GET: /Customer/GetCustomerOrders
        [HttpGet]
        public IActionResult GetCustomerOrders(int customerId)
        {
            // Retrieve orders associated with the customer ID
            var orders = _context.Orders
                .Where(o => o.CustomerId == customerId)
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

        // GET: /Customer/GetAvailableBooks
        [HttpGet]
        public IActionResult GetAvailableBooks()
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










        [HttpGet]
        [Route("Customer/Register")]
        public IActionResult Register()

        {
            ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            return View("~/Views/Home/Register.cshtml");
        }

      
        [HttpPost]
        public IActionResult Register(string name, string email, string address, string phoneNumber, string gender, string username, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Passwords do not match.";
                return RedirectToAction("Register");
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Username == username);
            if (existingUser != null)
            {
                TempData["ErrorMessage"] = "Username already exists.";
                return RedirectToAction("Register");
            }

            // Create a new user and add it to the database
            var newUser = new User
            {
                Username = username,
                Password = password
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Create a new customer and add it to the database
            var newCustomer = new Customer
            {
                Name = name,
                Email = email,
                Address = address,
                PhoneNumber = phoneNumber,
                Gender = gender
            };

            _context.Customers.Add(newCustomer);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful. You can now log in.";
            return RedirectToAction("Register");
        }








        // POST: /Customer/Add
        [HttpPost]
        public IActionResult AddCustomer(string name, string email, string address, string phoneNumber, string gender)
        {
            // Check if all required parameters are provided
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
            {
                ViewBag.ErrorMessage = "Name and Email are required.";
                return View("~/Views/Home/ManageCustomer.cshtml");
            }

            try
            {
                // Create a new customer instance
                var newCustomer = new Customer
                {
                    Name = name,
                    Email = email,
                    Address = address,
                    PhoneNumber = phoneNumber,
                    Gender = gender
                };

                // Add the new customer to the context
                _context.Customers.Add(newCustomer);
                // Save changes to the database
                _context.SaveChanges();

                ViewBag.SuccessMessage = "Customer added successfully!";
                return View("~/Views/Home/ManageCustomer.cshtml");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while adding the customer: " + ex.Message +
                    " Inner Exception: " + ex.InnerException?.Message;
                return View("~/Views/Home/ManageCustomer.cshtml"); // Return the ManageCustomers view with error message
            }

        
    }
        //delete
        [HttpPost]
        public IActionResult DeleteCustomer(int id)
        {
            try
            {
                // Find the customer by ID
                var customerToDelete = _context.Customers.Find(id);

                // Check if the customer exists
                if (customerToDelete == null)
                {
                    ViewBag.DeleteErrorMessage = "Customer not found.";
                    return View("~/Views/Home/ManageCustomer.cshtml");
                }

                // Remove the customer from the context
                _context.Customers.Remove(customerToDelete);
                // Save changes to the database
                _context.SaveChanges();

                ViewBag.DeleteSuccessMessage = "Customer deleted successfully!";
                return View("~/Views/Home/ManageCustomer.cshtml");
            }
            catch (Exception ex)
            {
                ViewBag.DeleteErrorMessage = "An error occurred while deleting the customer: " + ex.Message +
                    " Inner Exception: " + ex.InnerException?.Message;
                return View("~/Views/Home/ManageCustomer.cshtml"); // Return the ManageCustomer view with error message
            }
        }

        //update

        [HttpPost]
        public IActionResult UpdateCustomer(int id, string name, string email, string address, string phoneNumber, string gender)
        {
            try
            {
                // Find the customer by ID
                var customerToUpdate = _context.Customers.Find(id);

                // Check if the customer exists
                if (customerToUpdate == null)
                {
                    ViewBag.UpdateErrorMessage = "Customer not found.";
                    return View("~/Views/Home/ManageCustomer.cshtml");
                }

                // Update only the provided properties
                if (!string.IsNullOrEmpty(name))
                    customerToUpdate.Name = name;
                if (!string.IsNullOrEmpty(email))
                    customerToUpdate.Email = email;
                if (!string.IsNullOrEmpty(address))
                    customerToUpdate.Address = address;
                if (!string.IsNullOrEmpty(phoneNumber))
                    customerToUpdate.PhoneNumber = phoneNumber;
                if (!string.IsNullOrEmpty(gender))
                    customerToUpdate.Gender = gender;

                // Save changes to the database
                _context.SaveChanges();

                ViewBag.UpdateSuccessMessage = "Customer updated successfully!";
                return View("~/Views/Home/ManageCustomer.cshtml");
            }
            catch (Exception ex)
            {
                ViewBag.UpdateErrorMessage = "An error occurred while updating the customer: " + ex.Message +
                    " Inner Exception: " + ex.InnerException?.Message;
                return View("~/Views/Home/ManageCustomer.cshtml");
            }
        }


        // GET: /Customer/GetCustomers
        [HttpGet]
        public IActionResult GetCustomers()
        {
            var customers = _context.Customers
                .Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name ?? "Unknown",
                    Email = c.Email ?? "Unknown",
                    Address = c.Address ?? "Unknown",
                    PhoneNumber = c.PhoneNumber ?? "Unknown",
                    Gender = c.Gender ?? "Unknown"
                })
                .ToList();

            return Json(customers);
        }




        //search customers

        [HttpPost]
        public IActionResult SearchCustomer(int? searchId, string searchName)
        {
            try
            {
                Customer customer;

                if (searchId.HasValue)
                {
                    // Find the customer in the database by ID
                    customer = _context.Customers.FirstOrDefault(c => c.Id == searchId);
                }
                else if (!string.IsNullOrEmpty(searchName))
                {
                    // Find the customer in the database by name
                    customer = _context.Customers.FirstOrDefault(c => c.Name == searchName);
                }
                else
                {
                    ViewBag.SearchErrorMessage = "Please provide either the ID or the name of the customer.";
                    ViewBag.CustomerId = null; // Set ViewBag properties to null if search criteria not provided
                    ViewBag.CustomerName = null;
                    return View("~/Views/Home/ManageOrders.cshtml");
                }

                // Check if the customer exists
                if (customer == null)
                {
                    ViewBag.SearchErrorMessage = "Customer not found.";
                    ViewBag.CustomerId = null; // Set ViewBag properties to null if customer not found
                    ViewBag.CustomerName = null;
                }
                else
                {
                    // Set ViewBag properties to store the customer's ID and name for display in the view
                    ViewBag.CustomerId = customer.Id.ToString();
                    ViewBag.CustomerName = customer.Name;
                    ViewBag.SearchErrorMessage = null; // Clear any previous error message
                }

                return View("~/Views/Home/ManageOrders.cshtml");
            }
            catch (Exception ex)
            {
                ViewBag.SearchErrorMessage = "An error occurred while searching for the customer: " + ex.Message +
                    " Inner Exception: " + ex.InnerException?.Message;
                ViewBag.CustomerId = null; // Set ViewBag properties to null in case of error
                ViewBag.CustomerName = null;
                return View("~/Views/Home/ManageCustomers.cshtml");
            }
        }




        //inherit to Orders page
        // GET: /Customer/GetCustomerNames
        [HttpGet]
        public IActionResult GetCustomerNames()
        {
            var customers = _context.Customers
                .Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name ?? "Unknown"
                })
                .ToList();

            return Json(customers);
        }



        [HttpPost]
        public IActionResult AddToCart(int customerId, int bookId, int quantity)
        {
            try
            {
                // Check if the customer exists
                var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);
                if (customer == null)
                {
                    return BadRequest("Customer not found.");
                }

                // Check if the book exists
                var book = _context.BookDetails.FirstOrDefault(b => b.Id == bookId);
                if (book == null)
                {
                    return BadRequest("Book not found.");
                }

                // Check if the requested quantity is available
                if (book.Quantity < quantity)
                {
                    return BadRequest("Not enough quantity available.");
                }

                // Fetch the unit price of the book
                var unitPrice = book.Price; 

                // Create a new cart item
                var cartItem = new Cart
                {
                    CustomerId = customerId,
                    BookId = bookId,
                    Quantity = quantity,
                    UnitPrice = unitPrice // Set the unit price
                };

                // Add the cart item to the database
                _context.Cart.Add(cartItem);
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the book to the cart: {ex.Message}");
            }
        }


        // Method to delete a cart item
        public ActionResult DeleteCartItem(int cartItemId)
        {
            var cartItem = _context.Cart.FirstOrDefault(c => c.CartItemId == cartItemId);

            if (cartItem == null)
            {
                ViewBag.ErrorMessage = "Cart item not found.";
                return View("Error");
            }

            _context.Cart.Remove(cartItem);
            _context.SaveChanges();



            // Redirect back to the customer dashboard or any appropriate page after deletion
            return RedirectToAction("Index", "Customer");
        }




        [HttpGet]
        public IActionResult GetCustomerCart(int customerId)
        {
            try
            {
                // Retrieve cart items for the given customer including book details
                var cartItems = _context.Cart
                    .Include(c => c.Book) // Include the associated book details
                    .Where(c => c.CustomerId == customerId)
                    .ToList();

                // Project the cart items to a DTO or ViewModel to send to the client
                var cartItemsDto = cartItems.Select(c => new
                {
                    c.CartItemId,
                    BookTitle = c.Book.Title, // Use the book title from the associated book details
                    c.Quantity,
                    c.UnitPrice,
                    c.TotalAmount
                });

                return Ok(cartItemsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching cart items: {ex.Message}");
            }
        }


        [HttpPost]
        public IActionResult AddToOrders(int customerId, int cartItemId)
        {
            try
            {
                // Check if the customer exists
                var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);
                if (customer == null)
                {
                    return BadRequest("Customer not found.");
                }

                // Get the cart item by ID
                var cartItem = _context.Cart.FirstOrDefault(c => c.CartItemId == cartItemId);
                if (cartItem == null)
                {
                    return BadRequest("Cart item not found.");
                }

                // Fetch the corresponding book details based on the bookId in the cart item
                var bookDetails = _context.BookDetails.FirstOrDefault(b => b.Id == cartItem.BookId);
                if (bookDetails == null)
                {
                    return BadRequest("Book details not found.");
                }

                // Ensure there are enough quantities available
                if (bookDetails.Quantity < cartItem.Quantity)
                {
                    return BadRequest("Not enough quantity available.");
                }

                // Create a new order
                var order = new Order
                {
                    CustomerId = customerId,
                    BookTitle = bookDetails.Title, // Use the book title from the associated book details
                    OrderDate = DateTime.Now,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    TotalAmount = cartItem.Quantity * cartItem.UnitPrice,
                    Status = "Pending" // Assuming initial status is pending
                };

                // Deduct the quantity from BookDetails
                bookDetails.Quantity -= cartItem.Quantity;

                // Add the order to the database
                _context.Orders.Add(order);

                // Remove the cart item
                _context.Cart.Remove(cartItem);

                // Save changes to the database
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while moving item to orders: {ex.Message}");
            }
        }


        [HttpDelete]
        public IActionResult DeleteOrder(int orderId)
        {
            // Find the order with the specified orderId
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound(); // Return 404 Not Found if the order is not found
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return Ok(); // Return  OK if the order is successfully deleted
        }






        [HttpPost]
        public IActionResult SubmitFeedback([FromBody] Feedback feedback)
        {
            if (feedback == null)
            {
                return BadRequest("Feedback object is null");
            }

            try
            {
                // Save feedback to your database
                _context.Feedback.Add(feedback);
                _context.SaveChanges();

                return Ok("Feedback submitted successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while submitting feedback: {ex.Message}");
            }
        }


        // Search In Home
        [HttpGet]
        public IActionResult SearchBooks(string title)
        {
            // Perform the search based on the provided title
            var searchResults = _context.BookDetails
                .Where(book => book.Title.Contains(title))
                .ToList();

            return Json(searchResults);
        }


        
        public IActionResult Feedback()
        {
            return View("~/Views/Home/Feedback.cshtml");
        }


        public IActionResult Password()
        {
            return View("~/Views/Home/Password.cshtml");
        }


        public IActionResult UpdatePassword(User user, string newPassword, string confirmPassword)
        {
            if (!ModelState.IsValid)
            {
                return View("Password", user);
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);

            if (existingUser == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Password");
            }

            if (existingUser.Password != user.Password)
            {
                TempData["ErrorMessage"] = "Old password is incorrect.";
                return RedirectToAction("Password");
            }

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "New password and confirm password do not match.";
                return RedirectToAction("Password");
            }

            existingUser.Password = newPassword;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Password updated successfully.";
            return RedirectToAction("Password");

        }


        // Action method to manage feedback
        public async Task<IActionResult> ManageFeedback()
        {
            var feedbacks = await _context.Feedback.ToListAsync();
            return View("~/Views/Home/ManageFeedback.cshtml", feedbacks);
        }

    }
}
