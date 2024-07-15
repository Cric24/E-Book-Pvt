namespace BookStore.Models
{
    public class BookDetails
    {
        public int Id { get; set; } // Unique identifier for the book
        public string Title { get; set; } // Title of the book
        public string Author { get; set; } // Author of the book
        public string ISBN { get; set; } // ISBN of the book
        public int Year { get; set; } // Year of publication
        public decimal Price { get; set; } // Price of the book
        public int Quantity { get; set; } // Quantity available
        public string Description { get; set; } // Description of the book
        public string Categories { get; set; } // Categories or genres of the book

    }


   
}
