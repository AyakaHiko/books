using books.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using books.Services;
using Microsoft.EntityFrameworkCore;

namespace books.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService _bookService;

        public HomeController(ILogger<HomeController> logger, IBookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        public async Task<ActionResult<IEnumerable<Book>>> Index()
        {
            IQueryable<Book> products = _bookService.GetBooks();

            return View(await products.ToListAsync());
        }

        public IActionResult Edit()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Details(int id)
        {
            return View(_bookService.GetBook(id));
        }
    }
}