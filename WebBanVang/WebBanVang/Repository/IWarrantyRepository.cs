using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public interface IWarrantyRepository
    {
        Task<List<Warranty>> GetAllWarranty();
        Task<Warranty> GetWarrantyById(int id);

        Task<Warranty> DeleteWarranty(int id);
 
        Task<Warranty> UpdateWarranty(int id, Warranty warranty);
    }
}
