using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDALServices(builder.Configuration);
builder.Services.AddBLLServices(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Electronic Store API",
        Version = "v1",
        Description = "API for managing products, orders, and users in the Electronic Store."
    });
});

var app = builder.Build();

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
