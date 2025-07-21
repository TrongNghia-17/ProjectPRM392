using BLL.DTOs.OdersDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProjectPRM392.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        public OrdersController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderRequest request)
        {
            try
            {
                var updatedOrder = await _orderService.UpdateOrderAsync(id, request);
                return Ok(new { Order = updatedOrder, Message = "Order updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message, Status = "Error" });
            }
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateOrder(Guid userId)
        {
            try
            {
                // Lấy UserId từ token JWT để kiểm tra quyền
                //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                //    ?? throw new UnauthorizedAccessException("Invalid user token.");
                //if (!Guid.TryParse(userIdClaim, out var currentUserId))
                //{
                //    //_logger.LogWarning("Invalid UserId format in token: {UserIdClaim}", userIdClaim);
                //    return Unauthorized(new { Status = "Error", Message = "Invalid user token." });
                //}

                var createdOrder = await _userService.UpdateUserAndCreateOrderAsync(userId);
                return Ok(new { Message = "User information updated, order created, and cart cleared successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message, Status = "Error" });
            }
        }

        [HttpGet("GetAllOrderOfUser")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new UnauthorizedAccessException("Invalid user token.");

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    throw new UnauthorizedAccessException("Invalid user token format.");
                }

                var orders = await _orderService.GetAllOrdersByUserIdAsync(userId);
                return Ok(orders);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("GetAllOrder")]
        public async Task<IActionResult> GetAllOrder()
        {
            try
            {                
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetOrderItems(Guid id)
        {
            try
            {
                var orderItems = await _orderService.GetOrderItemsByOrderIdAsync(id);
                if (orderItems == null || !orderItems.Any())
                    return NotFound(new { Message = "No items found for this order." });
                return Ok(orderItems);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetMonthlyRevenue([FromQuery] int month, [FromQuery] int year)
        {
            if (month < 1 || month > 12 || year < 2000)
                return BadRequest("Invalid month or year.");

            var totalRevenue = await _orderService.GetMonthlyRevenueAsync(month, year);

            return Ok(new
            {
                Month = month,
                Year = year,
                TotalRevenue = totalRevenue
            });
        }

        [HttpGet("monthly-revenue/{year}")]
        public async Task<IActionResult> GetMonthlyRevenue(int year)
        {
            var result = await _orderService.GetMonthlyRevenueAsync(year);
            return Ok(result);
        }
    }
}
