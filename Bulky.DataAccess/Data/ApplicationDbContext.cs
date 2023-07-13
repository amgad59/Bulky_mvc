using Empire.Models;
using Empire.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Empire.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category() { Id = 1,Name="Hoodie",DisplayOrder = 1},
                new Category() { Id = 2,Name="Action",DisplayOrder = 2},
                new Category() { Id = 3,Name="Drama",DisplayOrder = 3}
                );
            modelBuilder.Entity<ProductSize>().HasData(
                new ProductSize() { Id = 1,Name="Small", DisplayName ="S"},
                new ProductSize() { Id = 2,Name="Medium", DisplayName ="M"},
                new ProductSize() { Id = 3,Name="Large", DisplayName ="L"}
                );
            modelBuilder.Entity<Product>().HasData(
                new Product() { Id = 1, Description = "A white hoodie with butterflies on it", ListPrice = 350,CategoryId=1,ImageUrl=""},
                new Product() { Id = 2, Description = "A black hoodie with butterflies on it", ListPrice = 450, CategoryId = 1, ImageUrl = ""},
                new Product() { Id = 3, Description = "A white hoodie with Sinatraa on it", ListPrice = 400, CategoryId = 1, ImageUrl = "" }
                );
        }
    }
}
