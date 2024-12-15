using System.ComponentModel.DataAnnotations.Schema;
using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class OrderDetailsDTO
    {
        public int OrderDetailId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public ProductDTO Product { get; set; }
        public OrdersDTO Order { get; set; }
        public Users Sale { get; set; }
        public Users Cashier { get; set; }
        public Users Services { get; set; }
    }
}
