using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
	public interface IProductRepository : IRepository<Product>
	{
		void update(Product product);
		public void deleteSize(Product product, int productSizeId);
		public void addSize(Product product, int productSizeId);

    }
}
