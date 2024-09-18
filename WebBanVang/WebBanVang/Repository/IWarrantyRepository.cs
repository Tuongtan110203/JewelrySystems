using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public interface IWarrantyRepository
    {
        Task<string> GenerateWarrantyCodeAsync();
        Task<List<Warranty>> GetAllWarranty();
        Task<List<Warranty>> GetWarrantyByProductCode(string productCode);

        Task<Warranty?> GetWarrantyById(int id);

        Task<Warranty?> DeleteWarranty(int id);

        Task<Warranty?> UpdateWarranty(int id, Warranty warranty);
        Task<List<Warranty>> GetOutOfDateWarrantyAsync();
        Task<List<Warranty>> GetWarrantiesByIdAsync(int id);
        Task<List<Warranty>> GetWarrantyByCodeAsync(string warrantyCode);

    }
}
