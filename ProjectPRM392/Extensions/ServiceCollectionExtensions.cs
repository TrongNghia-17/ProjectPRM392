namespace DAL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProjectPRM392Services(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Electronic Store API",
                Version = "v1",
                Description = "API for managing products, orders, and users in the Electronic Store."
            });
        });

        return services;
    }
}