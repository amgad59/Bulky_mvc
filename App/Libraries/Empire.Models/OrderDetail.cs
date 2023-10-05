using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        [Required]
        public int OrderHeaderId { get; set; }
        [ForeignKey(nameof(OrderHeaderId))]
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        public int productSizeId { get; set; }
        [ForeignKey(nameof(productSizeId))]
        [ValidateNever]
        public ProductSize productSize { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
    }
}
