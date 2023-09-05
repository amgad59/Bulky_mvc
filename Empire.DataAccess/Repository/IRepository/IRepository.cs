using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Empire.DataAccess.Repository.IRepository
{
    public interface IRepository<T>
        where T : class
    {
        Task<IEnumerable<T>> GetAllEntities(Expression<Func<T, bool>>? expression = null, string? includeProperties = null);

        Task<T> GetEntity(Expression<Func<T, bool>> expression, string? includeProperties = null, bool isTracked = false);

        Task Add(T entity);

        void Delete(T entity);

        void DeleteAll(IEnumerable<T> entities);
    }
}
