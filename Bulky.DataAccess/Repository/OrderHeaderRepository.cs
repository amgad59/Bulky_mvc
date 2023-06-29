using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
		private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
			_db = db;
        }

		public void update(OrderHeader OrderHeader)
		{
			_db.OrderHeaders.Update(OrderHeader);
		}

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var order = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if (order != null)
			{
				order.OrderStatus = orderStatus;
				if (!string.IsNullOrEmpty(paymentStatus))
				{
					order.PaymentStatus = paymentStatus;
				}
			}
		}
		public void UpdatePayMobPaymentID(int id, string sessionId, string paymentIntentId)
		{
			var order = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if (!string.IsNullOrEmpty(sessionId))
			{
				order.SessionId = sessionId;
			}
			if (!string.IsNullOrEmpty(paymentIntentId))
			{
				order.PaymentIntentId = paymentIntentId;
				order.PaymentDate = DateTime.Now;
			}
		}

	}
}
