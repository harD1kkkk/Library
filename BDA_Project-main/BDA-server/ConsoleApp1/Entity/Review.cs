using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Entity
{
    public class Review
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int BookId { get; set; }
        [JsonProperty("reviewText")]
        public required string ReviewText { get; set; }

        public int Rating { get; set; }

        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        public bool IsModerated { get; set; } = false;

        public required User User { get; set; }

        public required Book Book { get; set; }
    }
}
