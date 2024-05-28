namespace WebBanVang.Models.DTO
{
    public class AddCustomerDTO
    {
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Status = "active";
    }
}
