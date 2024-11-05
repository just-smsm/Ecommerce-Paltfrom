
namespace Ecommerce_platforms.API.ModelsDTO
{
    public class CartItemResponse
    {
        public int Id { get; set; }
        public string PictureUrl { get; set; } // List of picture URLs
        public string ProductName { get; set; } // Name of the product
        public int Quantity { get; set; } // Quantity of the product
        public decimal Price { get; set; } // Price of the product
       // public decimal TotalPrice { get; set; }
    }
}
