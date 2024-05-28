namespace WebBanVang.Models.DTO
{
    public class WarrantyDTO
    {
        public int WarrantyId { get; set; }
        public int CustomerId { get; set; }
        public int OrderDetailId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Status { get; set; }
    }
}
