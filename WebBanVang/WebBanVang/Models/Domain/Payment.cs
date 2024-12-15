using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanVang.Models.Domain
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        //  [Required(ErrorMessage = "PaymentCode is required")]
        [RegularExpression(@"^(TM|CK)\d{4}$", ErrorMessage = "PaymentCode must be in the format TMxxxx or CKxxxx where x is a number")]
        public string? PaymentCode { get; set; }
        public int OrderId { get; set; }
        public string? PaymentType { get; set; } = string.Empty;

        public double? Cash { get; set; }


        public double? BankTransfer { get; set; }


        public string? TransactionId { get; set; } = string.Empty;

        public DateTime PaymentTime { get; set; } = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time");

        //  [Required(ErrorMessage = "Image is required")]
        public string? Image { get; set; } = string.Empty;
        public string? Status { get; set; } = string.Empty;
        [ForeignKey("OrderId")]
        public Orders Orders { get; set; }

    }
}
