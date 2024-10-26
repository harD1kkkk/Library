using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1.Entity
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public required string Password { get; set; } // Save hash of password

        public override string? ToString()
        {
            return $"Name: {Name}, Email: {Email}, Password: {Password}";
        }

        public int PositiveRating { get; set; } = 0;
        public int NegativeRating { get; set; } = 0;
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
    }
}
