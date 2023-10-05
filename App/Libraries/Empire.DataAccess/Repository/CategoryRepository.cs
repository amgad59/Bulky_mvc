using Empire.DataAccess.Data;
using Empire.DataAccess.Repository.IRepository;
using Empire.Models;

namespace Empire.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) 
            : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
            _db.Categories.Update(category);
        }
    }
}
