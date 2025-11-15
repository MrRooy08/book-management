using bai1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
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


        [HttpPost]
        public async Task<IActionResult> Login(LogOnModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Username && u.Password == model.Password);
                if (user != null)
                {
                    var roles = _context.Roles.Select(r => r.RoleName).ToList();
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                    };

                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("MyCookieAuth", principal);

                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
                }
            }
            ModelState.AddModelError("", "Invalid username or password");
            return PartialView("_LoginPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel) {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Some informations weren't correct");
                return PartialView("_RegisterPartial", registerModel);
            }
            bool isEmailExist = _context.Users.Any(u => u.Email == registerModel.Email);
            if (!isEmailExist)
            {
                User user = new User()
                {
                    Name = registerModel.Username,
                    Email = registerModel.Email,
                    BirthDay = registerModel.BirthDate,
                    Password = registerModel.Password,
                    Roles = new List<Role>() {
                        new Role() { 
                        RoleName = "User",
                        RoleDescription = "Người dùng thông thường"
                        }
                    }
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                return await Login(new LogOnModel { Username = registerModel.Email, Password = registerModel.Password });
            }
            return PartialView("_RegisterPartial", registerModel); 
        }
    }
}
