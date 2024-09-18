using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public class SQLCustomerRepository : ICustomerRepository
    {
        private readonly JewelrySalesSystemDbContext dbContext;
        public SQLCustomerRepository(JewelrySalesSystemDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Customers> CreateAsync(Customers customer)
        {
            await dbContext.Customers.AddAsync(customer);
            await dbContext.SaveChangesAsync();
            return customer;
        }
        public async Task<List<Customers>> SearchCustomersByPhoneOrFullname(string searchText)
        {
            return await dbContext.Customers
                .Where(x => x.Status == "active" && (x.PhoneNumber.Contains(searchText) || x.CustomerName.Contains(searchText)))
                .ToListAsync();
        }
        public async Task<Customers?> DeleteCustomersAsync(int id)
        {
            var checkExist = await dbContext.Customers.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.CustomerId == id);
            if (checkExist == null) { return null; }
            checkExist.Status = "inactive";
            await dbContext.SaveChangesAsync();
            return checkExist;
        }

        public async Task<List<Customers>> GetAllCustomersAsync()
        {
            return await dbContext.Customers.Where(x => x.Status == "active").ToListAsync();

        }

        public async Task<Customers?> GetCustomerByIdAsync(int id)
        {
            return await dbContext.Customers.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.CustomerId == id);
        }

        public async Task<List<Customers>> GetCustomersByNumberPhone(string phoneNumber)
        {
            return await dbContext.Customers.Where(x => x.PhoneNumber.Contains(phoneNumber)).ToListAsync();
        }


        public async Task<Customers?> UpdateCustomersAsync(int id, Customers customer)
        {
            var Checkexist = await dbContext.Customers.FirstOrDefaultAsync(x => x.CustomerId == id);
            if (Checkexist == null) return null;
            Checkexist.CustomerName = customer.CustomerName;
            Checkexist.PhoneNumber = customer.PhoneNumber;
            Checkexist.Email = customer.Email;
            Checkexist.Status = customer.Status;
            await dbContext.SaveChangesAsync();
            return Checkexist;
        }

        public async Task<Customers?> GetCustomerByNumberPhone(string phoneNumber)
        {
            return await dbContext.Customers.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        }
        public async Task<bool> IsPhoneNumberDuplicateAsync(string phoneNumber, int? id = null)
        {
            if (id.HasValue)
            {
                return await dbContext.Customers
                    .AnyAsync(c => c.PhoneNumber == phoneNumber && c.CustomerId != id.Value);
            }
            return await dbContext.Customers
                .AnyAsync(c => c.PhoneNumber == phoneNumber);
        }

        public async Task<bool> IsEmailDuplicateAsync(string email, int? id = null)
        {
            if (id.HasValue)
            {
                return await dbContext.Customers
                    .AnyAsync(c => c.Email == email && c.CustomerId != id.Value);
            }
            return await dbContext.Customers
                .AnyAsync(c => c.Email == email);
        }
    }
}
