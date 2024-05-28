using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.DTO
{
    public class UpdateProductDTO
    {
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }
        [Required]
        [MaxLength(100)]
        public string Description { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        [Range(1, 10000)]
        public int Quantity { get; set; }
        [Required]
        [Range(0.1, 100)]
        public double GoldWeight { get; set; }
        [Required]
        public double Wage { get; set; }
        [Required]
        public double PriceRatio { get; set; }

        [Required]
        public string Size { get; set; }
        [Required]
        public int WarrantyPeriod { get; set; }

        [Required]
        public string Status { get; set; }
        public double SellPrice;
        public double StonePrice;
        public GoldTypeDTO GoldTypes;
        public double Price => ((GoldWeight * SellPrice) + Wage + StonePrice) * PriceRatio;

    }
}
