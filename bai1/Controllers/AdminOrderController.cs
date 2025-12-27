using bai1.Models;
using bai1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bai1.Controllers
{
    /// <summary>
    /// Controller qu?n lý ??n hàng dành cho Admin
    /// Ch? Admin m?i có quy?n truy c?p
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderService _orderService;

        public AdminOrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: AdminOrder
        public async Task<IActionResult> Index(string status = "All")
        {
            var orders = await _orderService.GetAllOrdersAsync();

            if (status != "All")
            {
                orders = orders.Where(o => o.OrderStatus == status).ToList();
            }

            ViewBag.SelectedStatus = status;
            ViewBag.Statistics = await _orderService.GetOrderStatisticsAsync();

            return View(orders);
        }

        // GET: AdminOrder/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: AdminOrder/ConfirmOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            var result = await _orderService.ConfirmOrderAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "??n hàng ?ã ???c xác nh?n thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không th? xác nh?n ??n hàng!";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: AdminOrder/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status, string? note)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, status, note);
            if (result)
            {
                TempData["SuccessMessage"] = "C?p nh?t tr?ng thái ??n hàng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không th? c?p nh?t tr?ng thái!";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: AdminOrder/CancelOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["ErrorMessage"] = "Vui lòng nh?p lý do h?y ??n!";
                return RedirectToAction(nameof(Details), new { id });
            }

            var result = await _orderService.CancelOrderAsync(id, reason);
            if (result)
            {
                TempData["SuccessMessage"] = "??n hàng ?ã ???c h?y!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không th? h?y ??n hàng!";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
