namespace Ecommerce_platforms.API.ModelsDTO
{
    public class CartResponse
    {
        public decimal TotalPrice { get; set; }
        public List<CartItemResponse> Items { get; set; }
    }
}
