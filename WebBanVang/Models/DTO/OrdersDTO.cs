using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class OrdersDTO
    {
        public int OrderId { get; set; }
        public string UserName { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public double Total { get; set; }
        public string SaleById { get; set; }
        public string CashierId { get; set; }
        public string ServicerId { get; set; }
        public string Status { get; set; }
        public Users Users { get; set; }
        public Customers Customers { get; set; }

    }
}
