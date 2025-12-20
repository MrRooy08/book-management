using bai1.Models;
using bai1.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace bai1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string? searchTerm = null)
        {
            const int pageSize = 8;

            var query = _context.Books
                .Include(b => b.Images)
                .Include(b => b.Authors)
                .ThenInclude(ba => ba.Author)
                .Include(b => b.Publisher)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var st = searchTerm.Trim();
                query = query.Where(b => b.Title.Contains(st) || b.ISBN.Contains(st) || (b.ShortDescription != null && b.ShortDescription.Contains(st)));
            }

            var totalBooks = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);

            var books = await query
                .OrderByDescending(b => b.PublishDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var parentCats = _context.Categories.Where(c => c.ParentId == null).ToList();

            var translatorList = await _context.Persons.Where(p => p.BookTranslators.Count > 0)
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();
            var publisherList = await _context.Publishers.ToListAsync();

            ViewBag.PublisherList = new SelectList(publisherList, "Id", "Name");
            ViewBag.ParentList = new SelectList(parentCats, "Id", "Name");
            ViewBag.TranslatorList = new SelectList(translatorList, "Id", "Name");

            var model = new HomeIndexViewModel
            {
                Books = books,
                TotalBooks = totalBooks,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                SearchTerm = searchTerm
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Event()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
