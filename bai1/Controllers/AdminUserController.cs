using bai1.Models;
using bai1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bai1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly UserService _userService;

        public AdminUserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// L?y UserId c?a admin ?ang ??ng nh?p
        /// </summary>
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        /// <summary>
        /// Danh sách ng??i dùng v?i filter và phân trang
        /// </summary>
        public async Task<IActionResult> Index(
            string? search = null,
            string? status = null,
            string? role = null,
            bool? isVip = null,
            int page = 1,
            int pageSize = 10)
        {
            var (users, totalCount) = await _userService.GetUsersAsync(search, status, role, isVip, page, pageSize);
            var statistics = await _userService.GetUserStatisticsAsync();
            var roles = await _userService.GetAllRolesAsync();

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.Role = role;
            ViewBag.IsVip = isVip;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.Statistics = statistics;
            ViewBag.Roles = roles;
            ViewBag.CurrentUserId = GetCurrentUserId();

            return View(users);
        }

        /// <summary>
        /// Chi ti?t ng??i dùng
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm th?y ng??i dùng!";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userService.GetAllRolesAsync();
            ViewBag.AllRoles = roles;
            ViewBag.CurrentUserId = GetCurrentUserId();
            ViewBag.IsTargetUserAdmin = user.Roles.Any(r => r.RoleName == "Admin");

            return View(user);
        }

        /// <summary>
        /// Kích ho?t/Vô hi?u hóa tài kho?n
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id, string? note)
        {
            // Không cho phép t? khóa chính mình
            var currentUserId = GetCurrentUserId();
            if (currentUserId.HasValue && currentUserId.Value == id)
            {
                TempData["ErrorMessage"] = "B?n không th? khóa tài kho?n c?a chính mình!";
                return RedirectToAction(nameof(Details), new { id });
            }

            var (success, message) = await _userService.ToggleUserStatusAsync(id, note);
            
            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// Nâng c?p/H?y VIP
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVipStatus(int id, bool isVip, DateTime? vipEndDate, string? note)
        {
            var result = await _userService.UpdateVipStatusAsync(id, isVip, vipEndDate, note);

            if (result)
            {
                TempData["SuccessMessage"] = isVip ? "Nâng c?p VIP thành công!" : "H?y VIP thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không th? c?p nh?t tr?ng thái VIP!";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// C?p nh?t ghi chú admin
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNote(int id, string note)
        {
            var result = await _userService.UpdateAdminNoteAsync(id, note);

            if (result)
            {
                TempData["SuccessMessage"] = "C?p nh?t ghi chú thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không th? c?p nh?t ghi chú!";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// C?p nh?t roles
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoles(int id, List<int> roleIds)
        {
            if (roleIds == null || !roleIds.Any())
            {
                TempData["ErrorMessage"] = "Vui lòng ch?n ít nh?t m?t vai trò!";
                return RedirectToAction(nameof(Details), new { id });
            }

            var currentUserId = GetCurrentUserId();
            var (success, message) = await _userService.UpdateUserRolesAsync(id, roleIds, currentUserId);

            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// API l?y th?ng kê nhanh
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var stats = await _userService.GetUserStatisticsAsync();
            return Json(stats);
        }
    }
}
