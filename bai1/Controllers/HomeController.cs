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
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        // Test VnPay Config - Remove after debugging
        public IActionResult TestVnPayConfig()
        {
            var tmnCode = _configuration["VnPay:TmnCode"];
            var hashSecret = _configuration["VnPay:HashSecret"];
            var baseUrl = _configuration["VnPay:BaseUrl"];
            var returnUrl = _configuration["VnPay:ReturnUrl"];

            return Content($@"
VnPay Configuration Test:
- TmnCode: {tmnCode}
- HashSecret: {(string.IsNullOrEmpty(hashSecret) ? "NULL/EMPTY" : $"{hashSecret.Substring(0, 4)}...{hashSecret.Substring(hashSecret.Length - 4)} (length: {hashSecret.Length})")}
- BaseUrl: {baseUrl}
- ReturnUrl: {returnUrl}
");
        }

        // Temporary action to seed inventory data - Remove after use
        public async Task<IActionResult> SeedInventory()
        {
            var books = await _context.Books.Include(b => b.Inventory).ToListAsync();
            var seededCount = 0;

            foreach (var book in books)
            {
                if (book.Inventory == null)
                {
                    // T?o inventory m?i cho sách ch?a có
                    var inventory = new Inventory
                    {
                        BookId = book.Id,
                        Quantity = new Random().Next(20, 100), // Random t? 20-100
                        ReservedQuantity = 0,
                        SoldQuantity = 0,
                        LastUpdated = DateTime.Now
                    };
                    _context.Inventories.Add(inventory);
                    seededCount++;
                }
                else if (book.Inventory.Quantity <= 0)
                {
                    // C?p nh?t inventory ?ã có nh?ng h?t hàng
                    book.Inventory.Quantity = new Random().Next(20, 100);
                    book.Inventory.ReservedQuantity = 0;
                    book.Inventory.LastUpdated = DateTime.Now;
                    seededCount++;
                }
            }

            await _context.SaveChangesAsync();

            return Content($"?ã seed/c?p nh?t t?n kho cho {seededCount} sách. T?ng s? sách: {books.Count}");
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

        public async Task<IActionResult> BooksByCategory(int categoryId, int pageNumber = 1)
        {
            const int pageSize = 8;

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                return NotFound();
            }

            var categoryIds = new List<int> { categoryId };
            
            // N?u là danh m?c cha, thêm t?t c? danh m?c con
            if (category.SubCategories != null && category.SubCategories.Any())
            {
                categoryIds.AddRange(category.SubCategories.Select(s => s.Id));
            }

            var query = _context.Books
                .Include(b => b.Images)
                .Include(b => b.Authors)
                .ThenInclude(ba => ba.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Categories)
                .Where(b => b.Categories.Any(c => categoryIds.Contains(c.Id)))
                .AsQueryable();

            var totalBooks = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);

            var books = await query
                .OrderByDescending(b => b.PublishDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryId = categoryId;

            var model = new HomeIndexViewModel
            {
                Books = books,
                TotalBooks = totalBooks,
                TotalPages = totalPages,
                CurrentPage = pageNumber
            };

            return View("BooksByCategory", model);
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
