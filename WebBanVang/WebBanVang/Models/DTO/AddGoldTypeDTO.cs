using System.ComponentModel.DataAnnotations;
using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class AddGoldTypeDTO
    {

        [RegularExpression(@"^V\d{4}$", ErrorMessage = "GoldCode must be in the format Vxxxx where x is a number")]
        public string? GoldCode { get; set; }

        [Required(ErrorMessage = "GoldName is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "GoldName must be 3-30 characters long")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ0-9\s]+", ErrorMessage = "GoldName can only contain letters and no special characters or numbers")]
        public string GoldName { get; set; } = string.Empty;


        [Range(0, 100000000, ErrorMessage = "BuyPrice must be between 0  and 100,000,000")]
        public double BuyPrice { get; set; }


        [Range(0, 100000000, ErrorMessage = "SellPrice must be between 0 and 100,000,000")]
        public double SellPrice { get; set; }
        public DateTime UpdateTime = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time");

        public string Status { get; set; } = "active";
    }
}
