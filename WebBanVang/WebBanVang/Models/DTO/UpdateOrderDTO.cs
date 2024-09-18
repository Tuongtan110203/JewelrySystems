using System.ComponentModel.DataAnnotations;
using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class UpdateOrderDTO
    {
        public string UserName { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public DateTime OrderDate = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time");
        public double Total { get; set; }


        [StringLength(100, MinimumLength = 3, ErrorMessage = "SaleId must be at least 3 characters long")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "SaleId can only contain letters and no special characters or numbers")]
        public string? SaleById { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "CashierId must be at least 3 characters long")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "CashierId can only contain letters and no special characters or numbers")]
        public string? CashierId { get; set; }


        [StringLength(100, MinimumLength = 3, ErrorMessage = "ServicerId must be at least 3 characters long")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "ServicerId can only contain letters and no special characters or numbers")]
        public string? ServicerId { get; set; }

        [StringLength(30, MinimumLength = 3, ErrorMessage = "CustomerName must be between 3 and 30 characters")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ\s]+", ErrorMessage = "CustomerName cannot contain special characters like !@#%^&")]
        public string? CustomerName { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "PhoneNumber must be 10 digits and contain only numbers")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
