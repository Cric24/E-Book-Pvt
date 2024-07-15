using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;

namespace BookStore
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {

        }

        // DbSet for the Book entity
        public DbSet<BookDetails> BookDetails { get; set; }



        // DbSet for the Customer entity
        public DbSet<Customer> Customers { get; set; }

        // DbSet for the Order entity
        public DbSet<Order> Orders { get; set; }


        // DbSet for the Customer Login entity
        public DbSet<User> Users { get; set; }

        public DbSet<Cart> Cart { get; set; }


        // DbSet for the Feedback entity with table name Feedback
        public DbSet<Feedback> Feedback { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }




    }
}
