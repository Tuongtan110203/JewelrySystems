using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public class SQLWarrantyRepository : IWarrantyRepository
    {
        private readonly JewelrySalesSystemDbContext dbContext;

        public SQLWarrantyRepository(JewelrySalesSystemDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
       

        public async Task<Warranty?> DeleteWarranty(int id)
        {
            var checkExist = await dbContext.Warranties
                                            .Include(w => w.OrderDetails)
                                            .FirstOrDefaultAsync(x => x.WarrantyId == id);

            if (checkExist == null)
            {
                return null;
            }

            checkExist.Status = "Chưa hoàn thành";
            await dbContext.SaveChangesAsync();

            if (checkExist.OrderDetails == null)
            {
                return checkExist;
            }
            var order = await dbContext.Orders
                .Where(w => w.OrderId == checkExist.OrderDetails.OrderId && w.Status == "Đã hoàn thành")
                .ToListAsync();

            foreach (var orders in order)
            {
                orders.Status = "Đã thanh toán";
            }

            await dbContext.SaveChangesAsync();
            return checkExist;
        }


        public async Task<string> GenerateWarrantyCodeAsync()
        {
            var maxWarrantyCode = await dbContext.Warranties
                .OrderByDescending(p => p.WarrantyCode)
                .Select(p => p.WarrantyCode)
                .FirstOrDefaultAsync();

            int nextWarrantyCodeNumber = 1;

            if (!string.IsNullOrEmpty(maxWarrantyCode) && maxWarrantyCode.StartsWith("PBH"))
            {
                if (int.TryParse(maxWarrantyCode.Substring(3), out int currentMax))
                {
                    nextWarrantyCodeNumber = currentMax + 1;
                }
            }

            return $"PBH{nextWarrantyCodeNumber:D4}";
        }



        public async Task<List<Warranty>> GetAllWarranty()
        {
            return await dbContext.Warranties
                .Include(x => x.OrderDetails)
             .Include(x => x.Customers)
                .Include(x => x.OrderDetails.Orders)
                 .Include(x => x.OrderDetails.Orders.Customers)
                 .Include(x => x.OrderDetails.Orders.Users)
                .Include(x => x.OrderDetails.Products)
                 .Include(x => x.OrderDetails.Products.Stones)
                .Include(x => x.OrderDetails.Products.GoldTypes)
                .ToListAsync();

        }

        public async Task<List<Warranty>> GetOutOfDateWarrantyAsync()
        {
            return await dbContext.Warranties.Where(x => x.EndDate < DateTime.Now.Date).ToListAsync();
        }

        public async Task<List<Warranty>> GetWarrantiesByIdAsync(int id)
        {
            return await dbContext.Warranties
                      .Include(x => x.OrderDetails)
                    .Include(x => x.Customers)
               .Include(x => x.OrderDetails.Orders)
                .Include(x => x.OrderDetails.Orders.Customers)
                .Include(x => x.OrderDetails.Orders.Users)
               .Include(x => x.OrderDetails.Products)
                .Include(x => x.OrderDetails.Products.Stones)
               .Include(x => x.OrderDetails.Products.GoldTypes)
               .Where(x => x.WarrantyId.ToString().Contains(id.ToString())).ToListAsync();
        }

        public async Task<Warranty?> GetWarrantyById(int id)
        {
            return await dbContext.Warranties
                       .Include(x => x.OrderDetails)
                     .Include(x => x.Customers)
                .Include(x => x.OrderDetails.Orders)
                 .Include(x => x.OrderDetails.Orders.Customers)
                 .Include(x => x.OrderDetails.Orders.Users)
                .Include(x => x.OrderDetails.Products)
                 .Include(x => x.OrderDetails.Products.Stones)
                .Include(x => x.OrderDetails.Products.GoldTypes)
                .FirstOrDefaultAsync(x => x.WarrantyId == id);
        }

        public async Task<Warranty?> UpdateWarranty(int id, Warranty warranty)
        {
            var Checkexist = await dbContext.Warranties.FirstOrDefaultAsync(x => x.WarrantyId == id);
            if (Checkexist == null) return null;
            Checkexist.WarrantyCode = warranty.WarrantyCode;
            Checkexist.CustomerId = warranty.CustomerId;
            Checkexist.OrderDetailId = warranty.OrderDetailId;
            Checkexist.StartDate = warranty.StartDate;
            Checkexist.EndDate = warranty.EndDate;
            Checkexist.Status = warranty.Status;
            await dbContext.SaveChangesAsync();
            return Checkexist;
        }
        public async Task<List<Warranty>> GetWarrantyByCodeAsync(string warrantyCode)
        {
            return await dbContext.Warranties
                .Include(x => x.OrderDetails)
                .Include(x => x.Customers)
                .Include(x => x.OrderDetails.Orders)
                .Include(x => x.OrderDetails.Orders.Customers)
                .Include(x => x.OrderDetails.Orders.Users)
                .Include(x => x.OrderDetails.Products)
                .Include(x => x.OrderDetails.Products.Stones)
                .Include(x => x.OrderDetails.Products.GoldTypes)
                .Where(x => x.WarrantyCode.Contains(warrantyCode)).ToListAsync();
        }

        public async Task<List<Warranty>> GetWarrantyByProductCode(string productCode)
        {
            return await dbContext.Warranties
           .Where(x => x.OrderDetails.Products.ProductCode.Contains(productCode))
                           .ToListAsync();
        }
    }
}
