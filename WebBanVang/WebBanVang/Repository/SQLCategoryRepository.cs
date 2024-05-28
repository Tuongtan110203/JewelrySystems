using AutoMapper;
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

        
    }
}
