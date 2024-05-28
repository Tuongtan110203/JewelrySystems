using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public class SQLCustomerRepository : ICustomerRepository
    {
        private readonly JewelrySalesSystemDbContext dbContext;
        public SQLCustomerRepository(JewelrySalesSystemDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public async Task<Customers> CreateAsync(Customers customer)
        {
            await dbContext.Customers.AddAsync(customer);
            await dbContext.SaveChangesAsync();
            return customer;    
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

        public async Task<Customers> GetCustomerByNumberPhone(string phoneNumber)
        {
            return await dbContext.Customers.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        }

        public async Task<bool> IsPhoneNumberDuplicateAsync(string phoneNumber, int? customerId = null)
        {
            return await dbContext.Customers.AnyAsync(x => x.PhoneNumber == phoneNumber && (!customerId.HasValue || x.CustomerId != customerId.Value));
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
    }
}
