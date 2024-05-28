using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public interface IStoneRepository
    {
        Task<List<Stone>> GetAllStonesAsync();
        Task<Stone?> GetStoneByIdAsync(int id);
        Task<Stone?> UpdateStoneAsync(int id, Stone stone);
        Task<Stone> CreateAsync(Stone stone);
        Task<Stone?> DeleteStoneAsync(int id);
        

    }
}