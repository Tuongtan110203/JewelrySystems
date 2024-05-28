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


        public async Task<Warranty> DeleteWarranty(int id)
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


        public async Task<List<Warranty>> GetAllWarranty()
        {
            return await dbContext.Warranties.Where(x => x.Status == "Đã hoàn thành").ToListAsync();

        }

        public async Task<Warranty> GetWarrantyById(int id)
        {
            return await dbContext.Warranties.Where(x => x.Status == "Đã hoàn thành").FirstOrDefaultAsync(x => x.WarrantyId == id);
        }

        public async Task<Warranty> UpdateWarranty(int id, Warranty warranty)
        {
            var Checkexist = await dbContext.Warranties.FirstOrDefaultAsync(x => x.WarrantyId == id);
            if (Checkexist == null) return null;
            Checkexist.CustomerId = warranty.CustomerId;
            Checkexist.OrderDetailId = warranty.OrderDetailId;
            Checkexist.StartDate = warranty.StartDate;
            Checkexist.EndDate = warranty.EndDate;
            Checkexist.Status = warranty.Status;
            await dbContext.SaveChangesAsync();
            return Checkexist;
        }
    }
}
