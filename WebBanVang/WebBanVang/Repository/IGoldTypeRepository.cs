using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;

namespace WebBanVang.Repository
{
    public interface IGoldTypeRepository
    {
        Task<List<GoldType>> GetAllGoldType();
        Task<GoldType?> GetGoldTypeById(int id);

        Task<GoldType?> DeleteGoldType(int id);
        Task<GoldType> AddGoldType(GoldType goldType);
        Task<GoldType?> UpdateGoldTYpe(int id, GoldType goldType);
        Task<List<GoldType>> GetGoldTypeByNameAsync(string name);
        Task<List<GoldTypePercentageDTO>> GetGoldCodePercentagesForToday();
        Task<List<GoldTypePercentageDTO>> GetGoldCodePercentagesForThisWeek();
        Task<List<GoldTypePercentageDTO>> GetGoldCodePercentagesForThisMonth();
        Task<List<GoldTypePercentageDTO>> GetGoldCodePercentagesForThisYear();

    }
}
