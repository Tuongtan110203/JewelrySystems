using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class AddWarrantyDTO
    {
        public string CustomerPhone { get; set; }
        public int OrderId { get; set; }

        //public DateTime StartDate;
        //public DateTime EndDate;

        public string Status = "Đã hoàn thành";
        public Orders orders;
    }
}
