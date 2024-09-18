using System.ComponentModel.DataAnnotations;
using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class WarrantyDTO
    {

        public int WarrantyId { get; set; }

        [RegularExpression(@"^PBH\d{4}$", ErrorMessage = "Warranty code must be in the format PBHxxxx where xxxx are digits")]
        public string? WarrantyCode { get; set; }

        public int? CustomerId { get; set; }
        public int OrderDetailId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; } = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time");

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; } = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time");
        public string Status { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 3, ErrorMessage = "CashierId must be at least 3 characters long")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "CashierId can only contain letters and no special characters or numbers")]
        public string ServicerId { get; set; }
        public Customers? Customers { get; set; }
        public OrderDetails OrderDetails { get; set; }
    }
}
