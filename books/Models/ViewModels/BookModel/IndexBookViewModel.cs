using books.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace books.Models.ViewModels.BookModel
{
    public class IndexBookViewModel
    {
        public IEnumerable<BookDTO> Books { get; set; } = default!;
        public SelectList GenreSelect { get; set; } = default!;
        public SelectList AuthorSelect { get; set; } = default!;
        public int GenreId { get; set; }
        public int AuthorId { get; set; }
        public string? Search { get; set; }
    }
}
