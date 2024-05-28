using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class UpdateStoneDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
        public string Color { get; set; }
        public string Status { get; set; }

        public bool IsPrimary { get; set; }

    }
}
