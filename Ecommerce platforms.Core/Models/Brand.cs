using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_platforms.Core.Models
{
    public class Brand:ModelBase
    {
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public ICollection<Product> Products { get; set; }

    }
}
