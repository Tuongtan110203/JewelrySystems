using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;

namespace WebBanVang.Repository
{
    public class SQLOrderRepository : IOrderRepository
    {
        private readonly JewelrySalesSystemDbContext dbContext;
        private readonly IMapper mapper;
        public SQLOrderRepository(JewelrySalesSystemDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        //public async Task<Orders> AddOrders(Orders orders)
        //{
        //    await dbContext.Orders.AddAsync(orders);
        //    await dbContext.SaveChangesAsync();
        //    return orders;
        //}
        public async Task<Orders> AddOrders(Orders orders)
        {

            orders.OrderCode = await GenerateOrderCodeAsync();

            await dbContext.Orders.AddAsync(orders);
            await dbContext.SaveChangesAsync();
            return orders;
        }
        public async Task<List<Orders>> GetOrdersByOrderCode(string code)
        {
            return await dbContext.Orders.Where(x => x.OrderCode.Contains(code)).ToListAsync();
        }
        public async Task<string> GenerateOrderCodeAsync()
        {
            var maxOderCode = await dbContext.Orders
                .OrderByDescending(p => p.OrderCode)
                .Select(p => p.OrderCode)
                .FirstOrDefaultAsync();

            int nextProductCodeNumber = 1;

            if (!string.IsNullOrEmpty(maxOderCode) && maxOderCode.StartsWith("MDH"))
            {
                if (int.TryParse(maxOderCode.Substring(3), out int currentMax))
                {
                    nextProductCodeNumber = currentMax + 1;
                }
            }

            return $"MDH{nextProductCodeNumber:D6}";
        }
        public async Task<Orders?> DeleteOrders(int id)
        {
            var checkExist = await dbContext.Orders
                .Include(x => x.Customers)
                .FirstOrDefaultAsync(x => x.OrderId == id);

            if (checkExist == null)
            {
                return null;
            }
            if (checkExist.Status == "Đợi thanh toán")
            {
                //tìm order bằng id. Nếu tồn tại thì hủy thanh toán
                checkExist.Status = "Hủy thanh toán";
                //cập nhật lại quantiy ở bảng Product

                var orderDetails = await dbContext.OrderDetails.Where(od => od.OrderId == id)
                    .Include(od => od.Products).ToListAsync();
                foreach (var orderDetail in orderDetails)
                {
                    var product = orderDetail.Products;
                    if (product != null)
                    {
                        product.Quantity += orderDetail.Quantity;
                    }
                }
            }

            await dbContext.SaveChangesAsync();
            return checkExist;
        }



        public async Task<List<Orders>> GetAllOrders(int pageNumber = 1, int pageSize = 100)
        {
            var order = dbContext.Orders
                         .Include(x => x.Users)
                         .Include(x => x.Customers)
                         .Include(x => x.Payments)
                         .AsQueryable();
            var SkipResult = (pageNumber - 1) * pageSize;
            return await order.Skip(SkipResult).Take(pageSize).ToListAsync();
        }

        public async Task<int> GetNumberOfOrdersAsync()
        {
            var order = await dbContext.Orders.ToListAsync();
            return order.Count;
        }
        public async Task<int> GetNumberOfOrderDetailsAsync(int orderId)
        {
            return await dbContext.OrderDetails
                .Where(x => x.OrderId == orderId)
                .CountAsync();
        }

        //public async Task<Orders?> GetOrdersById(int id)
        //{
        //    var checkExist = await dbContext.Orders
        //        .Include(x => x.Users)
        //        .Include(x => x.Customers)
        //        .Include(x => x.Users.Roles)
        //        .FirstOrDefaultAsync(x => x.OrderId == id);

        //    return checkExist;
        //}




        public async Task<List<Orders>> GetAllOrder1s(int pageNumber = 1, int pageSize = 100)
        {
            var order = dbContext.Orders
                         .Include(x => x.Users)
                         .Include(x => x.Customers)
                         .AsQueryable();
            var SkipResult = (pageNumber - 1) * pageSize;
            return await order.Skip(SkipResult).Take(pageSize).ToListAsync();
        }

        public async Task<List<Orders>> GetPaidOrdersAsync(int pageNumber = 1, int pageSize = 100,
            string? sortBy = null, bool isAscending = true)
        {
            var order = dbContext.Orders.Where(x => x.Status == "Đã thanh toán")
                .Include(x => x.Users)
                .Include(x => x.Payments)
                .Include(x => x.Customers)
                .AsQueryable();



            if (sortBy != null)
            {
                if (sortBy.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    order = isAscending ? order.OrderBy(x => x.OrderDate) : order.OrderByDescending(x => x.OrderDate);
                }
            }
            else
            {
                order = order.OrderByDescending(x => x.OrderDate);
            }

            var skipResult = (pageNumber - 1) * pageSize;
            return await order.Skip(skipResult).Take(pageSize).ToListAsync();
        }


        public async Task<List<Orders>> GetDoneOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null,
     bool isAscending = true)
        {
            var order = dbContext.Orders.Where(x => x.Status == "Đã hoàn thành")
                 .Include(x => x.Users)
                 .Include(x => x.Payments)
                 .Include(x => x.Customers)
                 .AsQueryable();
            if (sortBy != null)
            {
                if (sortBy.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    order = isAscending ? order.OrderBy(x => x.OrderDate) : order.OrderByDescending(x => x.OrderDate);
                }
            }
            else
            {
                order = order.OrderByDescending(x => x.OrderDate);
            }

            var SkipResult = (pageNumber - 1) * pageSize;
            return await order.Skip(SkipResult).Take(pageSize).ToListAsync();
        }

