using bai1.Models;
using Microsoft.EntityFrameworkCore;

namespace bai1.Services
{
    public interface IReportService
    {
        Task<ReportSummary> GetReportSummaryAsync(DateTime? fromDate, DateTime? toDate);
        Task<List<DailyRevenue>> GetDailyRevenueAsync(DateTime? fromDate, DateTime? toDate);
        Task<List<MonthlyRevenue>> GetMonthlyRevenueAsync(int year);
        Task<List<QuarterlyRevenue>> GetQuarterlyRevenueAsync(int year);
        Task<List<TopSellingBook>> GetTopSellingBooksAsync(DateTime? fromDate, DateTime? toDate, int top = 10);
        Task<List<OrderReportItem>> GetOrdersReportAsync(DateTime? fromDate, DateTime? toDate, string? status, int page, int pageSize);
        Task<int> GetOrdersCountAsync(DateTime? fromDate, DateTime? toDate, string? status);
        Task<List<CategoryRevenue>> GetRevenueByCategoryAsync(DateTime? fromDate, DateTime? toDate);
    }

    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReportSummary> GetReportSummaryAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Orders.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(o => o.CreatedAt <= toDate.Value.AddDays(1));

            var totalOrders = await query.CountAsync();
            var deliveredOrders = await query.CountAsync(o => o.OrderStatus == "Delivered");
            var cancelledOrders = await query.CountAsync(o => o.OrderStatus == "Cancelled");
            var pendingOrders = await query.CountAsync(o => o.OrderStatus == "Pending" || o.OrderStatus == "Confirmed" || o.OrderStatus == "Processing" || o.OrderStatus == "Shipping");

            var totalRevenue = await query
                .Where(o => o.OrderStatus == "Delivered")
                .SumAsync(o => o.TotalAmount);

            var totalCost = await query
                .Where(o => o.OrderStatus == "Delivered")
                .SelectMany(o => o.OrderDetails)
                .SumAsync(od => od.Book.CostPrice * od.Quantity);

            var totalProfit = totalRevenue - totalCost;

            var totalProductsSold = await query
                .Where(o => o.OrderStatus == "Delivered")
                .SelectMany(o => o.OrderDetails)
                .SumAsync(od => od.Quantity);

