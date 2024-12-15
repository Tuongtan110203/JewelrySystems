using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;

namespace WebBanVang.Repository
{
    public class SQLGoldTypeRepository : IGoldTypeRepository
    {
        private readonly JewelrySalesSystemDbContext dbContext;

        public SQLGoldTypeRepository(JewelrySalesSystemDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<GoldType> AddGoldType(GoldType goldType)
        {
            await dbContext.GoldTypes.AddAsync(goldType);
            await dbContext.SaveChangesAsync();
            return goldType;
        }

        public async Task<GoldType?> DeleteGoldType(int id)
        {
            var checkExist = await dbContext.GoldTypes.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.GoldId == id);
            if (checkExist == null) { return null; }
            var productExistsInOrders = await dbContext.OrderDetails
        .Join(dbContext.Products,
              od => od.ProductId,
              p => p.ProductId,
              (od, p) => new { od, p })
        .AnyAsync(joined => joined.p.GoldId == checkExist.GoldId &&
                            dbContext.Orders
                                .Where(o => o.Status == "Đang thanh toán" ||
                                            o.Status == "Đã thanh toán" ||
                                            o.Status == "Đã hoàn thành" ||
                                            o.Status == "Hết hạn thanh toán")
                                .Select(o => o.OrderId)
                                .Contains(joined.od.OrderId));

            if (productExistsInOrders)
            {
                throw new InvalidOperationException("Loại vàng này vẫn còn sản phẩm. Không thể xoá");
            }
            checkExist.Status = "inactive";
            await dbContext.SaveChangesAsync();
            return checkExist;
        }

        public async Task<List<GoldType>> GetAllGoldType()
        {
            return await dbContext.GoldTypes.Where(x => x.Status == "active").ToListAsync();
        }

        public async Task<GoldType?> GetGoldTypeById(int id)
        {
            return await dbContext.GoldTypes.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.GoldId == id);
        }

        public async Task<List<GoldType>> GetGoldTypeByNameAsync(string name)
        {
            return await dbContext.GoldTypes.Where(x => x.Status == "active").Where(x => x.GoldName.Contains(name)).ToListAsync();
        }

        public async Task<GoldType?> UpdateGoldTYpe(int id, GoldType goldType)
        {
            var Checkexist = await dbContext.GoldTypes.FirstOrDefaultAsync(x => x.GoldId == id);
            if (Checkexist == null) return null;
            Checkexist.GoldCode = goldType.GoldCode;
            Checkexist.GoldName = goldType.GoldName;
            Checkexist.BuyPrice = goldType.BuyPrice;
            Checkexist.SellPrice = goldType.SellPrice;
            Checkexist.UpdateTime = goldType.UpdateTime;
            Checkexist.Status = goldType.Status;
            await dbContext.SaveChangesAsync();
            return Checkexist;
        }

        public async Task<List<GoldTypePercentageDTO>> CalculateGoldCodePercentagesAsync(DateTime startDate, DateTime endDate)
        {
            // Retrieve orders with the specified statuses and within the date range
            var orders = await dbContext.Orders
                .Where(o => (o.Status == "Đã thanh toán" || o.Status == "Đã hoàn thành") &&
                            o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Products)
                    .ThenInclude(p => p.GoldTypes)
                .ToListAsync();

            // Flatten the order details
            var orderDetails = orders.SelectMany(o => o.OrderDetails);

            // Group by GoldCode and calculate the total price for each group
            var goldCodeGroups = orderDetails
                .GroupBy(od => new { od.Products.GoldTypes.GoldCode, od.Products.GoldTypes.GoldName })
                .Select(g => new
                {
                    g.Key.GoldCode,
                    g.Key.GoldName,
                    TotalPrice = g.Sum(od => od.Price * od.Quantity)
                })
                .ToList();


            // Calculate the overall total price for all qualifying OrderDetails
            var overallTotalPrice = goldCodeGroups.Sum(g => g.TotalPrice);

            // Calculate the percentage for each GoldCode
            var goldCodePercentages = goldCodeGroups
                .Select(g => new GoldTypePercentageDTO
                {
                    GoldCode = g.GoldCode,
                    GoldName = g.GoldName,
                    TotalPrice = g.TotalPrice,
                    Percentage = overallTotalPrice == 0 ? 0 : (g.TotalPrice / overallTotalPrice) * 100
                })
                .ToList();

            return goldCodePercentages;
        }

        public async Task<List<GoldTypePercentageDTO>> GetGoldCodePercentagesForToday()
        {
            var today = DateTime.Today;
            return await CalculateGoldCodePercentagesAsync(today, today.AddDays(1).AddTicks(-1));
        }

        public async Task<List<GoldTypePercentageDTO>> GetGoldCodePercentagesForThisWeek()
        {
            var today = DateTime.Now;
            var currentDayOfWeek = (int)today.DayOfWeek;
            currentDayOfWeek = (currentDayOfWeek == 0) ? 7 : currentDayOfWeek;
            var startOfWeek = today.AddDays(-currentDayOfWeek + 1).Date;
            var endOfWeek = startOfWeek.AddDays(7).Date;
            return await CalculateGoldCodePercentagesAsync(startOfWeek, endOfWeek);

        }

        public async Task<List<GoldTypePercentageDTO>> GetGoldCodePercentagesForThisMonth()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);
            return await CalculateGoldCodePercentagesAsync(startOfMonth, endOfMonth);
        }

        public async Task<List<GoldTypePercentageDTO>> GetGoldCodePercentagesForThisYear()
        {
            var today = DateTime.Today;
            var startOfYear = new DateTime(today.Year, 1, 1);
            var endOfYear = startOfYear.AddYears(1).AddTicks(-1);
            return await CalculateGoldCodePercentagesAsync(startOfYear, endOfYear);
        }
    }
}
