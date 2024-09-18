using System.ComponentModel.DataAnnotations;
using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class OrdersDTO
    {
        public int OrderId { get; set; }

        [RegularExpression(@"^MDH\d{6}$", ErrorMessage = "OrderCode must be in the format MDHxxxxxx where x is a number")]
        public string OrderCode { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "UserName must be 3-20 characters long")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ0-9\s\.,;:/]+", ErrorMessage = "UserName can only contain letters and no special characters or numbers")]
        public string UserName { get; set; } = string.Empty;
        public int? CustomerId { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; } = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time");
        public double Total { get; set; }


        public string? SaleById { get; set; }


        public string? CashierId { get; set; }

        public string? ServicerId { get; set; }


        [StringLength(30, MinimumLength = 3, ErrorMessage = "CustomerName must be between 3 and 30 characters")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ\s]+", ErrorMessage = "CustomerName cannot contain special characters like !@#%^&")]
        public string? CustomerName { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "PhoneNumber must be 10 digits and contain only numbers")]
        public string? PhoneNumber { get; set; }


        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        public string Status { get; set; } = string.Empty;
        public Users Users { get; set; }
        public Users sale { get; set; }
        public Users cashier { get; set; }
        public Users services { get; set; }
        public Customers? Customers { get; set; }

        public int PaymentCount { get; set; }
        public double? PaymentMoney { get; set; }

        public double? TotalBankTransfer;
        public double? TotalCash;



    }
}
