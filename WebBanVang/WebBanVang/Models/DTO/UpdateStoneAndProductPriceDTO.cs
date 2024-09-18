using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.DTO
{
    public class UpdateStoneAndProductPriceDTO
    {
        //  public Product Product = new Product();
        public double StonePrice;

        [Range(0.1, 20, ErrorMessage = "Gold weight must be between 0.1 and 20")]
        public double GoldWeight { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Wage must be a number and cannot contain letters or special characters")]
        [Range(0, 100000000, ErrorMessage = "Wage must be between 0 and 100,000,000 VND")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        public double Wage { get; set; }

        [Range(0, 100000000, ErrorMessage = "SellPrice must be between 0 and 100,000,000")]
        public double SellPrice { get; set; }

        public double Price => ((GoldWeight * SellPrice) + Wage + StonePrice);


    }
}