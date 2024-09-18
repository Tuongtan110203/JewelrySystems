
using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;

namespace WebBanVang.Services
{
    public class DailyOrderCleanupService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public DailyOrderCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Calculate time until next 10 PM
            var now = DateTime.Now;
            var timeUntilTenPM = new DateTime(now.Year, now.Month, now.Day, 22, 0, 0) - now;
            if (timeUntilTenPM < TimeSpan.Zero)
            {
                timeUntilTenPM = timeUntilTenPM.Add(new TimeSpan(1, 0, 0, 0));
            }

            // Set the timer to run the task daily at 10 PM
            _timer = new Timer(CleanUpOrders, null, timeUntilTenPM, TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        private async void CleanUpOrders(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<JewelrySalesSystemDbContext>();

                var ordersToUpdate = await dbContext.Orders
                    .Where(o => o.Status == "Đang thanh toán")
                    .ToListAsync();

                foreach (var order in ordersToUpdate)
                {

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

                    // Update order status
                    order.Status = "Hết hạn thanh toán";

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



