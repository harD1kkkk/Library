
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using System.Text;


namespace eLibrary.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Genre { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public decimal AverageRating { get; set; }

        public int TotalReviews { get; set; }

        public DateTime CreatedAt { get; set; } 
    }
}
