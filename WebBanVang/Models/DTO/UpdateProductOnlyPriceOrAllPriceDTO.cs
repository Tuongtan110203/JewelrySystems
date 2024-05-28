using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.DTO
{
    public class UpdateProductOnlyPriceOrAllPriceDTO
    {
        public double GoldWeight;
        public double Wage;
        public double PriceRatio;
        public double SellPrice;
        public double StonePrice;
        public double Price => ((GoldWeight * SellPrice) + Wage + StonePrice) * PriceRatio;
    }
}
