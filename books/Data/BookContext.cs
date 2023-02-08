using books.Models;
using Microsoft.EntityFrameworkCore;

namespace books.Data
{
    public sealed class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
    }
}
