﻿using Empire.DataAccess.Data;
using Empire.DataAccess.Repository.IRepository;
using Empire.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire.DataAccess.Repository
{
	public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
		private ApplicationDbContext _db;
        public ProductImageRepository(ApplicationDbContext db) : base(db)
        {
			_db = db;
        }

		public void update(ProductImage productImage)
		{
			_db.ProductImages.Update(productImage);
		}
	}
}