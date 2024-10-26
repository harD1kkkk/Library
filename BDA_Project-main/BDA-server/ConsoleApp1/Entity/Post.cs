using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1.Entity
{
    public class Post
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "title is required")]
        [StringLength(255, ErrorMessage = "title cannot exceed 255 characters")]
        public required string title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public required string Content { get; set; }

        [Required(ErrorMessage = "author ID is required")]
        public int authorId { get; set; }

        public DateTime createdAt { get; set; } = DateTime.UtcNow;
    }
}
