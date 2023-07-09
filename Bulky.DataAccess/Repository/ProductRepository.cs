using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
	public class ProductRepository : Repository<Product>,IProductRepository
	{
		private ApplicationDbContext _db;
		public ProductRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}
		public void update(Product product)
        {
            _db.Products.Update(product);
		}
		public void deleteSize(Product product,int productSizeId)
		{
            if (product != null)
            {
                var sizeToRemove = product.ProductSizes.FirstOrDefault(ps => ps.Id == productSizeId);
                if (sizeToRemove != null)
                {
                    product.ProductSizes.Remove(sizeToRemove);
                    update(product);
                }
            }
        }		
        public async Task addSize(Product product,int productSizeId)
		{
            if (product != null)
            {
                var sizeToAdd = await _db.ProductSizes.FirstOrDefaultAsync(ps => ps.Id == productSizeId);
                if (sizeToAdd != null && !product.ProductSizes.Contains(sizeToAdd))
                {
                    product.ProductSizes.Add(sizeToAdd);
                    update(product);
                }
            }
        }
	}
}
