using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class AddProductDTO
    {
        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }


        [Required(ErrorMessage = "GoldId is required")]
        public int GoldId { get; set; }

        [Required(ErrorMessage = "ProductName is required")]
        [MaxLength(100)]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(100)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 1000)]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "GoldWeight is required")]
        [Range(0.1, 100)]
        public double GoldWeight { get; set; }

        [Required(ErrorMessage = "Wage is required")]
        public double Wage { get; set; }

        [Required(ErrorMessage = "priceRatio is required")]
        [Range(1, 2)]
        public double PriceRatio { get; set; }

        [Required(ErrorMessage = "Size is required")]
        public string Size { get; set; }
        [Required(ErrorMessage = "WarrantyPeriod is required")]
        public int WarrantyPeriod { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status = "active";
        public double SellPrice;
        public double StonePrice;
        public Stone stone;
        public double Price => ((GoldWeight * SellPrice) + Wage + StonePrice) * PriceRatio;
    }
}
