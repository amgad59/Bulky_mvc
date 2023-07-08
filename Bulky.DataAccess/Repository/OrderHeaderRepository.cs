using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.IdentityModel.Tokens;
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
		public void UpdatePayMobPaymentID(int id, int? orderId, int? TransactionId)
		{
			var order = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
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
