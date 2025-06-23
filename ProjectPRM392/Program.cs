var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDALServices(builder.Configuration);
builder.Services.AddBLLServices(builder.Configuration);
builder.Services.AddProjectPRM392Services(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        var error = new
        {
            message = exception?.Message ?? "An unexpected error occurred",
            status = "Error"
        };

        await context.Response.WriteAsJsonAsync(error);
    });
});
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Electronic Store API v1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();
