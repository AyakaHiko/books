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
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<User> Users  => Set<User>();

    }
}
