namespace ProjectPRM392.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartItemController(ICartItemService cartItemService) : ControllerBase
{
    private readonly ICartItemService _cartItemService = cartItemService;

    [HttpPost("{productId}/add-to-cart")]
    public async Task<IActionResult> Add(Guid productId, [FromQuery] Guid userId, [FromQuery] int quantity)
    {
        try
        {
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
}
