using BulkyRazorWeb_Temp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BulkyRazorWeb_Temp.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}
		public DbSet<Category> Categories { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Category>().HasData(
				new Category() { Id = 1, Name = "SciFi", DisplayOrder = 1 },
				new Category() { Id = 2, Name = "Action", DisplayOrder = 2 },
				new Category() { Id = 3, Name = "Drama", DisplayOrder = 3 }
				);
		}
	}
}
