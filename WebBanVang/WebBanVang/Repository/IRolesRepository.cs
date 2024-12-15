using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public interface IRolesRepository
    {
        Task<List<Roles>> GetAllRoles();
        Task<Roles?> GetRolesById(int id);

        Task<Roles?> DeleteRoles(int id);
        Task<Roles> AddRoles(Roles roles);
        Task<Roles?> UpdateRoles(int id, Roles roles);
    }
}
