using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BookStore.Models
{
    public class Cart
    {

        [Key]
        public int CartItemId { get; set; } // Primary key for the cart item

        [Required]
        public int CustomerId { get; set; } // Foreign key to link the cart item to the customer

        [Required]
        public int BookId { get; set; } // Foreign key to link the cart item to the book

        [Required]
        public int Quantity { get; set; } // Quantity of the book in the cart

        [Required]
        public decimal UnitPrice { get; set; } // Unit price of the book

        public decimal TotalAmount => Quantity * UnitPrice; // Total amount calculated as Quantity * UnitPrice

        // Navigation properties
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } // Navigation property to represent the customer associated with this cart item

        [ForeignKey("BookId")]
        public BookDetails Book { get; set; } // Navigation property to represent the book associated with this cart item

    }
}
