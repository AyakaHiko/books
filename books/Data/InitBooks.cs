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
                        //CoverPath = "BNW.jpg"
                        CoverPath = "https://i.pinimg.com/originals/9d/52/be/9d52be28804be34589bbd623273f789f.jpg"
                    },
                    new Book()
                    {
                        Title = "The Call of the Wild",
                        Author = "Jack London",
                        Genre = "Adventure fiction",
                        Publishing = "The Macmillan Company",
                        PublicYear = 1903,
                        //CoverPath = "CotW.jpg"
                        CoverPath = "http://books.disney.com/content/uploads/2019/11/1368060781-1-705x1024.jpg"
                    },
                    new Book()
                    {
                        Title = "1984",
                        Author = "George Orwell",
                        Genre = "Political fiction",
                        Publishing = "Secker & Warburg",
                        PublicYear = 1949,
                        //CoverPath = "1984.jpg"
                        CoverPath = "https://i.pinimg.com/originals/0d/2c/09/0d2c0915b3c86c8ac0680f3f6c88731d.jpg"

                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
