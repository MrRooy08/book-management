using bai1.Models;
using bai1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace bai1.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IInventoryService _inventoryService;

        public CartController(ApplicationDbContext context, IInventoryService inventoryService)
        {
            _context = context;
            _inventoryService = inventoryService;
        }

        public async Task<IActionResult> Index()
        {
            var cart = HttpContext.Session.GetString("cart");
            List<CartItem> items;
            if (string.IsNullOrEmpty(cart))
            {
                items = new List<CartItem>();
            }
            else
            {
                items = JsonSerializer.Deserialize<List<CartItem>>(cart) ?? new List<CartItem>();
            }

            // L?y thông tin t?n kho cho t?ng s?n ph?m (ch? ?? hi?n th?, không ch?n ??t hàng)
            var bookIds = items.Select(i => i.BookId).ToList();
            var inventoryList = await _context.Inventories
                .Where(i => bookIds.Contains(i.BookId))
                .Select(i => new { i.BookId, i.Quantity, i.ReservedQuantity })
                .ToListAsync();
            
            var inventories = inventoryList.ToDictionary(
                i => i.BookId, 
                i => i.Quantity - i.ReservedQuantity
            );

            ViewBag.Inventories = inventories;
            ViewBag.Total = items.Sum(i => i.Price * i.Quantity);
            
            // Không ch?n ??t hàng - cho phép overselling/backorder
            ViewBag.InsufficientItems = new List<int>();

            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int bookId, int quantity)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (string.IsNullOrEmpty(cart)) return RedirectToAction("Index");

            var items = JsonSerializer.Deserialize<List<CartItem>>(cart) ?? new List<CartItem>();
            var item = items.FirstOrDefault(i => i.BookId == bookId);
            
            if (item != null)
            {
                if (quantity > 0)
                {
                    // Cho phép ??t s? l??ng b?t k? - không ki?m tra t?n kho
                    item.Quantity = quantity;
                }
                else
                {
                    items.Remove(item);
                }
                HttpContext.Session.SetString("cart", JsonSerializer.Serialize(items));
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int bookId)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (string.IsNullOrEmpty(cart)) return RedirectToAction("Index");

            var items = JsonSerializer.Deserialize<List<CartItem>>(cart) ?? new List<CartItem>();
            var item = items.FirstOrDefault(i => i.BookId == bookId);
            
            if (item != null)
            {
                items.Remove(item);
                HttpContext.Session.SetString("cart", JsonSerializer.Serialize(items));
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("cart");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int bookId, int quantity = 1)
        {
            var book = await _context.Books
                .Include(b => b.Images)
                .Include(b => b.Inventory)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
            {
                return Json(new { success = false, message = "S?n ph?m không t?n t?i!" });
            }

            // Không ki?m tra t?n kho - cho phép ??t hàng t? do (overselling/backorder)
            var cart = HttpContext.Session.GetString("cart");
            var items = string.IsNullOrEmpty(cart) 
                ? new List<CartItem>() 
                : JsonSerializer.Deserialize<List<CartItem>>(cart) ?? new List<CartItem>();

            var existingItem = items.FirstOrDefault(i => i.BookId == bookId);
            if (existingItem != null)
            {
                // Cho phép t?ng s? l??ng không gi?i h?n
                existingItem.Quantity += quantity;
            }
            else
            {
                var primaryImage = book.Images?.FirstOrDefault(i => i.IsPrimary)?.ImageUrl 
                    ?? book.Images?.FirstOrDefault()?.ImageUrl;

                items.Add(new CartItem
                {
                    BookId = book.Id,
                    Title = book.Title,
                    Price = book.SalePrice,
                    Quantity = quantity,
                    ImageUrl = primaryImage
                });
            }

            HttpContext.Session.SetString("cart", JsonSerializer.Serialize(items));
            
            return Json(new { 
                success = true, 
                message = "?ã thêm vào gi? hàng!", 
                cartCount = items.Sum(i => i.Quantity) 
            });
        }
    }
}

