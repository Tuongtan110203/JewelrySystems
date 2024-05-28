namespace WebBanVang.Models.DTO
{
    public class UpdateGoldTypeDTO
    {
        public string GoldName { get; set; }
        public double BuyPrice { get; set; }
        public double SellPrice { get; set; }
        public DateTime? UpdateTime = DateTime.Now;
        public string Status { get; set; }
    }
}
