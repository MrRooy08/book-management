using bai1.Models;
using bai1.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;


namespace bai1.Controllers
{
    public class BookController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context) {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(BookAuthorsViewModels models) {
            var authors = JsonSerializer.Deserialize<List<AuthorDto>>(models.AuthorIds);
            Console.WriteLine(authors);
           
            if (ModelState.IsValid) {

                using var transaction = await _context.Database.BeginTransactionAsync();
                try {
                    var listAuthors = authors?
                    .Where(author => author.Id != null && !string.IsNullOrEmpty(author.Name) )
                    .Select(author => int.Parse(author.Id))
                    .ToList();
                    var newAuthors = authors?
                    .Where(author => author.Id == null && !string.IsNullOrEmpty(author.Name))
                    .Select(author => new Person { Name = author.Name })
                    .ToList();

                    var finalAuthors = new List<Person>();
                    if (newAuthors!.Count > 0)
                    {
                        _context.Persons.AddRange(newAuthors);
                        await _context.SaveChangesAsync();
                        finalAuthors.AddRange(newAuthors);
                    }
                    if (listAuthors!.Count > 0) {
                        var existedAuthors = _context.Persons
                            .Where(p => listAuthors.Contains(p.Id))
                            .Select(p => new Person { Id = p.Id })
                            .ToList();
                        finalAuthors.AddRange(existedAuthors);                       
                    }
                    Book book = new Book
                    {
                        ISBN = models.ISBN,
                        Title = models.Title,
                        Description = models.Description,
                        PublishDate = models.PublishDate,
                        CostPrice = (decimal)models.CostPrice,
                        ListPrice = (decimal)models.ListPrice,
                        SalePrice = (decimal)models.SalePrice,
                        Author = finalAuthors.Select(author => new BookAuthors
                        {
                            AuthorId = author.Id,
                        }).ToList(),

                    };
                    _context.Books.Add(book);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return RedirectToAction("Index");
                } catch (Exception ex) {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, ex.Message);
                    var errorModel = new ErrorViewModel
                    {
                        RequestId = HttpContext.TraceIdentifier,
                        Message = ex.Message
                    };
                    return View("Error", errorModel);
                }
                
            }
            return RedirectToAction("Index");
        }
    }
}


