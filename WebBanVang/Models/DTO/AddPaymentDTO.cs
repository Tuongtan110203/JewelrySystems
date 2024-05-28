using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class AddPaymentDTO
    {
        public int OrderId { get; set; }
        public string PaymentType { get; set; }
        public double Cash { get; set; }
        public double BankTransfer { get; set; }
        public string TransactionId { get; set; }
        public DateTime PaymentTime = DateTime.Now;
        public string Image { get; set; }
        public string Status = "Đã thanh toán";


    }
}
