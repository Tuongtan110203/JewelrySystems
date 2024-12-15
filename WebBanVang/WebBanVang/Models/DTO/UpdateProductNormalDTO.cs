using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.DTO
{
    public class UpdateProductNormalDTO
    {

        public int CategoryId { get; set; }

        public int GoldId { get; set; }


        [RegularExpression(@"^KHN\d{6}$", ErrorMessage = "ProductCode must be in the format KHNxxxxxx where x is a number")]
        public string ProductCode { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "ProductName must be between 8 and 100 characters")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ0-9\s]+", ErrorMessage = "Productname cannot contain special characters")]
        public string ProductName { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Description must be 3-50 characters long")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ0-9\s]+", ErrorMessage = "Description cannot contain special characters")]

        public string Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }

        [Range(0.1, 20, ErrorMessage = "Gold weight must be between 0.1 and 20")]
        public double GoldWeight { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Wage must be a number and cannot contain letters or special characters")]
        [Range(0, 100000000, ErrorMessage = "Wage must be between 0 and 100,000,000 VND")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        public double Wage { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Price must be a number and cannot contain letters or special characters")]
        [Range(0, 300000000, ErrorMessage = "Price must be between 0 and 300,000,000 VND")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        public double Price { get; set; }

        [Range(5, 60, ErrorMessage = "Size must be between 5 and 60")]
        public string Size { get; set; }

        [Range(6, 36, ErrorMessage = "Warranty Period must be between 6 and 36 months")]
        public int WarrantyPeriod { get; set; }
        public string Status { get; set; }
        public IFormFile? Image { get; set; }



    }
}
