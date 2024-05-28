using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public class SQLPaymentRepository : IPaymentRepository
    {
        private readonly JewelrySalesSystemDbContext dbContext;

        public SQLPaymentRepository(JewelrySalesSystemDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Payment> AddPayment(Payment payment)
        {
            await dbContext.Payments.AddAsync(payment);
            await dbContext.SaveChangesAsync();
          
            return payment;
        }

        public async Task<Payment> DeletePayment(int id)
        {

            var checkExist = await dbContext.Payments.FirstOrDefaultAsync(x => x.PaymentId == id);
            if (checkExist == null)
            {
                return null;
            }


            checkExist.Status = "Chưa thanh toán";
            await dbContext.SaveChangesAsync();


            var order = await dbContext.Orders
               .Where(w => w.OrderId == checkExist.OrderId && w.Status == "Đã thanh toán")
               .ToListAsync();

            foreach (var orders in order)
            {
                orders.Status = "Hủy thanh toán";
            }
            await dbContext.SaveChangesAsync();

            var paymentWithOrder = await dbContext.Payments
                .Include(p => p.Orders)
                .ThenInclude(o => o.Customers)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            var customerId = paymentWithOrder.Orders.CustomerId;
            var warranties = await dbContext.Warranties
                .Where(w => w.CustomerId == customerId && w.Status == "Đã hoàn thành")
                .ToListAsync();

            foreach (var warranty in warranties)
            {
                warranty.Status = "Chưa hoàn thành";
            }
            await dbContext.SaveChangesAsync();

            return checkExist;
        }


        public async Task<List<Payment>> GetAllPayment()
        {
            return await dbContext.Payments.ToListAsync();
        }

        public async Task<Payment> GetPaymentById(int id)
        {
            return await dbContext.Payments.FirstOrDefaultAsync(x => x.PaymentId == id);
        }

        public async Task<Payment> UpdatePayment(int id, Payment payment)
        {
            var Checkexist = await dbContext.Payments.FirstOrDefaultAsync(x => x.PaymentId == id);
            if (Checkexist == null) return null;
            Checkexist.OrderId = payment.OrderId;
            Checkexist.PaymentType = payment.PaymentType;
            Checkexist.Cash = payment.Cash;
            Checkexist.BankTransfer = payment.BankTransfer;
            Checkexist.TransactionId = payment.TransactionId;
            Checkexist.PaymentTime = payment.PaymentTime;
            Checkexist.Image = payment.Image;
            Checkexist.Status = payment.Status;
            await dbContext.SaveChangesAsync();
            return Checkexist;
        }

            public async Task<List<Orders>> UpdateStatusOrder(int orderId)
            {
                var OrdertoUpdate = await dbContext.Orders.Where(w => w.OrderId == orderId && w.Status == "Đợi Thanh Toán").ToListAsync();
                foreach (var order in OrdertoUpdate)
                {
                    order.Status = "Đã Thanh Toán";
                }
                await dbContext.SaveChangesAsync();
                return OrdertoUpdate;
            }

       
    }
}
