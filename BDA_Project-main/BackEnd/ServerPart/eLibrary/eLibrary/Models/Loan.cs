
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using System.Text;


namespace eLibrary.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int BookId { get; set; }

        public DateTime LoanDate { get; set; } 

        public DateTime? ReturnDate { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsExtended { get; set; } = false;

        public User User { get; set; }

        public Book Book { get; set; }
    }
}
