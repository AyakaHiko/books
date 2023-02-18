using books.Data;
using books.Models;

namespace books.Services;

public class BookService : IBookService
{
    private readonly BookContext _bookContext;

    public BookService(BookContext productsBookContext)
    {
        _bookContext = productsBookContext;
    }
        
    public IQueryable<Book> GetBooks()
    {
        IQueryable<Book> products = _bookContext.Books;
        return products;
    }

    public Book? GetBook(int id)
    {
        return _bookContext.Books.Find(id);
    }
}