            return new ReportSummary
            {
                TotalOrders = totalOrders,
                DeliveredOrders = deliveredOrders,
                CancelledOrders = cancelledOrders,
                PendingOrders = pendingOrders,
                TotalRevenue = totalRevenue,
                TotalCost = totalCost,
                TotalProfit = totalProfit,
                TotalProductsSold = totalProductsSold,
                AverageOrderValue = deliveredOrders > 0 ? totalRevenue / deliveredOrders : 0
            };
        }

        public async Task<List<DailyRevenue>> GetDailyRevenueAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Orders
                .Where(o => o.OrderStatus == "Delivered");

            if (fromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(o => o.CreatedAt <= toDate.Value.AddDays(1));

            return await query
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new DailyRevenue
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToListAsync();
        }

        public async Task<List<MonthlyRevenue>> GetMonthlyRevenueAsync(int year)
        {
            var result = await _context.Orders
                .Where(o => o.OrderStatus == "Delivered" && o.CreatedAt.Year == year)
                .GroupBy(o => o.CreatedAt.Month)
                .Select(g => new MonthlyRevenue
                {
                    Month = g.Key,
                    Year = year,
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count(),
                    ProductsSold = g.SelectMany(o => o.OrderDetails).Sum(od => od.Quantity)
                })
                .OrderBy(m => m.Month)
                .ToListAsync();

            // Fill missing months with zero
            var allMonths = new List<MonthlyRevenue>();
            for (int i = 1; i <= 12; i++)
            {
                var existing = result.FirstOrDefault(r => r.Month == i);
                if (existing != null)
                    allMonths.Add(existing);
                else
                    allMonths.Add(new MonthlyRevenue { Month = i, Year = year, Revenue = 0, OrderCount = 0, ProductsSold = 0 });
            }

            return allMonths;
        }

        public async Task<List<QuarterlyRevenue>> GetQuarterlyRevenueAsync(int year)
        {
            var monthlyData = await GetMonthlyRevenueAsync(year);

            return new List<QuarterlyRevenue>
            {
                new QuarterlyRevenue
                {
                    Quarter = 1,
                    Year = year,
                    Revenue = monthlyData.Where(m => m.Month >= 1 && m.Month <= 3).Sum(m => m.Revenue),
                    OrderCount = monthlyData.Where(m => m.Month >= 1 && m.Month <= 3).Sum(m => m.OrderCount),
                    ProductsSold = monthlyData.Where(m => m.Month >= 1 && m.Month <= 3).Sum(m => m.ProductsSold)
                },
                new QuarterlyRevenue
                {
                    Quarter = 2,
                    Year = year,
                    Revenue = monthlyData.Where(m => m.Month >= 4 && m.Month <= 6).Sum(m => m.Revenue),
                    OrderCount = monthlyData.Where(m => m.Month >= 4 && m.Month <= 6).Sum(m => m.OrderCount),
                    ProductsSold = monthlyData.Where(m => m.Month >= 4 && m.Month <= 6).Sum(m => m.ProductsSold)
                },
                new QuarterlyRevenue
                {
                    Quarter = 3,
                    Year = year,
                    Revenue = monthlyData.Where(m => m.Month >= 7 && m.Month <= 9).Sum(m => m.Revenue),
                    OrderCount = monthlyData.Where(m => m.Month >= 7 && m.Month <= 9).Sum(m => m.OrderCount),
                    ProductsSold = monthlyData.Where(m => m.Month >= 7 && m.Month <= 9).Sum(m => m.ProductsSold)
                },
                new QuarterlyRevenue
                {
                    Quarter = 4,
                    Year = year,
                    Revenue = monthlyData.Where(m => m.Month >= 10 && m.Month <= 12).Sum(m => m.Revenue),
                    OrderCount = monthlyData.Where(m => m.Month >= 10 && m.Month <= 12).Sum(m => m.OrderCount),
                    ProductsSold = monthlyData.Where(m => m.Month >= 10 && m.Month <= 12).Sum(m => m.ProductsSold)
                }
            };
        }

        public async Task<List<TopSellingBook>> GetTopSellingBooksAsync(DateTime? fromDate, DateTime? toDate, int top = 10)
        {
            var query = _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Book)
                .ThenInclude(b => b.Images)
                .Where(od => od.Order.OrderStatus == "Delivered");

            if (fromDate.HasValue)
                query = query.Where(od => od.Order.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(od => od.Order.CreatedAt <= toDate.Value.AddDays(1));

            var groupedData = await query
                .GroupBy(od => new { od.BookId, od.Book.Title, od.Book.SalePrice })
                .Select(g => new 
                {
                    BookId = g.Key.BookId,
                    BookTitle = g.Key.Title,
                    SalePrice = g.Key.SalePrice,
                    QuantitySold = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.Subtotal)
                })
                .OrderByDescending(t => t.QuantitySold)
                .Take(top)
                .ToListAsync();

            // Get book images separately
            var bookIds = groupedData.Select(g => g.BookId).ToList();
            var bookImages = await _context.Books
                .Where(b => bookIds.Contains(b.Id))
                .Include(b => b.Images)
                .Select(b => new { b.Id, ImageUrl = b.Images.FirstOrDefault().ImageUrl })
                .ToListAsync();

            return groupedData.Select(g => new TopSellingBook
            {
                BookId = g.BookId,
                BookTitle = g.BookTitle,
                SalePrice = g.SalePrice,
                QuantitySold = g.QuantitySold,
                TotalRevenue = g.TotalRevenue,
                ImageUrl = bookImages.FirstOrDefault(bi => bi.Id == g.BookId)?.ImageUrl
            }).ToList();
        }

        public async Task<List<OrderReportItem>> GetOrdersReportAsync(DateTime? fromDate, DateTime? toDate, string? status, int page, int pageSize)
        {
            var query = _context.Orders
                .Include(o => o.OrderDetails)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(o => o.CreatedAt <= toDate.Value.AddDays(1));
            if (!string.IsNullOrEmpty(status) && status != "All")
                query = query.Where(o => o.OrderStatus == status);

            return await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderReportItem
                {
                    OrderId = o.Id,
                    CustomerName = o.CustomerName,
                    Phone = o.Phone,
                    CreatedAt = o.CreatedAt,
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.PaymentMethod,
                    PaymentStatus = o.PaymentStatus,
                    OrderStatus = o.OrderStatus,
                    ItemCount = o.OrderDetails.Sum(od => od.Quantity)
                })
                .ToListAsync();
        }

        public async Task<int> GetOrdersCountAsync(DateTime? fromDate, DateTime? toDate, string? status)
        {
            var query = _context.Orders.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(o => o.CreatedAt <= toDate.Value.AddDays(1));
            if (!string.IsNullOrEmpty(status) && status != "All")
                query = query.Where(o => o.OrderStatus == status);

            return await query.CountAsync();
        }

        public async Task<List<CategoryRevenue>> GetRevenueByCategoryAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Book)
                .ThenInclude(b => b.Categories)
                .Where(od => od.Order.OrderStatus == "Delivered");

            if (fromDate.HasValue)
                query = query.Where(od => od.Order.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(od => od.Order.CreatedAt <= toDate.Value.AddDays(1));

            var data = await query
                .SelectMany(od => od.Book.Categories.Select(c => new { CategoryName = c.Name, od.Subtotal, od.Quantity }))
                .GroupBy(x => x.CategoryName)
                .Select(g => new CategoryRevenue
                {
                    CategoryName = g.Key,
                    Revenue = g.Sum(x => x.Subtotal),
                    ProductsSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(c => c.Revenue)
                .ToListAsync();

            return data;
        }
    }

    #region Report Models

    public class ReportSummary
    {
        public int TotalOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
        public int TotalProductsSold { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class DailyRevenue
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class MonthlyRevenue
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public int ProductsSold { get; set; }

        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM");
    }

    public class QuarterlyRevenue
    {
        public int Quarter { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public int ProductsSold { get; set; }

        public string QuarterName => $"Quý {Quarter}";
    }

    public class TopSellingBook
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public decimal SalePrice { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class OrderReportItem
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class CategoryRevenue
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int ProductsSold { get; set; }
    }

    #endregion
}
