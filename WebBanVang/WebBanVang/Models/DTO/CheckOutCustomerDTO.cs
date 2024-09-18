using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.DTO
{
    public class CheckOutCustomerDTO
    {
        public int CustomerId;
        public string? PhoneNumberCustomer { get; set; } = null;

        [StringLength(30, MinimumLength = 3, ErrorMessage = "CustomerName must be between 3 and 30 characters")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ\s]+", ErrorMessage = "CustomerName cannot contain special characters like !@#%^&")]
        public string? CustomerName { get; set; } = null;

        public string? Email { get; set; } = string.Empty;
    }
}
