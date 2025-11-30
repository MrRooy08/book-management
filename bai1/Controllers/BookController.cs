using bai1.Models;
using bai1.Models.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;


namespace bai1.Controllers
{
    public class BookController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.Images)
                .Include(b => b.Authors)
                .ThenInclude(ba => ba.Author)
                .Include(b => b.Publisher)
                .ToListAsync();

            var parentCats = _context.Categories.Where(c => c.ParentId == null).ToList();

            var translatorList = await _context.Persons.Where(p => p.BookTranslators.Count > 0)
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();
            var publisherList = await _context.Publishers.ToListAsync();

            ViewBag.PublisherList = new SelectList(publisherList, "Id", "Name");
            ViewBag.ParentList = new SelectList(parentCats, "Id", "Name");
            ViewBag.TranslatorList = new SelectList(translatorList, "Id", "Name");
            return View(books);
        }

        [HttpGet]
        public IActionResult GetSubCategories(int parentId)
        {
            // Lấy danh sách con dựa theo parentId
            var subCategories = _context.Categories
                .Where(c => c.ParentId == parentId)
                .Select(c => new {
                    id = c.Id,
                    name = c.Name
                }) // Chỉ lấy dữ liệu cần thiết
                .ToList();

            return Json(subCategories); // Trả về JSON để JavaScript đọc
        }
        private async Task<List<Person>> ProcessPersonDataAsync(string jsonInput)
        {
            if (string.IsNullOrEmpty(jsonInput)) return new List<Person>();

            // 1. Deserialize
            var inputList = JsonSerializer.Deserialize<List<PersonInputDto>>(jsonInput) ?? new List<PersonInputDto>();
            var finalPersons = new List<Person>();

            // 2. Lọc & Lấy những người CŨ (Đã có ID)
            var existingIds = inputList
                .Where(x => x.Id != 0) // Id is now an int
                .Select(x => x.Id)
                .ToList();

            if (existingIds.Any())
            {
                var existingPersons = await _context.Persons
                    .Where(p => existingIds.Contains(p.Id))
                    .ToListAsync();
                finalPersons.AddRange(existingPersons);
            }

            // 3. Lọc & Tạo những người MỚI (Chưa có ID)
            var newNames = inputList
                .Where(x => x.Id == 0 && !string.IsNullOrWhiteSpace(x.Name)) // Id is now an int
                .Select(x => x.Name)
                .Distinct() // Tránh tạo trùng tên trong cùng 1 lần submit
                .ToList();

            if (newNames.Any())
            {
                var newPersons = newNames.Select(name => new Person { Name = name }).ToList();

                // Add và Save ngay để sinh ra ID
                _context.Persons.AddRange(newPersons);
                await _context.SaveChangesAsync();

                finalPersons.AddRange(newPersons);
            }

            return finalPersons;
        }

                [HttpPost]
                public async Task<IActionResult> AddBook(BookAuthorsViewModels models, IFormFile image) {
                    if (ModelState.IsValid) {
                        using var transaction = await _context.Database.BeginTransactionAsync();
                        try {
                            var author = await ProcessPersonDataAsync(models.AuthorIds);
                            var translators = await ProcessPersonDataAsync(models.TranslatorData);
        
                            var imageName = image.FileName != null ? image.FileName : "" ;
        
                            if (image != null)
                            {
                                string nameFile = Directory.GetCurrentDirectory();
                                nameFile += @"\wwwroot\Images\" + imageName;
                                FileStream fs = new FileStream(nameFile, FileMode.Create);
                                image.CopyTo(fs);
                                fs.Close();
                            }
                            Book book = new Book
                            {
                                ISBN = models.ISBN,
                                Title = models.Title,
                                Description = models.Description,
                                PublishDate = models.PublishDate,
                                Dimensions = new BookDimensions
                                {
                                    Height = models.Height,
                                    Length = models.Length,
                                    Width = models.Width
                                },
                                Images = new List<BookImage>() {
                                    new BookImage {
                                        ImageUrl = imageName
                                    }
                                },
                                PageCount = models.PageCount,
                                Weight = models.Weight,
                                Language = models.Language,
                                Format = models.Format,
                                CostPrice = (decimal)models.CostPrice,
                                ListPrice = (decimal)models.ListPrice,
                                SalePrice = (decimal)models.SalePrice,
                                PublisherId = models.PublisherId,
                                Authors = author.Select(a => new BookAuthors { AuthorId = a.Id }).ToList(),
                                Translators = translators.Select(t => new BookTranslators { TranslatorId = t.Id }).ToList(),
                            };
                            var selectedCategory = _context.Categories.Find(models.CategoryIds);
        
                            // 3. Nếu tìm thấy, thêm vào danh sách Categories của cuốn sách đó
                            if (selectedCategory != null)
                            {
                                book.Categories.Add(selectedCategory);
                            }
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
                }    }
}


