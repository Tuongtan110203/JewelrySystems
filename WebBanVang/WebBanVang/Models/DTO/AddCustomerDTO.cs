using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.DTO
{
    public class AddCustomerDTO
    {

        [StringLength(30, MinimumLength = 3, ErrorMessage = "CustomerName must be between 3 and 30 characters")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ\s]+", ErrorMessage = "CustomerName cannot contain special characters like !@#%^&")]
        public string CustomerName { get; set; } = string.Empty;


        [RegularExpression(@"^\d{10}$", ErrorMessage = "PhoneNumber must be 10 digits and contain only numbers")]
        public string PhoneNumber { get; set; } = string.Empty.ToString();


        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;
        public string Status = "active";
    }
}
