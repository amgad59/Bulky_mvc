using Empire.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire.DataAccess.Repository.IRepository
{
	public interface IOrderHeaderRepository : IRepository<OrderHeader>
	{
		void update(OrderHeader OrderHeader);
		Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
		Task UpdatePayMobPaymentID(int id, int? orderId, int? TransactionId);
	}
}
