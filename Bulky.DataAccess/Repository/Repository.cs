using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Bulky.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext _db;
		internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
			_db = db;
			dbSet = db.Set<T>();
        }
        public void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public void Delete(T entity)
		{
			dbSet.Remove(entity);
		}

		public void DeleteAll(IEnumerable<T> entities)
		{
			dbSet.RemoveRange(entities);
		}

		public T Get(Expression<Func<T, bool>> expression, string? includeProperties = null, bool isTracked = false)
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
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.FirstOrDefault();
        }

		public IEnumerable<T> GetAll(string? includeProperties = null)
		{
			IQueryable<T> query = dbSet;
			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var property in includeProperties
					.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(property);
				}
			}
			return query.ToList();
		}
	}
}
