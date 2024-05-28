using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanVang.Models.Domain
{
    public class Warranty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WarrantyId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int OrderDetailId { get; set; }
        public int CustomerId { get; set; }
        [Required]
        [MaxLength(100)] 
        public string Status { get; set; }

        [ForeignKey("OrderDetailId")]
        public virtual OrderDetails OrderDetails { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customers Customers { get; set; }
    }
}
