using books.Models;

namespace books.Data
{
    public static class InitBooks
    {
        public static async Task Initialize(BookContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (context.Books.Any() == false)
            {
                context.Books.AddRange(
                    new Book()
                    {
                        Title = "Brave New World",
                        Author = "Aldous Huxley",
                        Genre = "Science fiction",
                        Publishing = "Chatto & Windus",
                        PublicYear = 1932,
                        CoverPath = "BNW.jpg"
                    },
                    new Book()
                    {
                        Title = "The Call of the Wild",
                        Author = "Jack London",
                        Genre = "Adventure fiction",
                        Publishing = "The Macmillan Company",
                        PublicYear = 1903,
                        CoverPath = "CotW.jpg"
                    },
                    new Book()
                    {
                        Title = "1984",
                        Author = "George Orwell",
                        Genre = "Political fiction",
                        Publishing = "Secker & Warburg",
                        PublicYear = 1949, 
                        CoverPath = "1984.jpg"
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
