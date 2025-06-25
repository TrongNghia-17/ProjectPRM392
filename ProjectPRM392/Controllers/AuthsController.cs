namespace ProjectPRM392.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthsController(IAuthService authService, ILogger<AuthsController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthsController> _logger = logger;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Registering user with email {Email}", request?.Email);
        try
        {
            var response = await _authService.RegisterAsync(request);
            return CreatedAtAction(
                actionName: nameof(AdminsController.GetById),
                controllerName: "Admins",
                routeValues: new { id = response.UserId },
                value: response
            );
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogWarning("Invalid register request: {Message}", ex.Message);
            return BadRequest(new { Status = "Error", Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Registration failed: {Message}", ex.Message);
            return BadRequest(new { Status = "Error", Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration");
            return StatusCode(500, new { Status = "Error", Message = "An unexpected error occurred." });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    
}
