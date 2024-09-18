using AutoMapper;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;

namespace WebBanVang.Repository
{
    public class SQLCategoryRepository : ICategoryRepository
    {
        private readonly JewelrySalesSystemDbContext _context;

        public SQLCategoryRepository(JewelrySalesSystemDbContext context)
        {
            this._context = context;
        }

        public async Task<Category> CreateAsync(Category category)
        {
           
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> DeleteCategoryAsync(int id)
        {
            var existingItem = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (existingItem == null)
            {
                return null;
            }
            var productExistsInOrders = await _context.OrderDetails
       .Join(_context.Products,
             od => od.ProductId,
             p => p.ProductId,
             (od, p) => new { od, p })
       .AnyAsync(joined => joined.p.CategoryId == existingItem.CategoryId &&
                           _context.Orders
                               .Where(o => o.Status == "Đang thanh toán" ||
                                           o.Status == "Đã thanh toán" ||
                                           o.Status == "Đã hoàn thành" ||
                                           o.Status == "Hết hạn thanh toán")
                               .Select(o => o.OrderId)
                               .Contains(joined.od.OrderId));

            if (productExistsInOrders)
            {
                throw new InvalidOperationException("Danh mục này vẫn còn sản phẩm. Không thể xoá");
            }
            existingItem.Status = "inactive";
            await _context.SaveChangesAsync();
            return existingItem;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            //var walks = dbContext.Products.Where(x => x.Status == "available").Include(x => x.Categories).AsQueryable();
            var category = _context.Categories.Where(x => x.Status == "active").AsQueryable();
            return await category.ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesByNameAsync(string name)
        {
            return await _context.Categories.Where(x => x.Status == "active").Where(x => x.Name.Contains(name)).ToListAsync();
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            return await _context.Categories.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.Name.Contains(name));
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.CategoryId == id);
        }

        public async Task<Category?> UpdateCategoryAsync(int id, Category category)
        {
            var existingItem = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (existingItem == null)
            {
                return null;
            }
           
          
            existingItem.Name = category.Name;
            existingItem.Status = category.Status;

            await _context.SaveChangesAsync();
            return existingItem;
        }

        public async Task<List<CategoryCodePercentageDTO>> CalculateCategoryCodePercentagesAsync(DateTime startDate, DateTime endDate)
        {
            // Retrieve orders with the specified statuses and within the date range
            var orders = await _context.Orders
                .Where(o => (o.Status == "Đã thanh toán" || o.Status == "Đã hoàn thành") &&
                            o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Products)
                    .ThenInclude(p => p.Categories)
                .ToListAsync();

            // Flatten the order details
            var orderDetails = orders.SelectMany(o => o.OrderDetails);

            // Group by CategoryCode and calculate the total price for each group
            var categoryCodeGroups = orderDetails
           .GroupBy(od => new { od.Products.Categories.CategoryCode, od.Products.Categories.Name })
           .Select(g => new
           {
               g.Key.CategoryCode,
               g.Key.Name,
               TotalPrice = g.Sum(od => od.Price * od.Quantity)
           })
           .ToList();


            // Calculate the overall total price for all qualifying OrderDetails
            var overallTotalPrice = categoryCodeGroups.Sum(g => g.TotalPrice);

            // Calculate the percentage for each CategoryCode
            var categoryCodePercentages = categoryCodeGroups
                .Select(g => new CategoryCodePercentageDTO
                {
                    CategoryCode = g.CategoryCode,
                    TotalPrice = g.TotalPrice,
                    CategoryName = g.Name,
                    Percentage = overallTotalPrice == 0 ? 0 : (g.TotalPrice / overallTotalPrice) * 100
                })
                .ToList();

            return categoryCodePercentages;
        }

        public async Task<List<CategoryCodePercentageDTO>> GetCategoryCodePercentagesForToday()
        {
            var today = DateTime.Today;
            return await CalculateCategoryCodePercentagesAsync(today, today.AddDays(1).AddTicks(-1));
        }

        public async Task<List<CategoryCodePercentageDTO>> GetCategoryCodePercentagesForThisWeek()
        {
            var today = DateTime.Now;
            var currentDayOfWeek = (int)today.DayOfWeek;
            currentDayOfWeek = (currentDayOfWeek == 0) ? 7 : currentDayOfWeek;
            var startOfWeek = today.AddDays(-currentDayOfWeek + 1).Date;
            var endOfWeek = startOfWeek.AddDays(7).Date;
            return await CalculateCategoryCodePercentagesAsync(startOfWeek, endOfWeek);
        }

        public async Task<List<CategoryCodePercentageDTO>> GetCategoryCodePercentagesForThisMonth()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);
            return await CalculateCategoryCodePercentagesAsync(startOfMonth, endOfMonth);
        }

        public async Task<List<CategoryCodePercentageDTO>> GetCategoryCodePercentagesForThisYear()
        {
            var today = DateTime.Today;
            var startOfYear = new DateTime(today.Year, 1, 1);
            var endOfYear = startOfYear.AddYears(1).AddTicks(-1);
            return await CalculateCategoryCodePercentagesAsync(startOfYear, endOfYear);
        }

    }
}
