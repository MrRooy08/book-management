using bai1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace bai1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoginPartial() { 
            return PartialView("_LoginPartial");
        }

        public IActionResult RegisterPartial() { 
            return PartialView("_RegisterPartial");
        }

        /// <summary>
        /// Trang hiển thị khi tài khoản bị khóa
        /// </summary>
        public IActionResult AccountLocked()
        {
            return View();
        }

        /// <summary>
        /// Trang Access Denied khi không có quyền truy cập
        /// </summary>
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// API Debug - Kiểm tra roles của user (chỉ dùng trong development)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DebugUserRoles(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { error = "Vui lòng cung cấp email" });
            }

            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return Json(new { error = "Không tìm thấy user với email: " + email });
            }

            var rolesInDb = user.Roles.Select(r => new { r.Id, r.RoleName }).ToList();

            // Lấy roles từ claims hiện tại (nếu đang đăng nhập)
            var rolesInClaims = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            return Json(new
            {
                userInfo = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email
                },
                rolesInDatabase = rolesInDb,
                currentSession = new
                {
                    isLoggedIn = User.Identity?.IsAuthenticated ?? false,
                    loggedInEmail = currentUserEmail,
                    rolesInClaims = rolesInClaims
                },
                message = "Nếu rolesInDatabase và rolesInClaims khác nhau, hãy ĐĂNG XUẤT và ĐĂNG NHẬP LẠI!"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LogOnModel model)
        {
            if (ModelState.IsValid)
            {
                // Lấy user kèm theo roles của user đó
                var user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Email == model.Username && u.Password == model.Password);
                
                if (user != null)
                {
                    // Kiểm tra tài khoản có bị khóa không
                    if (!user.IsActive)
                    {
                        return Json(new { 
                            success = false, 
                            isLocked = true,
                            message = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ admin để được hỗ trợ." 
                        });
                    }

                    // Chỉ lấy roles của user này, không phải tất cả roles
                    var userRoles = user.Roles.Select(r => r.RoleName).ToList();
                    
                    // DEBUG: Log ra console
                    Console.WriteLine($"=== LOGIN DEBUG ===");
                    Console.WriteLine($"User: {user.Email} (ID: {user.Id})");
                    Console.WriteLine($"Roles from DB: {string.Join(", ", userRoles)}");
                    Console.WriteLine($"===================");
                    
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Name ?? user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                    };

                    // Thêm roles của user vào claims
                    foreach (var role in userRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var principal = new ClaimsPrincipal(identity);

                    // Thiết lập thời gian hết hạn cookie
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Remember me
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    await HttpContext.SignInAsync("MyCookieAuth", principal, authProperties);

                    // Cập nhật thời gian đăng nhập cuối
                    user.LastLoginAt = DateTime.Now;
                    await _context.SaveChangesAsync();

                    // Lưu UserId vào Session
                    HttpContext.Session.SetInt32("UserId", user.Id);

                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
                }
            }
            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng!");
            return PartialView("_LoginPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Thông tin không hợp lệ!");
                return PartialView("_RegisterPartial", registerModel);
            }

            bool isEmailExist = await _context.Users.AnyAsync(u => u.Email == registerModel.Email);
            if (isEmailExist)
            {
                ModelState.AddModelError("", "Email đã được sử dụng!");
                return PartialView("_RegisterPartial", registerModel);
            }

            // Tìm hoặc tạo role "User" (role mặc định cho người dùng mới)
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");
            if (userRole == null)
            {
                userRole = new Role
                {
                    RoleName = "User",
                    RoleDescription = "Người dùng thông thường"
                };
                _context.Roles.Add(userRole);
                await _context.SaveChangesAsync();
            }

            var user = new User
            {
                Name = registerModel.Username,
                Email = registerModel.Email,
                BirthDay = registerModel.BirthDate,
                Password = registerModel.Password,
                IsActive = true, // Mặc định tài khoản mới là active
                CreatedAt = DateTime.Now,
                Roles = new List<Role> { userRole }
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Tự động đăng nhập sau khi đăng ký
            return await Login(new LogOnModel { Username = registerModel.Email, Password = registerModel.Password });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
