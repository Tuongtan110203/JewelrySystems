using System.ComponentModel.DataAnnotations;
using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class AddWarrantyDTO
    {

        [RegularExpression(@"^PBH\d{4}$", ErrorMessage = "WarrantyCode must be in the format PBHxxxx where x is a number")]
        public string? WarrantyCode;

        public int OrderId { get; set; }

        public string Status = "Đã hoàn thành";
        public Orders orders;
    }
}
