using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.DTO
{
    public class StoneDTOV2
    {

        [RegularExpression(@"^D\d{4}$", ErrorMessage = "StoneCode must be in the format Dxxxx where x is a number")]
        public string StoneCode { get; set; }
        public int ProductId;

        [StringLength(30, MinimumLength = 3, ErrorMessage = "Name must be 3-30 characters long")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ\s]+", ErrorMessage = "Name cannot contain special characters")]
        public string Name { get; set; } = string.Empty;

        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ0-9\s\.,;:/]+", ErrorMessage = "Type can only contain letters and spaces")]
        public string Type { get; set; } = string.Empty;

        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Price must be a number and cannot contain letters or special characters")]
        [Range(0, 100000000, ErrorMessage = "Price must be between 0 and 100,000,000 VND")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        public double Price { get; set; }
        public bool IsPrimary { get; set; } = false;

        [StringLength(20, MinimumLength = 2, ErrorMessage = "Color must be 2-20 character long")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ\s]+", ErrorMessage = "Color can only contain letters ")]
        public string Color { get; set; } = string.Empty;
        public string Status { get; set; }
    }

}
