using bai1.Models;
using Microsoft.EntityFrameworkCore;

namespace bai1.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// L?y danh sách t?t c? users v?i phân trang và filter
        /// </summary>
        public async Task<(List<User> Users, int TotalCount)> GetUsersAsync(
            string? search = null,
            string? status = null,
            string? role = null,
            bool? isVip = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.Users
                .Include(u => u.Roles)
                .Include(u => u.Orders)
                .AsQueryable();

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(u => 
                    u.Name.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search) ||
                    (u.Phone != null && u.Phone.Contains(search)));
            }

            // Filter by status
            if (status == "active")
            {
                query = query.Where(u => u.IsActive);
            }
            else if (status == "inactive")
            {
                query = query.Where(u => !u.IsActive);
            }

            // Filter by role
            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(u => u.Roles.Any(r => r.RoleName == role));
            }

            // Filter by VIP status
            if (isVip.HasValue)
            {
                query = query.Where(u => u.IsVip == isVip.Value);
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        /// <summary>
        /// L?y thông tin chi ti?t user
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.Orders)
                    .ThenInclude(o => o.OrderDetails)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// L?y user theo email
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Ki?m tra user có ph?i Admin không
        /// </summary>
        public async Task<bool> IsUserAdminAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            return user?.Roles.Any(r => r.RoleName == "Admin") ?? false;
        }

        /// <summary>
        /// Kích ho?t/Vô hi?u hóa tài kho?n
        /// Không cho phép khóa tài kho?n Admin
        /// </summary>
        public async Task<(bool Success, string Message)> ToggleUserStatusAsync(int userId, string? adminNote = null)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);
                
            if (user == null) 
                return (false, "Không tìm th?y ng??i dùng!");

            // Ki?m tra n?u ?ang c? khóa tài kho?n Admin
            bool isAdmin = user.Roles.Any(r => r.RoleName == "Admin");
            if (isAdmin && user.IsActive)
            {
                return (false, "Không th? khóa tài kho?n Qu?n tr? viên! Vui lòng xóa quy?n Admin tr??c khi khóa.");
            }

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.Now;
            
            if (!string.IsNullOrWhiteSpace(adminNote))
            {
                user.AdminNote = $"[{DateTime.Now:dd/MM/yyyy HH:mm}] {(user.IsActive ? "Kích ho?t" : "Vô hi?u hóa")}: {adminNote}\n{user.AdminNote}";
            }

            await _context.SaveChangesAsync();
            return (true, user.IsActive ? "?ã kích ho?t tài kho?n!" : "?ã khóa tài kho?n!");
        }

        /// <summary>
        /// C?p nh?t tr?ng thái VIP
        /// </summary>
        public async Task<bool> UpdateVipStatusAsync(int userId, bool isVip, DateTime? endDate = null, string? adminNote = null)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            if (isVip && !user.IsVip)
            {
                // Nâng c?p lên VIP
                user.IsVip = true;
                user.VipStartDate = DateTime.Now;
                user.VipEndDate = endDate;
            }
            else if (!isVip && user.IsVip)
            {
                // H?y VIP
                user.IsVip = false;
                user.VipEndDate = DateTime.Now;
            }
            else if (isVip && user.IsVip)
            {
                // Gia h?n VIP
                user.VipEndDate = endDate;
            }

            user.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(adminNote))
            {
                var action = isVip ? (user.VipEndDate.HasValue ? $"Gia h?n VIP ??n {user.VipEndDate:dd/MM/yyyy}" : "Nâng c?p VIP v?nh vi?n") : "H?y VIP";
                user.AdminNote = $"[{DateTime.Now:dd/MM/yyyy HH:mm}] {action}: {adminNote}\n{user.AdminNote}";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// C?p nh?t ghi chú admin
        /// </summary>
        public async Task<bool> UpdateAdminNoteAsync(int userId, string note)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.AdminNote = note;
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// C?p nh?t role cho user
        /// Không cho phép xóa role Admin c?a chính mình
        /// </summary>
        public async Task<(bool Success, string Message)> UpdateUserRolesAsync(int userId, List<int> roleIds, int? currentAdminUserId = null)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) 
                return (false, "Không tìm th?y ng??i dùng!");

            // L?y role Admin
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
            
            // Ki?m tra n?u user ?ang là Admin và ?ang c? xóa role Admin c?a chính mình
            if (currentAdminUserId.HasValue && currentAdminUserId.Value == userId)
            {
                bool currentlyIsAdmin = user.Roles.Any(r => r.RoleName == "Admin");
                bool willStillBeAdmin = adminRole != null && roleIds.Contains(adminRole.Id);
                
                if (currentlyIsAdmin && !willStillBeAdmin)
                {
                    return (false, "B?n không th? t? xóa quy?n Admin c?a chính mình!");
                }
            }

            // Clear existing roles
            user.Roles.Clear();

            // Add new roles
            var roles = await _context.Roles
                .Where(r => roleIds.Contains(r.Id))
                .ToListAsync();

            foreach (var role in roles)
            {
                user.Roles.Add(role);
            }

            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return (true, "C?p nh?t vai trò thành công!");
        }

        /// <summary>
        /// C?p nh?t th?ng kê user sau khi có ??n hàng m?i
        /// </summary>
        public async Task UpdateUserOrderStatsAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return;

            user.TotalOrders = user.Orders?.Count(o => o.OrderStatus != "Cancelled") ?? 0;
            user.TotalSpent = user.Orders?
                .Where(o => o.OrderStatus == "Delivered")
                .Sum(o => o.TotalAmount) ?? 0;

            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// L?y th?ng kê t?ng quan v? users
        /// </summary>
        public async Task<UserStatistics> GetUserStatisticsAsync()
        {
            var users = await _context.Users.Include(u => u.Roles).ToListAsync();

            return new UserStatistics
            {
                TotalUsers = users.Count,
                ActiveUsers = users.Count(u => u.IsActive),
                InactiveUsers = users.Count(u => !u.IsActive),
                VipUsers = users.Count(u => u.IsVip),
                AdminUsers = users.Count(u => u.Roles.Any(r => r.RoleName == "Admin")),
                NewUsersThisMonth = users.Count(u => u.CreatedAt.Month == DateTime.Now.Month && u.CreatedAt.Year == DateTime.Now.Year),
                TotalRevenue = users.Sum(u => u.TotalSpent)
            };
        }

        /// <summary>
        /// L?y danh sách t?t c? roles
        /// </summary>
        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        /// <summary>
        /// Ki?m tra VIP h?t h?n và t? ??ng h?y
        /// </summary>
        public async Task CheckAndExpireVipAsync()
        {
            var expiredVipUsers = await _context.Users
                .Where(u => u.IsVip && u.VipEndDate.HasValue && u.VipEndDate < DateTime.Now)
                .ToListAsync();

            foreach (var user in expiredVipUsers)
            {
                user.IsVip = false;
                user.AdminNote = $"[{DateTime.Now:dd/MM/yyyy HH:mm}] VIP h?t h?n t? ??ng\n{user.AdminNote}";
                user.UpdatedAt = DateTime.Now;
            }

            if (expiredVipUsers.Any())
            {
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// ??m s? l??ng Admin trong h? th?ng
        /// </summary>
        public async Task<int> CountAdminsAsync()
        {
            return await _context.Users
                .Include(u => u.Roles)
                .CountAsync(u => u.Roles.Any(r => r.RoleName == "Admin"));
        }
    }

    public class UserStatistics
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int VipUsers { get; set; }
        public int AdminUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
