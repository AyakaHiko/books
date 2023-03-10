namespace books.Models.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = default!;
        public int? AuthorId { get; set; } = default!;
        public AuthorDTO? Author { get; set; }
        public int? GenreId { get; set; }
        public GenreDTO? Genre { get; set; }
        public string Publishing { get; set; } = default!;
        public int PublicYear { get; set; }
        public List<Tag>? Tags { get; set; } = default!;

        public string? CoverPath { get; set; } = default!;
    }

    public class GenreDTO
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = default!;
        public List<BookDTO>? Books { get; set; }

    }

    public class AuthorDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
        public List<BookDTO>? Books { get; set; }
        public string FullName => $"{Name} {Surname}";
        public override string ToString() => FullName;
    }
}
