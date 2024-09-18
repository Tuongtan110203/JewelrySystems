using System.ComponentModel.DataAnnotations;
using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class UpdateWarrantyDTO
    {

        [RegularExpression(@"^PBH\d{4}$", ErrorMessage = "WarrantyCode must be in the format PBHxxxx where x is a number")]
        public string? WarrantyCode { get; set; }

        public int CustomerId { get; set; }

        public int OrderDetailId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; } = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time");

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; } = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time");

        public string status { get; set; }
    }
}
