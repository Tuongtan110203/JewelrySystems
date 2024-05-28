using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public class SQLUserRepository : IUsersRepository
    {
        private readonly JewelrySalesSystemDbContext dbContext;

        public SQLUserRepository(JewelrySalesSystemDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async  Task<Users> AddUsers(Users users)
        {
            await dbContext.Users.AddAsync(users);
            await dbContext.SaveChangesAsync();
            return users;
        }

        public async Task<Users> DeleteUsers(string name)
        {
            var checkExist = await dbContext.Users.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.UserName == name);
            if (checkExist == null) { return null; }
            checkExist.Status = "inactive";
            await dbContext.SaveChangesAsync();
            return checkExist;
        }

        public async Task<List<Users>> GetAllUsersAsync()
        {
            return await dbContext.Users.Include(u => u.Roles).ToListAsync();
        }

        public async Task<Users?> GetUsersByNameAsync(string name)
        {
            return await dbContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.UserName == name);
        }

        public async Task<Users> UpdateUsers(string name, Users users)
        {
            var Checkexist = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == name);
            if (Checkexist == null) return null;
            Checkexist.FullName = users.FullName;
            Checkexist.Password = users.Password;
            Checkexist.Address = users.Address;
            Checkexist.Phone = users.Phone;
            Checkexist.Email = users.Email;
            Checkexist.Dob = users.Dob;
            Checkexist.Level = users.Level;
            Checkexist.RoleId = users.RoleId;
            await dbContext.SaveChangesAsync();
            return Checkexist;
        }
    }
}
