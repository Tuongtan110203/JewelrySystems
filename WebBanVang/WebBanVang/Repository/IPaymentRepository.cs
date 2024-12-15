using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetAllPayment();
        Task<Payment?> GetPaymentById(int id);
        Task<List<Payment?>> GetPaymentByOrderCode(string orderCode);

        Task<Payment?> DeletePayment(int id);
        Task<Payment> AddPayment(Payment payment);
        Task<Payment?> UpdatePayment(int id, Payment payment);
        Task<List<Payment>> SearchPaymentByPaymentCodeOrOrderCode(string keySeach);

        Task UpdateStatusOrder(int orderId);
        Task<List<Payment>> GetPaymentsToday();
        Task<List<Payment>> GetPaymentsThisWeek();
        Task<List<Payment>> GetPaymentsThisMonth();

        Task<List<Payment>> GetPaymentsThisYear();
        Task<List<Payment>> GetBankTransferPayment();
        Task<List<Payment>> GetByCashPayment();
    }
}
