using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;

namespace WebBanVang.Repository
{
    public interface IOrderRepository
    {
        Task<List<Orders>> GetAllOrders(int pageNumber = 1, int pageSize = 100);
        Task<Orders?> GetOrdersById(int id);
        Task<List<Orders>> GetOrdersByOrderCode(string code);

        Task<Orders?> DeleteOrders(int id);
        Task<Orders> AddOrders(Orders orders);
        Task<string> GenerateOrderCodeAsync();
        Task<Orders?> UpdateGOrders(int id, Orders orders);
        Task<int> GetNumberOfOrdersAsync();
        Task<List<Orders>> GetWaitingOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null,
          bool isAscending = true);
        Task<List<Orders>> GetPaidOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null,
          bool isAscending = true);
        Task<List<Orders>> GetDoneOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null,
          bool isAscending = true);
        Task<List<Orders>> GetWaitOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null,
        bool isAscending = true);
        Task<List<Orders>> GetCancelOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null,
        bool isAscending = true);
        Task<List<Orders>> GetPayingOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null,
        bool isAscending = true);
        Task<List<Orders>> GetExpiredOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null,
        bool isAscending = true);
        Task<List<Orders>> GetStatusOrdersAsync(int pageNumber = 1, int pageSize = 100, string? sortBy = null,
        bool isAscending = true);

        //  Task<List<OrderDetails>> GetNumberOfOrderDetailsAsync(int id);
        Task<List<Orders>> GetTodayOrdersAsync();
        Task<List<Orders>> GetThisWeekOrdersAsync();
        Task<List<Orders>> GetThisMonthOrdersAsync();
        Task<List<Orders>> GetThisYearOrdersAsync();
        Task<int> GetNumberOfByCashOrdersAsync();
        Task<int> GetNumberOfByBankTransferOrdersAsync();
        Task<int> GetNumberOfByCashAndBankTransferOrdersAsync();

        Task<int> GetNumberOfOrderDetailsAsync(int id);
        Task<List<OrderDetails>> GetOrderDetailsAsync(int id);
        Task<double> GetTotalPriceOfTodayOrdersAsync();
        Task<double> GetTotalPriceOfThisWeekOrdersAsync();
        Task<double> GetTotalPriceOfThisMonthOrdersAsync();
        Task<double> GetTotalPriceOfThisYearOrdersAsync();

        Task<double?> GetTotalPriceOfBankTransferAsync();
        Task<double?> GetTotalPriceOfByCashAsync();

        Task<int> GetNumberOfOrderQuantitAsync(int id);
        Task<int> GetNumberPaidOrdersAsync(int pageNumber, int pageSize);
        Task<int> GetNumberDoneOrdersAsync(int pageNumber, int pageSize);
        Task<int> GetNumberWaitingOrdersAsync(int pageNumber, int pageSize);
        Task<int> GetNumberWaitOrdersAsync(int pageNumber, int pageSize);
        Task<int> GetNumberCancelOrdersAsync(int pageNumber, int pageSize);
        Task<int> GetNumberExpiredOrdersAsync(int pageNumber, int pageSize);
        Task<int> GetNumberPayingOrdersAsync(int pageNumber, int pageSize);
        Task<int> GetNumberStatusOrdersAsync(int pageNumber, int pageSize);

        Task<int> GetNumberOfByCashOrdersAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetNumberOfByBankTransferOrdersAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<double?> GetTotalPriceOfBankTransferAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<double?> GetTotalPriceOfByCashAsync(DateTime? startDate = null, DateTime? endDate = null);

        Task<List<Orders>> GetTodayOrdersTotalAsync();
        Task<List<Orders>> GetThisWeekOrdersTotalAsync();
        Task<List<Orders>> GetThisMonthOrdersTotalAsync();
        Task<List<Orders>> GetThisYearOrdersTotalAsync();

        Task<List<OrdersDTO>> GetTodayOrdersTotalAsyncReve();
        Task<List<OrdersDTO>> GetThisWeekOrdersTotalAsyncReve();
        Task<List<OrdersDTO>> GetThisMonthOrdersTotalAsyncReve();
        Task<List<OrdersDTO>> GetThisYearOrdersTotalAsyncReve();

    }
}
