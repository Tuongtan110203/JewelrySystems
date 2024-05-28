using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class AddUserDTO
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime Dob { get; set; }
        public int? Level { get; set; }
        public string Status { get; set; }
        public int RoleId { get; set; }

    }
}
