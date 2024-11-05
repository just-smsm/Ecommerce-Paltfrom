using AutoMapper;
using Ecommerce_platforms.API.ModelsDTO;
using Ecommerce_platforms.Core.IRepository;
using Ecommerce_platforms.Core.Models;
using Ecommerce_platforms.Repository.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Address = Ecommerce_platforms.Core.Models.Address;

namespace Ecommerce_platforms.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public CartController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ILogger<CartController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            var email = GetEmailFromToken(authorizationHeader);
            if (string.IsNullOrEmpty(email))
                return Unauthorized("Invalid token or user email not found.");

            var cart = await _unitOfWork.Cart.GetCartBYEmail(email);
            if (cart == null)
                return NotFound("Cart not found.");

            cart.Items.Clear();
            cart.TotalPrice = 0;

            if (!await SaveChangesAsync(email, "clearing cart"))
                return StatusCode(500, "An error occurred while saving changes.");

            return Ok(new { totalPrice = cart.TotalPrice });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCart([FromHeader(Name = "Authorization")] string authorizationHeader, [FromBody] int productId)
        {
            var email = GetEmailFromToken(authorizationHeader);
            if (string.IsNullOrEmpty(email))
                return Unauthorized("Invalid token or user not found.");

            var cart = await _unitOfWork.Cart.CreateCart(email, productId);
            if (cart == null)
            {
                _logger.LogError("Error adding product {productId} to cart for user: {email}", productId, email);
                return BadRequest("Error while adding product to cart.");
            }

            return Ok(cart);
        }

        [HttpGet]
        public async Task<IActionResult> GetCart([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            var email = GetEmailFromToken(authorizationHeader);
            if (string.IsNullOrEmpty(email))
                return Unauthorized("Invalid token or user not found.");

            var cart = await _unitOfWork.Cart.GetCartBYEmail(email);
            return Ok(cart);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProductFromCart([FromHeader(Name = "Authorization")] string authorizationHeader, [FromRoute] int productId)
        {
            var email = GetEmailFromToken(authorizationHeader);
            if (string.IsNullOrEmpty(email))
                return Unauthorized("Invalid token or user not found.");

            var cart = await _unitOfWork.Cart.DeleteCartProduct(email, productId);
            if (cart == null)
            {
                _logger.LogError("Error deleting product {productId} from cart for user: {email}", productId, email);
                return BadRequest("Error while deleting product.");
            }

            return Ok(cart);
        }

        [HttpPost("pay")]
        public async Task<IActionResult> Pay([FromHeader(Name = "Authorization")] string authorizationHeader, [FromBody] PayRequestDTO payRequest)
        {
            var email = GetEmailFromToken(authorizationHeader);
            if (string.IsNullOrEmpty(email))
                return Unauthorized("Invalid token or user email not found.");

            var cart = await _unitOfWork.Cart.GetCartBYEmail(email);
            if (cart == null)
                return NotFound("Cart not found.");

            var user = await _userManager.FindByEmailAsync(email);
            UpdateShippingAddress(cart, user, payRequest);

            if (!await SaveChangesAsync(email, "updating cart and user details"))
                return StatusCode(500, "An error occurred while saving changes.");

            return await ProcessPayment(cart, user, payRequest);
        }

        [HttpGet("cartItems")]
        public async Task<IActionResult> GetCartItems([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            var email = GetEmailFromToken(authorizationHeader);
            if (string.IsNullOrEmpty(email))
                return Unauthorized("Invalid token or user email not found.");

            var items = await _unitOfWork.Cart.GetCartItem(email);
            var cart = await _unitOfWork.Cart.GetCartBYEmail(email);
            var cartItemResponses = _mapper.Map<List<CartItemResponse>>(items);

            return Ok(new CartResponse
            {
                Items = cartItemResponses,
                TotalPrice = cart.TotalPrice
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProductQuantity([FromHeader(Name = "Authorization")] string authorizationHeader, [FromBody] UpdateQuantityDTo updateQuantityDto)
        {
            var email = GetEmailFromToken(authorizationHeader);
            if (string.IsNullOrEmpty(email))
                return Unauthorized("Invalid token or user email not found.");

            var cart = await _unitOfWork.Cart.UpdateProductQuantity(email, updateQuantityDto.ProductId, updateQuantityDto.Quantity);
            if (cart == null)
                return BadRequest("Error while updating product quantity.");

            return Ok(new CartDto
            {
                Email = cart.Email,
                TotalPrice = cart.TotalPrice,
                Items = cart.Items.Select(ci => new CartItemDto
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Price
                }).ToList()
            });
        }

        private string GetEmailFromToken(string authorizationHeader)
        {
            var token = GetTokenFromHeader(authorizationHeader);
            if (string.IsNullOrEmpty(token))
                return null;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email")?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing token");
                return null;
            }
        }

        private string GetTokenFromHeader(string authorizationHeader)
        {
            return string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ")
                ? null
                : authorizationHeader.Substring("Bearer ".Length).Trim();
        }

        private void UpdateShippingAddress(Cart cart, AppUser user, PayRequestDTO payRequest)
        {
            cart.ShippingAddress ??= new Address();
            cart.ShippingAddress.Name = payRequest.Name;
            cart.ShippingAddress.Phone = payRequest.Phone;
            cart.ShippingAddress.City = payRequest.City;
            cart.ShippingAddress.Detail = payRequest.Details;

            user.Street = payRequest.Street;
            user.City = payRequest.City;
            user.Country = payRequest.Country;
            user.Details = payRequest.Details;
        }

        private async Task<bool> SaveChangesAsync(string email, string action)
        {
            try
            {
                await _unitOfWork.Complete();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes for user: {email} during {action}", email, action);
                return false;
            }
        }

        private async Task<IActionResult> ProcessPayment(Cart cart, AppUser user, PayRequestDTO payRequest)
        {
            try
            {
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = cart.Items.Select(item => new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName,
                            },
                            UnitAmount = (long)(item.Price * 100),
                        },
                        Quantity = item.Quantity,
                    }).ToList(),
                    Mode = "payment",
                    SuccessUrl = $"http://localhost:62870/success?items={Uri.EscapeDataString(string.Join(",", cart.Items.Select(i => $"{i.ProductId}-{i.ProductName}-{i.Quantity}-{i.Price}")))}",
                    CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel",
                };

                var service = new Stripe.Checkout.SessionService();
                var session = await service.CreateAsync(options);

                await _unitOfWork.Cart.UpdateCount(cart.Items);
                await _unitOfWork.Order.AddAsync(CreateOrder(cart, user));
                cart.Items.Clear();

                if (!await SaveChangesAsync(user.Email, "finalizing order"))
                    return StatusCode(500, "An error occurred while saving order details.");

                return Ok(new { sessionUrl = session.Url });
            }
            catch (Stripe.StripeException ex)
            {
                _logger.LogError(ex, "Stripe API error for user: {email}", user.Email);
                return StatusCode(500, "An error occurred with the Stripe payment process.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error for user: {email}", user.Email);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        private Order CreateOrder(Cart cart, AppUser user)
        {
            return new Order
            {
                UserEmail = user.Email,
                OrderItems = cart.Items.Select(cartItem => new OrderItems
                {
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.ProductName,
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity,
                    PictureUrl = cartItem.PictureUrl,
                    SubTotal = cartItem.Price * cartItem.Quantity
                }).ToList(),
                OrderStatus = SD.Shipped,
                UserCity = cart.ShippingAddress.City,
                Details = cart.ShippingAddress.Detail,
                Phone = cart.ShippingAddress.Phone
            };
        }
    }
}
