namespace WebBanVang.Models.DTO
{
    public class AddOrderDTO
    {
        public string UserName { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public double Total { get; set; }
        public string SaleById { get; set; }
        public string CashierId { get; set; }
        public string ServicerId { get; set; }
        public string Status = "Đợi thanh toán";

    }
}
