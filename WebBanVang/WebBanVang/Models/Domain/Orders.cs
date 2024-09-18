using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanVang.Models.Domain
{
    public class Orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        // [Required(ErrorMessage = "SaleId is required")]

        public string? SaleById { get; set; }
        // [Required(ErrorMessage = "CashierId is required")]

        public string? CashierId { get; set; }

        // [Required(ErrorMessage = "ServicerId is required")]

        public string? ServicerId { get; set; }

        //  [Required(ErrorMessage = "CustomerName is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "CustomerName must be between 3 and 30 characters")]
        [RegularExpression(@"[a-zA-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ\s]+", ErrorMessage = "CustomerName cannot contain special characters like !@#%^&")]
        public string? CustomerName { get; set; }
        //  [Required(ErrorMessage = "PhoneNumber is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "PhoneNumber must be 10 digits and contain only numbers")]
        public string? PhoneNumber { get; set; }

        //  [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        public string Status { get; set; } = "Đợi thanh toán";
        [ForeignKey("UserName")]
        public Users Users { get; set; }


        [ForeignKey("CustomerId")]
        public Customers? Customers { get; set; }

        public ICollection<Payment> Payments { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
