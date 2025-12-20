using bai1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace bai1.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
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

            ViewBag.Total = items.Sum(i => i.Price * i.Quantity);
            return View(items);
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int bookId, int quantity)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (string.IsNullOrEmpty(cart)) return RedirectToAction("Index");

            var items = JsonSerializer.Deserialize<List<CartItem>>(cart) ?? new List<CartItem>();
            var item = items.FirstOrDefault(i => i.BookId == bookId);
            
            if (item != null)
            {
                if (quantity > 0)
                {
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
    }
}

