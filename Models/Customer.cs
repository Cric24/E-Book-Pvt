using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }

        public string Gender { get; set; }

        

        
        // public DateTime CreatedAt { get; set; }
    }

    



}
