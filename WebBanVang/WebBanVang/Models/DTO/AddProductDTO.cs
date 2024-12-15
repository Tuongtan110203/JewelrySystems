using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.DTO
{
    public class AddProductDTO
    {

        [RegularExpression(@"^\d+$", ErrorMessage = "CategoryId must contain only numbers")]
        public int CategoryId { get; set; }


        [RegularExpression(@"^\d+$", ErrorMessage = "GoldId must contain only numbers")]
        public int GoldId { get; set; }

        [RegularExpression(@"^KHN\d{6}$", ErrorMessage = "ProductCode must be in the format KHNxxxxxx where x is a number")]
        public string ProductCode { get; set; }


        [MaxLength(100)]
        public string ProductName { get; set; } = string.Empty;


        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }


        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        public int Quantity { get; set; }


        [Range(0.1, 100, ErrorMessage = "GoldWeight must be between 0.1 and 100")]
        public double GoldWeight { get; set; }


        [Range(0, double.MaxValue, ErrorMessage = "Wage must be at least 0")]
        public double Wage { get; set; }

        [Range(0, 300000000, ErrorMessage = "Price must be between 0 and 300000000")]
        public double Price { get; set; }


        public string Size { get; set; } = string.Empty;


        [Range(6, 36, ErrorMessage = "WarrantyPeriod must be between 6 and 36 months")]
        public int WarrantyPeriod { get; set; }

        public string Status = "active";

    }
}
