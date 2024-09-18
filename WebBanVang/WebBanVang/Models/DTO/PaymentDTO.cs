using System.ComponentModel.DataAnnotations;
using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class PaymentDTO
    {
        public int PaymentId { get; set; }

        [RegularExpression(@"^(TM|CK)\d{4}$", ErrorMessage = "PaymentCode must be in the format TMxxxx or CKxxxx where x is a number")]
        public string? PaymentCode { get; set; }

        public string PaymentType { get; set; } = string.Empty;

        public double Cash { get; set; }

        public double BankTransfer { get; set; }


        public string TransactionId { get; set; } = string.Empty;

        public DateTime PaymentTime { get; set; } = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time");

        public string Image { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Orders Orders { get; set; }

    }
}
