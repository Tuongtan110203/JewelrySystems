namespace WebBanVang.Models.DTO
{
    public class StoneDTO
    {
        public int StoneId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
        public string Color { get; set; }
        public bool IsPrimary { get; set; }

        public string Status { get; set; }
    }
}
