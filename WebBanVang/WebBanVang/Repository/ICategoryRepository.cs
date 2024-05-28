using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;

namespace WebBanVang.Repository
{
    public interface ICategoryRepository
    {
        Task<List<Category>>  GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category?> UpdateCategoryAsync(int id, Category category);
        Task<Category> CreateAsync(Category category);
        Task<Category?> DeleteCategoryAsync(int id);
    }
}
