using bai1.Models;
using bai1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bai1.Controllers
{
    /// <summary>
    /// Controller cho khách vãng lai tra c?u ??n hàng
    /// Không yêu c?u ??ng nh?p
    /// </summary>
    [AllowAnonymous]
    public class TrackOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrackOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Trang tra c?u ??n hàng
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Tìm ki?m ??n hàng theo s? ?i?n tho?i
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                TempData["ErrorMessage"] = "Vui lòng nh?p s? ?i?n tho?i!";
                return RedirectToAction(nameof(Index));
            }

            // Chu?n hóa s? ?i?n tho?i (lo?i b? kho?ng tr?ng, d?u g?ch)
            phone = phone.Trim().Replace(" ", "").Replace("-", "");

            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                        .ThenInclude(b => b.Images)
                .Where(o => o.Phone == phone)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            if (!orders.Any())
            {
                TempData["ErrorMessage"] = "Không tìm th?y ??n hàng nào v?i s? ?i?n tho?i này!";
                TempData["SearchPhone"] = phone;
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Phone = phone;
            return View("SearchResult", orders);
        }

        /// <summary>
        /// Xem chi ti?t ??n hàng (yêu c?u xác th?c b?ng s? ?i?n tho?i)
        /// </summary>
        public async Task<IActionResult> Details(int id, string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                TempData["ErrorMessage"] = "Vui lòng nh?p s? ?i?n tho?i ?? xem ??n hàng!";
                return RedirectToAction(nameof(Index));
            }

            phone = phone.Trim().Replace(" ", "").Replace("-", "");

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                        .ThenInclude(b => b.Images)
                .FirstOrDefaultAsync(o => o.Id == id && o.Phone == phone);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm th?y ??n hàng ho?c s? ?i?n tho?i không kh?p!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Phone = phone;
            return View(order);
        }
    }
}
