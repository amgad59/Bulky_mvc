using System.Linq.Expressions;
using Empire.DataAccess.Data;
using Empire.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Empire.DataAccess.Repository
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            ArgumentNullException.ThrowIfNull(db);
            dbSet = db.Set<T>();
        }

        public async Task Add(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void DeleteAll(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public async Task<T> GetEntity(Expression<Func<T, bool>> expression, string? includeProperties = null, bool isTracked = false)
        {
            IQueryable<T> query;
            if (isTracked)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }

            query = query.Where(expression);
            char[] splitter = new char[] { ',' };
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties
                    .Split(splitter, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllEntities(Expression<Func<T, bool>>? expression = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (expression != null)
            {
                query = query.Where(expression);
            }

            char[] splitter = new char[] { ',' };
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties
                    .Split(splitter, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return await query.ToListAsync();
        }
    }
}
