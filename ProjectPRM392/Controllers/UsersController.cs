namespace ProjectPRM392.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService, ILogger<AuthsController> logger) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly ILogger<AuthsController> _logger = logger;

    [HttpPut("me")]
    [Authorize]
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
            _logger.LogInformation("User with ID {UserId} updated their information successfully.", userId);
            return NoContent();
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
}
