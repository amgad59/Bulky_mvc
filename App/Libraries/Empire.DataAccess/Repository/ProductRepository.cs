using Empire.DataAccess.Data;
using Empire.DataAccess.Repository.IRepository;
using Empire.Models;
using Microsoft.EntityFrameworkCore;

namespace Empire.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == product.Id);
            if (objFromDb != null)
            {
                objFromDb.ListPrice = product.ListPrice;
                objFromDb.Discount = product.Discount;
                objFromDb.CategoryId = product.CategoryId;
                objFromDb.Description = product.Description;
                objFromDb.ProductSizes = product.ProductSizes;
                objFromDb.ProductImages = product.ProductImages;
            }
        }

        public void DeleteSize(Product product, int productSizeId)
        {
            if (product != null)
            {
                var sizeToRemove = product.ProductSizes.FirstOrDefault(ps => ps.Id == productSizeId);
                if (sizeToRemove != null)
                {
                    product.ProductSizes.Remove(sizeToRemove);
                    Update(product);
                }
            }
        }

        public async Task AddSize(Product product, int productSizeId)
        {
            if (product != null)
            {
                var sizeToAdd = await _db.ProductSizes.FirstOrDefaultAsync(ps => ps.Id == productSizeId);
                if (sizeToAdd != null && !product.ProductSizes.Contains(sizeToAdd))
                {
                    product.ProductSizes.Add(sizeToAdd);
                    Update(product);
                }
            }
        }
    }
}