        public async Task<List<Orders>> GetWaitingOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null, bool isAscending = true)
        {
            var order = dbContext.Orders.Where(x => x.Status == "Đợi thanh toán")
                .Include(x => x.Users)
                .Include(x => x.Payments)
                .Include(x => x.Customers)
                .AsQueryable();
            if (sortBy != null)
            {
                if (sortBy.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    order = isAscending ? order.OrderBy(x => x.OrderDate) : order.OrderByDescending(x => x.OrderDate);
                }
            }
            else
            {
                order = order.OrderByDescending(x => x.OrderDate);
            }
            var SkipResult = (pageNumber - 1) * pageSize;
            return await order.Skip(SkipResult).Take(pageSize).ToListAsync();
        }

        public async Task<List<Orders>> GetPayingOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null, bool isAscending = true)
        {
            var order = dbContext.Orders.Where(x => x.Status == "Đang thanh toán")
                .Include(x => x.Users)
                .Include(x => x.Payments)
                .Include(x => x.Customers)
                .AsQueryable();
            if (sortBy != null)
            {
                if (sortBy.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    order = isAscending ? order.OrderBy(x => x.OrderDate) : order.OrderByDescending(x => x.OrderDate);
                }
            }
            else
            {
                order = order.OrderByDescending(x => x.OrderDate);
            }
            var SkipResult = (pageNumber - 1) * pageSize;
            return await order.Skip(SkipResult).Take(pageSize).ToListAsync();
        }

        public async Task<List<Orders>> GetCancelOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null, bool isAscending = true)
        {
            var order = dbContext.Orders.Where(x => x.Status == "Hủy thanh toán")
                .Include(x => x.Users)
                .Include(x => x.Payments)
                .Include(x => x.Customers)
                .AsQueryable();
            if (sortBy != null)
            {
                if (sortBy.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    order = isAscending ? order.OrderBy(x => x.OrderDate) : order.OrderByDescending(x => x.OrderDate);
                }
            }
            else
            {
                order = order.OrderByDescending(x => x.OrderDate);
            }
            var SkipResult = (pageNumber - 1) * pageSize;
            return await order.Skip(SkipResult).Take(pageSize).ToListAsync();
        }

        public async Task<List<Orders>> GetExpiredOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null, bool isAscending = true)
        {
            var order = dbContext.Orders.Where(x => x.Status == "Hết hạn thanh toán")
                .Include(x => x.Users)
                .Include(x => x.Payments)
                .Include(x => x.Customers)
                .AsQueryable();
            if (sortBy != null)
            {
                if (sortBy.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    order = isAscending ? order.OrderBy(x => x.OrderDate) : order.OrderByDescending(x => x.OrderDate);
                }
            }
            else
            {
                order = order.OrderByDescending(x => x.OrderDate);
            }
            var SkipResult = (pageNumber - 1) * pageSize;
            return await order.Skip(SkipResult).Take(pageSize).ToListAsync();
        }

        public async Task<List<Orders>> GetWaitOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null, bool isAscending = true)
        {
            var order = dbContext.Orders.Where(x => x.Status == "Đợi thanh toán" || x.Status == "Đang thanh toán")
                .Include(x => x.Users)
                .Include(x => x.Payments)
                .Include(x => x.Customers)
                .AsQueryable();
            if (sortBy != null)
            {
                if (sortBy.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    order = isAscending ? order.OrderBy(x => x.OrderDate) : order.OrderByDescending(x => x.OrderDate);
                }
            }
            else
            {
                order = order.OrderByDescending(x => x.OrderDate);
            }
            var SkipResult = (pageNumber - 1) * pageSize;
            return await order.Skip(SkipResult).Take(pageSize).ToListAsync();
        }

        public async Task<List<Orders>> GetStatusOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null, bool isAscending = true)
        {
            var order = dbContext.Orders.Include(x => x.Users)
                .Include(x => x.Customers)
                .Include(x => x.Payments)
                .AsQueryable();
            if (sortBy != null)
            {
                if (sortBy.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    order = isAscending ? order.OrderBy(x => x.OrderDate) : order.OrderByDescending(x => x.OrderDate);
                }
            }
            else
            {
                order = order.OrderByDescending(x => x.OrderDate);
            }
            var SkipResult = (pageNumber - 1) * pageSize;
            return await order.Skip(SkipResult).Take(pageSize).ToListAsync();
        }
        public async Task<int> GetNumberPaidOrdersAsync(int pageNumber = 1, int pageSize = 100)
        {
            int count = 0;
            var order = await GetPaidOrdersAsync(pageNumber, pageSize);
            foreach (var item in order)
            {
                count += 1;
            }
            return count;
        }

        public async Task<int> GetNumberDoneOrdersAsync(int pageNumber = 1, int pageSize = 100)
        {
            int count = 0;
            var order = await GetDoneOrdersAsync(pageNumber, pageSize);
            foreach (var item in order)
            {
                count += 1;
            }
            return count;
        }

        public async Task<int> GetNumberWaitingOrdersAsync(int pageNumber = 1, int pageSize = 100)
        {
            int count = 0;
            var order = await GetWaitingOrdersAsync(pageNumber, pageSize);
            foreach (var item in order)
            {
                count += 1;
            }
            return count;
        }

        public async Task<int> GetNumberCancelOrdersAsync(int pageNumber = 1, int pageSize = 100)
        {
            int count = 0;
            var order = await GetCancelOrdersAsync(pageNumber, pageSize);
            foreach (var item in order)
            {
                count += 1;
            }
            return count;
        }
        public async Task<int> GetNumberPayingOrdersAsync(int pageNumber = 1, int pageSize = 100)
        {
            int count = 0;
            var order = await GetPayingOrdersAsync(pageNumber, pageSize);
            foreach (var item in order)
            {
                count += 1;
            }
            return count;
        }
        public async Task<int> GetNumberExpiredOrdersAsync(int pageNumber = 1, int pageSize = 100)
        {
            int count = 0;
            var order = await GetExpiredOrdersAsync(pageNumber, pageSize);
            foreach (var item in order)
            {
                count += 1;
            }
            return count;
        }

        public async Task<int> GetNumberWaitOrdersAsync(int pageNumber = 1, int pageSize = 100)
        {
            int count = 0;
            var order = await GetWaitOrdersAsync(pageNumber, pageSize);
            foreach (var item in order)
            {
                count += 1;
            }
            return count;
        }

        public async Task<int> GetNumberStatusOrdersAsync(int pageNumber = 1, int pageSize = 100)
        {
            int count = 0;
            var order = await GetStatusOrdersAsync(pageNumber, pageSize);
            foreach (var item in order)
            {
                count += 1;
            }
            return count;
        }



        public async Task<Orders?> UpdateGOrders(int id, Orders orders)
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
            Checkexist.CustomerName = orders.CustomerName;
            Checkexist.PhoneNumber = orders.PhoneNumber;
            Checkexist.Email = orders.Email;
            Checkexist.Status = orders.Status;
            await dbContext.SaveChangesAsync();
            return Checkexist;
        }

        public async Task<Orders?> GetOrdersById(int id)
        {
            var order = await dbContext.Orders
                .Include(o => o.Customers)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order != null)
            {
                var saleUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == order.SaleById);
                var cashierUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == order.CashierId);
                var servicesUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == order.ServicerId);

            }

            return order;
        }
        public async Task<List<OrderDetails>> GetOrderDetailsAsync(int orderId)
        {
            var orderDetails = await dbContext.OrderDetails
                .AsNoTracking()
                .Include(od => od.Products)
                    .ThenInclude(p => p.Stones)
                .Include(od => od.Orders)
                    .ThenInclude(o => o.Customers)
                .Include(od => od.Orders)
                    .ThenInclude(o => o.Users)
                    .ThenInclude(u => u.Roles)
                .Include(od => od.Products.Categories)
                .Include(od => od.Products.GoldTypes)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            return orderDetails;
        }

        public async Task<List<Payment>> GetTodayPaymentsAsync()
        {
            return await dbContext.Payments.Where(x => x.Orders.OrderDate.Date == DateTime.Now.Date
            && (x.Orders.Status == "Đang thanh toán" || x.Orders.Status == "Hết hạn thanh toán")).ToListAsync();
            // return await dbContext.Payments.Where(x => x.Orders.OrderDate.Date == DateTime.Now.Date).ToListAsync();
        }
        public async Task<List<Payment>> GetThisWeekPaymentsAsync()
        {
            var today = DateTime.Now;
            var currentDayOfWeek = (int)today.DayOfWeek;
            currentDayOfWeek = (currentDayOfWeek == 0) ? 7 : currentDayOfWeek;
            var startOfWeek = today.AddDays(-currentDayOfWeek + 1).Date;
            var endOfWeek = startOfWeek.AddDays(7).Date;

            var paymentsForThisWeek = await dbContext.Payments
                .Where(p => p.Orders.OrderDate >= startOfWeek && p.Orders.OrderDate < endOfWeek && (p.Orders.Status == "Đang thanh toán" || p.Orders.Status == "Hết hạn thanh toán"))
                .Include(p => p.Orders.Users)
                .Include(p => p.Orders.Customers)
                .ToListAsync();

            return paymentsForThisWeek;
        }

        public async Task<List<Payment>> GetThisMonthPaymentsAsync()
        {
            var today = DateTime.Now;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            var paymentsForThisMonth = await dbContext.Payments.Where(x => x.Orders.OrderDate >= startOfMonth && x.Orders.OrderDate < startOfNextMonth
            && (x.Orders.Status == "Đang thanh toán" || x.Orders.Status == "Hết hạn thanh toán")).ToListAsync();
            return paymentsForThisMonth;
        }

        public async Task<List<Payment>> GetThisYearPaymentsAsync()
        {
            var today = DateTime.Now;
            var startOfYear = new DateTime(today.Year, 1, 1);
            var startOfNextYear = startOfYear.AddYears(1);

            var paymentsForThisYear = await dbContext.Payments.Where(x => x.Orders.OrderDate >= startOfYear && x.Orders.OrderDate < startOfNextYear
            && (x.Orders.Status == "Đang thanh toán" || x.Orders.Status == "Hết hạn thanh toán")).ToListAsync();
            return paymentsForThisYear;
        }

        public async Task<List<Payment>> GetDailyPaymentsAsync(DateTime date)
        {
            return await dbContext.Payments.Where(x => x.Orders.OrderDate.Date == date.Date
            && (x.Orders.Status == "Đang thanh toán" || x.Orders.Status == "Hết hạn thanh toán")).ToListAsync();
        }

        public async Task<List<Payment>> GetMonthlyPaymentsAsync(int month, int year)
        {

            var paymentsForThisMonth = await dbContext.Payments.Where(x => x.Orders.OrderDate.Month == month && x.Orders.OrderDate.Year == year
            && (x.Orders.Status == "Đang thanh toán" || x.Orders.Status == "Hết hạn thanh toán")).ToListAsync();
            return paymentsForThisMonth;
        }

        public async Task<List<Payment>> GetYearlyPaymentsAsync(int year)
        {

            var paymentsForThisYear = await dbContext.Payments.Where(x => x.Orders.OrderDate.Year == year
            && (x.Orders.Status == "Đang thanh toán" || x.Orders.Status == "Hết hạn thanh toán")).ToListAsync();
            return paymentsForThisYear;
        }

        public async Task<List<Orders>> GetTodayOrdersAsync()
        {
            return await dbContext.Orders.Where(x => x.OrderDate.Date == DateTime.Now.Date).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
        }

        public async Task<List<Orders>> GetThisWeekOrdersAsync()
        {
            var today = DateTime.Now;
            var currentDayOfWeek = (int)today.DayOfWeek;
            currentDayOfWeek = (currentDayOfWeek == 0) ? 7 : currentDayOfWeek;
            var startOfWeek = today.AddDays(-currentDayOfWeek + 1).Date;
            var endOfWeek = startOfWeek.AddDays(7).Date;

            var ordersForThisWeek = await dbContext.Orders.Where(x => x.OrderDate >= startOfWeek && x.OrderDate < endOfWeek).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
            return ordersForThisWeek;
        }

        public async Task<List<Orders>> GetThisMonthOrdersAsync()
        {
            var today = DateTime.Now;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            var ordersForThisMonth = await dbContext.Orders.Where(x => x.OrderDate >= startOfMonth && x.OrderDate < startOfNextMonth).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
            return ordersForThisMonth;
        }

        public async Task<List<Orders>> GetThisYearOrdersAsync()
        {
            var today = DateTime.Now;
            var startOfYear = new DateTime(today.Year, 1, 1);
            var startOfNextYear = startOfYear.AddYears(1);

            var ordersForThisYear = await dbContext.Orders.Where(x => x.OrderDate >= startOfYear && x.OrderDate < startOfNextYear).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
            return ordersForThisYear;
        }
        public async Task<List<Orders>> GetTodayOrdersTotalAsync()
        {
            return await dbContext.Orders.Where(x => x.OrderDate.Date == DateTime.Now.Date
            && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán")).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
        }

        public async Task<List<Orders>> GetThisWeekOrdersTotalAsync()
        {
            var today = DateTime.Now;
            var currentDayOfWeek = (int)today.DayOfWeek;
            currentDayOfWeek = (currentDayOfWeek == 0) ? 7 : currentDayOfWeek;
            var startOfWeek = today.AddDays(-currentDayOfWeek + 1).Date;
            var endOfWeek = startOfWeek.AddDays(7).Date;

            var ordersForThisWeek = await dbContext.Orders.Where(x => x.OrderDate >= startOfWeek && x.OrderDate < endOfWeek
            && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán")).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
            return ordersForThisWeek;
        }

        public async Task<List<Orders>> GetThisMonthOrdersTotalAsync()
        {
            var today = DateTime.Now;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            var ordersForThisMonth = await dbContext.Orders.Where(x => x.OrderDate >= startOfMonth && x.OrderDate < startOfNextMonth
            && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán")).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
            return ordersForThisMonth;
        }

        public async Task<List<Orders>> GetThisYearOrdersTotalAsync()
        {
            var today = DateTime.Now;
            var startOfYear = new DateTime(today.Year, 1, 1);
            var startOfNextYear = startOfYear.AddYears(1);

            var ordersForThisYear = await dbContext.Orders.Where(x => x.OrderDate >= startOfYear && x.OrderDate < startOfNextYear
            && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán")).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
            return ordersForThisYear;
        }


        public async Task<List<OrdersDTO>> GetTodayOrdersTotalAsyncReve()
        {
            var todayOrder = await dbContext.Orders.Include(x => x.Payments).Where(x => x.OrderDate.Date == DateTime.Now.Date
            && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán")).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
            return mapper.Map<List<OrdersDTO>>(todayOrder);
        }

        public async Task<List<OrdersDTO>> GetThisWeekOrdersTotalAsyncReve()
        {
            var today = DateTime.Now;
            var currentDayOfWeek = (int)today.DayOfWeek;
            currentDayOfWeek = (currentDayOfWeek == 0) ? 7 : currentDayOfWeek;
            var startOfWeek = today.AddDays(-currentDayOfWeek + 1).Date;
            var endOfWeek = startOfWeek.AddDays(7).Date;

            var ordersForThisWeek = await dbContext.Orders.Include(x => x.Payments).Where(x => x.OrderDate >= startOfWeek && x.OrderDate < endOfWeek
            && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán")).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
            return mapper.Map<List<OrdersDTO>>(ordersForThisWeek);
            // return ordersForThisWeek;
        }

        public async Task<List<OrdersDTO>> GetThisMonthOrdersTotalAsyncReve()
        {
            var today = DateTime.Now;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            var ordersForThisMonth = await dbContext.Orders.Include(x => x.Payments).Where(x => x.OrderDate >= startOfMonth && x.OrderDate < startOfNextMonth
            && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán")).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
            return mapper.Map<List<OrdersDTO>>(ordersForThisMonth);
        }

        public async Task<List<OrdersDTO>> GetThisYearOrdersTotalAsyncReve()
        {
            var today = DateTime.Now;
            var startOfYear = new DateTime(today.Year, 1, 1);
            var startOfNextYear = startOfYear.AddYears(1);

            var ordersForThisYear = await dbContext.Orders.Include(x => x.Payments).Where(x => x.OrderDate >= startOfYear && x.OrderDate < startOfNextYear
            && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán")).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
            return mapper.Map<List<OrdersDTO>>(ordersForThisYear);
        }

        public async Task<List<Orders>> GetWeeklyOrdersAsync(int year)
        {
            var today = DateTime.Now;
            var currentDayOfWeek = (int)today.DayOfWeek;
            currentDayOfWeek = (currentDayOfWeek == 0) ? 7 : currentDayOfWeek;
            var startOfWeek = today.AddDays(-currentDayOfWeek + 1).Date;
            var endOfWeek = startOfWeek.AddDays(7).Date;


            var ordersForThisMonth = await dbContext.Orders.Where(x => x.OrderDate >= startOfWeek && x.OrderDate <= endOfWeek && x.OrderDate.Year == year
            && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán")).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
            return ordersForThisMonth;
        }
        public async Task<List<Payment>> GetWeeklyPaymentsAsync(int year)
        {
            DateTime today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7).AddTicks(-1);

            var paymentsForThisMonth = await dbContext.Payments.Where(x => x.Orders.OrderDate >= startOfWeek && x.Orders.OrderDate <= endOfWeek && x.Orders.OrderDate.Year == year
            && (x.Orders.Status == "Đang thanh toán" || x.Orders.Status == "Hết hạn thanh toán")).ToListAsync();
            return paymentsForThisMonth;
        }
        public async Task<List<Orders>> GetDailyOrdersAsync(DateTime date)
        {
            return await dbContext.Orders.Where(x => x.OrderDate.Date == date.Date
            && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán")).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
        }

        public async Task<List<Orders>> GetMonthlyOrdersAsync(int month, int year)
        {

            var ordersForThisMonth = await dbContext.Orders.Where(x => x.OrderDate.Month == month && x.OrderDate.Year == year
            && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán")).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
            return ordersForThisMonth;
        }

        public async Task<List<Orders>> GetYearlyOrdersAsync(int year)
        {
            var ordersForThisYear = await dbContext.Orders.Where(x => x.OrderDate.Year == year
            && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán")).Include(x => x.Users).Include(x => x.Customers).ToListAsync();
            return ordersForThisYear;
        }
        public async Task<int> GetNumberOfByCashOrdersAsync()
        {
            int count = 0;
            var order = await dbContext.Payments.Where(x => x.PaymentType == "Tiền mặt").ToListAsync();
            foreach (var item in order)
            {
                count += 1;
            }
            return count;
        }

        public async Task<int> GetNumberOfByBankTransferOrdersAsync()
        {
            int count = 0;
            var order = await dbContext.Payments.Where(x => x.PaymentType == "Chuyển khoản").ToListAsync();
            foreach (var item in order)
            {
                count += 1;
            }
            return count;
        }

        public async Task<int> GetNumberOfByCashAndBankTransferOrdersAsync()
        {
            int count = 0;
            var order = await dbContext.Payments.Where(x => x.PaymentType == "Both").ToListAsync();
            foreach (var item in order)
            {
                count += 1;
            }
            return count;
        }

        public async Task<double> GetTotalPriceOfTodayOrdersAsync()
        {
            var allTodayOrder = await GetTodayOrdersAsync();
            var order = allTodayOrder.Where(x => x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành").ToList();
            var allTodayPayment = await GetTodayPaymentsAsync();
            double sum = 0;
            foreach (var payment in allTodayPayment)
            {
                if (payment.BankTransfer.HasValue)
                {
                    sum += payment.BankTransfer.Value;
                }
                if (payment.Cash.HasValue)
                {
                    sum += payment.Cash.Value;
                }
            }

            foreach (var item in order)
            {
                sum += item.Total;
            }
            return sum;
        }

        public async Task<double> GetTotalPriceOfThisWeekOrdersAsync()
        {
            var allThisWeekOrder = await GetThisWeekOrdersAsync();
            var order = allThisWeekOrder.Where(x => x.Status == "Đang thanh toán" || x.Status == "Hết hạn thanh toán" || x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành");
            var allThisWeekPayment = await GetThisWeekPaymentsAsync();
            double sum = 0;
            foreach (var payment in allThisWeekPayment)
            {
                if (payment.BankTransfer.HasValue)
                {
                    sum += payment.BankTransfer.Value;
                }
                if (payment.Cash.HasValue)
                {
                    sum += payment.Cash.Value;
                }
            }
            foreach (var item in order)
            {
                sum += item.Total;
            }
            return sum;
        }

        public async Task<double> GetTotalPriceOfThisMonthOrdersAsync()
        {
            var allThisMonthOrder = await GetThisMonthOrdersAsync();
            var order = allThisMonthOrder.Where(x => x.Status == "Đang thanh toán" || x.Status == "Hết hạn thanh toán" || x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành");

            var allThisMonthPayment = await GetThisMonthPaymentsAsync();
            double sum = 0;
            foreach (var payment in allThisMonthPayment)
            {
                if (payment.BankTransfer.HasValue)
                {
                    sum += payment.BankTransfer.Value;
                }
                if (payment.Cash.HasValue)
                {
                    sum += payment.Cash.Value;
                }
            }
            foreach (var item in order)
            {
                sum += item.Total;
            }
            return sum;
        }

        public async Task<double> GetTotalPriceOfThisYearOrdersAsync()
        {
            var allThisYearOrder = await GetThisYearOrdersAsync();
            var order = allThisYearOrder.Where(x => x.Status == "Đang thanh toán" || x.Status == "Hết hạn thanh toán" || x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành");
            var allThisYearPayment = await GetThisYearPaymentsAsync();
            double sum = 0;
            foreach (var payment in allThisYearPayment)
            {
                if (payment.BankTransfer.HasValue)
                {
                    sum += payment.BankTransfer.Value;
                }
                if (payment.Cash.HasValue)
                {
                    sum += payment.Cash.Value;
                }
            }
            foreach (var item in order)
            {
                sum += item.Total;
            }
            return sum;
        }

        public async Task<double?> GetTotalPriceOfBankTransferAsync()
        {
            var order = await dbContext.Payments.Where(x => x.PaymentType == "Chuyển khoản").ToListAsync();
            double? sum = 0;
            foreach (var item in order)
            {
                sum += item.BankTransfer;
            }
            return sum;
        }

        public async Task<double?> GetTotalPriceOfByCashAsync()
        {
            var order = await dbContext.Payments.Where(x => x.PaymentType == "Tiền mặt").ToListAsync();
            double? sum = 0;
            foreach (var item in order)
            {
                sum += item.Cash;
            }
            return sum;
        }
        public async Task<int> GetNumberOfByCashOrdersAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = dbContext.Payments.Where(x => x.PaymentType == "Tiền mặt");

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(x => x.PaymentTime >= startDate.Value && x.PaymentTime <= endDate.Value);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetNumberOfByBankTransferOrdersAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = dbContext.Payments.Where(x => x.PaymentType == "Chuyển khoản");

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(x => x.PaymentTime >= startDate.Value && x.PaymentTime <= endDate.Value);
            }

            return await query.CountAsync();
        }

        public async Task<double?> GetTotalPriceOfBankTransferAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = dbContext.Payments.Where(x => x.PaymentType == "Chuyển khoản");

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(x => x.PaymentTime >= startDate.Value && x.PaymentTime <= endDate.Value);
            }

            return await query.SumAsync(x => (double?)x.BankTransfer);
        }

        public async Task<double?> GetTotalPriceOfByCashAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = dbContext.Payments.Where(x => x.PaymentType == "Tiền mặt");

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(x => x.PaymentTime >= startDate.Value && x.PaymentTime <= endDate.Value);
            }

            return await query.SumAsync(x => (double?)x.Cash);
        }

        public async Task<int> GetNumberOfOrderQuantitAsync(int id)
        {
            int count = 0;
            var orderDetails = await dbContext.OrderDetails
                .Where(x => x.OrderId == id)
                .ToListAsync();
            foreach (var item in orderDetails)
            {
                count += item.Quantity;
            }
            return count;
        }


    }
}
