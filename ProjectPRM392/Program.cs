var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["CONNECTION_STRING_ELECTRONICSTOREDB"]
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is not configured. Please set CONNECTION_STRING_ELECTRONICSTOREDB environment variable.");
}

builder.Services.AddDALServices(builder.Configuration);
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
