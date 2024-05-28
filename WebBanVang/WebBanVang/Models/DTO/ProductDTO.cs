using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public double GoldWeight { get; set; }

        public double Wage { get; set; }
        public double PriceRatio { get; set; }
        public double Price { get; set; }
        public int WarrantyPeriod { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public IEnumerable<StoneDTO> Stones { get; set; }
        //public double StonePrice { get; set; }

        public Category Categories { get; set; }
        public GoldType GoldTypes { get; set; }

    }
}
