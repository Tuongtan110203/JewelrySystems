using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;

namespace WebBanVang.Repository
{
    public class SQLProductRepository : IProductRepository
    {
        private readonly JewelrySalesSystemDbContext dbContext;

        public SQLProductRepository(JewelrySalesSystemDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            var categoryExists = await dbContext.Categories.AnyAsync(c => c.CategoryId == product.CategoryId);
            if (!categoryExists)
            {
                throw new ArgumentException("Invalid categoryId");
            }
            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
            return product;
        }

     

        public async Task<List<Product>> GetAllProductAsync(string filterOn = null, string filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 10)
        {
            var walks = dbContext.Products
                                          .Where(x => x.Status == "active")
                                          .Include(x => x.Categories)
                                          .Include(x => x.GoldTypes)
                                          .Include(x => x.Stones)
                                          .AsQueryable();
         

            // filtering
            if (filterOn != null && filterQuery != null)
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase) || filterOn.Equals("description", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.ProductName.Contains(filterQuery) || x.Description.Contains(filterQuery));
                }
            }
            //sorting
            if (sortBy != null && sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                walks = isAscending ? walks.OrderBy(x => x.ProductName) : walks.OrderByDescending(x => x.ProductName);
            }
            else if (sortBy != null && sortBy.Equals("goldWeight", StringComparison.OrdinalIgnoreCase))
            {
                walks = isAscending ? walks.OrderBy(x => x.GoldWeight) : walks.OrderByDescending(x => x.GoldWeight);
            }
            else if (sortBy != null && sortBy.Equals("Price", StringComparison.OrdinalIgnoreCase))
            {
                walks = isAscending ? walks.OrderBy(x => x.Price) : walks.OrderByDescending(x => x.Price);
            }
            else if (sortBy != null && sortBy.Equals("wage", StringComparison.OrdinalIgnoreCase))
            {
                walks = isAscending ? walks.OrderBy(x => x.Wage) : walks.OrderByDescending(x => x.Wage);
            }
            // paging
            var SkipResult = (pageNumber - 1) * pageSize;
            return await walks.Skip(SkipResult).Take(pageSize).ToListAsync();
        }


        public async Task<Product?> GetByIdProductAsync(int id)
        {
            return await dbContext.Products.Where(x => x.Status == "active").Include(x => x.Categories).Include(x => x.GoldTypes).Include(x => x.Stones).FirstOrDefaultAsync(x => x.ProductId == id);
        }

      
        public async Task<Product> DeleteProductAsync(int id)
        {
            var checkExistProduct = await dbContext.Products.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.ProductId == id);
            if (checkExistProduct == null) { return null; }
            checkExistProduct.Status = "inactive";
            await dbContext.SaveChangesAsync();
            return checkExistProduct;

        }

        public async Task<double> GetSellPriceByGoldIdAsync(int goldId)
        {
            var goldType = await dbContext.GoldTypes.FirstOrDefaultAsync(g => g.GoldId == goldId);
            if (goldType != null)
            {
                return goldType.SellPrice;
            }
            return 0;
        }

        public async Task<double> GetSellPriceByProductIdAsync(int productId)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product != null)
            {
                int goldId = product.GoldId;
                var goldType = await dbContext.GoldTypes.FirstOrDefaultAsync(g => g.GoldId == goldId);

                if (goldType != null)
                {
                    return goldType.SellPrice;
                }
            }
            return 0;
        }
        public async Task<double> GetGoldWeightByProductIdAsync(int id)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);

            if (product != null)
            {
                return product.GoldWeight;
            }
            return 0;
        }
        public async Task<double> GetWageByProductIdAsync(int id)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);

            if (product != null)
            {
                return product.Wage;
            }
            return 0;
        }
        public  async Task<double> GetPriceRatioByProductIdAsync(int id)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);

            if (product != null)
            {
                return product.PriceRatio;
            }
            return 0;
        }
        public async Task<double> GetStonePriceByProductIdAsync(int id)
        {
            var product = await dbContext.Products
                                         .Include(p => p.Stones) 
                                         .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product != null && product.Stones != null)
            {
                var stonePrice = product.Stones.Sum(s => s.Price); 
                return stonePrice;
            }
            return 0;
        }

        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            if (existingProduct == null) { return null; }
            existingProduct.ProductName = product.ProductName;
            existingProduct.Description = product.Description;
            existingProduct.Image = product.Image;
            existingProduct.Quantity = product.Quantity;
            existingProduct.GoldWeight = product.GoldWeight;
            existingProduct.Wage = product.Wage;
            existingProduct.PriceRatio = product.PriceRatio;
            existingProduct.Price = product.Price;
            existingProduct.Size = product.Size;
            existingProduct.WarrantyPeriod = product.WarrantyPeriod;
            existingProduct.Status = product.Status;
            await dbContext.SaveChangesAsync();
            return existingProduct;
        }
        public async Task<Product> UpdateProductByTickOne(int id, Product product)
        {
            var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            if (existingProduct == null) { return null; }
            existingProduct.Price = product.Price;

            await dbContext.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<List<Product>> UpdateProductByTickALL()
        {
            var existingProducts = await dbContext.Products.ToListAsync();
            if (existingProducts == null || existingProducts.Count == 0) { return null; }

            foreach (var existingProduct in existingProducts)
            {
                double sellPrice = await GetSellPriceByProductIdAsync(existingProduct.ProductId);
                double goldWeight = existingProduct.GoldWeight;
                double wage = existingProduct.Wage;
                double priceRatio = existingProduct.PriceRatio;
                double stonePrice = await GetStonePriceByProductIdAsync(existingProduct.ProductId);
                existingProduct.Price = ((goldWeight * sellPrice) + wage + stonePrice) * priceRatio;
            }

            await dbContext.SaveChangesAsync();
            return existingProducts;
        }

        public async Task<Product> UpdateProductNormal(int id, Product product)
        {
            var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            if (existingProduct == null) { return null; }
            existingProduct.ProductName = product.ProductName;
            existingProduct.Description = product.Description;
            existingProduct.Image = product.Image;
            existingProduct.Quantity = product.Quantity;
            existingProduct.GoldWeight = product.GoldWeight;
            existingProduct.Wage = product.Wage;
            existingProduct.PriceRatio = product.PriceRatio;
            existingProduct.Price = product.Price;
            existingProduct.Size = product.Size;
            existingProduct.Status = product.Status;
            await dbContext.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<Product> UpdateProductPriceWithStone(int id, Product product)
        {
            
            var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            if (existingProduct == null) { return null; }

            existingProduct.Price = product.Price;

            await dbContext.SaveChangesAsync();
            return existingProduct;
        }
    }
}
