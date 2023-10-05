using Empire.DataAccess.Data;
using Empire.DataAccess.Repository.IRepository;
using Empire.Models;

namespace Empire.DataAccess.Repository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private ApplicationDbContext _db;

        public ProductImageRepository(ApplicationDbContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(ProductImage productImage)
        {
            _db.ProductImages.Update(productImage);
        }
    }
}
