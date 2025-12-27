using bai1.Models;
using bai1.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;


namespace bai1.Controllers
{
    /// <summary>
    /// Controller quản lý sách
    /// Index (Admin Dashboard) - Chỉ Admin
    /// Details, GetSubCategories - Public (cho tất cả user xem)
    /// AddToCart - Public
    /// AddBook, Edit, Delete - Chỉ Admin
    /// </summary>
    public class BookController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Trang quản lý sách - Chỉ Admin mới có quyền truy cập
        /// </summary>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.Images)
                .Include(b => b.Authors)
                .ThenInclude(ba => ba.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Inventory)
                .ToListAsync();

            var parentCats = _context.Categories.Where(c => c.ParentId == null).ToList();

            var translatorList = await _context.Persons.Where(p => p.BookTranslators.Count >0)
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();
            var publisherList = await _context.Publishers.ToListAsync();

            ViewBag.PublisherList = new SelectList(publisherList, "Id", "Name");
            ViewBag.ParentList = new SelectList(parentCats, "Id", "Name");
            ViewBag.TranslatorList = new SelectList(translatorList, "Id", "Name");
            return View(books);
        }

        /// <summary>
        /// API lấy danh mục con - Public
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
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

        /// <summary>
        /// Chi tiết sách - Public (cho tất cả user xem)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books
                .Include(b => b.Images)
                .Include(b => b.Authors)
                .ThenInclude(ba => ba.Author)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            var primary = book.Images?.FirstOrDefault(i => i.IsPrimary) ?? book.Images?.FirstOrDefault();
            var discount =0m;
            if (book.ListPrice >0)
            {
                discount = ((book.ListPrice - book.SalePrice) / book.ListPrice) *100;
            }

            var model = new BookDetailsViewModel
            {
                Book = book,
                PrimaryImageUrl = primary?.ImageUrl,
                DiscountPercent = discount
            };

            return View(model);
        }

        /// <summary>
        /// Thêm vào giỏ hàng - Public
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult AddToCart(int bookId, int quantity =1)
        {
            var book = _context.Books.Include(b => b.Images).FirstOrDefault(b => b.Id == bookId);
            if (book == null) return NotFound();

            var cart = HttpContext.Session.GetString("cart");
            List<CartItem> items;
            if (string.IsNullOrEmpty(cart)) items = new List<CartItem>();
            else items = JsonSerializer.Deserialize<List<CartItem>>(cart) ?? new List<CartItem>();

            var existing = items.FirstOrDefault(i => i.BookId == bookId);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                items.Add(new CartItem { BookId = book.Id, Title = book.Title, Price = book.SalePrice, Quantity = quantity, ImageUrl = book.Images?.FirstOrDefault()?.ImageUrl });
            }

            HttpContext.Session.SetString("cart", JsonSerializer.Serialize(items));
            return RedirectToAction("Details", new { id = bookId });
        }

        /// <summary>
        /// Mua ngay - Chuyển thẳng đến trang Checkout mà không cần thêm vào giỏ hàng
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult BuyNow(int bookId, int quantity = 1)
        {
            var book = _context.Books.Include(b => b.Images).FirstOrDefault(b => b.Id == bookId);
            if (book == null) return NotFound();

            if (quantity < 1) quantity = 1;

            // Tạo giỏ hàng tạm thời chỉ chứa sản phẩm này
            var buyNowItems = new List<CartItem>
            {
                new CartItem
                {
                    BookId = book.Id,
                    Title = book.Title,
                    Price = book.SalePrice,
                    Quantity = quantity,
                    ImageUrl = book.Images?.FirstOrDefault()?.ImageUrl
                }
            };

            // Lưu vào session với key riêng để phân biệt với giỏ hàng thông thường
            // Hoặc ghi đè giỏ hàng hiện tại (tùy logic mong muốn)
            HttpContext.Session.SetString("cart", JsonSerializer.Serialize(buyNowItems));

            // Chuyển thẳng đến trang Checkout
            return RedirectToAction("Index", "Checkout");
        }

        private async Task<List<Person>> ProcessPersonDataAsync(string jsonInput)
        {
            if (string.IsNullOrEmpty(jsonInput)) return new List<Person>();

            //1. Deserialize
            var inputList = JsonSerializer.Deserialize<List<PersonInputDto>>(jsonInput) ?? new List<PersonInputDto>();
            var finalPersons = new List<Person>();

            //2. Lọc & Lấy những người CÙ (Đã có ID)
            var existingIds = inputList
                .Where(x => x.Id !=0) // Id is now an int
                .Select(x => x.Id)
                .ToList();

            if (existingIds.Any())
            {
                var existingPersons = await _context.Persons
                    .Where(p => existingIds.Contains(p.Id))
                    .ToListAsync();
                finalPersons.AddRange(existingPersons);
            }

            //3. Lọc & Tạo những người MỚI (Chưa có ID)
            var newNames = inputList
                .Where(x => x.Id ==0 && !string.IsNullOrWhiteSpace(x.Name)) // Id is now an int
                .Select(x => x.Name)
                .Distinct() // Tránh tạo trùng tên trong cùng1 lần submit
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

        /// <summary>
        /// Thêm sách mới - Chỉ Admin
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBook(BookAuthorsViewModels models, IFormFile image)
        {
            // Parse giá từ string (hỗ trợ format: 150000, 150.000, 150,000)
            var costPrice = models.GetCostPriceDecimal();
            var listPrice = models.GetListPriceDecimal();
            var salePrice = models.GetSalePriceDecimal();

            // Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(models.ISBN))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập ISBN!";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(models.Title))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập tên sách!";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(models.AuthorIds))
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một tác giả!";
                return RedirectToAction("Index");
            }

            // Validate giá: Giá niêm yết phải >= Giá bán
            if (salePrice > listPrice)
            {
                TempData["ErrorMessage"] = "Giá bán không được cao hơn giá niêm yết!";
                return RedirectToAction("Index");
            }

            // Validate giá phải > 0
            if (listPrice <= 0 || salePrice <= 0)
            {
                TempData["ErrorMessage"] = "Giá niêm yết và giá bán phải lớn hơn 0!";
                return RedirectToAction("Index");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var author = await ProcessPersonDataAsync(models.AuthorIds);
                
                // Dịch giả không bắt buộc - chỉ xử lý nếu có dữ liệu
                var translators = await ProcessPersonDataAsync(models.TranslatorData);

                var imageName = image?.FileName ?? string.Empty;
                string relativePath = string.Empty;

                if (image != null)
                {
                    // đường dẫn vật lý
                    string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", imageName);
                    await using var fs = new FileStream(physicalPath, FileMode.Create);
                    await image.CopyToAsync(fs);

                    // đường dẫn tương đối để lưu DB (dùng khi hiển thị)
                    relativePath = Path.Combine("Images", imageName).Replace("\\", "/");
                }

                Book book = new Book
                {
                    ISBN = models.ISBN,
                    Title = models.Title,
                    Description = models.Description,
                    PublishDate = models.PublishDate,
                    Dimensions = new BookDimensions
                    {
                        Height = models.GetHeightFloat(),
                        Length = models.GetLengthFloat(),
                        Width = models.GetWidthFloat()
                    },
                    Images = new List<BookImage>()
                    {
                        new BookImage
                        {
                            ImageUrl = relativePath // ví dụ: "Images/book1.jpg"
                        }
                    },
                    PageCount = models.PageCount,
                    Weight = models.GetWeightFloat(),
                    Language = models.Language,
                    Format = models.Format,
                    CostPrice = costPrice,
                    ListPrice = listPrice,
                    SalePrice = salePrice,
                    PublisherId = models.PublisherId,
                    Authors = author.Select(a => new BookAuthors { AuthorId = a.Id }).ToList(),
                    // Chỉ thêm translators nếu có dữ liệu
                    Translators = translators.Any() 
                        ? translators.Select(t => new BookTranslators { TranslatorId = t.Id }).ToList() 
                        : new List<BookTranslators>(),
                };
                var selectedCategory = _context.Categories.Find(models.CategoryIds);

                //3. Nếu tìm thấy, thêm vào danh sách Categories của cuốn sách đó
                if (selectedCategory != null)
                {
                    book.Categories.Add(selectedCategory);
                }
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                // Tạo Inventory cho sách mới
                var inventory = new Inventory
                {
                    BookId = book.Id,
                    Quantity = models.Inventory,
                    ReservedQuantity = 0,
                    SoldQuantity = 0,
                    LastUpdated = DateTime.Now
                };
                _context.Inventories.Add(inventory);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                TempData["SuccessMessage"] = $"Đã thêm sách '{book.Title}' thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Lỗi khi thêm sách: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Chỉnh sửa sách - Chỉ Admin
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books
                .Include(b => b.Images)
                .Include(b => b.Authors)
                .ThenInclude(ba => ba.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            var parentCats = _context.Categories.Where(c => c.ParentId == null).ToList();
            var publisherList = await _context.Publishers.ToListAsync();

            ViewBag.PublisherList = new SelectList(publisherList, "Id", "Name", book.PublisherId);
            ViewBag.ParentList = new SelectList(parentCats, "Id", "Name");

            // Redirect to book management tab with edit mode
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xóa sách - Chỉ Admin
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books
                .Include(b => b.Images)
                .Include(b => b.Authors)
                .Include(b => b.Translators)
                .Include(b => b.Inventory)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            try
            {
                // Delete related images from file system
                if (book.Images != null)
                {
                    foreach (var image in book.Images)
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", image.ImageUrl);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                }

                // Remove related entities
                _context.BookAuthors.RemoveRange(book.Authors);
                _context.BookTranslators.RemoveRange(book.Translators);
                
                // Remove inventory if exists
                if (book.Inventory != null)
                {
                    _context.Inventories.Remove(book.Inventory);
                }

                // Remove the book
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Book '{book.Title}' has been deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting book: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}

































































































































































































































































































































































































































































































































































































































































































































































































