using Empire.DataAccess.Data;
using Empire.DataAccess.Repository.IRepository;
using Empire.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire.DataAccess.Repository
{
    public class ProductSizeRepository : Repository<ProductSize>, IProductSizeRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductSizeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void update(ProductSize productSize)
        {
            _db.ProductSizes.Update(productSize);
        }
    }
}
