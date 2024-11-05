namespace Ecommerce_platforms.API.ModelsDTO
{
    public class CartDto
    {
        public string Email { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItemDto> Items { get; set; }
    }
}
