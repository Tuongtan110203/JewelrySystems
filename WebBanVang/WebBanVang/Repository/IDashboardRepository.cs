namespace WebBanVang.Repository
{
    public interface IDashboardRepository
    {
        Task<double> GetTotalStockValueAsync();
        Task<int> GetTotalStockQuantityAsync();
        Task<int> GetOutOfStockProductCountAsync();
        Task<double> GetDailyRevenueAsync(DateTime date);
        Task<double> GetMonthlyRevenueAsync(int month, int year);
        Task<double> GetYearlyRevenueAsync(int year);
        Task<int> GetTotalDailyOrderAsync(DateTime date);
        Task<int> GetTotalMonthOrderAsync(int month, int year);
        Task<int> GetTotalYearOrderAsync(int year);
        Task<double> GetWeeklyRevenueAsync(int currentYear);
        Task<int> GetTotalWeeklyOrderAsync(int currentYear);
    }
}