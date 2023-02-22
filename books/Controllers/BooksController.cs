using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using books.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using books.Data;
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
        public BooksController(BookContext context, ILoggerFactory factory, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _logger = factory.CreateLogger<BooksController>();
        }


        private  void _checkAccess()
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
                bookContext = bookContext.Where(b => b.Title.Contains(search));
            }
            
            IEnumerable<GenreDTO> genreDtos = _mapper.Map<IEnumerable<GenreDTO>>(await _context.Genres.ToListAsync());
            IEnumerable<AuthorDTO> authorDtos = _mapper.Map<IEnumerable<AuthorDTO>>(await _context.Authors.ToListAsync());
           
            IEnumerable<BookDTO> bookDtos = _mapper.Map<IEnumerable<BookDTO>>(bookContext);
            
            SelectList genreList = new SelectList(items: genreDtos, dataValueField: "Id", dataTextField: "Name", selectedValue: genreId);
            SelectList authorList = new SelectList(items: authorDtos, dataValueField: "Id", dataTextField: "FullName", selectedValue: genreId);

            IndexBookViewModel vm = new IndexBookViewModel
            {
                Books = bookDtos, GenreSelect = genreList, AuthorSelect = authorList
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
                    .FirstOrDefault(b => b.Id ==id);

            
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
            SelectList authorList = new SelectList(items: authorDtos, dataValueField: "Id", dataTextField: "Surname");
            CreateBookModel vm = new CreateBookModel
            {
                GenreList = genreList, AuthorList = authorList
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
                byte[]? dataImage = null;
                using (var reader = new BinaryReader(viewModel.Cover.OpenReadStream()))
                {
                    dataImage = reader.ReadBytes((int)viewModel.Cover.Length);
                    viewModel.Book.Cover = dataImage;
                }

                Book book  = _mapper.Map<Book>(viewModel.Book);
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<GenreDTO> genreDtos = _mapper.Map<IEnumerable<GenreDTO>>(await _context.Genres.ToListAsync());
                IEnumerable<AuthorDTO> authorDtos = _mapper.Map<IEnumerable<AuthorDTO>>(await _context.Authors.ToListAsync());

                SelectList genreList = new SelectList(items: genreDtos, dataValueField: "Id", dataTextField: "Name", selectedValue: viewModel.Book.GenreId);
                SelectList authorList = new SelectList(items: authorDtos, dataValueField: "Id", dataTextField: "Surname", selectedValue:viewModel.Book.AuthorId);
                viewModel.AuthorList = authorList;
                viewModel.GenreList = genreList;
                foreach (var modelError in ModelState.Values.SelectMany(e=>e.Errors))
                {
                    _logger.LogError(modelError.ErrorMessage);
                }

                return View(viewModel);
            }
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin,SuperAdmin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            IEnumerable<GenreDTO> genreDtos = _mapper.Map<IEnumerable<GenreDTO>>(await _context.Genres.ToListAsync());
            IEnumerable<AuthorDTO> authorDtos = _mapper.Map<IEnumerable<AuthorDTO>>(await _context.Authors.ToListAsync());

            SelectList genreList = new SelectList(items: genreDtos, dataValueField: "Id", dataTextField: "Name");
            SelectList authorList = new SelectList(items: authorDtos, dataValueField: "Id", dataTextField: "Surname");
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
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Edit(int id,EditBookModel viewModel)
        {
            if (id != viewModel.Book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewModel.Cover is not null)
                    {
                        byte[]? dataImage = null;
                        using (var reader = new BinaryReader(viewModel.Cover.OpenReadStream()))
                        {
                            dataImage = reader.ReadBytes((int)viewModel.Cover.Length);
                            viewModel.Book.Cover = dataImage;
                        }
                    }
                    var book = _mapper.Map<Book>(viewModel.Book);

                    _context.Update(book);
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

            SelectList genreList = new SelectList(items: genreDtos, dataValueField: "Id", dataTextField: "Name", selectedValue:viewModel.Book.GenreId);
            SelectList authorList = new SelectList(items: authorDtos, dataValueField: "Id", dataTextField: "Surname", selectedValue:viewModel.Book.AuthorId);
            var vm = new EditBookModel()
            {
                GenreList = genreList,
                AuthorList = authorList
            };


            return View(vm);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin,SuperAdmin")]
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
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'BookContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return _context.Books.Any(e => e.Id == id);
        }
    }
}
