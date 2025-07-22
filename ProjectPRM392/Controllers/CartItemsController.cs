namespace ProjectPRM392.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartItemsController(ICartItemService cartItemService) : ControllerBase
{
    private readonly ICartItemService _cartItemService = cartItemService;

    [HttpPost("{productId}/add-to-cart")]
    public async Task<IActionResult> Add(Guid productId, [FromQuery] Guid userId, [FromQuery] int quantity)
    {
        try
        {
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            //   ?? throw new UnauthorizedAccessException("Invalid user token.");
            //if (!Guid.TryParse(userIdClaim, out var userId))
            //{
            //    //_logger.LogWarning("Invalid UserId format in token: {UserIdClaim}", userIdClaim);
            //    throw new UnauthorizedAccessException("Invalid user token.");
            //}

            await _cartItemService.AddAsync(userId, productId, quantity);
            return Ok(new { Message = $"Product {productId} added to cart for user {userId} successfully.", Status = "Success" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message, Status = "Error" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message, Status = "Error" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message, Status = "Error" });
        }
    }

    [HttpPut("{productId}/decrease-cart-quantity")]
    public async Task<IActionResult> DecreaseQuantity(Guid productId, [FromQuery] Guid userId, [FromQuery] int quantity)
    {
        try
        {
            await _cartItemService.DecreaseQuantityAsync(userId, productId, quantity);
            return Ok(new { Message = $"Cart quantity for product {productId} decreased by {quantity} for user {userId}.", Status = "Success" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message, Status = "Error" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message, Status = "Error" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message, Status = "Error" });
        }
    }

    [HttpGet("cart-of-user")]
    public async Task<IActionResult> GetCartItemsByUserId([FromQuery] Guid userId, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 3)
    {
        try
        {
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            //   ?? throw new UnauthorizedAccessException("Invalid user token.");
            //if (!Guid.TryParse(userIdClaim, out var userId))
            //{
            //    //_logger.LogWarning("Invalid UserId format in token: {UserIdClaim}", userIdClaim);
            //    throw new UnauthorizedAccessException("Invalid user token.");
            //}

            var result = await _cartItemService.GetCartItemsByUserIdAsync(userId, pageIndex, pageSize);
            return Ok(new
            {
                Status = "Success",
                Data = result.CartItems,
                Pagination = new
                {
                    result.TotalCount,
                    result.PageIndex,
                    result.PageSize,
                    result.TotalPages
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message, Status = "Error" });
        }
    }
}
