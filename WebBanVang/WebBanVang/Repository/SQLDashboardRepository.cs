using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;

namespace WebBanVang.Repository
{
    public class SQLDashboardRepository : IDashboardRepository
    {
        private readonly JewelrySalesSystemDbContext dbContext;
        private readonly SQLOrderRepository _orderRepo;

        public SQLDashboardRepository(JewelrySalesSystemDbContext dbContext, SQLOrderRepository orderRepo)
        {
            this.dbContext = dbContext;
            _orderRepo = orderRepo;
        }

        public async Task<double> GetTotalStockValueAsync()
        {
            return await dbContext.Products
                .Where(p => p.Quantity > 0)
                .SumAsync(p => p.Price * p.Quantity);
        }

        public async Task<int> GetTotalStockQuantityAsync()
        {
            return await dbContext.Products
                .Where(p => p.Quantity > 0)
                .SumAsync(p => p.Quantity);
        }

        public async Task<int> GetOutOfStockProductCountAsync()
        {
            return await dbContext.Products
                .Where(p => p.Quantity == 0)
                .CountAsync();
        }

        public async Task<double> GetDailyRevenueAsync(DateTime date)
        {
            /*return await dbContext.Orders
                .Where(o => o.OrderDate.Date == date.Date)
                .SumAsync(o => o.Total);
*/
            var allTodayOrder = await _orderRepo.GetDailyOrdersAsync(date);
            var order = allTodayOrder.Where(x => x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành").ToList();
            var allTodayPayment = await _orderRepo.GetDailyPaymentsAsync(date);
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
            // return await _orderRepo.GetTotalPriceOfTodayOrdersAsync();
        }

        public async Task<double> GetMonthlyRevenueAsync(int month, int year)
        {
            /* return await dbContext.Orders
                 .Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year)
                 .SumAsync(o => o.Total);*/

            /*return await _orderRepo.GetTotalPriceOfThisMonthOrdersAsync();*/
            var allThisMonthOrder = await _orderRepo.GetMonthlyOrdersAsync(month, year);
            var order = allThisMonthOrder.Where(x => x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành");

            var allThisMonthPayment = await _orderRepo.GetMonthlyPaymentsAsync(month, year);
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

        public async Task<double> GetYearlyRevenueAsync(int year)
        {
            /*return await dbContext.Orders
                .Where(o => o.OrderDate.Year == year)
                .SumAsync(o => o.Total);*/
            /*return await _orderRepo.GetTotalPriceOfThisYearOrdersAsync();*/
            var allThisYearOrder = await _orderRepo.GetYearlyOrdersAsync(year);
            var order = allThisYearOrder.Where(x => x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành");
            var allThisYearPayment = await _orderRepo.GetYearlyPaymentsAsync(year);
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
        public async Task<int> GetTotalDailyOrderAsync(DateTime date)
        {
            return await dbContext.Orders
                .Where(x => x.OrderDate.Date == date.Date
                && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán"))
                .CountAsync();
        }

        public async Task<int> GetTotalMonthOrderAsync(int month, int year)
        {
            return await dbContext.Orders
                .Where(x => x.OrderDate.Month == month && x.OrderDate.Year == year
                && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán"))
                .CountAsync();
        }

        public async Task<int> GetTotalYearOrderAsync(int year)
        {
            return await dbContext.Orders
                .Where(x => x.OrderDate.Year == year
                && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán"))
                .CountAsync();
        }

        public async Task<double> GetWeeklyRevenueAsync(int currentYear)
        {
            var allThisMonthOrder = await _orderRepo.GetWeeklyOrdersAsync(currentYear);
            var order = allThisMonthOrder.Where(x => x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành");

            var allThisMonthPayment = await _orderRepo.GetWeeklyPaymentsAsync(currentYear);
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

        public async Task<int> GetTotalWeeklyOrderAsync(int currentYear)
        {
            /*DateTime today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7).AddTicks(-1);*/
            var today = DateTime.Now;
            var currentDayOfWeek = (int)today.DayOfWeek;
            currentDayOfWeek = (currentDayOfWeek == 0) ? 7 : currentDayOfWeek;
            var startOfWeek = today.AddDays(-currentDayOfWeek + 1).Date;
            var endOfWeek = startOfWeek.AddDays(7).Date;

            var totalOrders = await dbContext.Orders.Where(x => x.OrderDate >= startOfWeek && x.OrderDate <= endOfWeek && x.OrderDate.Year == currentYear
                && (x.Status == "Đã thanh toán" || x.Status == "Đã hoàn thành" || x.Status == "Hết hạn thanh toán" || x.Status == "Đang thanh toán"))
                .CountAsync();

            return totalOrders;
        }

    }

}
