using Empire.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Empire.DataAccess.Repository.IRepository
{
	public interface IProductRepository : IRepository<Product>
	{
		void update(Product product);
		void deleteSize(Product product, int productSizeId);
		Task addSize(Product product, int productSizeId);

    }
}
