using Empire.Models;

namespace Empire.DataAccess.Repository.IRepository
{
    public interface IProductSizeRepository : IRepository<ProductSize>
    {
        void Update(ProductSize product);
    }
}
