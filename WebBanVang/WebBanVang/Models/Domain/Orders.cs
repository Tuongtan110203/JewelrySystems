using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanVang.Models.Domain
{
    public class Orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public string UserName { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public double Total { get; set; }
        public string SaleById { get; set; }
        public string CashierId { get; set; }
        public string ServicerId { get; set; }
        public string Status { get; set; } = "Đợi thanh toán";
        [ForeignKey("UserName")]
        public Users Users { get; set; }
        [ForeignKey("CustomerId")]
        public Customers Customers { get; set; }
    }
}
