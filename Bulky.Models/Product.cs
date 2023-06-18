using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [Range(0, 10000, ErrorMessage = "must be 0-100")]
        public int ListPrice { get; set; }
	}
}
/*
		[Key]
		public int Id { get; set; }
		[Required]
		public string description;
		[Required]
		[Display(Name = "List price")]
		public int ListPrice;*/