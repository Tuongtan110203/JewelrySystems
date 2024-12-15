using Microsoft.VisualBasic;

namespace WebBanVang.Models.Domain
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedDate;
    }
}
