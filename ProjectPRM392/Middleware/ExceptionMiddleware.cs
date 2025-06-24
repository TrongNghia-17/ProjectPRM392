namespace ProjectPRM392.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
            context.Response.ContentType = "application/json";
            var response = new ErrorResponse
            {
                StatusCode = GetStatusCode(ex),
                Message = GetErrorMessage(ex),
                Details = ex.StackTrace,
                Timestamp = DateTime.UtcNow
            };

            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }

    private int GetStatusCode(Exception ex)
    {
        return ex switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private string GetErrorMessage(Exception ex)
    {
        return ex switch
        {
            ArgumentException argEx => argEx.Message,
            KeyNotFoundException keyEx => keyEx.Message,
            UnauthorizedAccessException unauthEx => unauthEx.Message,
            _ => "An unexpected error occurred. Please try again later."
        };
    }
}