using System.Runtime.CompilerServices;
using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public interface ICustomerRepository
    {
        Task<List<Customers>> GetAllCustomersAsync();
        Task<Customers?> GetCustomerByIdAsync(int id);
        Task<Customers?> UpdateCustomersAsync(int id, Customers customer);
        Task<Customers> CreateAsync(Customers customer);
        Task<Customers?> DeleteCustomersAsync(int id);

        Task<bool> IsPhoneNumberDuplicateAsync(string phoneNumber, int? customerId = null); 

        Task<Customers> GetCustomerByNumberPhone (string phoneNumber);
    }
}
