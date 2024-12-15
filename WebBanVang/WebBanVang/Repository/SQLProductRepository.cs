using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WebBanVang.Data; // Adjust the namespace based on where JewelrySalesSystemDbContext is defined
using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public class SQLProductRepository : IProductRepository
    {
        private readonly JewelrySalesSystemDbContext dbContext;

        public SQLProductRepository(JewelrySalesSystemDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<int> GetLatestProductIdAsync()
        {
            var latestProductId = await dbContext.Products
                                                 .OrderByDescending(p => p.ProductId)
                                                 .Select(p => p.ProductId)
                                                 .FirstOrDefaultAsync();

            return latestProductId;
        }


        public async Task<Product> CreateProductAsync(Product product)
        {


            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
            return product;
        }

        //private async Task<string> GenerateProductCodeAsync()
        //{
        //    var maxProductCode = await dbContext.Products
        //        .OrderByDescending(p => p.ProductCode)
        //        .Select(p => p.ProductCode)
        //        .FirstOrDefaultAsync();

        //    int nextProductCodeNumber = 1;

        //    if (!string.IsNullOrEmpty(maxProductCode) && maxProductCode.StartsWith("KHN"))
        //    {
        //        if (int.TryParse(maxProductCode.Substring(3), out int currentMax))
        //        {
        //            nextProductCodeNumber = currentMax + 1;
        //        }
        //    }

        //    return $"KHN{nextProductCodeNumber:D6}";
        //}


        //public Task<List<Product>> FilterSellPriceGoldTypes(string? filterOn = null, string? filterQuery = null, double? minPrice = null, double? maxPrice = null)
        //{
        //    var products = dbContext.Products.Where(x => x.Status == "active")
        //                                    ///  .Include(x => x.Categories)
        //                                      .Include(x => x.GoldTypes)
        //                                    //  .Include(x => x.Stones)
        //                                      .AsQueryable();

        //    if (!string.IsNullOrEmpty(filterOn) && filterOn.Equals("GoldName", StringComparison.OrdinalIgnoreCase))
        //    {
        //        if (!string.IsNullOrEmpty(filterQuery))
        //        {
        //            products = products.Where(x => x.GoldTypes.GoldName.Contains(filterQuery));
        //        }
        //        if (minPrice.HasValue)
        //        {
        //            products = products.Where(x => x.GoldTypes.SellPrice >= minPrice.Value);
        //        }

        //        if (maxPrice.HasValue)
        //        {
        //            products = products.Where(x => x.GoldTypes.SellPrice <= maxPrice.Value);
        //        }

        //    }
        //    else
        //    {
        //        if (minPrice.HasValue)
        //        {
        //            products = products.Where(x => x.GoldTypes.SellPrice >= minPrice.Value);
        //        }

        //        if (maxPrice.HasValue)
        //        {
        //            products = products.Where(x => x.GoldTypes.SellPrice <= maxPrice.Value);
        //        }
        //    }

        //    return products.ToListAsync();
        //}



        public async Task<List<Product>> GetAllProductAsync(List<FilterCriteria>? filters = null, string? sortBy = null,
     bool isAscending = true, int pageNumber = 1, int pageSize = 100,
     double? minPrice = null, double? maxPrice = null)
        {
            var products = dbContext.Products
                                     .Where(x => x.Status == "active" || x.Status == "Dừng bán")
                                     .Include(x => x.Categories)
                                     .Include(x => x.GoldTypes)
                                     .Include(x => x.Stones)
                                     .AsQueryable();

            // Filtering
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    if (!string.IsNullOrEmpty(filter.FilterOn) && !string.IsNullOrEmpty(filter.FilterQuery))
                    {
                        if (filter.FilterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                        {
                            products = products.Where(x => x.ProductName.Contains(filter.FilterQuery));
                        }
                        else if (filter.FilterOn.Equals("Code", StringComparison.OrdinalIgnoreCase))
                        {
                            products = products.Where(x => x.ProductCode.Contains(filter.FilterQuery));
                        }
                        else if (filter.FilterOn.Equals("ID", StringComparison.OrdinalIgnoreCase) && int.TryParse(filter.FilterQuery, out int productId))
                        {
                            products = products.Where(x => x.ProductId == productId);
                        }
                        else if (filter.FilterOn.Equals("CategoryID", StringComparison.OrdinalIgnoreCase))
                        {
                            var categoryIds = filter.FilterQuery.Split(',').Select(int.Parse).ToList();
                            products = products.Where(x => categoryIds.Contains(x.CategoryId));
                        }
                        else if (filter.FilterOn.Equals("GoldId", StringComparison.OrdinalIgnoreCase) && int.TryParse(filter.FilterQuery, out int goldId))
                        {
                            products = products.Where(x => x.GoldTypes.GoldId == goldId);
                        }
                    }
                }
                if (minPrice.HasValue)
                {
                    products = products.Where(x => x.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    products = products.Where(x => x.Price <= maxPrice.Value);
                }
            }
            else
            {
                if (minPrice.HasValue)
                {
                    products = products.Where(x => x.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    products = products.Where(x => x.Price <= maxPrice.Value);
                }
            }

            // Sorting
            if (sortBy != null)
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    products = isAscending ? products.OrderBy(x => x.ProductName) : products.OrderByDescending(x => x.ProductName);
                }
                else if (sortBy.Equals("Code", StringComparison.OrdinalIgnoreCase))
                {
                    products = isAscending ? products.OrderBy(x => x.ProductCode) : products.OrderByDescending(x => x.ProductCode);
                }
                else if (sortBy.Equals("GoldWeight", StringComparison.OrdinalIgnoreCase))
                {
                    products = isAscending ? products.OrderBy(x => x.GoldWeight) : products.OrderByDescending(x => x.GoldWeight);
                }
                else if (sortBy.Equals("Price", StringComparison.OrdinalIgnoreCase))
                {
                    products = isAscending ? products.OrderBy(x => x.Price) : products.OrderByDescending(x => x.Price);
                }
                else if (sortBy.Equals("Wage", StringComparison.OrdinalIgnoreCase))
                {
                    products = isAscending ? products.OrderBy(x => x.Wage) : products.OrderByDescending(x => x.Wage);
                }
            }

            // Paging
            var skip = (pageNumber - 1) * pageSize;
            return await products.Skip(skip).Take(pageSize).ToListAsync();
        }

        public class FilterCriteria
        {
            public string? FilterOn { get; set; }
            public string? FilterQuery { get; set; }
        }


        public async Task<Product?> GetByIdProductAsync(int id)
        {
            return await dbContext.Products.Where(x => x.Status == "active" || x.Status == "Dừng bán").Include(x => x.Categories).Include(x => x.GoldTypes).Include(x => x.Stones).FirstOrDefaultAsync(x => x.ProductId == id);
        }
        public async Task<Product?> GetByProductCodeAsync(string code)
        {
            return await dbContext.Products.Where(x => x.Status == "active" || x.Status == "Dừng bán").Include(x => x.Categories).Include(x => x.GoldTypes).Include(x => x.Stones).FirstOrDefaultAsync(x => x.ProductCode == code);
        }
        public async Task<List<Product>> GetByProductByNameOrCode(string? nameCode = null, int pageNumber = 1, int pageSize = 100)
        {
            var query = dbContext.Products
                                 .Where(x => x.Status == "active" || x.Status == "Dừng bán")
                                 .Include(x => x.Categories)
                                 .Include(x => x.GoldTypes)
                                 .Include(x => x.Stones)
                                 .AsQueryable();

            if (!string.IsNullOrEmpty(nameCode))
            {
                string normalizedCode = nameCode.ToUpper();
                query = query.Where(x => x.ProductCode.ToUpper().Contains(normalizedCode) ||
                                         x.ProductName.ToUpper().Contains(normalizedCode));
            }

            int skip = (pageNumber - 1) * pageSize;
            return await query.Skip(skip).Take(pageSize).ToListAsync();
        }


        public async Task<Product?> DeleteProductAsync(int id)
        {
            var checkExistProduct = await dbContext.Products.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.ProductId == id);
            if (checkExistProduct == null) { return null; }

            //lấy danh sách các OrderId của các Order có status 3456
            var list3456Order = await dbContext.Orders
                    .Where(x => x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán")
                    .Select(x => x.OrderId)
                    .ToListAsync();

            //Lấy danh sách các ProductId của các OrderDetail có OrderID thuộc list 3456Order
            var listProductIdIn3456Order = await dbContext.OrderDetails
                            .Where(x => list3456Order.Contains(x.OrderId))
                            .Select(x => x.ProductId)
                            .Distinct()
                            .ToListAsync();
            //check xem product được chọn có trong list listProductIdIn3456Order hay không
            bool check = listProductIdIn3456Order.Contains(checkExistProduct.ProductId);

            //Nếu có => "đã từng bán" => Dừng bán
            if (check)
            {
                checkExistProduct.Status = "Dừng bán";
            }
            //còn chưa từng thì set "inactive"
            else
            {
                checkExistProduct.Status = "inactive";
            }
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
            existingProduct.ProductCode = product.ProductCode;
            existingProduct.ProductName = product.ProductName;
            existingProduct.Description = product.Description;
            existingProduct.Image = product.Image;
            existingProduct.Quantity = product.Quantity;
            existingProduct.GoldWeight = product.GoldWeight;
            existingProduct.Wage = product.Wage;
            existingProduct.Price = product.Price;
            existingProduct.Size = product.Size;
            existingProduct.WarrantyPeriod = product.WarrantyPeriod;
            existingProduct.Status = product.Status;
            await dbContext.SaveChangesAsync();
            return existingProduct;
        }
        public async Task<Product?> UpdateProductByTickOne(int id, Product product)
        {
            var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            if (existingProduct == null) { return null; }
            existingProduct.Price = product.Price;

            await dbContext.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<List<Product>?> UpdateProductByTickALL()
        {
            var existingProducts = await dbContext.Products.ToListAsync();
            if (existingProducts == null || existingProducts.Count == 0) { return null; }

            foreach (var existingProduct in existingProducts)
            {
                double sellPrice = await GetSellPriceByProductIdAsync(existingProduct.ProductId);
                double goldWeight = existingProduct.GoldWeight;
                double wage = existingProduct.Wage;
                double stonePrice = await GetStonePriceByProductIdAsync(existingProduct.ProductId);
                existingProduct.Price = ((goldWeight * sellPrice) + wage + stonePrice);
            }

            await dbContext.SaveChangesAsync();
            return existingProducts;
        }

        public async Task<Product?> UpdateProductNormal(int id, Product product)
        {
            var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            if (existingProduct == null) { return null; }
            existingProduct.ProductName = product.ProductName;
            existingProduct.Description = product.Description;
            existingProduct.Image = product.Image;
            existingProduct.Quantity = product.Quantity;
            existingProduct.GoldWeight = product.GoldWeight;
            existingProduct.Wage = product.Wage;
            existingProduct.Price = product.Price;
            existingProduct.Size = product.Size;
            existingProduct.Status = product.Status;

            await dbContext.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<Product?> UpdateProductPriceWithStone(int id, Product product)
        {

            var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            if (existingProduct == null) { return null; }

            existingProduct.Price = product.Price;

            await dbContext.SaveChangesAsync();
            return existingProduct;
        }


        public async Task<List<Product>?> GetOutOfStockProductsAsync()
        {
            var product = await dbContext.Products.Where(x => x.Quantity == 0).ToListAsync();
            return product;
        }

        public async Task<List<Product>?> GetTopBestSellerProductsAsync(int topNum)
        {
            var orderDetails = await dbContext.OrderDetails.ToListAsync();
            Dictionary<int, int> sellQuantityAndProduct = new Dictionary<int, int>();
            foreach (var order in orderDetails)
            {
                if (sellQuantityAndProduct.ContainsKey(order.ProductId))
                {
                    sellQuantityAndProduct[order.ProductId] += order.Quantity;
                }
                else
                {
                    sellQuantityAndProduct.Add(order.ProductId, order.Quantity);
                }
            }

            var topSellProductDictionary = sellQuantityAndProduct.OrderByDescending(x => x.Value).Take(topNum).Select(x => x.Key).ToList();
            List<Product> topSellProductList = new List<Product>();
            foreach (var productId in topSellProductDictionary)
            {
                var product = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
                if (product != null)
                {
                    topSellProductList.Add(product);
                }
            }
            return topSellProductList;
        }

        public async Task<Product?> UpdateNotSoldProductToActiveAsync(int id)
        {
            var notSoldProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id && x.Status == "Dừng bán");
            if (notSoldProduct != null)
            {
                notSoldProduct.Status = "active";
            }
            var activeProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id && x.Status == "active");
            if (activeProduct != null)
            {
                activeProduct.Status = "Dừng bán";
            }

            await dbContext.SaveChangesAsync();
            return notSoldProduct;
        }

        public async Task<Product> GetProductByWarrantyId(int id)
        {
            return null;
        }
    }
}
