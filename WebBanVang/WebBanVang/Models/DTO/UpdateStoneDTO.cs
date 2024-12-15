using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.DTO
{
    public class UpdateStoneDTO
    {

        [RegularExpression(@"^D\d{4}$", ErrorMessage = "StoneCode must be in the format Dxxxx where x is a number")]
        public string StoneCode { get; set; }

        [RegularExpression(@"^KHN\d{6}$", ErrorMessage = "ProductCode must be in the format KHNxxxxxx where x is a number")]
        public string ProductCode { get; set; }

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
        public string Status { get; set; }


    }
}
