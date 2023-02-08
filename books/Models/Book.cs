using System.ComponentModel.DataAnnotations;

namespace books.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Author { get; set; } = default!;
        public string Genre { get; set; } = default!;
        public string Publishing { get; set; } = default!;
        public int PublicYear { get; set; } 
        public string? CoverPath { get; set; } = default!;
    }
}
