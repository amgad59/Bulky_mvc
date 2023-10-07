using Empire.DataAccess.Data;
using Empire.DataAccess.Repository.IRepository;
using Empire.Models;
using Empire.Models.ViewModels;
using Empire.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Empire.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Update(orderHeader);
        }

        public async Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var order = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.Id == id);
            if (order != null)
            {
                order.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    order.PaymentStatus = paymentStatus;
                }
            }
        }

        public async Task UpdatePayMobPaymentID(int id, int? orderId, int? transactionId)
        {
            var order = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.Id == id);
            if (order != null)
            {
                if (orderId != null)
                {
                    order.OrderId = orderId;
                }

                if (transactionId != null)
                {
                    order.TransactionId = transactionId;
                    order.PaymentDate = DateTime.Now;
                }
            }
        }

        public async Task<int> CheckAvailability(string userId)
        {
            OrderHeader? orderHeader = await _db
                                .OrderHeaders
                                .AsNoTracking()
                                .FirstOrDefaultAsync(
                                u => u.ApplicationUserId == userId
                                && u.PaymentStatus == SD.PaymentStatusPending);

            if (orderHeader == null)
            {
                return 0;
            }

            IEnumerable<OrderDetail> orderDetails = _db
                                                    .OrderDetails
                                                    .Where(
                                                    u => u.OrderHeaderId == orderHeader.Id);
            _db.OrderDetails.RemoveRange(orderDetails);
            return orderHeader.Id;
        }
    }
}
