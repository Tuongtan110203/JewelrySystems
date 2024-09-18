using WebBanVang.Models.DTO;

namespace WebBanVang.Repository
{
    public class RevenueService
    {
        private readonly IOrderRepository orderRepository;

        public RevenueService(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }
        public List<OrdersDTO> GetOrdersOfToday()
        {
            return orderRepository.GetTodayOrdersTotalAsyncReve().Result;
        }
        public List<OrdersDTO> GetThisWeekOrdersAsync()
        {
            return orderRepository.GetThisWeekOrdersTotalAsyncReve().Result;
        }
        public List<OrdersDTO> GetThisMonthOrdersAsync()
        {
            return orderRepository.GetThisMonthOrdersTotalAsyncReve().Result;
        }
        public List<OrdersDTO> GetThisYearOrdersAsync()
        {
            return orderRepository.GetThisYearOrdersTotalAsyncReve().Result;
        }
    }
}
