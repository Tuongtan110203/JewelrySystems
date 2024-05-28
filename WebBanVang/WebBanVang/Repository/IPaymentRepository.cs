using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetAllPayment();
        Task<Payment> GetPaymentById(int id);

        Task<Payment> DeletePayment(int id);
        Task<Payment> AddPayment(Payment payment);
        Task<Payment> UpdatePayment(int id, Payment payment);

        Task<List<Orders>> UpdateStatusOrder(int orderId);

        
    }
}
