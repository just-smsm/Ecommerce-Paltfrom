using Ecommerce_platforms.API.ModelsDTO;
using Ecommerce_platforms.Core.IRepository;
using Ecommerce_platforms.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_platforms.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("OrderByIdAndDeliverList")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _unitOfWork.Order.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            var deliveryMethods = await _unitOfWork.DeliveryMethod.GetAllAsync();
            if (deliveryMethods == null || !deliveryMethods.Any())
            {
                return NotFound("No delivery methods found.");
            }

            var result = new
            {
                Order = order,
                DeliveryMethods = deliveryMethods
            };

            return Ok(result);
        }


        [HttpGet("AllOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _unitOfWork.Order.GetAllAsync();
            if (orders == null || !orders.Any())
            {
                return NotFound("No orders found.");
            }

            return Ok(orders);
        }
        [HttpGet("AllDeliverOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllDeliverOrders()
        {
            return await _unitOfWork.Order.GetAllOrdersWithDeliveryAsync();
        }
        [HttpGet("GetNotDeliverOrder")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrdersWithPendingDeliveryAsync()
        {
            return await _unitOfWork.Order.GetAllOrdersWithPendingDeliveryAsync();
        }
        [HttpPost("deliverOrder")]
        public async Task<IActionResult> DeliverOrder(DeliverOrderDTO orderDTO)
        {
            var updatedOrder = await _unitOfWork.Order.DeliverOrder(orderDTO.OrderId, orderDTO.DeliveryMethodId);
            if (updatedOrder == null)
            {

                return NotFound($"Order with ID {orderDTO.OrderId} not found.");
            }

            return Ok(updatedOrder); // Return the updated order
        }

    }
}
