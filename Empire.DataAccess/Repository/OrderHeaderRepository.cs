using Empire.DataAccess.Data;
using Empire.DataAccess.Repository.IRepository;
using Empire.Models;
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

		public void Update(OrderHeader OrderHeader)
        {
			_db.OrderHeaders.Update(OrderHeader);
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
		public async Task UpdatePayMobPaymentID(int id, int? orderId, int? TransactionId)
		{
            var order = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.Id == id);
			if(order != null)
            {
                if (orderId != null)
                {
                    order.OrderId = orderId;
                }
                if (TransactionId != null)
                {
                    order.TransactionId = TransactionId;
                    order.PaymentDate = DateTime.Now;
                }
            }
        }
	}
}
