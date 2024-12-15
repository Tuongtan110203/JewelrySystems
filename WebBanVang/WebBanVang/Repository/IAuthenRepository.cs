using WebBanVang.Models.DTO;

namespace WebBanVang.Repository
{
    public interface IAuthenRepository
    {
        Task<string> Login(Login model);
        Task<string> Register(RegisterDTO registerDTO);
    }
}
