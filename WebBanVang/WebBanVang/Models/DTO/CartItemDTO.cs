using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.DTO
{
    public class CartItemDTO
    {
        public int ProductId { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "ProductName must be between 8 and 100 characters")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ0-9\s]+", ErrorMessage = "Productname cannot contain special characters")]
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string? Image { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }
        public double SubTotal { get; set; }
        public string AddedDateFormatted { get; set; }
    }
}
