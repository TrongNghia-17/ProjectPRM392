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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 3)
    {
        var response = await _userService.GetAllUsersAsync(pageIndex, pageSize);
        return Ok(new
        {
            Status = "Success",
            Data = response.Users,
            Pagination = new
            {
                response.TotalCount,
                response.PageIndex,
                response.PageSize,
                response.TotalPages
            }
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        await _userService.UpdateUserAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
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
