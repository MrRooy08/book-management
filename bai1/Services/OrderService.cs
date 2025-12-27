using bai1.Models;
using Microsoft.EntityFrameworkCore;

namespace bai1.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus, string? note = null);
        Task<bool> ConfirmOrderAsync(int orderId);
        Task<bool> CancelOrderAsync(int orderId, string reason);
        Task<Dictionary<string, int>> GetOrderStatisticsAsync();
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IInventoryService _inventoryService;

        public OrderService(ApplicationDbContext context, IInventoryService inventoryService)
        {
            _context = context;
            _inventoryService = inventoryService;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .ThenInclude(b => b.Images)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .ThenInclude(b => b.Images)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus, string? note = null)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            var oldStatus = order.OrderStatus;
            order.OrderStatus = newStatus;

            switch (newStatus)
            {
                case OrderStatusConstants.Delivered:
                    order.DeliveredAt = DateTime.Now;
                    order.PaymentStatus = PaymentStatusConstants.Paid;
                    order.PaidAt = DateTime.Now;
                    
                    // C?p nh?t t?n kho: gi?m ReservedQuantity, gi?m Quantity, t?ng SoldQuantity
                    await _inventoryService.ConfirmStockSoldAsync(orderId);
                    break;
                    
                case OrderStatusConstants.Cancelled:
                    order.CancelledAt = DateTime.Now;
                    if (!string.IsNullOrEmpty(note))
                        order.CancelReason = note;
                    
                    // Gi?i phóng hàng ?ã ??t tr??c khi h?y ??n
                    await _inventoryService.ReleaseReservedStockAsync(orderId);
                    break;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.OrderStatus = OrderStatusConstants.Confirmed;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelOrderAsync(int orderId, string reason)
        {
            return await UpdateOrderStatusAsync(orderId, OrderStatusConstants.Cancelled, reason);
        }

        public async Task<Dictionary<string, int>> GetOrderStatisticsAsync()
        {
            var stats = await _context.Orders
                .GroupBy(o => o.OrderStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return stats.ToDictionary(s => s.Status, s => s.Count);
        }
    }
}
