namespace ProjectPRM392.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("users")]
    //[Authorize(Roles = "Admin")] 
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
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}
