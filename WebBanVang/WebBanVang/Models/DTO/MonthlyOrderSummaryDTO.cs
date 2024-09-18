namespace WebBanVang.Models.DTO
{
    public class MonthlyOrderSummaryDTO
    {
        public int Month { get; set; } 
        public int TotalOrders { get; set; }
        public double TotalRevenue { get; set; }
    }
}
