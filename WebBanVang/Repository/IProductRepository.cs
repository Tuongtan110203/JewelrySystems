using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;

namespace WebBanVang.Repository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductAsync(string filterOn = null, string filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        Task<Product> GetByIdProductAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(int id, Product product);
        Task<Product> UpdateProductNormal(int id, Product product);

        Task<Product> UpdateProductByTickOne(int id, Product product);
        Task<List<Product>> UpdateProductByTickALL();

        Task<Product> DeleteProductAsync(int id);
        Task<double> GetGoldWeightByProductIdAsync(int id);
        Task<double> GetWageByProductIdAsync(int id);
        Task<double> GetPriceRatioByProductIdAsync(int id);

        Task<double> GetSellPriceByGoldIdAsync(int goldId);
        Task<double> GetSellPriceByProductIdAsync(int productId);
        Task<double> GetStonePriceByProductIdAsync(int productId);

        
        Task<Product> UpdateProductPriceWithStone(int id, Product product);

    }
}
