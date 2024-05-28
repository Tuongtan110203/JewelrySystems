using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.Domain
{
    public class GoldType
    {
        [Key]
        public int GoldId { get; set; }
        public string GoldName { get; set; }
        public double BuyPrice { get; set; }
        public double SellPrice { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Status { get; set; }
    }
}
