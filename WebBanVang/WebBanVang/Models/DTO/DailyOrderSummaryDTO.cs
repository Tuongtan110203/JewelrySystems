namespace WebBanVang.Models.DTO
{
    public class DailyOrderSummaryDTO
    {
        public DateTime Date { get; set; }
        public int TotalOrders { get; set; }
        public double TotalRevenue { get; set; }
    }
}
