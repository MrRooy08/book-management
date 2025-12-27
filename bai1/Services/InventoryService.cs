using bai1.Models;
using Microsoft.EntityFrameworkCore;

namespace bai1.Services
{
    public interface IInventoryService
    {
        Task<bool> CheckAvailabilityAsync(int bookId, int quantity);
        Task<Dictionary<int, int>> CheckAvailabilityAsync(List<CartItem> cartItems);
        Task<bool> ReserveStockAsync(int orderId, List<OrderDetail> orderDetails);
        Task<bool> ConfirmStockSoldAsync(int orderId);
        Task<bool> ReleaseReservedStockAsync(int orderId);
        Task<int> GetAvailableQuantityAsync(int bookId);
        Task<int> GetTotalQuantityAsync(int bookId);
    }

    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(ApplicationDbContext context, ILogger<InventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Ki?m tra xem m?t s?n ph?m có ?? s? l??ng không (ch? ?? hi?n th?, không ch?n ??t hàng)
        /// </summary>
        public async Task<bool> CheckAvailabilityAsync(int bookId, int quantity)
        {
            var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.BookId == bookId);
            if (inventory == null) return false;
            
            return inventory.AvailableQuantity >= quantity;
        }

        /// <summary>
        /// Ki?m tra t?t c? s?n ph?m trong gi? hàng, tr? v? dictionary v?i BookId và s? l??ng thi?u
        /// L?u ý: K?t qu? ch? ?? hi?n th? thông tin, không ch?n ??t hàng
        /// </summary>
        public async Task<Dictionary<int, int>> CheckAvailabilityAsync(List<CartItem> cartItems)
        {
            var insufficientItems = new Dictionary<int, int>();
            var bookIds = cartItems.Select(c => c.BookId).ToList();
            
            var inventories = await _context.Inventories
                .Where(i => bookIds.Contains(i.BookId))
                .ToDictionaryAsync(i => i.BookId, i => i);

            foreach (var item in cartItems)
            {
                if (!inventories.TryGetValue(item.BookId, out var inventory))
                {
                    // Không có inventory record -> thi?u toàn b?
                    insufficientItems[item.BookId] = item.Quantity;
                }
                else if (inventory.AvailableQuantity < item.Quantity)
                {
                    // Không ?? s? l??ng
                    insufficientItems[item.BookId] = item.Quantity - inventory.AvailableQuantity;
                }
            }

            return insufficientItems;
        }

        /// <summary>
        /// ??t tr??c hàng khi t?o ??n - CHO PHÉP OVERSELLING (không ki?m tra t?n kho)
        /// ReservedQuantity có th? v??t quá Quantity
        /// </summary>
        public async Task<bool> ReserveStockAsync(int orderId, List<OrderDetail> orderDetails)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var detail in orderDetails)
                {
                    var inventory = await _context.Inventories
                        .FirstOrDefaultAsync(i => i.BookId == detail.BookId);

                    if (inventory == null)
                    {
                        // N?u ch?a có inventory record, t?o m?i v?i s? l??ng 0
                        inventory = new Inventory
                        {
                            BookId = detail.BookId,
                            Quantity = 0,
                            ReservedQuantity = 0,
                            SoldQuantity = 0,
                            LastUpdated = DateTime.Now
                        };
                        _context.Inventories.Add(inventory);
                        await _context.SaveChangesAsync();
                    }

                    // Cho phép overselling - t?ng ReservedQuantity không gi?i h?n
                    inventory.ReservedQuantity += detail.Quantity;
                    inventory.LastUpdated = DateTime.Now;
                    
                    _logger.LogInformation("??t tr??c {Quantity} cu?n cho BookId {BookId}. Reserved: {Reserved}, Available: {Available}",
                        detail.Quantity, detail.BookId, inventory.ReservedQuantity, inventory.AvailableQuantity);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                _logger.LogInformation("?ã ??t tr??c hàng cho ??n hàng {OrderId}", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi ??t tr??c hàng cho ??n hàng {OrderId}", orderId);
                await transaction.RollbackAsync();
                return false;
            }
        }

        /// <summary>
        /// Xác nh?n bán hàng sau khi giao hàng thành công
        /// </summary>
        public async Task<bool> ConfirmStockSoldAsync(int orderId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    _logger.LogWarning("Không tìm th?y ??n hàng {OrderId}", orderId);
                    return false;
                }

                foreach (var detail in order.OrderDetails)
                {
                    var inventory = await _context.Inventories
                        .FirstOrDefaultAsync(i => i.BookId == detail.BookId);

                    if (inventory == null)
                    {
                        _logger.LogWarning("Không tìm th?y inventory cho BookId {BookId}", detail.BookId);
                        continue;
                    }

                    // Chuy?n t? Reserved sang Sold
                    inventory.ReservedQuantity -= detail.Quantity;
                    inventory.Quantity -= detail.Quantity;
                    inventory.SoldQuantity += detail.Quantity;
                    inventory.LastUpdated = DateTime.Now;

                    // ??m b?o không âm
                    if (inventory.ReservedQuantity < 0) inventory.ReservedQuantity = 0;
                    if (inventory.Quantity < 0) inventory.Quantity = 0;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                _logger.LogInformation("?ã xác nh?n bán hàng cho ??n hàng {OrderId}", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi xác nh?n bán hàng cho ??n hàng {OrderId}", orderId);
                await transaction.RollbackAsync();
                return false;
            }
        }

        /// <summary>
        /// Gi?i phóng hàng ?ã ??t tr??c khi thanh toán th?t b?i ho?c h?y ??n
        /// </summary>
        public async Task<bool> ReleaseReservedStockAsync(int orderId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    _logger.LogWarning("Không tìm th?y ??n hàng {OrderId}", orderId);
                    return false;
                }

                foreach (var detail in order.OrderDetails)
                {
                    var inventory = await _context.Inventories
                        .FirstOrDefaultAsync(i => i.BookId == detail.BookId);

                    if (inventory == null) continue;

                    inventory.ReservedQuantity -= detail.Quantity;
                    inventory.LastUpdated = DateTime.Now;

                    // ??m b?o không âm
                    if (inventory.ReservedQuantity < 0) inventory.ReservedQuantity = 0;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                _logger.LogInformation("?ã gi?i phóng hàng ??t tr??c cho ??n hàng {OrderId}", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi gi?i phóng hàng ??t tr??c cho ??n hàng {OrderId}", orderId);
                await transaction.RollbackAsync();
                return false;
            }
        }

        /// <summary>
        /// L?y s? l??ng có th? bán c?a m?t s?n ph?m (Quantity - ReservedQuantity)
        /// Có th? tr? v? s? âm n?u ?ã overselling
        /// </summary>
        public async Task<int> GetAvailableQuantityAsync(int bookId)
        {
            var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.BookId == bookId);
            return inventory?.AvailableQuantity ?? 0;
        }

        /// <summary>
        /// L?y t?ng s? l??ng t?n kho (không tr? reserved)
        /// </summary>
        public async Task<int> GetTotalQuantityAsync(int bookId)
        {
            var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.BookId == bookId);
            return inventory?.Quantity ?? 0;
        }
    }
}
