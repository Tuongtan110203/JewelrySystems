using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;

namespace WebBanVang.Repository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category?> UpdateCategoryAsync(int id, Category category);
        Task<Category> CreateAsync(Category category);
        Task<Category?> DeleteCategoryAsync(int id);
        Task<List<Category>> GetCategoriesByNameAsync(string name);
        Task<Category> GetCategoryByNameAsync(string name);
        Task<List<CategoryCodePercentageDTO>> GetCategoryCodePercentagesForToday();
       Task<List<CategoryCodePercentageDTO>> GetCategoryCodePercentagesForThisWeek();
       Task<List<CategoryCodePercentageDTO>> GetCategoryCodePercentagesForThisMonth();
       Task<List<CategoryCodePercentageDTO>> GetCategoryCodePercentagesForThisYear();
    }
}
