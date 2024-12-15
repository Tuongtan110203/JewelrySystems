using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.DTO
{
    public class AddCategoryDTO
    {
        [Required(ErrorMessage = "CategoryCode is required")]
        [RegularExpression(@"^LSP\d{4}$", ErrorMessage = "CategoryCode must be in the format LSPxxxx where x is a number")]
        public string CategoryCode { get; set; }

        [Required(ErrorMessage = "CategoryName is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "CategoryName must be between 3 and 20 characters")]
       [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ\s]+", ErrorMessage = "CategoryName cannot contain special characters like @#$%^&")]
        public string Name { get; set; } = string.Empty;


        public string Status { get; set; } = "active";
    }
}
