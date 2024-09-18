using System.ComponentModel.DataAnnotations;
using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class AddPaymentDTO
    {
        //  [Required(ErrorMessage = "PaymentCode is required")]
        [RegularExpression(@"^(TM|CK)\d{4}$", ErrorMessage = "PaymentCode must be in the format TMxxxx or CKxxxx where x is a number")]
        public string? PaymentCode;

        public int OrderId { get; set; }
        public string? PaymentType { get; set; } = string.Empty;

        public double? Cash { get; set; }

        public double? BankTransfer { get; set; }

       
        public string? TransactionId { get; set; } = string.Empty;

        public DateTime PaymentTime = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time");

        public IFormFile? Image { get; set; }
        public string Status;


    }
}
