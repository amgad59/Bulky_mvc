using Empire.DataAccess.Data;
using Empire.DataAccess.Repository.IRepository;
using Empire.Models;

namespace Empire.DataAccess.Repository
{
    public class ProductSizeRepository : Repository<ProductSize>, IProductSizeRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductSizeRepository(ApplicationDbContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(ProductSize product)
        {
            _db.ProductSizes.Update(product);
        }
    }
}
