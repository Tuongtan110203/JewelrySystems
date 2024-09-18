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
        public async Task<List<Payment>> GetPaymentsToday()
        {
            var today = DateTime.Today;
            return await dbContext.Payments
                .Where(p => p.PaymentTime.Date == today)
                .Include(x => x.Orders)
                .ToListAsync();
        }
        public async Task<List<Payment>> SearchPaymentByPaymentCodeOrOrderCode(string keySearch)
        {
            return await dbContext.Payments
            .Where(x => x.PaymentCode.Contains(keySearch) || x.Orders.OrderCode.Contains(keySearch)).Include(x => x.Orders)
                            .ToListAsync();
        }
        public async Task<List<Payment>> GetPaymentsThisWeek()
        {

            var today = DateTime.Now;
            var currentDayOfWeek = (int)today.DayOfWeek;
            currentDayOfWeek = (currentDayOfWeek == 0) ? 7 : currentDayOfWeek;
            var startOfWeek = today.AddDays(-currentDayOfWeek + 1).Date;
            var endOfWeek = startOfWeek.AddDays(7).Date;

            var ordersForThisWeek = await dbContext.Payments.Where(x => x.PaymentTime >= startOfWeek && x.PaymentTime < endOfWeek).Include(x => x.Orders).ToListAsync();
            return ordersForThisWeek;

        }

        public async Task<List<Payment>> GetPaymentsThisMonth()
        {
            var today = DateTime.Now;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            var ordersForThisMonth = await dbContext.Payments.Where(x => x.PaymentTime >= startOfMonth && x.PaymentTime < startOfNextMonth).Include(x => x.Orders).ToListAsync();
            return ordersForThisMonth;
        }

        public async Task<List<Payment>> GetPaymentsThisYear()
        {
            var today = DateTime.Now;
            var startOfYear = new DateTime(today.Year, 1, 1);
            var startOfNextYear = startOfYear.AddYears(1);

            var ordersForThisYear = await dbContext.Payments.Where(x => x.PaymentTime >= startOfYear && x.PaymentTime < startOfNextYear).Include(x => x.Orders).ToListAsync();
            return ordersForThisYear;
        }

        public async Task<Payment> AddPayment(Payment payment)
        {
            payment.PaymentCode = await GeneratePaymentCodeAsync(payment.PaymentType);
            await dbContext.Payments.AddAsync(payment);
            await dbContext.SaveChangesAsync();
            return payment;
        }

        private async Task<string> GeneratePaymentCodeAsync(string paymentType)
        {
            string prefix;
            if (paymentType == "Tiền mặt")
            {
                prefix = "TM";
            }
            else if (paymentType == "Chuyển khoản")
            {
                prefix = "CK";
            }
            else
            {
                throw new ArgumentException("Invalid payment type");
            }

            var lastPaymentCode = await dbContext.Payments
                .Where(p => p.PaymentCode.StartsWith(prefix))
                .OrderByDescending(p => p.PaymentCode)
                .Select(p => p.PaymentCode)
                .FirstOrDefaultAsync();

            int nextPaymentCodeNumber = 1;

            if (!string.IsNullOrEmpty(lastPaymentCode) && lastPaymentCode.StartsWith(prefix))
            {
                if (int.TryParse(lastPaymentCode.Substring(2), out int currentMax))
                {
                    nextPaymentCodeNumber = currentMax + 1;
                }
            }

            return $"{prefix}{nextPaymentCodeNumber:D4}";
        }

        public async Task UpdateStatusOrder(int orderId)
        {
            var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order != null)
            {
                var totalPaid = await dbContext.Payments.Where(p => p.OrderId == orderId).SumAsync(p => (p.Cash ?? 0) + (p.BankTransfer ?? 0));
                if (totalPaid == order.Total)
                {
                    order.Status = "Đã thanh toán";
                }
                else
                {
                    order.Status = "Đang thanh toán";
                }
                dbContext.Orders.Update(order);
                await dbContext.SaveChangesAsync();
            }
        }



        public async Task<Payment?> DeletePayment(int id)
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
            return await dbContext.Payments
                .Include(x => x.Orders)
                .ToListAsync();
        }

        public async Task<Payment?> GetPaymentById(int id)
        {
            return await dbContext.Payments.FirstOrDefaultAsync(x => x.PaymentId == id);
        }
        public async Task<List<Payment>> GetPaymentByOrderID(int orderId)
        {
            return await dbContext.Payments.Where(x => x.OrderId == orderId).ToListAsync();
        }

        public async Task<Payment?> UpdatePayment(int id, Payment payment)
        {
            var Checkexist = await dbContext.Payments.FirstOrDefaultAsync(x => x.PaymentId == id);
            if (Checkexist == null) return null;
            Checkexist.PaymentCode = payment.PaymentCode;
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

        public async Task<List<Payment?>> GetPaymentByOrderCode(string orderCode)
        {
            return await dbContext.Payments.Where(x => x.Orders.OrderCode.Contains(orderCode)).ToListAsync();
        }

        public async Task<List<Payment>> GetBankTransferPayment()
        {
            return await dbContext.Payments.Include(x => x.Orders).Where(x => x.PaymentType == "Chuyển khoản").ToListAsync();
        }

        public async Task<List<Payment>> GetByCashPayment()
        {
            return await dbContext.Payments.Include(x => x.Orders).Where(x => x.PaymentType == "Tiền mặt").ToListAsync();
        }

        //public async Task<List<Orders>> UpdateStatusOrder(int orderId)
        //{
        //    var OrdertoUpdate = await dbContext.Orders.Where(w => w.OrderId == orderId && w.Status == "Đợi thanh toán").ToListAsync();
        //    foreach (var order in OrdertoUpdate)
        //    {
        //        order.Status = "Đã thanh toán";
        //    }
        //    await dbContext.SaveChangesAsync();
        //    return OrdertoUpdate;
        //}

    }
}
