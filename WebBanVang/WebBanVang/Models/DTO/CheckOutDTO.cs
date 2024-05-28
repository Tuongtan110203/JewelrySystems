    namespace WebBanVang.Models.DTO
    {
        public class CheckOutDTO
        {
        public int OrderId { get; set; }
            public string CustomerName { get; set; }
            public string PhoneNumber {  get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            
            public double Price { get; set; }
            public int Quantity { get; set; }
            public string SaleById { get; set; }
            public string CashierId { get; set; }
            public string ServicerId { get; set; }
            public double SubTotal {  get; set; }

        }
    }
