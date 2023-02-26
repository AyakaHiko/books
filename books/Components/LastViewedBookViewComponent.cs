using books.Extensions;
using books.Models;
using Microsoft.AspNetCore.Mvc;

namespace books.Components
{
    public class LastViewedBookViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            List<Book?> books = new List<Book?>();
            foreach (var key in HttpContext.Session.Keys.Where(k=> k.Contains("LastViewedBook")))
            {
                books.Add(HttpContext.Session.Get<Book>(key));
            }
            return View(books);
        }
    }
}
