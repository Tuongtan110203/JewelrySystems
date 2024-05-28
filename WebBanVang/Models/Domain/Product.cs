using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanVang.Models.Domain
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int GoldId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public double GoldWeight { get; set; }
        public double Wage { get; set; }
        public double PriceRatio { get; set; }
        public double Price { get; set; }
        public string Size { get; set; }
        public int WarrantyPeriod { get; set; }
        public string Status { get; set; }
        [ForeignKey("GoldId")]
        public GoldType GoldTypes { get; set; }
        [ForeignKey("CategoryId")]
        public Category Categories { get; set; }
       public ICollection<Stone> Stones { get; set; }

    }
}
