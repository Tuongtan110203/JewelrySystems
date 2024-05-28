namespace WebBanVang.Models.DTO
{
    public class AddStoneDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
        public bool IsPrimary { get; set; } = false;
        public string Color { get; set; }
        public string Status = "active";

    }
}
