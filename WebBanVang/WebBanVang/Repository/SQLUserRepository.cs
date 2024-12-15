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
        public async Task<Users> AddUsers(Users users)
        {
            users.Password = BCrypt.Net.BCrypt.HashPassword(users.Password);
            await dbContext.Users.AddAsync(users);
            await dbContext.SaveChangesAsync();
            return users;
        }

        public async Task<Users?> DeleteUsers(string name)
        {
            var checkExist = await dbContext.Users.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.UserName == name);
            if (checkExist == null) { return null; }
            checkExist.Status = "inactive";
            await dbContext.SaveChangesAsync();
            return checkExist;
        }

        public async Task<List<Users>> GetAllUsersAsync()
        {
            return await dbContext.Users.Include(u => u.Roles).Where(x => x.Status == "active").ToListAsync();
        }

        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            return await dbContext.Users.Where(x => x.Status == "active").FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<Users?> GetUserByPhoneAsync(string phone)
        {
            return await dbContext.Users.Where(x => x.Status == "active").FirstOrDefaultAsync(u => u.Phone == phone);
        }
        public async Task<Users?> GetUserByUserNameAsync(string name)
        {
            return await dbContext.Users.Where(x => x.Status == "active").Include(u => u.Roles).FirstOrDefaultAsync(u => u.UserName == name);
        }

        public async Task<List<Users>> GetUsersByFullNameAsync(string name)
        {
            return await dbContext.Users.Where(x => x.Status == "active").Include(u => u.Roles).Where(u => u.FullName.Contains(name)).ToListAsync();
        }

        public async Task<Users?> UpdateUsers(string name, Users users)
        {
            var Checkexist = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == name);
            if (Checkexist == null) return null;
            Checkexist.FullName = users.FullName;
            Checkexist.Password = BCrypt.Net.BCrypt.HashPassword(users.Password);
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
