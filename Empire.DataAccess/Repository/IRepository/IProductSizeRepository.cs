using Empire.DataAccess.Data;
using Empire.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire.DataAccess.Repository.IRepository
{
    public interface IProductSizeRepository : IRepository<ProductSize>
    {
        void update(ProductSize product);
    }
}
