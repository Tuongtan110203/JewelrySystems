using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
namespace WebBanVang.Services
{
    public class OrderCleanupService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public OrderCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Set the timer to run the task every hour
            _timer = new Timer(CleanUpOrders, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
            return Task.CompletedTask;
        }

        private async void CleanUpOrders(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<JewelrySalesSystemDbContext>();
                //sau 2h thì tự động chuyển status từ "Đợi thành Hủy thanh toán" và cập nhật lại quantity
                // var twoHoursAgo = DateTime.Now.AddHours(-2);

                //test: sau 5p thì tự động chuyển
                // var twoHoursAgo = order.OrderDate.AddMinutes(-1);

                var ordersToUpdate = await dbContext.Orders
               .Where(o => o.Status == "Đợi thanh toán")
               .ToListAsync();

                foreach (var order in ordersToUpdate)
                {
                    var thresholdTime = order.OrderDate.AddHours(2);

                    // Check if the current time exceeds the threshold time
                    if (DateTime.Now >= thresholdTime)
                    {
                        // Get the order details for the current order
                        var orderDetails = await dbContext.OrderDetails
                        .Where(od => od.OrderId == order.OrderId)
                        .Include(od => od.Products)
                        .ToListAsync();

                        // Update product quantities
                        foreach (var orderDetail in orderDetails)
                        {
                            if (orderDetail.Products != null)
                            {
                                orderDetail.Products.Quantity += orderDetail.Quantity;
                            }
                        }

                        // Update the order status
                        order.Status = "Hủy thanh toán";
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }


}
