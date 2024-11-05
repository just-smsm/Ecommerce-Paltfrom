using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Ecommerce_platforms.Core.Models
{
    public class Order : ModelBase
    {
        public string OrderStatus { get; set; }
        public string UserEmail { get; set; }
        public string UserCity { get; set; }
        public string Phone { get; set; }
        public string Details { get; set; }
        public int? DeliveryMethodId { get; set; }
        public ICollection<OrderItems> OrderItems { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
    }
}
