namespace ProjectPRM392.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred: {Message}. Request: {Method} {Path}",
                ex.Message, context.Request.Method, context.Request.Path);
            context.Response.ContentType = "application/json";
            var response = new ErrorResponse
            {
                StatusCode = GetStatusCode(ex),
                Message = GetErrorMessage(ex),
                Details = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment() ? ex.StackTrace : null,
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
            InvalidOperationException => StatusCodes.Status400BadRequest, 
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
            InvalidOperationException invEx => invEx.Message, 
            _ => "An unexpected error occurred. Please try again later."
        };
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; }
}