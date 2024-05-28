namespace WebBanVang.Models.DTO
{
    public class UpdateWarrantyDTO
    {
        public int CustomerId { get; set; }
        public int OrderDetailId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string status { get; set; }
    }
}
