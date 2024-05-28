using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public interface IOrderRepository
    {
        Task<List<Orders>> GetAllOrders();
        Task<Orders> GetOrdersById(int id);

        Task<Orders> DeleteOrders(int id);
        Task<Orders> AddOrders(Orders orders);
        Task<Orders> UpdateGOrders(int id, Orders orders);
        Task<int> GetNumberOfOrdersAsync();
        Task<List<Orders>> GetWaitingOrdersAsync();
        Task<List<Orders>> GetPaidOrdersAsync();
        Task<List<Orders>> GetDoneOrdersAsync();
        Task<List<OrderDetails>> GetNumberOfOrderDetailsAsync(int id);

    }
}
