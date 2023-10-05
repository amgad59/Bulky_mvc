using Empire.DataAccess.Data;
using Empire.DataAccess.Repository.IRepository;
using Empire.Models;

namespace Empire.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private ApplicationDbContext _db;

        public OrderDetailRepository(ApplicationDbContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetail orderDetail)
        {
            _db.OrderDetails.Update(orderDetail);
        }
    }
}
