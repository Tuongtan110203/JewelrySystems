using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanVang.Models.Domain
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime Dob { get; set; }
        public int? Level {  get; set; }
        public string Status { get; set; }
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Roles Roles { get; set; }
    }
}
