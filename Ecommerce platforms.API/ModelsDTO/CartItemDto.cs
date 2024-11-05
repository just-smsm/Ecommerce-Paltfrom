namespace Ecommerce_platforms.API.ModelsDTO
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string PictureUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; }
    }
}
