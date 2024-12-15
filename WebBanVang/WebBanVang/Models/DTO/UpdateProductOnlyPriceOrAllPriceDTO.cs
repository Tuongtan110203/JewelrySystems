using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.DTO
{
    public class UpdateProductOnlyPriceOrAllPriceDTO
    {

        [Range(0.1, 20, ErrorMessage = "Gold weight must be between 0.1 and 20")]
        public double GoldWeight;

        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Wage must be a number and cannot contain letters or special characters")]
        [Range(0, 100000000, ErrorMessage = "Wage must be between 0 and 100,000,000 VND")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        public double Wage;

        [Range(0, 100000000, ErrorMessage = "SellPrice must be between 0 and 100,000,000")]
        public double SellPrice;

        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Price must be a number and cannot contain letters or special characters")]
        [Range(0, 100000000, ErrorMessage = "Price must be between 0 and 100,000,000 VND")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        public double StonePrice;
        public double Price => ((GoldWeight * SellPrice) + Wage + StonePrice);
    }
}
