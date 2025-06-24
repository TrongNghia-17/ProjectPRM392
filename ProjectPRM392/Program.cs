var builder = WebApplication.CreateBuilder(args);

// Lấy chuỗi kết nối từ biến môi trường, nếu không có thì dùng appsettings.json
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING_ELECTRONICSTOREDB")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is not configured. Please set CONNECTION_STRING_ELECTRONICSTOREDB environment variable or update appsettings.json.");
}

builder.Services.AddDALServices(connectionString);
builder.Services.AddBLLServices(builder.Configuration);
builder.Services.AddProjectPRM392Services(builder.Configuration);

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
