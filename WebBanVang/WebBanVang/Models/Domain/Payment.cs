using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanVang.Models.Domain
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string PaymentType { get; set; }
        public double Cash { get; set; }
        public double BankTransfer { get; set; }
        public string TransactionId { get; set; }
        public DateTime PaymentTime { get; set; }
        public string Image {  get; set; }
        public string Status { get; set; }
        [ForeignKey("OrderId")]
        public Orders Orders { get; set; }
    }
}
