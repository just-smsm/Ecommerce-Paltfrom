using AutoMapper;
using Ecommerce_platforms.API.ModelsDTO;
using Ecommerce_platforms.Core.IRepository;
using Ecommerce_platforms.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Ecommerce_platforms.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            try
            {
                // Await the asynchronous operation to get the actual product list
                var products = await _unitOfWork.Product.GetAllProductsWithPictures();

                if (products == null || !products.Any())
                {
                    return NotFound("No products found.");
                }

                // Map the collection of Product entities to a collection of ProductDTO
                var mappedProducts = _mapper.Map<IEnumerable<ProductDTO>>(products);

                return Ok(mappedProducts); // Wrap the result in an ActionResult
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching products.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            try
            {
                // Await the asynchronous operation to get the product
                var product = await _unitOfWork.Product.GetAllProductWithPictures(id);

                if (product == null)
                {
                    // Return 404 Not Found if the product does not exist
                    return NotFound();
                }

                // Map the Product entity to ProductDTO
                var productDto = _mapper.Map<ProductDTO>(product);

                // Return the DTO with 200 OK status
                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the product.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNew(CreateNewProductDTO NewProduct)
        {
            try
            {
                // Map the ProductDTO to Product entity
                var product = _mapper.Map<Product>(NewProduct);

                // Perform the add operation asynchronously
                var createdProduct = await _unitOfWork.Product.AddAsync(product);

                if (createdProduct == null)
                {
                    // Return 400 Bad Request if the creation failed
                    return BadRequest("Product could not be created.");
                }

                // Return 201 Created with the created product's DTO
                var createdProductDTO = _mapper.Map<ProductDTO>(createdProduct);
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProductDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the product.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPut]
        public async Task<ActionResult<ProductDTO>> UpdateProduct(ProductDTO productDTO)
        {
            try
            {
                // Map the ProductDTO to Product entity
                var productToUpdate = _mapper.Map<Product>(productDTO);

                // Perform the update operation asynchronously
                var updatedProduct = await _unitOfWork.Product.UpdateAsync(productToUpdate);

                if (updatedProduct == null)
                {
                    // Return 404 Not Found if the update failed or the product does not exist
                    return NotFound();
                }

                // Map the updated Product entity back to ProductDTO
                var updatedProductDTO = _mapper.Map<ProductDTO>(updatedProduct);

                // Return the updated ProductDTO with 200 OK status
                return Ok(updatedProductDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                // Perform the get operation asynchronously
                var product = await _unitOfWork.Product.GetByIdAsync(id);

                if (product == null)
                {
                    // Return 404 Not Found if the product does not exist
                    return NotFound();
                }

                // Perform the delete operation asynchronously
                var deleted = await _unitOfWork.Product.DeleteAsync(product.Id);

                if (!deleted)
                {
                    // Return 500 Internal Server Error if the deletion failed
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting the product.");
                }

                // Return 200 OK with a success message
                return Ok(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        private string GetTokenFromHeader(string authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return null;
            }

            return authorizationHeader.Substring("Bearer ".Length).Trim();
        }

        private string GetEmailFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(token))
                {
                    _logger.LogWarning("Cannot read token: {token}", token);
                    return null;
                }

                var jwtToken = handler.ReadJwtToken(token);

                if (jwtToken == null)
                {
                    _logger.LogWarning("Invalid token: {token}", token);
                    return null;
                }

                var emailClaim = jwtToken.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.Email ||
                    c.Type == "email" ||
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

                return emailClaim?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing token");
                return null;
            }
        }
    }
}
