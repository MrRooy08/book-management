using bai1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bai1.Controllers
{
    /// <summary>
    /// Controller báo cáo doanh thu dành cho Admin
    /// Ch? Admin m?i có quy?n truy c?p
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: Report
        public async Task<IActionResult> Index(
            string filterType = "month",
            int? year = null,
            int? month = null,
            int? quarter = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            year ??= DateTime.Now.Year;

            // Calculate date range based on filter type
            DateTime startDate, endDate;
            switch (filterType)
            {
                case "month":
                    month ??= DateTime.Now.Month;
                    startDate = new DateTime(year.Value, month.Value, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                    break;
                case "quarter":
                    quarter ??= (DateTime.Now.Month - 1) / 3 + 1;
                    int startMonth = (quarter.Value - 1) * 3 + 1;
                    startDate = new DateTime(year.Value, startMonth, 1);
                    endDate = startDate.AddMonths(3).AddDays(-1);
                    break;
                case "year":
                    startDate = new DateTime(year.Value, 1, 1);
                    endDate = new DateTime(year.Value, 12, 31);
                    break;
                case "custom":
                    startDate = fromDate ?? DateTime.Now.AddMonths(-1);
                    endDate = toDate ?? DateTime.Now;
                    break;
                default:
                    startDate = new DateTime(year.Value, DateTime.Now.Month, 1);
                    endDate = DateTime.Now;
                    break;
            }

            var summary = await _reportService.GetReportSummaryAsync(startDate, endDate);
            var monthlyRevenue = await _reportService.GetMonthlyRevenueAsync(year.Value);
            var quarterlyRevenue = await _reportService.GetQuarterlyRevenueAsync(year.Value);
            var topSellingBooks = await _reportService.GetTopSellingBooksAsync(startDate, endDate, 10);
            var categoryRevenue = await _reportService.GetRevenueByCategoryAsync(startDate, endDate);

            ViewBag.FilterType = filterType;
            ViewBag.Year = year;
            ViewBag.Month = month ?? DateTime.Now.Month;
            ViewBag.Quarter = quarter ?? (DateTime.Now.Month - 1) / 3 + 1;
            ViewBag.FromDate = startDate;
            ViewBag.ToDate = endDate;
            ViewBag.MonthlyRevenue = monthlyRevenue;
            ViewBag.QuarterlyRevenue = quarterlyRevenue;
            ViewBag.TopSellingBooks = topSellingBooks;
            ViewBag.CategoryRevenue = categoryRevenue;

            return View(summary);
        }

        // GET: Report/Orders
        public async Task<IActionResult> Orders(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string status = "All",
            int page = 1,
            int pageSize = 10)
        {
            fromDate ??= DateTime.Now.AddMonths(-1);
            toDate ??= DateTime.Now;

            var orders = await _reportService.GetOrdersReportAsync(fromDate, toDate, status, page, pageSize);
            var totalCount = await _reportService.GetOrdersCountAsync(fromDate, toDate, status);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.Status = status;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;

            return View(orders);
        }

        // GET: Report/ExportExcel
        public async Task<IActionResult> ExportExcel(DateTime? fromDate, DateTime? toDate, string status = "All")
        {
            // TODO: Implement Excel export using EPPlus or ClosedXML
            TempData["InfoMessage"] = "Ch?c n?ng xu?t Excel ?ang ???c phát tri?n!";
            return RedirectToAction(nameof(Orders), new { fromDate, toDate, status });
        }

        // API for Chart Data
        [HttpGet]
        public async Task<IActionResult> GetChartData(int year)
        {
            var monthlyRevenue = await _reportService.GetMonthlyRevenueAsync(year);
            return Json(monthlyRevenue.Select(m => new
            {
                month = m.MonthName,
                revenue = m.Revenue,
                orders = m.OrderCount
            }));
        }

        [HttpGet]
        public async Task<IActionResult> GetQuarterlyChartData(int year)
        {
            var quarterlyRevenue = await _reportService.GetQuarterlyRevenueAsync(year);
            return Json(quarterlyRevenue.Select(q => new
            {
                quarter = q.QuarterName,
                revenue = q.Revenue,
                orders = q.OrderCount
            }));
        }
    }
}
