using System.Security.Claims;
using books.Authorization;
using books.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace books.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
        {
            DbContextOptions<BookContext> options =
                serviceProvider.GetRequiredService<DbContextOptions<BookContext>>();
            BookContext context = new BookContext(options);
            UserManager<User> userManager =
                serviceProvider.GetRequiredService<UserManager<User>>();
            //await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            if (!context.Users.Any())
            {
                var sa = await _createUser(userManager, "superadmin@example.com", "1");
                await userManager.AddClaimsAsync(sa, new[] { Claims.MemberClaim, Claims.AdminClaim, Claims.SuperAdminClaim });
                var a = await _createUser(userManager, "admin@example.com", "1");
                await userManager.AddClaimsAsync(a, new[] { Claims.MemberClaim, Claims.AdminClaim });
                var m = await _createUser(userManager, "member@example.com", "1");
                await userManager.AddClaimsAsync(m, new[] { Claims.MemberClaim });
            }

            if (!context.Books.Any())
            {

                var author1 = new Author { Name = "George", Surname = "Orwell" };
                var author2 = new Author { Name = "Aldous", Surname = "Huxley" };
                var author3 = new Author { Name = "Chuck", Surname = "Palahniuk" };
                var author4 = new Author { Name = "George", Surname = "Orwell" };

                await context.Authors.AddRangeAsync(author1, author2, author3, author4);

                var genre1 = new Genre { Name = "Science Fiction" };
                var genre2 = new Genre { Name = "Satire" };
                var genre3 = new Genre { Name = "Dystopia" };

                await context.Genres.AddRangeAsync(genre1, genre2, genre3);

                var book1 = new Book
                {
                    Title = "1984",
                    Author = author1,
                    Genre = genre1,
                    Publishing = "Secker and Warburg",
                    PublicYear = 1949,
                    Cover = await File.ReadAllBytesAsync($"{webHostEnvironment.WebRootPath}\\img\\1984.jpg")
                };

                var book2 = new Book
                {
                    Title = "Brave New World",
                    Author = author2,
                    Genre = genre1,
                    Publishing = "Chatto & Windus",
                    PublicYear = 1932,
                    Cover = await File.ReadAllBytesAsync($"{webHostEnvironment.WebRootPath}\\img\\BNW.jpg")
                };

                var book3 = new Book
                {
                    Title = "Animal Farm",
                    Author = author4,
                    Genre = genre2,
                    Publishing = "Secker and Warburg",
                    PublicYear = 1945,
                    Cover = await File.ReadAllBytesAsync($"{webHostEnvironment.WebRootPath}\\img\\AF.jpg")

                };

                var book4 = new Book
                {
                    Title = "Fight Club",
                    Author = author3,
                    Genre = genre2,
                    Publishing = "W. W. Norton & Company",
                    PublicYear = 1996,
                    Cover = await File.ReadAllBytesAsync($"{webHostEnvironment.WebRootPath}\\img\\FC.jpg")
                };

                await context.Books.AddRangeAsync(book1, book2, book3, book4);
            }

            await context.SaveChangesAsync();
        }
        

        private static async void _createUsers(UserManager<User> userManager)
        {

        }

        private static async Task<User> _createUser(UserManager<User> userManager, string email, string password)
        {
            var user = new User { UserName = email, Email = email, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create user: {result.Errors.First().Description}");
            }

            return user;
        }
    }

}
