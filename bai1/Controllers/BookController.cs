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
            if (!ModelState.IsValid) {

                using var transaction = await _context.Database.BeginTransactionAsync();
                try {
                    var listAuthors = authors?
                    .Where(author =>!string.IsNullOrEmpty(author.Name))
                    .Select(author => new { Id = author.Id, Name = author.Name })
                    .ToList();

                    if (listAuthors.Count > 0) {
                        var submittedIds = listAuthors.Select(a => a.Id).ToList();
                        var notValidExistingAuthors = _context.Persons
                            .Where(p => !submittedIds.Contains(p.Id))
                            .Select(p => p.Name)
                            .ToList();

                        if (notValidExistingAuthors.Count > 0) {
                            _context.Persons.AddRange(notValidExistingAuthors.Select(
                                Name => new Person { Name = Name }
                            ));
                            await _context.SaveChangesAsync();
                        }
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
                        Author = authors!.Select(a => new BookAuthors
                        {
                            AuthorId = a.Id,
                        }).ToList()
                    };
                    _context.Books.Add(book);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return RedirectToAction("Index");
                } catch (Exception ex) {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                
            }
            return View();
        }
    }
}


