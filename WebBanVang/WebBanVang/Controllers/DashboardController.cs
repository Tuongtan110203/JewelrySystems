using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WebBanVang.Data;
using WebBanVang.Models.DTO;
using WebBanVang.Repository;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowLocalhost3000And15723035249")]
    [Authorize(Roles = "Manager")]
    //[AllowAnonymous]
    public class DashboardController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext _dbContext;
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(JewelrySalesSystemDbContext dbContext, IDashboardRepository dashboardRepository)
        {
            _dbContext = dbContext;
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet("stock-summary")]
        public async Task<ActionResult<StockSummaryDTO>> GetStockSummary()
        {
            var totalStockValue = await _dashboardRepository.GetTotalStockValueAsync();
            var totalStockQuantity = await _dashboardRepository.GetTotalStockQuantityAsync();
            var outOfStockProductCount = await _dashboardRepository.GetOutOfStockProductCountAsync();

            var stockSummary = new StockSummaryDTO
            {
                TotalStockValue = totalStockValue,
                TotalStockQuantity = totalStockQuantity,
                OutOfStockProductCount = outOfStockProductCount
            };

            return Ok(stockSummary);
        }
        [HttpGet("revenue-summary")]
        public async Task<ActionResult<RevenueSummaryDTO>> GetRevenueSummary()
        {
            DateTime today = DateTime.Today;
            int currentMonth = today.Month;
            int currentYear = today.Year;


            var dailyRevenue = await _dashboardRepository.GetDailyRevenueAsync(today);
            var totalDailyOrder = await _dashboardRepository.GetTotalDailyOrderAsync(today);

            var weeklyRevenue = await _dashboardRepository.GetWeeklyRevenueAsync(currentYear);
            var totalWeeklyOrder = await _dashboardRepository.GetTotalWeeklyOrderAsync(currentYear);

            var monthlyRevenue = await _dashboardRepository.GetMonthlyRevenueAsync(currentMonth, currentYear);
            var totalMonthOrder = await _dashboardRepository.GetTotalMonthOrderAsync(currentMonth, currentYear);

            var yearlyRevenue = await _dashboardRepository.GetYearlyRevenueAsync(currentYear);
            var totalYearOrder = await _dashboardRepository.GetTotalYearOrderAsync(currentYear);

            var revenueSummary = new RevenueSummaryDTO
            {
                DailyRevenue = dailyRevenue,
                TotalDailyOrder = totalDailyOrder,
                WeeklyRevenue = weeklyRevenue,
                TotalWeekOrder = totalWeeklyOrder,
                MonthlyRevenue = monthlyRevenue,
                TotalMonthOrder = totalMonthOrder,
                YearlyRevenue = yearlyRevenue,
                TotalYearOrder = totalYearOrder
            };
            return Ok(revenueSummary);
        }
        [HttpGet("daily-revenue-last-7-days")]
        public async Task<ActionResult<IEnumerable<DailyRevenueDTO>>> GetDailyRevenueLast7Days()
        {
            var today = DateTime.Today;
            var dates = Enumerable.Range(0, 7).Select(i => today.AddDays(-i)).Reverse().ToList();

            var dailyRevenues = new List<DailyRevenueDTO>();
            foreach (var date in dates)
            {
                var dailyRevenue = await _dashboardRepository.GetDailyRevenueAsync(date);
                var totalOrders = await _dashboardRepository.GetTotalDailyOrderAsync(date);

                var dailyRevenueDTO = new DailyRevenueDTO
                {
                    Date = date,
                    DailyRevenue = dailyRevenue,
                    TotalOrders = totalOrders
                };

                dailyRevenues.Add(dailyRevenueDTO);
            }

            return Ok(dailyRevenues);
        }
        [HttpGet("orders-and-revenue-to-current-day")]
        public async Task<ActionResult<IEnumerable<DailyOrderSummaryDTO>>> GetOrdersAndRevenueThisMonth()
        {
            DateTime today = DateTime.Today;
            DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var dailyOrderSummaries = new List<DailyOrderSummaryDTO>();

            for (DateTime date = firstDayOfMonth; date <= today; date = date.AddDays(1))
            {
                var totalOrders = await _dashboardRepository.GetTotalDailyOrderAsync(date);
                var totalRevenue = await _dashboardRepository.GetDailyRevenueAsync(date);

                dailyOrderSummaries.Add(new DailyOrderSummaryDTO
                {
                    Date = date,
                    TotalOrders = totalOrders,
                    TotalRevenue = totalRevenue
                });
            }

            return Ok(dailyOrderSummaries);
        }
        [HttpGet("orders-and-revenue-to-current-month")]
        public async Task<ActionResult<IEnumerable<MonthlyOrderSummaryDTO>>> GetOrdersAndRevenueThisYearh()
        {
            DateTime today = DateTime.Today;
            var monthlyOrderSummaries = new List<MonthlyOrderSummaryDTO>();

            for (int month = 1; month <= today.Month; month++)
            {
                var totalMonthlyOrders = await _dashboardRepository.GetTotalMonthOrderAsync(month, today.Year);
                var totalMonthlyRevenue = await _dashboardRepository.GetMonthlyRevenueAsync(month, today.Year);

                monthlyOrderSummaries.Add(new MonthlyOrderSummaryDTO
                {
                    Month = month,
                    TotalOrders = totalMonthlyOrders,
                    TotalRevenue = totalMonthlyRevenue
                });
            }

            return Ok(monthlyOrderSummaries);
        }

    }
}