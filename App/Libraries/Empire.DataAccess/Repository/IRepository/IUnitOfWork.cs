using Microsoft.AspNetCore.Identity.UI.Services;

namespace Empire.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }

        IProductRepository Product { get; }

        IProductSizeRepository ProductSize { get; }

        IShoppingCartRepository ShoppingCart { get; }

        IApplicationUserRepository ApplicationUser { get; }

        IOrderDetailRepository OrderDetail { get; }

        IOrderHeaderRepository OrderHeader { get; }

        IProductImageRepository ProductImage { get; }

        IEmailSender EmailSender { get; }

        Task Save();
    }
}
