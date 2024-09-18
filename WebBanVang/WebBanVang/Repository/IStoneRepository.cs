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
        Task<List<Stone>> GetStonesByStoneNameOrStoneCodeOrProductCodeAsync(string KeySearch);
        Task<bool> IsStoneCodeExistsAsync(string stoneCode, int? id = null);
        Task<Stone> GetStoneByCodeAsync(string stoneCode);
        Task<Stone?> DetachStoneFromProductAsync(int id);
    }
}