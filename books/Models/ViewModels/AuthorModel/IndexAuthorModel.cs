
using books.Models.DTO;

namespace books.Models.ViewModels.AuthorModel
{
    public class IndexAuthorModel
    {
        public IEnumerable<Author> Authors { get; set; } = default!;
    }
}
