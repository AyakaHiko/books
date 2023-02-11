using System.ComponentModel.DataAnnotations;

namespace books.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public int? AuthorId { get; set; } = default!;
        public Author? Author { get; set; }
        public int? GenreId { get; set; }
        public Genre? Genre { get; set; }
        public string Publishing { get; set; } = default!;
        public int PublicYear { get; set; } 
        public byte[]? Cover { get; set; }
    }
}
