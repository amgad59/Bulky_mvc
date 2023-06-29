using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
	public interface IRepository<T> where T : class
	{
		IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression=null, string? includeProperties = null);
		T Get(Expression<Func<T,bool>> expression,string? includeProperties = null,bool isTracked = false);
		void Add(T entity);
		void Delete(T entity);
		void DeleteAll(IEnumerable<T> entities);
	}
}
