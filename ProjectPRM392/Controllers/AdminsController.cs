using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ProjectPRM392.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminsController(IUserService userService, ILogger<AuthsController> logger) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly ILogger<AuthsController> _logger = logger;

    [HttpGet("users")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _userService.GetAllUsersAsync();
        return Ok(new
        {
            response
        });
    }

    [HttpGet("{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPut()]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? throw new UnauthorizedAccessException("Invalid user token.");
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Invalid UserId format in token: {UserIdClaim}", userIdClaim);
            throw new UnauthorizedAccessException("Invalid user token.");
        }

        var user = await _userService.UpdateUserAsync(userId, request);
        return Ok(user);
    }

    [HttpDelete("{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            _logger.LogInformation("User with ID {UserId} deleted successfully.", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Delete failed: {Message}", ex.Message);
            return NotFound(new { Status = "Error", Message = ex.Message });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while deleting user with ID {UserId}.", id);
            return StatusCode(500, new { Status = "Error", Message = "Failed to delete user due to database error." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting user with ID {UserId}.", id);
            return StatusCode(500, new { Status = "Error", Message = "An unexpected error occurred." });
        }
    }
}
