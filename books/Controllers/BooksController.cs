using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using books.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using books.Data;
using books.Extensions;
using books.Models;
using books.Models.DTO;
using books.Models.ViewModels.BookModel;
using Microsoft.AspNetCore.Authorization;

namespace books.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookContext _context;
        private readonly ILogger _logger;

        private readonly IMapper _mapper;
        public BooksController(BookContext context,
            ILoggerFactory factory,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _mapper = mapper;
            _logger = factory.CreateLogger<BooksController>();
        }


        private void _checkAccess()
        {
            ViewBag.IsAdmin = User.Claims.Any(c => c.Type is Claims.Admin or Claims.SuperAdmin);
        }
        // GET: Books
        public async Task<IActionResult> Index(int genreId, int authorId, string? search)
        {
            _checkAccess();
            IQueryable<Book> bookContext = _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .Include(b => b.Tags)

                ;
            if (genreId > 0)
            {
                bookContext = bookContext.Where(b => b.GenreId == genreId);
            }

            if (authorId > 0)
            {
                bookContext = bookContext.Where(b => b.AuthorId == authorId);
            }

            if (search != null)
            {
                bookContext = bookContext.Where(b => b.Title.Contains(search) ||
                                                     b.Tags.AsEnumerable()
                                                         .Any(t =>
                                                             t.Name.Contains(search)));

            }

            IEnumerable<GenreDTO> genreDtos = _mapper.Map<IEnumerable<GenreDTO>>(await _context.Genres.ToListAsync());
            IEnumerable<AuthorDTO> authorDtos = _mapper.Map<IEnumerable<AuthorDTO>>(await _context.Authors.ToListAsync());

            IEnumerable<BookDTO> bookDtos = _mapper.Map<IEnumerable<BookDTO>>(bookContext);

            SelectList genreList = new SelectList(items: genreDtos, dataValueField: "Id", dataTextField: "Name", selectedValue: genreId);
            SelectList authorList = new SelectList(items: authorDtos, dataValueField: "Id", dataTextField: "FullName", selectedValue: authorId);

            IndexBookViewModel vm = new IndexBookViewModel
            {
                Books = bookDtos,
                GenreSelect = genreList,
                AuthorSelect = authorList
            };
            return View(vm);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var book = _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .Include(b => b.Tags)
                    .FirstOrDefault(b => b.Id == id);

            HttpContext.Session.Set<Book>("LastViewedBook" + book.Id, book);


            DetailsBookModel vm = new DetailsBookModel()
            {
                Book = _mapper.Map<BookDTO>(book)
            };

            return View(vm);

        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            IEnumerable<GenreDTO> genreDtos = _mapper.Map<IEnumerable<GenreDTO>>(await _context.Genres.ToListAsync());
            IEnumerable<AuthorDTO> authorDtos = _mapper.Map<IEnumerable<AuthorDTO>>(await _context.Authors.ToListAsync());

            SelectList genreList = new SelectList(items: genreDtos, dataValueField: "Id", dataTextField: "Name");
            SelectList authorList = new SelectList(items: authorDtos, dataValueField: "Id", dataTextField: "FullName");
            CreateBookModel vm = new CreateBookModel
            {
                GenreList = genreList,
                AuthorList = authorList
            };


            return View(vm);
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Book.CoverPath = await _coverUpload(viewModel.Cover);
                if (viewModel.Tags != null)
                {
                    viewModel.Book.Tags = await _handleTags(viewModel.Tags);
                }
                Book book = _mapper.Map<Book>(viewModel.Book);
                await _context.AddAsync(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<GenreDTO> genreDtos = _mapper.Map<IEnumerable<GenreDTO>>(await _context.Genres.ToListAsync());
                IEnumerable<AuthorDTO> authorDtos = _mapper.Map<IEnumerable<AuthorDTO>>(await _context.Authors.ToListAsync());

                SelectList genreList = new SelectList(items: genreDtos, dataValueField: "Id", dataTextField: "Name", selectedValue: viewModel.Book.GenreId);
                SelectList authorList = new SelectList(items: authorDtos, dataValueField: "Id", dataTextField: "FullName");
                viewModel.AuthorList = authorList;
                viewModel.GenreList = genreList;
                foreach (var modelError in ModelState.Values.SelectMany(e => e.Errors))
                {
                    _logger.LogError(modelError.ErrorMessage);
                }

                return View(viewModel);
            }
        }
        
        private async Task<List<Tag>> _handleTags(IEnumerable<string>? tags)
        {
            return tags.Select(tag =>
                    _context.Tags.AsEnumerable().FirstOrDefault(t =>
                        String.Compare(t.Name, tag, StringComparison.CurrentCultureIgnoreCase) == 0)
                ??
                    new Tag
                    {
                        Name = CultureInfo.CurrentCulture
                .TextInfo.ToTitleCase(tag.ToLower())
                    })
                .ToList();
        }


        private IWebHostEnvironment _webHostEnvironment;
        private async Task<string> _coverUpload(IFormFile? file)
        {
            if (file == null)
                return "";
            var uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
            var filePath = Path.Combine("img", uniqueFileName);
            await using var fileStream = new FileStream(Path.Combine(_webHostEnvironment.WebRootPath, filePath), FileMode.Create);
            await file.CopyToAsync(fileStream);
            return filePath;
        }
        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = _context.Books
                .Include(b => b.Tags)
                .FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            IEnumerable<GenreDTO> genreDtos = _mapper.Map<IEnumerable<GenreDTO>>(await _context.Genres.ToListAsync());
            IEnumerable<AuthorDTO> authorDtos = _mapper.Map<IEnumerable<AuthorDTO>>(await _context.Authors.ToListAsync());

            SelectList genreList = new SelectList(items: genreDtos, dataValueField: "Id", dataTextField: "Name");
            SelectList authorList = new SelectList(items: authorDtos, dataValueField: "Id", dataTextField: "FullName");
            var vm = new EditBookModel()
            {
                Book = _mapper.Map<BookDTO>(book),
                GenreList = genreList,
                AuthorList = authorList
            };


            return View(vm);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditBookModel viewModel)
        {
            if (id != viewModel.Book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (viewModel.Tags != null && viewModel.Tags.Any())
                    {
                        var b = _context.Books.Include(b => b.Tags).FirstOrDefault(b => b.Id == id);
                        if (b != null) 
                            b.Tags = await _handleTags(viewModel.Tags);
                    }


                    if (viewModel.Cover != null)
                    {
                        _deleteFile(viewModel.Book.CoverPath);
                        viewModel.Book.CoverPath = await _coverUpload(viewModel.Cover);
                    }
                    var book = _mapper.Map<Book>(viewModel.Book);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(viewModel.Book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            IEnumerable<GenreDTO> genreDtos = _mapper.Map<IEnumerable<GenreDTO>>(await _context.Genres.ToListAsync());
            IEnumerable<AuthorDTO> authorDtos = _mapper.Map<IEnumerable<AuthorDTO>>(await _context.Authors.ToListAsync());

            SelectList genreList = new SelectList(items: genreDtos, dataValueField: "Id", dataTextField: "Name", selectedValue: viewModel.Book.GenreId);
            SelectList authorList = new SelectList(items: authorDtos, dataValueField: "Id", dataTextField: "Surname", selectedValue: viewModel.Book.AuthorId);
            var vm = new EditBookModel()
            {
                GenreList = genreList,
                AuthorList = authorList
            };


            return View(vm);
        }

        private void _deleteFile(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            string webRootPath = _webHostEnvironment.WebRootPath;
            string fullPath = Path.Combine(webRootPath, filePath.TrimStart('/'));

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }


            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }


            var vm = new DeleteBookModel()
            {
                Book = _mapper.Map<BookDTO>(book)
            };
            return View(vm);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'BookContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _deleteFile(book.CoverPath);
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
        public PartialViewResult AddNewTagPartial(string tag)
        {
            return PartialView("_AddNewTagPartial");
        }
        public PartialViewResult BooksViewedPartial(BookDTO book)
        {
            return PartialView("_BooksViewedPartial");
        }
    }
}
