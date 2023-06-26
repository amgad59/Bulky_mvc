using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Drawing;
using Bulky.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Bulky.Models
{
	public class Product
	{
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Product Description")]
        public string Description { get; set; }
        [Required]
        [DisplayName("List Price")]
        [Range(0, 10000, ErrorMessage = "must be 0-10000")]
        public int ListPrice { get; set; }
		public int CategoryId { get; set; }
		[ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        public string ImageUrl { get; set; }
        public int Discount { get; set; }
        [ValidateNever]
        public virtual List<ProductSize> ProductSizes { get; set; }
    }
}