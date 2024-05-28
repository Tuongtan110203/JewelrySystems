using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public class SQLRolesRepository : IRolesRepository
    {
        private readonly JewelrySalesSystemDbContext dbContext;
      

        public SQLRolesRepository(JewelrySalesSystemDbContext dbContext)
        {
            this.dbContext = dbContext;
          
        }
        public async Task<Roles> AddRoles(Roles roles)
        {
            await dbContext.Roles.AddAsync(roles);
            await dbContext.SaveChangesAsync();
            return roles;
        }

        public async Task<Roles> DeleteRoles(int id)
        {
            var checkExist = await dbContext.Roles.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.RoleId == id);
            if (checkExist == null) { return null; }
            checkExist.Status = "inactive";
            await dbContext.SaveChangesAsync();
            return checkExist;
        }

        public async Task<List<Roles>> GetAllRoles()
        {
            return await dbContext.Roles.Where(x => x.Status == "active").ToListAsync();
        }

        public async Task<Roles> GetRolesById(int id)
        {
            return await dbContext.Roles.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.RoleId == id);
        }

        public async Task<Roles> UpdateRoles(int id, Roles roles)
        {
            var Checkexist = await dbContext.Roles.FirstOrDefaultAsync(x => x.RoleId == id);
            if (Checkexist == null) return null;
            Checkexist.RoleName = roles.RoleName;
            Checkexist.Status = roles.Status;
            await dbContext.SaveChangesAsync();
            return Checkexist;
        }
    }
}
