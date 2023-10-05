using Empire.DataAccess.Data;
using Empire.DataAccess.Repository.IRepository;
using Empire.Models;

namespace Empire.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _db.ShoppingCarts.Update(shoppingCart);
        }
    }
}
