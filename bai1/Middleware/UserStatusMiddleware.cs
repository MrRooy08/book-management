using bai1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace bai1.Middleware
{
    /// <summary>
    /// Middleware ki?m tra tr?ng thái tài kho?n ng??i dùng
    /// N?u tài kho?n b? khóa, s? ??ng xu?t và chuy?n h??ng ??n trang thông báo
    /// </summary>
    public class UserStatusMiddleware
    {
        private readonly RequestDelegate _next;

        public UserStatusMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            // Ch? ki?m tra n?u user ?ã ??ng nh?p
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
                
                if (!string.IsNullOrEmpty(userEmail))
                {
                    // Ki?m tra tr?ng thái user trong database
                    var user = await dbContext.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Email == userEmail);

                    if (user != null && !user.IsActive)
                    {
                        // Các path ???c phép truy c?p khi b? khóa
                        var allowedPaths = new[]
                        {
                            "/Account/Logout",
                            "/Account/AccountLocked",
                            "/Account/LoginPartial",
                            "/Account/Login"
                        };

                        var currentPath = context.Request.Path.Value ?? "";

                        // N?u không ph?i path ???c phép, ??ng xu?t và redirect
                        if (!allowedPaths.Any(p => currentPath.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                        {
                            // ??ng xu?t user
                            await context.SignOutAsync("MyCookieAuth");

                            // L?u thông tin vào session ?? hi?n th? thông báo
                            context.Session.SetString("AccountLocked", "true");
                            context.Session.SetString("LockedEmail", userEmail);

                            // Redirect ??n trang thông báo tài kho?n b? khóa
                            context.Response.Redirect("/Account/AccountLocked");
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method ?? ??ng ký middleware
    /// </summary>
    public static class UserStatusMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserStatusCheck(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserStatusMiddleware>();
        }
    }
}
