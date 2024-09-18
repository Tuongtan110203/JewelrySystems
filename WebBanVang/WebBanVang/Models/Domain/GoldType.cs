using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.Domain
{
    public class GoldType
    {
        [Key]
        public int GoldId { get; set; }

        //   [Required(ErrorMessage = "GoldCode is required")]
        [RegularExpression(@"^V\d{4}$", ErrorMessage = "GoldCode must be in the format Vxxxx where x is a number")]
        public string? GoldCode { get; set; }


        [Required(ErrorMessage = "GoldName is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "GoldName must be 3-30 characters long")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ0-9\s]+", ErrorMessage = "GoldName can only contain letters and no special characters or numbers")]
        public string GoldName { get; set; } = string.Empty;

        [Required(ErrorMessage = "BuyPrice is required")]
        [Range(0, 100000000, ErrorMessage = "BuyPrice must be between 0  and 100,000,000")]
        public double BuyPrice { get; set; }

        [Required(ErrorMessage = "SellPrice is required")]
        [Range(0, 100000000, ErrorMessage = "SellPrice must be between 0 and 100,000,000")]
        public double SellPrice { get; set; }

        [Required(ErrorMessage = "Update time is required")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? UpdateTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
