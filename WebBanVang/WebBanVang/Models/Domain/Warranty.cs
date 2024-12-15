using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanVang.Models.Domain
{
    public class Warranty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Warranty ID is required")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Warranty ID must be a number and cannot contain letters or special characters")]
        public int WarrantyId { get; set; }
        //   [Required(ErrorMessage = "Warranty code is required")]
        [RegularExpression(@"^PBH\d{4}$", ErrorMessage = "Warranty code must be in the format PBHxxxx where xxxx are digits")]
        public string? WarrantyCode { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; } = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time");

        [Required(ErrorMessage = "End date is required")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; } = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time");

        public int OrderDetailId { get; set; }
        public int? CustomerId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Status { get; set; } = string.Empty;

        [ForeignKey("OrderDetailId")]
        public virtual OrderDetails OrderDetails { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customers? Customers { get; set; }
    }
}
