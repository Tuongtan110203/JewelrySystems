namespace WebBanVang.Models.DTO
{
    public class RevenueSummaryDTO
    {
        public double DailyRevenue { get; set; }
        public int TotalDailyOrder { get; set; }
        public double WeeklyRevenue { get; set; }
        public int TotalWeekOrder { get; set; }
        public double MonthlyRevenue { get; set; }
        public int TotalMonthOrder { get; set; } 
        public double YearlyRevenue { get; set; }
        public int TotalYearOrder { get; set; } 
    }
}
