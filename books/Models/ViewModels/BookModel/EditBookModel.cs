using books.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace books.Models.ViewModels.BookModel
{
    public class EditBookModel
    {
        public BookDTO Book { get; set; } = default!;
        public IFormFile? Cover { get; set; }
        public SelectList? GenreList { get; set; }
        public SelectList? AuthorList { get; set; }
        public string[]? Tags { get; set; }

    }
}
