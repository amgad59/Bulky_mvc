using Bulky.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Category>().HasData(
                new Category() { Id = 1,Name="Hoodie",DisplayOrder = 1},
                new Category() { Id = 2,Name="Action",DisplayOrder = 2},
                new Category() { Id = 3,Name="Drama",DisplayOrder = 3}
                );
            modelBuilder.Entity<Product>().HasData(
                new Product() { Id = 1, Description = "A white hoodie with butterflies on it", ListPrice = 350 },
                new Product() { Id = 2, Description = "A black hoodie with butterflies on it", ListPrice = 450 },
                new Product() { Id = 3, Description = "A white hoodie with Sinatraa on it", ListPrice = 400 }
                );

        }
    }
}
