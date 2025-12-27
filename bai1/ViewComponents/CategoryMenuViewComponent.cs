using bai1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bai1.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoryMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var parentCategories = await _context.Categories
                .Where(c => c.ParentId == null)
                .Include(c => c.SubCategories)
                .ToListAsync();

            return View(parentCategories);
        }
    }
}
