using Empire.Models;

namespace Empire.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);

        void DeleteSize(Product product, int productSizeId);

        Task AddSize(Product product, int productSizeId);
    }
}
