using System.ComponentModel.DataAnnotations;

namespace Ecommerce_platforms.API.ModelsDTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string PictureUrl { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string Description { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Count { get; set; }
        
        public bool IsAvailable { get; set; }
        
        public string BrandName { get; set; }
        
    }
}
