using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public interface IUsersRepository
    {
        Task<List<Users>> GetAllUsersAsync();
        Task<Users> GetUsersByNameAsync(string name);

        Task<Users> DeleteUsers(string name);
        Task<Users> AddUsers(Users users);
        Task<Users> UpdateUsers(string name, Users users);
    }
}
