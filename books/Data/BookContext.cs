using books.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace books.Data
{
    public sealed class BookContext : IdentityDbContext<User>
    {
        public BookContext(DbContextOptions<BookContext> options)
            : base(options)
        {
            Database.EnsureCreated();   
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Tag> Tags => Set<Tag>();
        

    }
}
