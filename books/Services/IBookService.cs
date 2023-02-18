using books.Models;

namespace books.Services
{
    public interface IBookService
    {
        IQueryable<Book> GetBooks();
        Book? GetBook(int id);
    }
}
