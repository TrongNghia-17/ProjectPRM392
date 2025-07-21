using BLL.DTOs.OdersDTO;

namespace ProjectPRM392.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService, ILogger<AuthsController> logger) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly ILogger<AuthsController> _logger = logger;

    [HttpPut("SelfUpdateUser")]
    //[Authorize]
    public async Task<IActionResult> SelfUpdateUser([FromBody] SelfUpdateUserRequest request)
    {
        try
        {
            // Lấy UserId từ token JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Invalid user token.");
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid UserId format in token: {UserIdClaim}", userIdClaim);
                throw new UnauthorizedAccessException("Invalid user token.");
            }

            await _userService.SelfUpdateUserAsync(userId, request);
            var updatedUser = await _userService.GetUserByIdAsync(userId);
            _logger.LogInformation("User with ID {UserId} updated their information successfully.", userId);
            return Ok(updatedUser);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Self-update failed: {Message}", ex.Message);
            return NotFound(new { Status = "Error", Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Self-update failed: {Message}", ex.Message);
            return BadRequest(new { Status = "Error", Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Self-update failed: {Message}", ex.Message);
            return Unauthorized(new { Status = "Error", Message = ex.Message });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error during self-update for user.");
            return StatusCode(500, new { Status = "Error", Message = "Failed to update user due to database error." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during self-update for user.");
            return StatusCode(500, new { Status = "Error", Message = "An unexpected error occurred." });
        }
    }

    [HttpGet("GetUser")]
    //[Authorize]
    public async Task<IActionResult> GetUser()
    {
        try
        {
            // Lấy UserId từ token JWT để kiểm tra quyền
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Invalid user token.");
            if (!Guid.TryParse(userIdClaim, out var currentUserId))
            {
                _logger.LogWarning("Invalid UserId format in token: {UserIdClaim}", userIdClaim);
                return Unauthorized(new { Status = "Error", Message = "Invalid user token." });
            }

            // Kiểm tra quyền: Chỉ admin hoặc chính người dùng đó có thể xem thông tin
            // Giả sử bạn có logic kiểm tra vai trò trong User (ví dụ: Role == "Admin")
            var user = await _userService.GetUserByIdAsync(currentUserId);

            _logger.LogInformation("User with ID {UserId} retrieved successfully.", currentUserId);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Get user failed: {Message}", ex.Message);
            return NotFound(new { Status = "Error", Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Get user failed: {Message}", ex.Message);
            return Unauthorized(new { Status = "Error", Message = ex.Message });
        }
    }

    [HttpPut("update-and-create-order")]
    public async Task<IActionResult> UpdateUserAndCreateOrder([FromBody] UpdateOrderUserInforRequest request)
    {
        try
        {
            // Lấy UserId từ token JWT để kiểm tra quyền
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Invalid user token.");
            if (!Guid.TryParse(userIdClaim, out var currentUserId))
            {
                _logger.LogWarning("Invalid UserId format in token: {UserIdClaim}", userIdClaim);
                return Unauthorized(new { Status = "Error", Message = "Invalid user token." });
            }

            var createdOrder = await _userService.UpdateUserAndCreateOrderAsync(currentUserId, request);
            return Ok(new { Message = "User information updated, order created, and cart cleared successfully."});
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message, Status = "Error" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message, Status = "Error" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message, Status = "Error" });
        }
    }
}
