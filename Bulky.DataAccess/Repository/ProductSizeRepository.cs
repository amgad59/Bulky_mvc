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
