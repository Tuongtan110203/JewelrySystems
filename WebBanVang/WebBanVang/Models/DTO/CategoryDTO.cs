using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.DTO
{
    public class CategoryDTO
    {


        [RegularExpression(@"^CT\d{4}$", ErrorMessage = "CategoryCode must be in the format CTxxxx where x is a number")]
        public string CategoryCode { get; set; }
        public int CategoryId { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "CategoryName must be between 3 and 20 characters")]
       [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ\s]+", ErrorMessage = "CategoryName cannot contain special characters like @#$%^&")]
        public string Name { get; set; } = string.Empty;
    }
}
