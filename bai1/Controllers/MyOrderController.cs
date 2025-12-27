using bai1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bai1.Controllers
{
    /// <summary>
    /// Controller xem ??n hàng c?a User
    /// Yêu c?u ??ng nh?p
    /// </summary>
    [Authorize]
    public class MyOrderController : Controller
    {
        private readonly IOrderService _orderService;

        public MyOrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// L?y UserId t? Claims ho?c Session
        /// </summary>
        private int? GetCurrentUserId()
        {
            // ?u tiên l?y t? Claims (Authentication)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            // Fallback: L?y t? Session
            return HttpContext.Session.GetInt32("UserId");
        }

        // GET: MyOrder
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId.Value);
            return View(orders);
        }

        // GET: MyOrder/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Ki?m tra order có thu?c v? user hi?n t?i không
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (order.UserId != userId)
            {
                TempData["ErrorMessage"] = "B?n không có quy?n xem ??n hàng này!";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        // POST: MyOrder/CancelOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id, string reason)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Ki?m tra quy?n
            var userId = GetCurrentUserId();
            if (order.UserId != userId)
            {
                TempData["ErrorMessage"] = "B?n không có quy?n h?y ??n hàng này!";
                return RedirectToAction(nameof(Index));
            }

            // Ch? cho phép h?y ??n hàng ? tr?ng thái Pending ho?c Confirmed
            if (order.OrderStatus != "Pending" && order.OrderStatus != "Confirmed")
            {
                TempData["ErrorMessage"] = "Không th? h?y ??n hàng ? tr?ng thái này!";
                return RedirectToAction(nameof(Details), new { id });
            }

            var result = await _orderService.CancelOrderAsync(id, reason);
            if (result)
            {
                TempData["SuccessMessage"] = "??n hàng ?ã ???c h?y thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không th? h?y ??n hàng!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
