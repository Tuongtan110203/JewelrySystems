using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public interface IGoldTypeRepository
    {
        Task<List<GoldType>> GetAllGoldType();
        Task<GoldType> GetGoldTypeById(int id);

        Task<GoldType> DeleteGoldType(int id);
        Task<GoldType> AddGoldType(GoldType goldType);
        Task<GoldType> UpdateGoldTYpe(int id, GoldType goldType);
    }
}
