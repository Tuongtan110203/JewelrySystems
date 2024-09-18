using WebBanVang.Models.Domain;
using static WebBanVang.Repository.SQLProductRepository;

namespace WebBanVang.Repository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductAsync(List<FilterCriteria>? filters = null, string? sortBy = null,
          bool isAscending = true, int pageNumber = 1, int pageSize = 100,
          double? minPrice = null, double? maxPrice = null);
        //Task<List<Product>> FilterSellPriceGoldTypes(string? filterOn = null, string? filterQuery = null, double ? minPrice = null, double? maxPrice = null);
        Task<Product?> GetByIdProductAsync(int id);
        Task<Product?> GetByProductCodeAsync(string code);
        Task<Product> GetProductByWarrantyId(int id);
        Task<List<Product>> GetByProductByNameOrCode(string? nameCode = null, int pageNumber = 1, int pageSize = 100);

        Task<Product> CreateProductAsync(Product product);
        Task<Product?> UpdateProductAsync(int id, Product product);
        Task<Product?> UpdateProductNormal(int id, Product product);

        Task<Product?> UpdateProductByTickOne(int id, Product product);
        Task<List<Product>?> UpdateProductByTickALL();

        Task<Product?> DeleteProductAsync(int id);
        Task<double> GetGoldWeightByProductIdAsync(int id);
        Task<double> GetWageByProductIdAsync(int id);

        Task<double> GetSellPriceByGoldIdAsync(int goldId);
        Task<double> GetSellPriceByProductIdAsync(int productId);
        Task<double> GetStonePriceByProductIdAsync(int productId);


        Task<Product?> UpdateProductPriceWithStone(int id, Product product);
        Task<List<Product>?> GetOutOfStockProductsAsync();
        Task<List<Product>?> GetTopBestSellerProductsAsync(int topNum);

        Task<int> GetLatestProductIdAsync();
        Task<Product?> UpdateNotSoldProductToActiveAsync(int id);
    }
}
