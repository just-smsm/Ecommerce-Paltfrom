using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Ecommerce_platforms.Core.Models
{
    public class OrderItems : ModelBase
    {
        public int ProductId { get; set; } // Foreign key to Product

        [Required]
        public string ProductName { get; set; } // Name of the product

        public int Quantity { get; set; } // Quantity of the product

        public decimal Price { get; set; } // Price of the product
        public string PictureUrl { get; set; }

        // Foreign key relationship to Cart
        public int OrderId  { get; set; }
        public Order Order { get; set; }
        public decimal SubTotal { get; set; }
    }
}
