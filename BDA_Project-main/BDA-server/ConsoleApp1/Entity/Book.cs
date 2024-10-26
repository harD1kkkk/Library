using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Entity
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "title is required.")]
        [StringLength(255, ErrorMessage = "title cannot exceed 255 characters.")]
        public required string title { get; set; }

        [Required(ErrorMessage = "author is required.")]
        [StringLength(100, ErrorMessage = "author cannot exceed 100 characters.")]
        public required string author { get; set; }

        public required string genre { get; set; }

      
        public required string description { get; set; }

        public required string imagePath { get; set; }

        public decimal averageRating { get; set; } = 0;

        public int totalReviews { get; set; } = 0;

        public DateTime createdAt { get; set; } = DateTime.UtcNow;
    }
}
