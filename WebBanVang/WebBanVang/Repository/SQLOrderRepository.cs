using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public class SQLOrderRepository : IOrderRepository
    {
        private readonly JewelrySalesSystemDbContext dbContext;

        public SQLOrderRepository(JewelrySalesSystemDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Orders> AddOrders(Orders orders)
        {
            await dbContext.Orders.AddAsync(orders);
            await dbContext.SaveChangesAsync();
            return orders;
        }

        public async Task<Orders> DeleteOrders(int id)
        {
            var checkExist = await dbContext.Orders
                .Include(x => x.Customers)
                .FirstOrDefaultAsync(x => x.OrderId == id);

            if (checkExist == null)
            {
                return null;
            }
            checkExist.Status = "Hủy thanh toán";
            await dbContext.SaveChangesAsync();

            var warranties = await dbContext.Warranties
                .Where(w => w.CustomerId == checkExist.CustomerId && w.Status == "Đã hoàn thành")
                .ToListAsync();

            foreach (var warranty in warranties)
            {
                warranty.Status = "Chưa hoàn thành";
            }
            await dbContext.SaveChangesAsync();

            var payment = await dbContext.Payments
               .Where(w => w.OrderId == checkExist.OrderId && w.Status == "Đã thanh toán")
               .ToListAsync();

            foreach (var warranty in payment)
            {
                warranty.Status = "Chưa thanh toán";
            }
            await dbContext.SaveChangesAsync();
            return checkExist;
        }


        public async Task<List<Orders>> GetAllOrders()
        {
            return await dbContext.Orders.Include(x => x.Users).Include(x => x.Customers).ToListAsync();
        }

        public async Task<Orders?> GetOrdersById(int id)
        {
            var checkExist = await dbContext.Orders
                .Include(x => x.Users)
                .Include(x => x.Customers)
                .FirstOrDefaultAsync(x => x.OrderId == id);

            return checkExist;
        }

        public async Task<List<Orders>> GetPaidOrdersAsync()
        {
            return await dbContext.Orders.Where(x => x.Status == "Đã thanh toán")
                .Include(x => x.Users)
                .Include(x => x.Customers)
                .ToListAsync();
        }

        public async Task<List<Orders>> GetDoneOrdersAsync()
        {
            return await dbContext.Orders.Where(x => x.Status == "Đã hoàn thành")
                .Include(x => x.Users)
                .Include(x => x.Customers)
                .ToListAsync();
        }

        public async Task<List<Orders>> GetWaitingOrdersAsync()
        {
            return await dbContext.Orders.Where(x => x.Status == "Đợi thanh toán")
                .Include(x => x.Users)
                .Include(x => x.Customers)
                .ToListAsync();
        }

        public async Task<Orders> UpdateGOrders(int id, Orders orders)
        {
            var Checkexist = await dbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == id);
            if (Checkexist == null) return null;
            Checkexist.UserName = orders.UserName;
            Checkexist.CustomerId = orders.CustomerId;
            Checkexist.OrderDate = orders.OrderDate;
            Checkexist.Total = orders.Total;
            Checkexist.SaleById = orders.SaleById;
            Checkexist.CashierId = orders.CashierId;
            Checkexist.ServicerId = orders.ServicerId;
            Checkexist.Status = orders.Status;
            await dbContext.SaveChangesAsync();
            return Checkexist;
        }

        public async Task<List<OrderDetails>> GetNumberOfOrderDetailsAsync(int orderId)
        {
            return await dbContext.OrderDetails
                .Include(x => x.Orders.Customers)
                .Include(x => x.Orders.Users)
                .Include(x => x.Orders.Users.Roles)
               .Include(od => od.Products)
                .Include(od => od.Orders)
                .Include(od => od.Products.Categories)
                .Include(od => od.Products.GoldTypes)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

        }


    }
}
