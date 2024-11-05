using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Ecommerce_platforms.Core.Models
{
    public class Product:ModelBase
    {
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Count { get; set; }
        [NotMapped]
        public bool IsAvailable
        {
            get { return Count > 0; }
        }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        
    }
}
