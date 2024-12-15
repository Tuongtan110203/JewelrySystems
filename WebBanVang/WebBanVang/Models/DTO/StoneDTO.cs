using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class StoneDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [RegularExpression(@"^\d+$", ErrorMessage = "Stone ID must be a number and cannot contain letters or special characters")]
        public int StoneId { get; set; }

        [RegularExpression(@"^D\d{4}$", ErrorMessage = "Stone code must be in the format Dxxxx where xxxx are digits")]
        public string StoneCode { get; set; }
        public int ProductId { get; set; }

        [StringLength(30, MinimumLength = 3, ErrorMessage = "Name must be 3-30 characters long")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ\s]+", ErrorMessage = "Name cannot contain special characters")]
        public string Name { get; set; } = string.Empty;



        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Price must be a number and cannot contain letters or special characters")]
        [Range(0, 100000000, ErrorMessage = "Price must be between 0 and 100,000,000 VND")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        public double Price { get; set; }

        [StringLength(20, MinimumLength = 2, ErrorMessage = "Color must be 2-20 character long")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ\s]+", ErrorMessage = "Color can only contain letters ")]
        public string Color { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
        public Product Products { get; set; }

    }
}
