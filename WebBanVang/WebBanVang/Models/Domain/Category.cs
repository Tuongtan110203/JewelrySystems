using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanVang.Models.Domain
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "CategoryCode is required")]
        [RegularExpression(@"^CT\d{4}$", ErrorMessage = "CategoryCode must be in the format CTxxxx where x is a number")]
        public string CategoryCode { get; set; }


        [Required(ErrorMessage = "CategoryName is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "CategoryName must be between 3 and 20 characters")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ\s]+", ErrorMessage = "CategoryName cannot contain special characters like @#$%^&")]
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
