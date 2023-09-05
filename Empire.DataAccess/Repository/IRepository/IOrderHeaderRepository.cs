using Empire.Models;

namespace Empire.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);

        Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null);

        Task UpdatePayMobPaymentID(int id, int? orderId, int? transactionId);
    }
}
