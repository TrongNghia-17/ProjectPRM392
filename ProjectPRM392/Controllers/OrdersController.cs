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
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var orderId = await _orderService.CreateOrderAsync(request);
                return Ok(new { OrderId = orderId, Message = "Order created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message, Status = "Error" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
              ?? throw new UnauthorizedAccessException("Invalid user token.");
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                //_logger.LogWarning("Invalid UserId format in token: {UserIdClaim}", userIdClaim);
                throw new UnauthorizedAccessException("Invalid user token.");
            }
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
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
    }
}
