using System.ComponentModel.DataAnnotations;

namespace books.Models;

    public class Tag
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public List<Book>? Books { get; set; }
    }