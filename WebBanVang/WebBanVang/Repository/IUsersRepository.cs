using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public interface IUsersRepository
    {
        Task<List<Users>> GetAllUsersAsync();
        Task<Users?> GetUserByUserNameAsync(string name);
        Task<Users?> GetUserByEmailAsync(string email);
        Task<Users?> GetUserByPhoneAsync(string email);

        Task<Users?> DeleteUsers(string name);
        Task<Users> AddUsers(Users users);
        Task<Users?> UpdateUsers(string name, Users users);
        Task<List<Users>> GetUsersByFullNameAsync(string name);
    }
}
